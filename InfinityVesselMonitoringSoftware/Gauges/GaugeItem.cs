//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.UndoRedoFramework.Props;
using InfinityGroup.VesselMonitoring.Utilities;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;

namespace InfinityVesselMonitoringSoftware.Gauges
{
    /// <summary>
    /// This class contains the information telling which page the gauge should contain this page (pageId), the
    /// type of gauge (tank, text, arc, etc.) its location, height, width, sensorId, ...
    /// </summary>
    public class GaugeItem : ObservableObject, IGaugeItem
    {
        protected UndoRedoContext _context;
        private PropertyBag _propertyBag;

        private UndoableProperty<DateTime> _changeDate;
        private UndoableProperty<GaugeTypeEnum> _gaugeType;
        private UndoableProperty<Int64> _pageId;
        private UndoableProperty<double> _gaugeHeight;
        private UndoableProperty<double> _gaugeWidth;
        private UndoableProperty<double> _gaugeLeft;
        private UndoableProperty<double> _gaugeTop;
        private UndoableProperty<Color> _gaugeColor;
        private UndoableProperty<int> _divisions;
        private UndoableProperty<int> _minorTicsPerMajorTic;
        private UndoableProperty<int> _mediumTicsPerMajorTic;
        private UndoableProperty<int> _resolution;
        private UndoableProperty<Windows.UI.Xaml.Visibility> _gaugeOutlineVisibility;
        private UndoableProperty<int> _middleCircleDelta;
        private UndoableProperty<int> _innerCircleDelta;
        private UndoableProperty<double> _valueFontSize;
        private UndoableProperty<double> _unitsFontSize;
        private UndoableProperty<double> _majorTicLength;
        private UndoableProperty<double> _mediumTicLength;
        private UndoableProperty<double> _minorTicLength;
        private UndoableProperty<string> _text;
        private UndoableProperty<double> _textFontSize;
        private UndoableProperty<double> _textAngle;
        private UndoableProperty<Color> _textFontColor;
        private UndoableProperty<CanvasHorizontalAlignment> _textHorizontalAlignment;
        private UndoableProperty<CanvasVerticalAlignment> _textVerticalAlignment;
        private UndoableProperty<Int64> _sensorId;
        private UndoableProperty<Units> _units;

        public const string ChangeDatePropertyName = "ChangeDate";
        public const string GaugeTypePropertyName = "GaugeType";
        public const string PageIdPropertyName = "PageId";
        public const string SensorIdPropertyName = "SensorId";
        public const string GaugeHeightPropertyName = "GaugeHeight";
        public const string GaugeWidthPropertyName = "GaugeWidth";
        public const string GaugeLeftPropertyName = "GaugeLeft";
        public const string GaugeTopPropertyName = "GaugeTop";
        public const string GaugeColorPropertyName = "GaugeColor";
        public const string DivisionsPropertyName = "Divisions";
        public const string MinorTicsPerMajorTicPropertyName = "MinorTicsPerMajorTic";
        public const string MediumTicsPerMajorTicPropertyName = "MediumTicsPerMajorTic";
        public const string ResolutionPropertyName = "Resolution";
        public const string GaugeOutlineVisibilityPropertyName = "GaugeOutlineVisibility";
        public const string MiddleCircleDeltaPropertyName = "MiddleCircleDelta";
        public const string InnerCircleDeltaPropertyName = "InnerCircleDelta";
        public const string ValueFontSizePropertyName = "ValueFontSize";
        public const string UnitsFontSizePropertyName = "UnitsFontSize";
        public const string MajorTicLengthPropertyName = "MajorTicLength";
        public const string MediumTicLengthPropertyName = "MediumTicLength";
        public const string MinorTicLengthPropertyName = "MinorTicLength";
        public const string TextPropertyName = "Text";
        public const string TextFontSizePropertyName = "TextFontSize";
        public const string TextAnglePropertyName = "TextAngle";
        public const string TextFontColorPropertyName = "TextFontColor";
        public const string TextHorizontalAlignmentPropertyName = "TextHorizontalAlignment";
        public const string TextVerticalAlignmentPropertyName = "TextVerticalAlignment";
        public const string UnitsPropertyName = "Units";

        /// <summary>
        /// Call this constructor when building a new gauge from scratch
        /// </summary>
        public GaugeItem(Int64 pageId)
        {
            this._context = new UndoRedoContext();
            this.UndoCommand = this._context.GetUndoCommand();
            this.RedoCommand = this._context.GetRedoCommand();

            this.Row = App.BuildDBTables.GaugeTable.CreateRow();
            App.BuildDBTables.GaugeTable.AddRow(this.Row);

            this.LoadFromRow();
            this.PageId = pageId;

            Task.Run(async () =>
            {
                await this.BeginCommit();
            }).Wait();
        }

