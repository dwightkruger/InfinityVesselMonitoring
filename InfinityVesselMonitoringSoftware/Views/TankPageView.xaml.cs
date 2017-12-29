//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityVesselMonitoringSoftware.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityVesselMonitoringSoftware.Views
{
    public sealed partial class TankPageView : UserControl
    {
        private bool _ignorePropertyChange;
        private ObservableCollection<IGaugeItem> _gaugeItemSelectedList = new ObservableCollection<IGaugeItem>();
        private List<Adorner> _adornerList = new List<Adorner>();


        public TankPageView()
        {
            this.InitializeComponent();
        }

        public TankPageViewModel ViewModel
        {
            get
            {
                return this.VM;
            }
        }

        #region public int Rows

        /// <summary>
        /// Gets or sets the number of rows that are in the grid.
        /// </summary>
        /// <returns>The number of rows that are in the grid. The default is 0.</returns>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                "Rows",
                typeof(int),
                typeof(TankPageView),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        #endregion

        #region public int Columns

        /// <summary>
        /// Gets or sets the number of columns that are in the grid.
        /// </summary>
        /// <returns>The number of columns that are in the grid. The default is 0.</returns>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                "Columns",
                typeof(int),
                typeof(TankPageView),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        private static void OnRowsColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TankPageView source = (TankPageView)d;
            int value = (int)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            if (value < 0)
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, (int)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Properties.Resources.GaugePageView_RowsColumnsChanged_InvalidValue",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        #endregion

        private void MainCanvas_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var ctrlKey = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            bool isCtrlKeyPressed = ctrlKey.HasFlag(CoreVirtualKeyStates.Down);

            switch (e.Key)
            {
                case VirtualKey.C:       // CTRL+C = Copy
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.CopyCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.V:       // CTRL+V = Paste
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.PasteCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.Z:       // CTRL+Z = Undo
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.UndoCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.Y:      // CTRL+Y = Redo
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.RedoCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}
