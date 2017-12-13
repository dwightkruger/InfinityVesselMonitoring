//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Interfaces;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace InfinityGroup.VesselMonitoring.Controls
{
    public class BaseGauge : UserControl, IGauge
    {
        IGaugeItem _gaugeItem = null;

        public BaseGauge()
        {
        }

        #region Divisions
        public static readonly DependencyProperty DivisionsProperty = DependencyProperty.Register(
            "Divisions",
            typeof(int),
            typeof(BaseGauge),
            new PropertyMetadata(7,
                                 new PropertyChangedCallback(OnDivisionsPropertyChanged)));

        public int Divisions
        {
            get { return (int)this.GetValue(DivisionsProperty); }
            set { this.SetValue(DivisionsProperty, value); }
        }

        protected static void OnDivisionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }

        #endregion

        #region GaugeArcBrush
        public static readonly DependencyProperty GaugeArcBrushProperty = DependencyProperty.Register(
            "GaugeArcBrush",
            typeof(Brush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.LightGray),
                                 new PropertyChangedCallback(OnGaugeArcBrushPropertyChanged)));

        public Brush GaugeArcBrush
        {
            get { return (Brush)this.GetValue(GaugeArcBrushProperty); }
            set { this.SetValue(GaugeArcBrushProperty, value); }
        }

        protected static void OnGaugeArcBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }

        #endregion

        #region GaugeLabelsBrush
        public static readonly DependencyProperty GaugeLabelsBrushProperty = DependencyProperty.Register(
            "GaugeLabelsBrush",
            typeof(Brush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.White),
                                 new PropertyChangedCallback(OnGaugeLabelsBrushPropertyChanged)));

        public Brush GaugeLabelsBrush
        {
            get { return (Brush)this.GetValue(GaugeLabelsBrushProperty); }
            set { this.SetValue(GaugeLabelsBrushProperty, value); }
        }

        protected static void OnGaugeLabelsBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }

        #endregion

        #region GaugeHeight
        public static readonly DependencyProperty GaugeHeightProperty = DependencyProperty.Register(
            "GaugeHeight", typeof(double), typeof(BaseGauge),
            new PropertyMetadata(300.0D,
            new PropertyChangedCallback(OnGaugeHeightPropertyChanged)));

        public double GaugeHeight
        {
            get { return (double)this.GetValue(GaugeHeightProperty); }
            set { this.SetValue(GaugeHeightProperty, value); }
        }

        protected static void OnGaugeHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region GaugeWidth
        public static readonly DependencyProperty GaugeWidthProperty = DependencyProperty.Register(
            "GaugeWidth", typeof(double), typeof(BaseGauge),
            new PropertyMetadata(200.0D,
            new PropertyChangedCallback(OnGaugeWidthPropertyChanged)));

        public double GaugeWidth
        {
            get { return (double)this.GetValue(GaugeWidthProperty); }
            set { this.SetValue(GaugeWidthProperty, value); }
        }

        protected static void OnGaugeWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region HighAlarmBrush
        public static readonly DependencyProperty HighAlarmBrushProperty = DependencyProperty.Register(
            "HighAlarmBrush",
            typeof(SolidColorBrush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Red),
                                 new PropertyChangedCallback(OnHighAlarmBrushPropertyChanged)));

        public SolidColorBrush HighAlarmBrush
        {
            get { return (SolidColorBrush)this.GetValue(HighAlarmBrushProperty); }
            set { this.SetValue(HighAlarmBrushProperty, value); }
        }

        protected static void OnHighAlarmBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshAlarmColors();
        }
        #endregion

        #region HighAlarmValue
        public static readonly DependencyProperty HighAlarmValueProperty = DependencyProperty.Register(
            "HighAlarmValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(95.0D,
                                new PropertyChangedCallback(OnHighAlarmValuePropertyChanged)));

        public double HighAlarmValue
        {
            get { return (double)this.GetValue(HighAlarmValueProperty); }
            set { this.SetValue(HighAlarmValueProperty, value); }
        }

        protected static void OnHighAlarmValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
            g.RefreshHighAlarmValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region HighWarningBrush
        public static readonly DependencyProperty HighWarningBrushProperty = DependencyProperty.Register(
            "HighWarningBrush",
            typeof(SolidColorBrush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Orange),
                                 new PropertyChangedCallback(OnHighWarningBrushPropertyChanged)));

        public SolidColorBrush HighWarningBrush
        {
            get { return (SolidColorBrush)this.GetValue(HighWarningBrushProperty); }
            set { this.SetValue(HighWarningBrushProperty, value); }
        }

        protected static void OnHighWarningBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshAlarmColors();
        }
        #endregion

        #region HighWarningValue
        public static readonly DependencyProperty HighWarningValueProperty = DependencyProperty.Register(
            "HighWarningValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(90.0D,
                                 new PropertyChangedCallback(OnHighWarningValuePropertyChanged)));

        public double HighWarningValue
        {
            get { return (double)this.GetValue(HighWarningValueProperty); }
            set { this.SetValue(HighWarningValueProperty, value); }
        }

        protected static void OnHighWarningValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
            g.RefreshHighWarningValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region InnerCircleDelta
        public static readonly DependencyProperty InnerCircleDeltaProperty = DependencyProperty.Register(
            "InnerCircleDelta",
            typeof(float),
            typeof(BaseGauge),
            new PropertyMetadata(30F,
                                 new PropertyChangedCallback(OnInnerCircleDeltaPropertyChanged)));

        public float InnerCircleDelta
        {
            get { return (float)this.GetValue(InnerCircleDeltaProperty); }
            set { this.SetValue(InnerCircleDeltaProperty, value); }
        }

        protected static void OnInnerCircleDeltaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region IsAlarmSounding
        public static readonly DependencyProperty IsAlarmSoundingProperty = DependencyProperty.Register(
            "IsAlarmSounding",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(true,
                                 new PropertyChangedCallback(OnIsAlarmSoundingPropertyChanged)));

        public bool IsAlarmSounding
        {
            get { return (bool)this.GetValue(IsAlarmSoundingProperty); }
            set { this.SetValue(IsAlarmSoundingProperty, value); }
        }

        protected static void OnIsAlarmSoundingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.SoundAlarm();
        }
        #endregion

        #region IsSensorEnabled
        public static readonly DependencyProperty IsSensorEnabledProperty = DependencyProperty.Register(
            "IsSensorEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsSensorEnabledPropertyChanged)));

        public bool IsSensorEnabled
        {
            get { return (bool)this.GetValue(IsSensorEnabledProperty); }
            set { this.SetValue(IsSensorEnabledProperty, value); }
        }

        protected static void OnIsSensorEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshIsSensorEnabled(e.OldValue, e.NewValue);
        }
        #endregion

        #region IsHighAlarmEnabled
        public static readonly DependencyProperty IsHighAlarmEnabledProperty = DependencyProperty.Register(
            "IsHighAlarmEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsHighAlarmEnabledPropertyChanged)));

        public bool IsHighAlarmEnabled
        {
            get { return (bool)this.GetValue(IsHighAlarmEnabledProperty); }
            set { this.SetValue(IsHighAlarmEnabledProperty, value); }
        }

        protected static void OnIsHighAlarmEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region IsHighWarningEnabled
        public static readonly DependencyProperty IsHighWarningEnabledProperty = DependencyProperty.Register(
            "IsHighWarningEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsHighWarningEnabledPropertyChanged)));

        public bool IsHighWarningEnabled
        {
            get { return (bool)this.GetValue(IsHighWarningEnabledProperty); }
            set { this.SetValue(IsHighWarningEnabledProperty, value); }
        }

        protected static void OnIsHighWarningEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region IsLowAlarmEnabled
        public static readonly DependencyProperty IsLowAlarmEnabledProperty = DependencyProperty.Register(
            "IsLowAlarmEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsLowAlarmEnabledPropertyChanged)));

        public bool IsLowAlarmEnabled
        {
            get { return (bool)this.GetValue(IsLowAlarmEnabledProperty); }
            set { this.SetValue(IsLowAlarmEnabledProperty, value); }
        }

        protected static void OnIsLowAlarmEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region IsLowWarningEnabled
        public static readonly DependencyProperty IsLowWarningEnabledProperty = DependencyProperty.Register(
            "IsLowWarningEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsLowWarningEnabledPropertyChanged)));

        public bool IsLowWarningEnabled
        {
            get { return (bool)this.GetValue(IsLowWarningEnabledProperty); }
            set { this.SetValue(IsLowWarningEnabledProperty, value); }
        }

        protected static void OnIsLowWarningEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region IsOnline
        public static readonly DependencyProperty IsOnlineProperty = DependencyProperty.Register(
            "IsOnline",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(
                false,
                new PropertyChangedCallback(OnIsOnlinePropertyChanged)));

        public bool IsOnline
        {
            get { return (bool)this.GetValue(IsOnlineProperty); }
            set { this.SetValue(IsOnlineProperty, value); }
        }

        protected static void OnIsOnlinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            if (Convert.ToBoolean(e.NewValue))
            {
                g.RefreshValue(g.Value, g.Value);
                g.RefreshTimeSpanValue(TimeSpan.FromSeconds(0), g.TimeSpanValue);
                g.RefreshAlarmColors();
            }
            else
            {
                g.RefreshValue(0.0, double.NaN);
                g.RefreshTimeSpanValue(TimeSpan.FromSeconds(0), g.TimeSpanValue);
                g.RefreshAlarmColors();
            }
        }
        #endregion

        #region GaugeOutlineVisibility
        public static readonly DependencyProperty GaugeOutlineVisibilityProperty = DependencyProperty.Register(
            "GaugeOutlineVisibility",
            typeof(Visibility),
            typeof(BaseGauge),
            new PropertyMetadata(Visibility.Visible,
                                 new PropertyChangedCallback(OnGaugeOutlineVisibilityPropertyChanged)));

        public Visibility GaugeOutlineVisibility
        {
            get { return (Visibility)this.GetValue(GaugeOutlineVisibilityProperty); }
            set { this.SetValue(GaugeOutlineVisibilityProperty, value); }
        }

        protected static void OnGaugeOutlineVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshGaugeOutlineVisibility(e.OldValue, e.NewValue);
        }
        #endregion

        #region IsNominalValueEnabled
        public static readonly DependencyProperty IsNominalValueEnabledProperty = DependencyProperty.Register(
            "IsNominalValueEnabled",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(true,
                                 new PropertyChangedCallback(OnIsNominalValueEnabledPropertyChanged)));

        public bool IsNominalValueEnabled
        {
            get { return (bool)this.GetValue(IsNominalValueEnabledProperty); }
            set { this.SetValue(IsNominalValueEnabledProperty, value); }
        }

        protected static void OnIsNominalValueEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region IsSwitchOn
        public static readonly DependencyProperty IsSwitchOnProperty = DependencyProperty.Register(
            "IsSwitchOn",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(true,
                                 new PropertyChangedCallback(OnIsSwitchOnPropertyChanged)));

        public bool IsSwitchOn
        {
            get { return (bool)this.GetValue(IsSwitchOnProperty); }
            set { this.SetValue(IsSwitchOnProperty, value); }
        }

        protected static void OnIsSwitchOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.SwitchOn((bool)e.OldValue, (bool)e.NewValue);
        }
        #endregion

        #region IsSwitchCounterVisible
        public static readonly DependencyProperty IsSwitchCounterVisibleProperty = DependencyProperty.Register(
            "IsSwitchCounterVisible",
            typeof(bool),
            typeof(BaseGauge),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsSwitchCounterVisiblePropertyChanged)));

        public bool IsSwitchCounterVisible
        {
            get { return (bool)this.GetValue(IsSwitchCounterVisibleProperty); }
            set { this.SetValue(IsSwitchCounterVisibleProperty, value); }
        }

        protected static void OnIsSwitchCounterVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region LowAlarmBrush
        public static readonly DependencyProperty LowAlarmBrushProperty = DependencyProperty.Register(
            "LowAlarmBrush",
            typeof(SolidColorBrush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Red),
            new PropertyChangedCallback(OnLowAlarmBrushPropertyChanged)));

        public SolidColorBrush LowAlarmBrush
        {
            get { return (SolidColorBrush)this.GetValue(LowAlarmBrushProperty); }
            set { this.SetValue(LowAlarmBrushProperty, value); }
        }

        protected static void OnLowAlarmBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshAlarmColors();
        }
        #endregion

        #region LowAlarmValue
        public static readonly DependencyProperty LowAlarmValueProperty = DependencyProperty.Register(
            "LowAlarmValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(5.0D,
            new PropertyChangedCallback(OnLowAlarmValuePropertyChanged)));

        public double LowAlarmValue
        {

            get { return (double)this.GetValue(LowAlarmValueProperty); }
            set { this.SetValue(LowAlarmValueProperty, value); }
        }

        protected static void OnLowAlarmValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
            g.RefreshLowAlarmValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region LowWarningBrush
        public static readonly DependencyProperty LowWarningBrushProperty = DependencyProperty.Register(
            "LowWarningBrush",
            typeof(SolidColorBrush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Orange),
            new PropertyChangedCallback(OnLowWarningBrushPropertyChanged)));

        public SolidColorBrush LowWarningBrush
        {
            get { return (SolidColorBrush)this.GetValue(LowWarningBrushProperty); }
            set { this.SetValue(LowWarningBrushProperty, value); }
        }

        protected static void OnLowWarningBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshAlarmColors();
        }
        #endregion

        #region LowWarningValue
        public static readonly DependencyProperty LowWarningValueProperty = DependencyProperty.Register(
            "LowWarningValue",
            typeof(double), typeof(BaseGauge),
            new PropertyMetadata(10.0D,
                                  new PropertyChangedCallback(OnLowWarningValuePropertyChanged)));

        public double LowWarningValue
        {
            get { return (double)this.GetValue(LowWarningValueProperty); }
            set { this.SetValue(LowWarningValueProperty, value); }
        }

        protected static void OnLowWarningValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
            g.RefreshLowWarningValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region MajorTicLength
        public static readonly DependencyProperty MajorTicLengthProperty = DependencyProperty.Register(
            "MajorTicLength",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(18D,
                                 new PropertyChangedCallback(OnMajorTicLengthPropertyChanged)));

        public double MajorTicLength
        {
            get { return (double)this.GetValue(MajorTicLengthProperty); }
            set { this.SetValue(MajorTicLengthProperty, value); }
        }

        protected static void OnMajorTicLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(100.0D,
                                 new PropertyChangedCallback(OnMaxValuePropertyChanged)));

        public double MaxValue
        {
            get { return (double)this.GetValue(MaxValueProperty); }
            set { this.SetValue(MaxValueProperty, value); }
        }

        protected static void OnMaxValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshValue(g.Value, g.Value);
            g.RefreshMaxValue(e.OldValue, e.NewValue);
            g.RefreshAlarmColors();
        }
        #endregion

        #region MediumTicLength
        public static readonly DependencyProperty MediumTicLengthProperty = DependencyProperty.Register(
            "MediumTicLength",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(12D,
                                 new PropertyChangedCallback(OnMediumTicLengthPropertyChanged)));

        public double MediumTicLength
        {
            get { return (double)this.GetValue(MediumTicLengthProperty); }
            set { this.SetValue(MediumTicLengthProperty, value); }
        }

        protected static void OnMediumTicLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MediumTicsPerMajorTic
        public static readonly DependencyProperty MediumTicsPerMajorTicProperty = DependencyProperty.Register(
            "MediumTicsPerMajorTic",
            typeof(int),
            typeof(BaseGauge),
            new PropertyMetadata(3,
                                 new PropertyChangedCallback(OnMediumTicsPerMajorTicPropertyChanged)));

        public int MediumTicsPerMajorTic
        {
            get { return (int)this.GetValue(MediumTicsPerMajorTicProperty); }
            set { this.SetValue(MediumTicsPerMajorTicProperty, value); }
        }

        protected static void OnMediumTicsPerMajorTicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MiddleCircleDelta
        public static readonly DependencyProperty MiddleCircleDeltaProperty = DependencyProperty.Register(
            "MiddleCircleDelta",
            typeof(float),
            typeof(BaseGauge),
            new PropertyMetadata(70F,
                                 new PropertyChangedCallback(OnMiddleCircleDeltaPropertyChanged)));

        public float MiddleCircleDelta
        {
            get { return (float)this.GetValue(MiddleCircleDeltaProperty); }
            set { this.SetValue(MiddleCircleDeltaProperty, value); }
        }

        protected static void OnMiddleCircleDeltaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MinorTicLength
        public static readonly DependencyProperty MinorTicLengthProperty = DependencyProperty.Register(
            "MinorTicLength",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(6D,
                                 new PropertyChangedCallback(OnMinorTicLengthPropertyChanged)));

        public double MinorTicLength
        {
            get { return (double)this.GetValue(MinorTicLengthProperty); }
            set { this.SetValue(MinorTicLengthProperty, value); }
        }

        protected static void OnMinorTicLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MinorTicsPerMajorTic
        public static readonly DependencyProperty MinorTicsPerMajorTicProperty = DependencyProperty.Register(
            "MinorTicsPerMajorTic",
            typeof(int),
            typeof(BaseGauge),
            new PropertyMetadata(6,
                                 new PropertyChangedCallback(OnMinorTicsPerMajorTicPropertyChanged)));

        public int MinorTicsPerMajorTic
        {
            get { return (int)this.GetValue(MinorTicsPerMajorTicProperty); }
            set { this.SetValue(MinorTicsPerMajorTicProperty, value); }
        }

        protected static void OnMinorTicsPerMajorTicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(0.0D,
                                 new PropertyChangedCallback(OnMinValuePropertyChanged)));

        public double MinValue
        {
            get { return (double)this.GetValue(MinValueProperty); }
            set { this.SetValue(MinValueProperty, value); }
        }

        protected static void OnMinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshValue(g.Value, g.Value);
            g.RefreshMinValue(e.OldValue, e.NewValue);
            g.RefreshAlarmColors();
        }
        #endregion

        #region NominalValue
        public static readonly DependencyProperty NominalValueProperty = DependencyProperty.Register(
            "NominalValue",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(0.0D,
                new PropertyChangedCallback(OnNominalValuePropertyChanged)));

        public double NominalValue
        {
            get { return (double)this.GetValue(NominalValueProperty); }
            set { this.SetValue(NominalValueProperty, value); }
        }

        protected static void OnNominalValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshNominalValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region Points
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points",
            typeof(ObservableCollection<Point>),
            typeof(BaseGauge),
            new PropertyMetadata(new ObservableCollection<Point>() { new Point(0, 0), new Point(0, 0) },
                                 new PropertyChangedCallback(OnPointsPropertyChanged)));

        public ObservableCollection<Point> Points
        {
            get { return (ObservableCollection<Point>)this.GetValue(PointsProperty); }
            set { this.SetValue(PointsProperty, value); }
        }

        protected static void OnPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region Resolution
        public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register(
            "Resolution",
            typeof(int),
            typeof(BaseGauge),
            new PropertyMetadata(1,
                new PropertyChangedCallback(OnResolutionPropertyChanged)));

        public int Resolution
        {
            get { return (int)this.GetValue(ResolutionProperty); }
            set { this.SetValue(ResolutionProperty, value); }
        }

        protected static void OnResolutionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshValue(g.Value, g.Value);
        }
        #endregion

        #region GaugeItem
        public IGaugeItem GaugeItem
        {
            get { return _gaugeItem; }
            set
            {
                Binding divisionsBinding = new Binding();
                divisionsBinding.Source = value;
                divisionsBinding.Path = new PropertyPath("Divisions");
                this.SetBinding(DivisionsProperty, divisionsBinding);

                Binding gaugeHeightBinding = new Binding();
                gaugeHeightBinding.Source = value;
                gaugeHeightBinding.Path = new PropertyPath("GaugeHeight");
                this.SetBinding(GaugeHeightProperty, gaugeHeightBinding);

                Binding gaugeLeftBinding = new Binding();
                gaugeLeftBinding.Source = value;
                gaugeLeftBinding.Path = new PropertyPath("GaugeLeft");
                this.SetBinding(LeftProperty, gaugeLeftBinding);

                Binding gaugeTopBinding = new Binding();
                gaugeTopBinding.Source = value;
                gaugeTopBinding.Path = new PropertyPath("GaugeTop");
                this.SetBinding(TopProperty, gaugeTopBinding);

                Binding gaugeWidthBinding = new Binding();
                gaugeWidthBinding.Source = value;
                gaugeWidthBinding.Path = new PropertyPath("GaugeWidth");
                this.SetBinding(GaugeWidthProperty, gaugeWidthBinding);

                Binding innerCircleDeltaBinding = new Binding();
                innerCircleDeltaBinding.Source = value;
                innerCircleDeltaBinding.Path = new PropertyPath("InnerCircleDelta");
                this.SetBinding(InnerCircleDeltaProperty, innerCircleDeltaBinding);

                Binding majorTicLengthBinding = new Binding();
                majorTicLengthBinding.Source = value;
                majorTicLengthBinding.Path = new PropertyPath("MajorTicLength");
                this.SetBinding(MajorTicLengthProperty, majorTicLengthBinding);

                Binding mediumTicLengthBinding = new Binding();
                mediumTicLengthBinding.Source = value;
                mediumTicLengthBinding.Path = new PropertyPath("MediumTicLength");
                this.SetBinding(MediumTicLengthProperty, mediumTicLengthBinding);

                Binding minorTicLengthBinding = new Binding();
                minorTicLengthBinding.Source = value;
                minorTicLengthBinding.Path = new PropertyPath("MinorTicLength");
                this.SetBinding(MinorTicLengthProperty, minorTicLengthBinding);

                Binding minorTicsPerMajorTicBinding = new Binding();
                minorTicsPerMajorTicBinding.Source = value;
                minorTicsPerMajorTicBinding.Path = new PropertyPath("MinorTicsPerMajorTic");
                this.SetBinding(MinorTicsPerMajorTicProperty, minorTicsPerMajorTicBinding);

                Binding mediumTicsPerMajorTicBinding = new Binding();
                mediumTicsPerMajorTicBinding.Source = value;
                mediumTicsPerMajorTicBinding.Path = new PropertyPath("MediumTicsPerMajorTic");
                this.SetBinding(MediumTicsPerMajorTicProperty, mediumTicsPerMajorTicBinding);

                Binding middleCircleDeltaBinding = new Binding();
                middleCircleDeltaBinding.Source = value;
                middleCircleDeltaBinding.Path = new PropertyPath("MiddleCircleDelta");
                this.SetBinding(MiddleCircleDeltaProperty, middleCircleDeltaBinding);

                Binding resolutionBinding = new Binding();
                resolutionBinding.Source = value;
                resolutionBinding.Path = new PropertyPath("Resolution");
                this.SetBinding(ResolutionProperty, resolutionBinding);

                Binding unitsFontSizeBinding = new Binding();
                unitsFontSizeBinding.Source = value;
                unitsFontSizeBinding.Path = new PropertyPath("UnitsFontSize");
                this.SetBinding(UnitsFontSizeProperty, unitsFontSizeBinding);

                Binding textBinding = new Binding();
                textBinding.Source = value;
                textBinding.Path = new PropertyPath("Text");
                this.SetBinding(TextProperty, textBinding);

                Binding textFontSizeBinding = new Binding();
                textFontSizeBinding.Source = value;
                textFontSizeBinding.Path = new PropertyPath("TextFontSize");
                this.SetBinding(TextFontSizeProperty, textFontSizeBinding);

                Binding textFontColorBinding = new Binding();
                textFontColorBinding.Source = value;
                textFontColorBinding.Path = new PropertyPath("TextFontColor");
                this.SetBinding(TextFontColorProperty, textFontColorBinding);

                Binding valueFontSizeBinding = new Binding();
                valueFontSizeBinding.Source = value;
                valueFontSizeBinding.Path = new PropertyPath("ValueFontSize");
                this.SetBinding(ValueFontSizeProperty, valueFontSizeBinding);

                _gaugeItem = value;
            }
        }
        #endregion GaugeItem

        #region SensorItem
        public ISensorItem SensorItem
        {
            set
            {
                Binding isOnlineBinding = new Binding();
                isOnlineBinding.Source = value;
                isOnlineBinding.Path = new PropertyPath("IsOnline");
                this.SetBinding(IsOnlineProperty, isOnlineBinding);

                Binding maxValueBinding = new Binding();
                maxValueBinding.Source = value;
                maxValueBinding.Path = new PropertyPath("MaxValue");
                this.SetBinding(MaxValueProperty, maxValueBinding);

                Binding minValueBinding = new Binding();
                minValueBinding.Source = value;
                minValueBinding.Path = new PropertyPath("MinValue");
                this.SetBinding(MinValueProperty, minValueBinding);

                Binding nominalValueBinding = new Binding();
                nominalValueBinding.Source = value;
                nominalValueBinding.Path = new PropertyPath("NominalValue");
                this.SetBinding(NominalValueProperty, nominalValueBinding);

                Binding sensorValueBinding = new Binding();
                sensorValueBinding.Source = value;
                sensorValueBinding.Path = new PropertyPath("SensorValue");
                this.SetBinding(ValueProperty, sensorValueBinding);
            }
        }
        #endregion SensorItem

        #region SwitchCounter
        public static readonly DependencyProperty SwitchCounterProperty = DependencyProperty.Register(
            "SwitchCounter",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(0.0D,
            new PropertyChangedCallback(OnSwitchCounterPropertyChanged)));

        public double SwitchCounter
        {
            get { return (double)this.GetValue(SwitchCounterProperty); }
            set { this.SetValue(SwitchCounterProperty, value); }
        }

        protected static void OnSwitchCounterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;

            g.RefreshSwitchCounter(e.OldValue, e.NewValue);
        }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "TextValue",
            typeof(string),
            typeof(BaseGauge),
            new PropertyMetadata(string.Empty,
                new PropertyChangedCallback(OnTextPropertyChanged)));

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        protected static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TextAngle
        public static readonly DependencyProperty TextAngleProperty = DependencyProperty.Register(
            "TextAngle",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(0D,
                new PropertyChangedCallback(OnTextAnglePropertyChanged)));

        public double TextAngle
        {
            get { return (double)this.GetValue(TextAngleProperty); }
            set { this.SetValue(TextAngleProperty, value); }
        }

        protected static void OnTextAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TextFontColor
        public static readonly DependencyProperty TextFontColorProperty = DependencyProperty.Register(
            "TextFontColor",
            typeof(Color),
            typeof(BaseGauge),
            new PropertyMetadata(Colors.White,
                new PropertyChangedCallback(OnTextFontColorPropertyChanged)));

        public Color TextFontColor
        {
            get { return (Color)this.GetValue(TextFontColorProperty); }
            set { this.SetValue(TextFontColorProperty, value); }
        }

        protected static void OnTextFontColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TextFontSize
        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register(
            "TextFontSize",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(12D,
                new PropertyChangedCallback(OnTextFontSizePropertyChanged)));

        public double TextFontSize
        {
            get { return (double)this.GetValue(TextFontSizeProperty); }
            set { this.SetValue(TextFontSizeProperty, value); }
        }

        protected static void OnTextFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TextHorizontalAlignment
        public static readonly DependencyProperty TextHorizontalAlignmentProperty = DependencyProperty.Register(
            "TextHorizontalAlignment",
            typeof(CanvasHorizontalAlignment),
            typeof(BaseGauge),
            new PropertyMetadata(CanvasHorizontalAlignment.Left,
                new PropertyChangedCallback(OnTextHorizontalAlignmentPropertyChanged)));

        public CanvasHorizontalAlignment TextHorizontalAlignment
        {
            get { return (CanvasHorizontalAlignment)this.GetValue(TextHorizontalAlignmentProperty); }
            set { this.SetValue(TextHorizontalAlignmentProperty, value); }
        }

        protected static void OnTextHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TextVerticalAlignment
        public static readonly DependencyProperty TextVerticalAlignmentProperty = DependencyProperty.Register(
            "TextVerticalAlignment",
            typeof(CanvasVerticalAlignment),
            typeof(BaseGauge),
            new PropertyMetadata(CanvasVerticalAlignment.Top,
                new PropertyChangedCallback(OnTextVerticalAlignmentPropertyChanged)));

        public CanvasVerticalAlignment TextVerticalAlignment
        {
            get { return (CanvasVerticalAlignment)this.GetValue(TextHorizontalAlignmentProperty); }
            set { this.SetValue(TextVerticalAlignmentProperty, value); }
        }

        protected static void OnTextVerticalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TimeSpanValue
        public static readonly DependencyProperty TimeSpanValueProperty = DependencyProperty.Register(
            "TimeSpanValue",
            typeof(TimeSpan),
            typeof(BaseGauge),
            new PropertyMetadata(TimeSpan.FromSeconds(0),
                new PropertyChangedCallback(OnTimeSpanValuePropertyChanged)));

        public TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)this.GetValue(TimeSpanValueProperty); }
            set { this.SetValue(TimeSpanValueProperty, value); }
        }

        protected static void OnTimeSpanValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshTimeSpanValue(e.OldValue, e.NewValue);
        }
        #endregion

        #region Title
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(BaseGauge),
            new PropertyMetadata("Your Gauge Name",
                                  new PropertyChangedCallback(OnTitlePropertyChanged)));

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        protected static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region TitleBrush
        public static readonly DependencyProperty TitleBrushProperty = DependencyProperty.Register(
            "TitleBrush",
            typeof(Brush),
            typeof(BaseGauge),
            new PropertyMetadata(
                new SolidColorBrush(Windows.UI.Colors.LightSkyBlue),
                new PropertyChangedCallback(OnTitleBrushPropertyChanged)));

        public Brush TitleBrush
        {
            get { return (Brush)this.GetValue(TitleBrushProperty); }
            set { this.SetValue(TitleBrushProperty, value); }
        }

        protected static void OnTitleBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region TitleFontSize
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(
            "TitleFontSize",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                20.0D,
                new PropertyChangedCallback(OnTitleFontSizePropertyChanged)));

        public double TitleFontSize
        {
            get { return (double)this.GetValue(TitleFontSizeProperty); }
            set { this.SetValue(TitleFontSizeProperty, value); }
        }

        protected static void OnTitleFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region Units
        public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register(
            "Units",
            typeof(string),
            typeof(BaseGauge),
            new PropertyMetadata(
                "gal/hr",
                new PropertyChangedCallback(OnUnitsPropertyChanged)));

        public string Units
        {
            get { return (string)this.GetValue(UnitsProperty); }
            set { this.SetValue(UnitsProperty, value); }
        }

        protected static void OnUnitsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                0.0D,
                new PropertyChangedCallback(OnValuePropertyChanged)));

        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        protected static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshValue(e.OldValue, e.NewValue);
            g.RefreshAlarmColors();
        }
        #endregion

        #region ValueBrush
        public static readonly DependencyProperty ValueBrushProperty = DependencyProperty.Register(
            "ValueBrush",
            typeof(Brush),
            typeof(BaseGauge),
            new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.White),
                new PropertyChangedCallback(OnValueBrushPropertyChanged)));

        public Brush ValueBrush
        {
            get { return (Brush)this.GetValue(ValueBrushProperty); }
            set { this.SetValue(ValueBrushProperty, value); }
        }

        protected static void OnValueBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshAlarmColors();
        }
        #endregion

        #region ValueFontSize
        public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register(
            "ValueFontSize",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                53.0,
                new PropertyChangedCallback(OnValueFontSizePropertyChanged)));

        public double ValueFontSize
        {
            get { return (double)this.GetValue(ValueFontSizeProperty); }
            set { this.SetValue(ValueFontSizeProperty, value); }
        }

        protected static void OnValueFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshValueFontSize(e.OldValue, e.NewValue);
        }
        #endregion

        #region UnitsFontSize
        public static readonly DependencyProperty UnitsFontSizeProperty = DependencyProperty.Register(
            "UnitsFontSize",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                27.0,
                new PropertyChangedCallback(OnUnitsFontSizePropertyChanged)));

        public double UnitsFontSize
        {
            get { return (double)this.GetValue(UnitsFontSizeProperty); }
            set { this.SetValue(UnitsFontSizeProperty, value); }
        }

        protected static void OnUnitsFontSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = d as BaseGauge;
            g.RefreshUnitsFontSize(e.OldValue, e.NewValue);
        }
        #endregion

        #region Left
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                0.0,
                new PropertyChangedCallback(OnLeftPropertyChanged)));

        public double Left
        {
            get { return (double)this.GetValue(LeftProperty); }
            set { this.SetValue(LeftProperty, value); }
        }

        protected static void OnLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = (BaseGauge)d;
            g.RefreshLeft(e.OldValue, e.NewValue);
        }
        #endregion

        #region Top
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top",
            typeof(double),
            typeof(BaseGauge),
            new PropertyMetadata(
                0.0,
                new PropertyChangedCallback(OnTopPropertyChanged)));

        public double Top
        {
            get { return (double)this.GetValue(TopProperty); }
            set { this.SetValue(TopProperty, value); }
        }

        protected static void OnTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseGauge g = (BaseGauge)d;
            g.RefreshTop(e.OldValue, e.NewValue);
        }
        #endregion

        protected double PercentFullValue(double value)
        {
            if (double.IsNaN(value)) return 0;
            if (double.IsNaN(MinValue)) return 0;
            if (double.IsNaN(MaxValue)) return 0;
            if (Math.Abs(MaxValue - MinValue) <= double.Epsilon) return 0;

            return Math.Max(0.0, (value - MinValue) / (MaxValue - MinValue));
        }

        protected double PercentEmptyValue(double value)
        {
            return 1 - PercentFullValue(value);
        }

        public double PercentFull
        {
            get
            {
                if (double.IsNaN(Value)) return 0;
                if (double.IsNaN(MinValue)) return 0;
                if (double.IsNaN(MaxValue)) return 0;

                return (Value - MinValue) / (MaxValue - MinValue);
            }
        }

        public double PercentEmpty
        {
            get { return 1.0 - PercentFull; }
        }

        virtual protected void RefreshAlarmColors()
        {
        }

        virtual protected void RefreshValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshValueFontSize(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshTimeSpanValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshMaxValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshMinValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshHighAlarmValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshNominalValue(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshSwitchCounter(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshGaugeOutlineVisibility(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshUnitFontSize(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshLeft(object oldValue, object newValue)
        {
        }
        virtual protected void RefreshTop(object oldValue, object newValue)
        {
        }
        #region Gauge Display Percent Properties

        public double HighAlarmStartPercent
        {
            get
            {
                if (!IsHighAlarmEnabled) return 0;
                if (Math.Abs(MaxValue - MinValue) <= double.Epsilon) return 0;

                double value = (HighAlarmValue - MinValue) / (MaxValue - MinValue);
                value = Math.Max(value, 0);
                value = Math.Min(value, 1);

                return value;
            }
        }

        public double HighAlarmEndPercent
        {
            get
            {
                if (!IsHighAlarmEnabled) return 0;
                return 1;
            }
        }

        public double HighWarningStartPercent
        {
            get
            {
                if (!IsHighWarningEnabled) return 0;
                if (Math.Abs(MaxValue - MinValue) <= double.Epsilon) return 0;

                double value = (HighWarningValue - MinValue) / (MaxValue - MinValue);
                value = Math.Max(value, 0);
                value = Math.Min(value, 1);

                return value;
            }
        }

        public double HighWarningEndPercent
        {
            get
            {
                if (!IsHighWarningEnabled) return 0;

                return 1;
            }
        }

        public double LowAlarmStartPercent
        {
            get
            {
                return 0;
            }
        }

        public double LowAlarmEndPercent
        {
            get
            {
                if (!IsLowAlarmEnabled) return 0;
                if (Math.Abs(MaxValue - MinValue) <= double.Epsilon) return 0;

                double value = (LowAlarmValue - MinValue) / (MaxValue - MinValue);
                value = Math.Max(value, 0);
                value = Math.Min(value, 1);

                // Don't allow the angles to be reversed.  Start must be < End
                value = Math.Max(value, LowAlarmStartPercent);
                return value;
            }
        }

        public double LowWarningStartPercent
        {
            get
            {
                if (!IsLowWarningEnabled) return 0;

                return 0;
            }
        }

        public double LowWarningEndPercent
        {
            get
            {
                if (!IsLowWarningEnabled) return 0;
                if (Math.Abs(MaxValue - MinValue) <= double.Epsilon) return 0;

                double value = (LowWarningValue - MinValue) / (MaxValue - MinValue);
                value = Math.Max(value, 0);
                value = Math.Min(value, 1);

                // Don't allow the angles to be reversed.  Start must be < End
                value = Math.Max(value, LowWarningStartPercent);

                return value;

            }
        }

        #endregion

        virtual protected void SoundAlarm()
        {
        }

        virtual protected void SwitchOn(bool oldValue, bool newValue)
        {
        }

        virtual protected void RefreshIsSensorEnabled(object oldValue, object newValue)
        {
        }

        virtual protected void RefreshUnitsFontSize(object oldValue, object newValue)
        {
        }
    }
}
