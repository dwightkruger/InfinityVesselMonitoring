//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.UndoRedoFramework.Props;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    /// <summary>
    /// This class contains the information telling which page the gauge should contain this page (pageId), the
    /// type of gauge (tank, text, arc, etc.) its location, height, width, sensor, ...
    /// </summary>
    public class GaugeItem : ObservableObject, IGaugeItem
    {
        protected UndoRedoContext _context;
        private PropertyBag _propertyBag;

        private UndoableProperty<DateTime> _changeDate;
        private UndoableProperty<Int64> _gaugeId;
        private UndoableProperty<GaugeTypeEnum> _gaugeType;
        private UndoableProperty<Int64> _pageId;
        private UndoableProperty<double> _gaugeHeight;
        private UndoableProperty<double> _gaugeWidth;
        private UndoableProperty<double> _gaugeLeft;
        private UndoableProperty<double> _gaugeTop;
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

        public const string ChangeDatePropertyName = "ChangeDate";
        public const string GaugeIdPropertyName = "GaugeId";
        public const string GaugeTypePropertyName = "GaugeType";
        public const string PageIdPropertyName = "PageId";
        public const string SensorIdPropertyName = "SensorId";
        public const string GaugeHeightPropertyName = "GaugeHeight";
        public const string GaugeWidthPropertyName = "GaugeWidth";
        public const string GaugeLeftPropertyName = "GaugeLeft";
        public const string GaugeTopPropertyName = "GaugeTop";
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

        /// <summary>
        /// Call this constructor when building a new gauge from scratch
        /// </summary>
        public GaugeItem()
        {
            this._context = new UndoRedoContext();
            this.UndoCommand = this._context.GetUndoCommand();
            this.RedoCommand = this._context.GetRedoCommand();

            _changeDate = new UndoableProperty<DateTime>(this, ChangeDatePropertyName, this._context, DateTime.UtcNow);
            _gaugeId = new UndoableProperty<Int64>(this, GaugeIdPropertyName, this._context, -1);
            _gaugeType = new UndoableProperty<GaugeTypeEnum>(this, GaugeTypePropertyName, this._context, GaugeTypeEnum.Unknown);
            _pageId = new UndoableProperty<Int64>(this, PageIdPropertyName, this._context, -1);

            _gaugeLeft = new UndoableProperty<double>(this, GaugeLeftPropertyName, this._context, 0);
            _gaugeTop = new UndoableProperty<double>(this, GaugeTopPropertyName, this._context, 0);
            _gaugeHeight = new UndoableProperty<double>(this, GaugeHeightPropertyName, this._context, 400);
            _gaugeWidth = new UndoableProperty<double>(this, GaugeWidthPropertyName, this._context, 400);
            _sensorId = new UndoableProperty<Int64>(this, SensorIdPropertyName, this._context, -1);

            _divisions = new UndoableProperty<int>(this, DivisionsPropertyName, this._context, 7);
            _minorTicsPerMajorTic = new UndoableProperty<int>(this, MinorTicsPerMajorTicPropertyName, this._context, 3);
            _mediumTicsPerMajorTic = new UndoableProperty<int>(this, MediumTicsPerMajorTicPropertyName, this._context, 6);
            _resolution = new UndoableProperty<int>(this, ResolutionPropertyName, this._context, 0);
            _gaugeOutlineVisibility = new UndoableProperty<Windows.UI.Xaml.Visibility>(this, GaugeOutlineVisibilityPropertyName, this._context, Windows.UI.Xaml.Visibility.Visible);
            _middleCircleDelta = new UndoableProperty<int>(this, MiddleCircleDeltaPropertyName, this._context, 70);
            _innerCircleDelta = new UndoableProperty<int>(this, InnerCircleDeltaPropertyName, this._context, 30);
            _valueFontSize = new UndoableProperty<double>(this, ValueFontSizePropertyName, this._context, 13);
            _unitsFontSize = new UndoableProperty<double>(this, UnitsFontSizePropertyName, this._context, 13);
            _majorTicLength = new UndoableProperty<double>(this, MajorTicLengthPropertyName, this._context, 18D);
            _mediumTicLength = new UndoableProperty<double>(this, MediumTicLengthPropertyName, this._context, 12D);
            _minorTicLength = new UndoableProperty<double>(this, MinorTicLengthPropertyName, this._context, 6D);
            _text = new UndoableProperty<string>(this, TextPropertyName, this._context, string.Empty);
            _textFontSize = new UndoableProperty<double>(this, TextFontSizePropertyName, this._context, 14D);
            _textAngle = new UndoableProperty<double>(this, TextAnglePropertyName, this._context, 0D);
            _textFontColor = new UndoableProperty<Color>(this, TextFontColorPropertyName, this._context, Colors.White);
            _textHorizontalAlignment = new UndoableProperty<CanvasHorizontalAlignment>(this, TextHorizontalAlignmentPropertyName, this._context, CanvasHorizontalAlignment.Left);
            _textVerticalAlignment = new UndoableProperty<CanvasVerticalAlignment>(this, TextVerticalAlignmentPropertyName, this._context, CanvasVerticalAlignment.Top);

            this.Row = BuildDBTables.GaugeTable.CreateRow();
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

            _changeDate = new UndoableProperty<DateTime>(this, ChangeDatePropertyName, this._context, this.Row.Field<DateTime>(ChangeDatePropertyName));
            _gaugeId = new UndoableProperty<Int64>(this, GaugeIdPropertyName, this._context, this.Row.Field<long>(GaugeIdPropertyName));
            _gaugeType = new UndoableProperty<GaugeTypeEnum>(this, GaugeTypePropertyName, this._context, this.Row.Field<GaugeTypeEnum>(GaugeTypePropertyName));
            _pageId = new UndoableProperty<Int64>(this, PageIdPropertyName, this._context, this.Row.Field<long>(PageIdPropertyName));

            _gaugeLeft = new UndoableProperty<double>(this, GaugeLeftPropertyName, this._context, this.Row.Field<double>(GaugeLeftPropertyName));
            _gaugeTop = new UndoableProperty<double>(this, GaugeTopPropertyName, this._context, this.Row.Field<double>(GaugeTopPropertyName));
            _gaugeHeight = new UndoableProperty<double>(this, GaugeHeightPropertyName, this._context, this.Row.Field<double>(GaugeHeightPropertyName));
            _gaugeWidth = new UndoableProperty<double>(this, GaugeWidthPropertyName, this._context, this.Row.Field<double>(GaugeWidthPropertyName));
            _sensorId = new UndoableProperty<Int64>(this, SensorIdPropertyName, this._context, this.Row.Field<long>(SensorIdPropertyName));

            _divisions = new UndoableProperty<int>(this, DivisionsPropertyName, this._context, this.Row.Field<int>(DivisionsPropertyName));
            _minorTicsPerMajorTic = new UndoableProperty<int>(this, MinorTicsPerMajorTicPropertyName, this._context, this.Row.Field<int>(MinorTicsPerMajorTicPropertyName));
            _mediumTicsPerMajorTic = new UndoableProperty<int>(this, MediumTicsPerMajorTicPropertyName, this._context, this.Row.Field<int>(MediumTicsPerMajorTicPropertyName));
            _resolution = new UndoableProperty<int>(this, ResolutionPropertyName, this._context, this.Row.Field<int>(ResolutionPropertyName));
            _gaugeOutlineVisibility = new UndoableProperty<Windows.UI.Xaml.Visibility>(this, GaugeOutlineVisibilityPropertyName, this._context, this.Row.Field<Windows.UI.Xaml.Visibility>(GaugeOutlineVisibilityPropertyName));
            _middleCircleDelta = new UndoableProperty<int>(this, MiddleCircleDeltaPropertyName, this._context, this.Row.Field<int>(MiddleCircleDeltaPropertyName));
            _innerCircleDelta = new UndoableProperty<int>(this, InnerCircleDeltaPropertyName, this._context, this.Row.Field<int>(InnerCircleDeltaPropertyName));
            _valueFontSize = new UndoableProperty<double>(this, ValueFontSizePropertyName, this._context, this.Row.Field<double>(ValueFontSizePropertyName));
            _unitsFontSize = new UndoableProperty<double>(this, UnitsFontSizePropertyName, this._context, this.Row.Field<double>(UnitsFontSizePropertyName));
            _majorTicLength = new UndoableProperty<double>(this, MajorTicLengthPropertyName, this._context, this.Row.Field<double>(MajorTicLengthPropertyName));
            _mediumTicLength = new UndoableProperty<double>(this, MediumTicLengthPropertyName, this._context, this.Row.Field<double>(MediumTicLengthPropertyName));
            _minorTicLength = new UndoableProperty<double>(this, MinorTicLengthPropertyName, this._context, this.Row.Field<double>(MinorTicLengthPropertyName));
            _text = new UndoableProperty<string>(this, TextPropertyName, this._context, this.Row.Field<string>(TextPropertyName));
            _textFontSize = new UndoableProperty<double>(this, TextFontSizePropertyName, this._context, this.Row.Field<double>(TextFontSizePropertyName));
            _textAngle = new UndoableProperty<double>(this, TextAnglePropertyName, this._context, this.Row.Field<double>(TextAnglePropertyName));
            _textFontColor = new UndoableProperty<Color>(this, TextFontColorPropertyName, this._context, this.Row.Field<Color>(TextFontColorPropertyName));
            _textHorizontalAlignment = new UndoableProperty<CanvasHorizontalAlignment>(this, TextHorizontalAlignmentPropertyName, this._context, this.Row.Field<CanvasHorizontalAlignment>(TextHorizontalAlignmentPropertyName));
            _textVerticalAlignment = new UndoableProperty<CanvasVerticalAlignment>(this, TextVerticalAlignmentPropertyName, this._context, this.Row.Field<CanvasVerticalAlignment>(TextVerticalAlignmentPropertyName));

            NotifyOfPropertyChangeAll();
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
                Row.SetField<int>(DivisionsPropertyName, value);
                RaisePropertyChanged(() => Divisions);
            }
        }

        public long GaugeId
        {
            get
            {
                if (null == Row) return -1;

                long value = Row.Field<long>("GaugeId");
                return value;
            }
        }

        public GaugeTypeEnum GaugeType
        {
            get { return _gaugeType.GetValue(); }
            set
            {
                _gaugeType.SetValue(value);
                Row.SetField<GaugeTypeEnum>(GaugeTypePropertyName, value);
                RaisePropertyChanged(() => GaugeType);
            }
        }


        public double GaugeHeight
        {
            get { return _gaugeHeight.GetValue(); }
            set
            {
                _gaugeHeight.SetValue(value);
                Row.SetField<double>(GaugeHeightPropertyName, value);
                RaisePropertyChanged(() => GaugeHeight);
            }
        }

        public double GaugeWidth
        {
            get { return _gaugeWidth.GetValue(); }
            set
            {
                _gaugeWidth.SetValue(value);
                Row.SetField<double>(GaugeWidthPropertyName, value);
                RaisePropertyChanged(() => GaugeWidth);
            }
        }

        public double GaugeLeft
        {
            get { return _gaugeLeft.GetValue(); }
            set
            {
                _gaugeLeft.SetValue(value);
                Row.SetField<double>(GaugeLeftPropertyName, value);
                RaisePropertyChanged(() => GaugeLeft);
            }
        }

        public Windows.UI.Xaml.Visibility GaugeOutlineVisibility
        {
            get { return _gaugeOutlineVisibility.GetValue(); }
            set
            {
                _gaugeOutlineVisibility.SetValue(value);
                Row.SetField<Windows.UI.Xaml.Visibility>(GaugeOutlineVisibilityPropertyName, value);
                RaisePropertyChanged(() => GaugeOutlineVisibility);
            }
        }

        public double GaugeTop
        {
            get { return _gaugeTop.GetValue(); }
            set
            {
                _gaugeTop.SetValue(value);
                Row.SetField<double>(GaugeTopPropertyName, value);
                RaisePropertyChanged(() => GaugeTop);
            }
        }

        public int InnerCircleDelta
        {
            get { return _innerCircleDelta.GetValue(); }
            set
            {
                _innerCircleDelta.SetValue(value);
                Row.SetField<int>(InnerCircleDeltaPropertyName, value);
                RaisePropertyChanged(() => InnerCircleDelta);
            }
        }

        public int MinorTicsPerMajorTic
        {
            get { return _minorTicsPerMajorTic.GetValue(); }
            set
            {
                _minorTicsPerMajorTic.SetValue(value);
                Row.SetField<int>(MinorTicsPerMajorTicPropertyName, value);
                RaisePropertyChanged(() => MinorTicsPerMajorTic);
            }
        }

        public int MediumTicsPerMajorTic
        {
            get { return _mediumTicsPerMajorTic.GetValue(); }
            set
            {
                _mediumTicsPerMajorTic.SetValue(value);
                Row.SetField<int>(MediumTicsPerMajorTicPropertyName, value);
                RaisePropertyChanged(() => MediumTicsPerMajorTic);
            }
        }

        public long PageId
        {
            get { return _pageId.GetValue(); }
            set
            {
                _pageId.SetValue(value);
                Row.SetField<long>(PageIdPropertyName, value);
                RaisePropertyChanged(() => PageId);
            }
        }

        public int Resolution
        {
            get { return _resolution.GetValue(); }
            set
            {
                _resolution.SetValue(value);
                Row.SetField<int>(ResolutionPropertyName, value);
                RaisePropertyChanged(() => Resolution);
            }
        }

        public int MiddleCircleDelta
        {
            get { return _middleCircleDelta.GetValue(); }
            set
            {
                _middleCircleDelta.SetValue(value);
                Row.SetField<int>(MiddleCircleDeltaPropertyName, value);
                RaisePropertyChanged(() => MiddleCircleDelta);
            }
        }

        public long SensorId
        {
            get { return _sensorId.GetValue(); }
            set
            {
                _sensorId.SetValue(value);
                Row.SetField<long>(SensorIdPropertyName, value);
                RaisePropertyChanged(() => SensorId);
            }
        }

        public string Text
        {
            get { return _text.GetValue(); }
            set
            {
                _text.SetValue(value);
                Row.SetField<string>(TextPropertyName, value);
                RaisePropertyChanged(() => Text);
            }
        }

        public double TextFontSize
        {
            get { return _textFontSize.GetValue(); }
            set
            {
                _textFontSize.SetValue(value);
                Row.SetField<double>(TextFontSizePropertyName, value);
                RaisePropertyChanged(() => TextFontSize);
            }
        }

        public double TextAngle
        {
            get { return _textAngle.GetValue(); }
            set
            {
                _textAngle.SetValue(value);
                Row.SetField<double>(TextAnglePropertyName, value);
                RaisePropertyChanged(() => TextAngle);
            }
        }

        public Color TextFontColor
        {
            get { return _textFontColor.GetValue(); }
            set
            {
                _textFontColor.SetValue(value);
                Row.SetField<Color>(TextFontColorPropertyName, value);
                RaisePropertyChanged(() => TextFontColor);
            }
        }

        public CanvasHorizontalAlignment TextHorizontalAlignment
        {
            get { return _textHorizontalAlignment.GetValue(); }
            set
            {
                _textHorizontalAlignment.SetValue(value);
                Row.SetField<CanvasHorizontalAlignment>(TextHorizontalAlignmentPropertyName, value);
                RaisePropertyChanged(() => TextHorizontalAlignment);
            }
        }

        public CanvasVerticalAlignment TextVerticalAlignment
        {
            get { return _textVerticalAlignment.GetValue(); }
            set
            {
                _textVerticalAlignment.SetValue(value);
                Row.SetField<CanvasVerticalAlignment>(TextVerticalAlignmentPropertyName, value);
                RaisePropertyChanged(() => TextVerticalAlignment);
            }
        }

        public double ValueFontSize
        {
            get { return _valueFontSize.GetValue(); }
            set
            {
                _valueFontSize.SetValue(value);
                Row.SetField<double>(ValueFontSizePropertyName, value);
                RaisePropertyChanged(() => ValueFontSize);
            }
        }

        public double UnitsFontSize
        {
            get { return _unitsFontSize.GetValue(); }
            set
            {
                _unitsFontSize.SetValue(value);
                Row.SetField<double>(UnitsFontSizePropertyName, value);
                RaisePropertyChanged(() => UnitsFontSize);
            }
        }

        public double MajorTicLength
        {
            get { return _majorTicLength.GetValue(); }
            set
            {
                _majorTicLength.SetValue(value);
                Row.SetField<double>(MajorTicLengthPropertyName, value);
                RaisePropertyChanged(() => MajorTicLength);
            }
        }

        public double MediumTicLength
        {
            get { return _mediumTicLength.GetValue(); }
            set
            {
                _mediumTicLength.SetValue(value);
                Row.SetField<double>(MediumTicLengthPropertyName, value);
                RaisePropertyChanged(() => MediumTicLength);
            }
        }

        public double MinorTicLength
        {
            get { return _minorTicLength.GetValue(); }
            set
            {
                _minorTicLength.SetValue(value);
                Row.SetField<double>(MinorTicLengthPropertyName, value);
                RaisePropertyChanged(() => MinorTicLength);
            }
        }


        async public Task BeginCommit()
        {
            if (this.PropertyBag.IsDirty) Row.SetField<string>("PropertyBag", this.PropertyBag.JsonSerialize());

            // Persist the row into the database
            if (GaugeId == -1)
            {
                BuildDBTables.GaugeTable.AddRow(Row);
                await BuildDBTables.GaugeTable.BeginCommitRow(
                    Row,
                    () =>
                    {
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
            else if (this.IsDirty)
            {
                await BuildDBTables.GaugePageTable.BeginCommitRow(
                    Row,
                    () =>
                    {
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }

        }
        async public Task BeginDelete()
        {
            await BuildDBTables.GaugeTable.BeginRemove(this.Row);
        }

        public void Rollback()
        {
            // Rollback all of the changed values.
            if ((this.Row.RowState == ItemRowState.Modified) || PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                this.LoadPropertyBag();
                NotifyOfPropertyChangeAll();
            }
        }

        public void NotifyOfPropertyChangeAll()
        {
            foreach (ItemColumn column in BuildDBTables.GaugePageTable.Columns)
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
                if (Row.RowState != ItemRowState.Unchanged) return true;    // No changes have been made
                if (_propertyBag == null) return false;                     // We do not have a property blob

                return PropertyBag.IsDirty;
            }
        }
        public PropertyBag PropertyBag
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

        private ItemRow Row { get; set; }
    }
}
