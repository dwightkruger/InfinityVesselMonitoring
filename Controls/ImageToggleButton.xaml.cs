//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class ImageToggleButton : Button
    {
        private static SolidColorBrush c_TransparentBrush = new SolidColorBrush(Colors.Transparent);
        public ImageToggleButton()
        {
            this.InitializeComponent();
        }

        public Image Image
        {
            get { return VesselSettingsImage; }
        }

        #region Source
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ImageSource),
            typeof(ImageToggleButton),
            new PropertyMetadata(null,
                                 new PropertyChangedCallback(OnSourcePropertyChanged)));

        public ImageSource Source
        {
            get { return (ImageSource)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        protected static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageToggleButton g = d as ImageToggleButton;
        }
        #endregion


        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ImageToggleButton),
            new PropertyMetadata(string.Empty,
                                 new PropertyChangedCallback(OnTextPropertyChanged)));

        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        protected static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageToggleButton g = d as ImageToggleButton;
        }
        #endregion

        #region IsSelected
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(Nullable<bool>),
            typeof(ImageToggleButton),
            new PropertyMetadata(false,
                                 new PropertyChangedCallback(OnIsSelectedPropertyChanged)));

        public Nullable<bool> IsSelected
        {
            get { return (Nullable<bool>)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageToggleButton g = d as ImageToggleButton;
            Nullable<bool> isSelected = (Nullable<bool>)e.NewValue;
            if (isSelected.HasValue && isSelected.Value)
            {
                g.Background = g.IsSelectedColor;
            }
            else
            {
                g.Background = c_TransparentBrush;
            }
        }
        #endregion

        #region IsSelectedColor
        public static readonly DependencyProperty IsSelectedColorProperty = DependencyProperty.Register(
            "IsSelectedColor",
            typeof(SolidColorBrush),
            typeof(ImageToggleButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Magenta),
                                 new PropertyChangedCallback(OnIsSelectedColorPropertyChanged)));

        public SolidColorBrush IsSelectedColor
        {
            get { return (SolidColorBrush)this.GetValue(IsSelectedColorProperty); }
            set { this.SetValue(IsSelectedColorProperty, value); }
        }

        protected static void OnIsSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageToggleButton g = d as ImageToggleButton;
            if (g.IsSelected.HasValue && g.IsSelected.Value)
            {
                g.Background = g.IsSelectedColor;
            }
            else
            {
                g.Background = c_TransparentBrush;
            }
        }
        #endregion

    }
}