        /// <summary>
        /// Call this constructor when restoring a gauge from the DB
        /// </summary>
        /// <param name="row"></param>
        public GaugeItem(ItemRow row)
        {
            this.Row = row;

            this._context = new UndoRedoContext();
            this.UndoCommand = this._context.GetUndoCommand();
            this.RedoCommand = this._context.GetRedoCommand();

            this.LoadFromRow();
        }

        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }

        public DateTime ChangeDate
        {
            get { return _changeDate.GetValue(); }
            set
            {
                _changeDate.SetValue(value);
                Row.SetField<DateTime>(ChangeDatePropertyName, value);
                RaisePropertyChanged(() => ChangeDate);
            }
        }

        public int Divisions
        {
            get { return _divisions.GetValue(); }
            set
            {
                _divisions.SetValue(value);
                SetRowPropertyValue<int>(() => Divisions, value);
            }
        }

        public Int64 GaugeId
        {
            get
            {
                if (null == Row) return -1;

                Int64 value = Row.Field<Int64>("GaugeId");
                return value;
            }
        }

        public GaugeTypeEnum GaugeType
        {
            get { return _gaugeType.GetValue(); }
            set
            {
                _gaugeType.SetValue(value);
                SetRowPropertyValue<GaugeTypeEnum>(() => GaugeType, value);
            }
        }


        public double GaugeHeight
        {
            get { return _gaugeHeight.GetValue(); }
            set
            {
                _gaugeHeight.SetValue(value);
                SetRowPropertyValue<double>(() => GaugeHeight, value);
            }
        }

        public double GaugeWidth
        {
            get { return _gaugeWidth.GetValue(); }
            set
            {
                _gaugeWidth.SetValue(value);
                SetRowPropertyValue<double>(() => GaugeWidth, value);
            }
        }

        public double GaugeLeft
        {
            get { return _gaugeLeft.GetValue(); }
            set
            {
                _gaugeLeft.SetValue(value);
                SetRowPropertyValue<double>(() => GaugeLeft, value);
            }
        }

        public Windows.UI.Xaml.Visibility GaugeOutlineVisibility
        {
            get { return _gaugeOutlineVisibility.GetValue(); }
            set
            {
                _gaugeOutlineVisibility.SetValue(value);
                SetRowPropertyValue<Windows.UI.Xaml.Visibility>(() => GaugeOutlineVisibility, value);
            }
        }

        public double GaugeTop
        {
            get { return _gaugeTop.GetValue(); }
            set
            {
                _gaugeTop.SetValue(value);
                SetRowPropertyValue<double>(() => GaugeTop, value);
            }
        }

        public Color GaugeColor
        {
            get { return _gaugeColor.GetValue(); }
            set
            {
                _gaugeColor.SetValue(value);
                SetRowPropertyValue<Color>(() => GaugeColor, value);
            }
        }

        public int InnerCircleDelta
        {
            get { return _innerCircleDelta.GetValue(); }
            set
            {
                _innerCircleDelta.SetValue(value);
                SetRowPropertyValue<int>(() => InnerCircleDelta, value);
            }
        }

        public int MinorTicsPerMajorTic
        {
            get { return _minorTicsPerMajorTic.GetValue(); }
            set
            {
                _minorTicsPerMajorTic.SetValue(value);
                SetRowPropertyValue<int>(() => MinorTicsPerMajorTic, value);
            }
        }

        public int MediumTicsPerMajorTic
        {
            get { return _mediumTicsPerMajorTic.GetValue(); }
            set
            {
                _mediumTicsPerMajorTic.SetValue(value);
                SetRowPropertyValue<int>(() => MediumTicsPerMajorTic, value);
            }
        }

        public Int64 PageId
        {
            get { return _pageId.GetValue(); }
            protected set
            {
                _pageId.SetValue(value);
                SetRowPropertyValue<Int64>(() => PageId, value);
            }
        }

        public int Resolution
        {
            get { return _resolution.GetValue(); }
            set
            {
                _resolution.SetValue(value);
                SetRowPropertyValue<int>(() => Resolution, value);
            }
        }

        public int MiddleCircleDelta
        {
            get { return _middleCircleDelta.GetValue(); }
            set
            {
                _middleCircleDelta.SetValue(value);
                SetRowPropertyValue<int>(() => MiddleCircleDelta, value);
            }
        }

        public Int64 SensorId
        {
            get { return _sensorId.GetValue(); }
            set
            {
                _sensorId.SetValue(value);
                SetRowPropertyValue<Int64>(() => SensorId, value);
            }
        }

