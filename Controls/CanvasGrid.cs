using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InfinityGroup.VesselMonitoring.Controls
{
    public class CanvasGrid : Canvas
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignorePropertyChange;
        List<GridChild> _gridChildren = new List<GridChild>();

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
                typeof(CanvasGrid),
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
                typeof(CanvasGrid),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        #endregion

        /// <summary>
        /// Validity check on row or column. For now, just check that it is positive. This code could be much simplified.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRowsColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CanvasGrid source = (CanvasGrid)d;
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
                    "Properties.Resources.CanvasGrid_RowsColumnsChanged_InvalidValue",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        public void AddChildUIElement(UIElement child, int row, int column)
        {
            // Determine the top and left location of the child based on its row and column.
            this.Children.Add(child);
            _gridChildren.Add(new GridChild(child, row, column));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size newSize = base.MeasureOverride(constraint);

            // Position the children
            foreach (GridChild child in _gridChildren)
            {
                ((BaseGauge)child.Child).Left = child.Column * (constraint.Width / this.Columns);
                ((BaseGauge)child.Child).Top  = child.Row * (constraint.Height / this.Rows);
            }

            return newSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }

    public class GridChild
    {
        public GridChild(UIElement child, int row, int column)
        {
            this.Child = child;
            this.Row = row;
            this.Column = column;
        }

        public UIElement Child { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