        public string Text
        {
            get { return _text.GetValue(); }
            set
            {
                _text.SetValue(value);
                SetRowPropertyValue<string>(() => Text, value);
            }
        }

        public double TextFontSize
        {
            get { return _textFontSize.GetValue(); }
            set
            {
                _textFontSize.SetValue(value);
                SetRowPropertyValue<double>(() => TextFontSize, value);
            }
        }

        public double TextAngle
        {
            get { return _textAngle.GetValue(); }
            set
            {
                _textAngle.SetValue(value);
                SetRowPropertyValue<double>(() => TextAngle, value);
            }
        }

        public Color TextFontColor
        {
            get { return _textFontColor.GetValue(); }
            set
            {
                _textFontColor.SetValue(value);
                SetRowPropertyValue<Color>(() => TextFontColor, value);
            }
        }

        public CanvasHorizontalAlignment TextHorizontalAlignment
        {
            get { return _textHorizontalAlignment.GetValue(); }
            set
            {
                _textHorizontalAlignment.SetValue(value);
                SetRowPropertyValue<CanvasHorizontalAlignment>(() => TextHorizontalAlignment, value);
            }
        }

        public CanvasVerticalAlignment TextVerticalAlignment
        {
            get { return _textVerticalAlignment.GetValue(); }
            set
            {
                _textVerticalAlignment.SetValue(value);
                SetRowPropertyValue<CanvasVerticalAlignment>(() => TextVerticalAlignment, value);
            }
        }

        public double ValueFontSize
        {
            get { return _valueFontSize.GetValue(); }
            set
            {
                _valueFontSize.SetValue(value);
                SetRowPropertyValue<double>(() => ValueFontSize, value);
            }
        }

        public double UnitsFontSize
        {
            get { return _unitsFontSize.GetValue(); }
            set
            {
                _unitsFontSize.SetValue(value);
                SetRowPropertyValue<double>(() => UnitsFontSize, value);
            }
        }

        public double MajorTicLength
        {
            get { return _majorTicLength.GetValue(); }
            set
            {
                _majorTicLength.SetValue(value);
                SetRowPropertyValue<double>(() => MajorTicLength, value);
            }
        }

        public double MediumTicLength
        {
            get { return _mediumTicLength.GetValue(); }
            set
            {
                _mediumTicLength.SetValue(value);
                SetRowPropertyValue<double>(() => MediumTicLength, value);
            }
        }

        public double MinorTicLength
        {
            get { return _minorTicLength.GetValue(); }
            set
            {
                _minorTicLength.SetValue(value);
                SetRowPropertyValue<double>(() => MinorTicLength, value);
            }
        }

        public Units Units
        {
            get { return _units.GetValue(); }
            set
            {
                _units.SetValue(value);
                SetRowPropertyValue<Units>(() => Units, value);
            }
        }

        async public Task BeginCommit()
        {
            // Persist the row into the database
            if (this.IsDirty)
            {
                if (this.PropertyBag.IsDirty)
                {
                    this.Row.SetField<string>("PropertyBag", this.PropertyBag.JsonSerialize());
                }

                await App.BuildDBTables.GaugeTable.BeginCommitRow(
                    Row,
                    () =>
                    {
                        Debug.Assert(this.Row.Field<Int64>("GaugeId") > 0);
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
        }

        async public Task BeginDelete()
        {
            await App.BuildDBTables.GaugeTable.BeginRemove(this.Row);
        }

        public void Rollback()
        {
            // Rollback all of the changed values.
            if ((this.Row.RowState == ItemRowState.Modified) || PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                this.LoadPropertyBag();
                this.ReloadFromRow();
            }
        }

        public void NotifyOfPropertyChangeAll()
        {
            foreach (ItemColumn column in App.BuildDBTables.GaugeTable.Columns)
            {
                RaisePropertyChanged(column.ColumnName);
            }

            foreach (string propName in PropertyBag.PropertyNames())
            {
                RaisePropertyChanged(propName);
            }
        }

        protected void LoadPropertyBag()
        {
            this.PropertyBag = new PropertyBag();

            string json = this.Row.Field<string>("PropertyBag");
            if (!string.IsNullOrEmpty(json))
            {
                this.PropertyBag.JsonDeserialize(json);
            }
        }
        public bool IsDirty
        {
            get
            {
                if (this.Row == null) return false;                             // We do not have any data
                if (this.Row.RowState != ItemRowState.Unchanged)                // No changes have been made
                {
                    return true;
                }

                if (_propertyBag == null) return false;                         // We do not have a property blob

                return this.PropertyBag.IsDirty;
            }
        }
        private PropertyBag PropertyBag
        {
            get
            {
                if (_propertyBag == null)
                {
                    _propertyBag = new PropertyBag();
                }
                return _propertyBag;
            }

            protected set
            {
                if (_propertyBag != value)
                {
                    _propertyBag = value;
                    RaisePropertyChanged(() => PropertyBag);
                    Row.SetField<string>("PropertyBag", _propertyBag.JsonSerialize());
                }
            }
        }

        private bool SetRowPropertyValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (value.Equals(this.Row.Field<T>(propertyName)))
            {
                return false;
            }
            else
            {
                this.Row.SetField<T>(propertyName, (T)value);
                RaisePropertyChanged(() => propertyExpression);
                RaisePropertyChanged(() => IsDirty);
            }

            return true;
        }

        private bool SetPropertyBagValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            T curValue;

            var propertyName = GetPropertyName(propertyExpression);

            if (!this.PropertyBag.Get<T>(propertyName, out curValue) ||
                !curValue.Equals(value))
            {
                this.PropertyBag.Set<T>(propertyName, value);
                RaisePropertyChanged(propertyExpression);
                RaisePropertyChanged(() => IsDirty);
            }

            return true;
        }

        /// <summary>
        /// Call this function to load the properties while constructing this object.
        /// </summary>
        private void LoadFromRow()
        {
            _changeDate = new UndoableProperty<DateTime>(this, ChangeDatePropertyName, this._context, this.Row.Field<DateTime>(ChangeDatePropertyName));
            _divisions = new UndoableProperty<int>(this, DivisionsPropertyName, this._context, this.Row.Field<int>(DivisionsPropertyName));
            _gaugeColor = new UndoableProperty<Color>(this, GaugeColorPropertyName, this._context, this.Row.Field<Color>(GaugeColorPropertyName));
            _gaugeHeight = new UndoableProperty<double>(this, GaugeHeightPropertyName, this._context, this.Row.Field<double>(GaugeHeightPropertyName));
            _gaugeLeft = new UndoableProperty<double>(this, GaugeLeftPropertyName, this._context, this.Row.Field<double>(GaugeLeftPropertyName));
            _gaugeOutlineVisibility = new UndoableProperty<Windows.UI.Xaml.Visibility>(this, GaugeOutlineVisibilityPropertyName, this._context, this.Row.Field<Windows.UI.Xaml.Visibility>(GaugeOutlineVisibilityPropertyName));
            _gaugeTop = new UndoableProperty<double>(this, GaugeTopPropertyName, this._context, this.Row.Field<double>(GaugeTopPropertyName));
            _gaugeType = new UndoableProperty<GaugeTypeEnum>(this, GaugeTypePropertyName, this._context, this.Row.Field<GaugeTypeEnum>(GaugeTypePropertyName));
            _gaugeWidth = new UndoableProperty<double>(this, GaugeWidthPropertyName, this._context, this.Row.Field<double>(GaugeWidthPropertyName));
            _innerCircleDelta = new UndoableProperty<int>(this, InnerCircleDeltaPropertyName, this._context, this.Row.Field<int>(InnerCircleDeltaPropertyName));
            _majorTicLength = new UndoableProperty<double>(this, MajorTicLengthPropertyName, this._context, this.Row.Field<double>(MajorTicLengthPropertyName));
            _middleCircleDelta = new UndoableProperty<int>(this, MiddleCircleDeltaPropertyName, this._context, this.Row.Field<int>(MiddleCircleDeltaPropertyName));
            _mediumTicLength = new UndoableProperty<double>(this, MediumTicLengthPropertyName, this._context, this.Row.Field<double>(MediumTicLengthPropertyName));
            _mediumTicsPerMajorTic = new UndoableProperty<int>(this, MediumTicsPerMajorTicPropertyName, this._context, this.Row.Field<int>(MediumTicsPerMajorTicPropertyName));
            _minorTicLength = new UndoableProperty<double>(this, MinorTicLengthPropertyName, this._context, this.Row.Field<double>(MinorTicLengthPropertyName));
            _minorTicsPerMajorTic = new UndoableProperty<int>(this, MinorTicsPerMajorTicPropertyName, this._context, this.Row.Field<int>(MinorTicsPerMajorTicPropertyName));
            _pageId = new UndoableProperty<Int64>(this, PageIdPropertyName, this._context, this.Row.Field<Int64>(PageIdPropertyName));
            _resolution = new UndoableProperty<int>(this, ResolutionPropertyName, this._context, this.Row.Field<int>(ResolutionPropertyName));
            _sensorId = new UndoableProperty<Int64>(this, SensorIdPropertyName, this._context, this.Row.Field<Int64>(SensorIdPropertyName));
            _text = new UndoableProperty<string>(this, TextPropertyName, this._context, this.Row.Field<string>(TextPropertyName));
            _textAngle = new UndoableProperty<double>(this, TextAnglePropertyName, this._context, this.Row.Field<double>(TextAnglePropertyName));
            _textFontColor = new UndoableProperty<Color>(this, TextFontColorPropertyName, this._context, this.Row.Field<Color>(TextFontColorPropertyName));
            _textFontSize = new UndoableProperty<double>(this, TextFontSizePropertyName, this._context, this.Row.Field<double>(TextFontSizePropertyName));
            _textHorizontalAlignment = new UndoableProperty<CanvasHorizontalAlignment>(this, TextHorizontalAlignmentPropertyName, this._context, this.Row.Field<CanvasHorizontalAlignment>(TextHorizontalAlignmentPropertyName));
            _textVerticalAlignment = new UndoableProperty<CanvasVerticalAlignment>(this, TextVerticalAlignmentPropertyName, this._context, this.Row.Field<CanvasVerticalAlignment>(TextVerticalAlignmentPropertyName));
            _units = new UndoableProperty<Units>(this, UnitsPropertyName, this._context, this.Row.Field<Units>(UnitsPropertyName));
            _unitsFontSize = new UndoableProperty<double>(this, UnitsFontSizePropertyName, this._context, this.Row.Field<double>(UnitsFontSizePropertyName));
            _valueFontSize = new UndoableProperty<double>(this, ValueFontSizePropertyName, this._context, this.Row.Field<double>(ValueFontSizePropertyName));
        }

        /// <summary>
        /// Call this function to reload properties from the backing database.
        /// </summary>
        private void ReloadFromRow()
        {
            this.ChangeDate = this.Row.Field<DateTime>(ChangeDatePropertyName);
            this.Divisions = this.Row.Field<int>(DivisionsPropertyName);
            this.GaugeColor = this.Row.Field<Color>(GaugeColorPropertyName);
            this.GaugeHeight = this.Row.Field<double>(GaugeHeightPropertyName);
            this.GaugeLeft = this.Row.Field<double>(GaugeLeftPropertyName);
            this.GaugeOutlineVisibility = this.Row.Field<Windows.UI.Xaml.Visibility>(GaugeOutlineVisibilityPropertyName);
            this.GaugeTop = this.Row.Field<double>(GaugeTopPropertyName);
            this.GaugeType = this.Row.Field<GaugeTypeEnum>(GaugeTypePropertyName);
            this.GaugeWidth = this.Row.Field<double>(GaugeWidthPropertyName);
            this.InnerCircleDelta = this.Row.Field<int>(InnerCircleDeltaPropertyName);
            this.MajorTicLength = this.Row.Field<double>(MajorTicLengthPropertyName);
            this.MediumTicLength = this.Row.Field<double>(MediumTicLengthPropertyName);
            this.MediumTicsPerMajorTic = this.Row.Field<int>(MediumTicsPerMajorTicPropertyName);
            this.MiddleCircleDelta = this.Row.Field<int>(MiddleCircleDeltaPropertyName);
            this.MinorTicLength = this.Row.Field<double>(MinorTicLengthPropertyName);
            this.MinorTicsPerMajorTic = this.Row.Field<int>(MinorTicsPerMajorTicPropertyName);
            this.PageId = this.Row.Field<Int64>(PageIdPropertyName);
            this.Resolution = this.Row.Field<int>(ResolutionPropertyName);
            this.SensorId = this.Row.Field<Int64>(SensorIdPropertyName);
            this.Text = this.Row.Field<string>(TextPropertyName);
            this.TextFontSize = this.Row.Field<double>(TextFontSizePropertyName);
            this.TextAngle = this.Row.Field<double>(TextAnglePropertyName);
            this.TextFontColor = this.Row.Field<Color>(TextFontColorPropertyName);
            this.TextHorizontalAlignment = this.Row.Field<CanvasHorizontalAlignment>(TextHorizontalAlignmentPropertyName);
            this.TextVerticalAlignment = this.Row.Field<CanvasVerticalAlignment>(TextVerticalAlignmentPropertyName);
            this.Units = this.Row.Field<Units>(UnitsPropertyName);
            this.UnitsFontSize = this.Row.Field<double>(UnitsFontSizePropertyName);
            this.ValueFontSize = this.Row.Field<double>(ValueFontSizePropertyName);
        }

        private ItemRow Row { get; set; }
    }
}
