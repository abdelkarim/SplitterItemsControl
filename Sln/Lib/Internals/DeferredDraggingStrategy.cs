using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Lib.Primitives;

namespace Lib.Internals
{
    internal class DeferredDraggingStrategy : IDraggingStrategy
    {
        #region "Fields"

        private double _min;
        private double _max; 

        #endregion

        public void ComputeMinMax(SplitterGrip grip, out double min, out double max)
        {
            if (grip.Orientation == Orientation.Horizontal)
            {
                min = -(grip.LeftChild.ActualHeight - grip.LeftChild.MinLength);
                max = (grip.RightChild.ActualHeight - grip.RightChild.MinLength);
            }
            else
            {
                min = -(grip.LeftChild.ActualWidth - grip.LeftChild.MinLength);
                max = (grip.RightChild.ActualWidth - grip.RightChild.MinLength);
            }
        }

        public void OnDragStarted(SplitterGrip splitterGrip, DragStartedEventArgs args)
        {
            ComputeMinMax(splitterGrip, out _min, out _max);
            OnDragDelta(splitterGrip, new DragDeltaEventArgs(0, 0));
        }

        public void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args)
        {
            if (grip.Popup != null)
            {
                InitializePopup(grip);
                InvalidatePopupPlacement(grip, grip.Orientation == Orientation.Horizontal ? args.VerticalChange : args.HorizontalChange);
            }

            args.Handled = true;
        }

        private void InvalidatePopupPlacement(SplitterGrip grip, double change)
        {
            var popup = grip.Popup;
            var newOffset = GetGripOffset(grip);
            newOffset += change;
            CoerceOffset(ref newOffset);

            if (grip.Orientation == Orientation.Horizontal)
            {
                popup.VerticalOffset = newOffset;
                popup.HorizontalOffset = 0;
            }
            else
            {
                popup.HorizontalOffset = newOffset;
                popup.VerticalOffset = 0;
            }
        }

        private static double GetGripOffset(SplitterGrip grip)
        {
            return 0.0;
        }

        private void CoerceOffset(ref double offset)
        {
            //ensure the min
            if (offset < _min)
                offset = _min;

            // ensure the max
            if (offset > _max)
                offset = _max;
        }

        private static void InitializePopup(SplitterGrip grip)
        {
            var popup = grip.Popup;
            if (popup == null || popup.IsOpen)
                return;

            popup.Placement = PlacementMode.Relative;
            popup.PlacementTarget = grip;
            popup.IsOpen = true;
        }

        public void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs args)
        {
            var change = grip.Orientation == Orientation.Horizontal ? args.VerticalChange : args.HorizontalChange;
            if (grip.Popup != null)
                grip.Popup.IsOpen = false;

            OnDragCompleted(grip, change);
        }

        private void OnDragCompleted(SplitterGrip grip, double change)
        {
            /*
             * if the change is negative the pop up was dragged to the left(top),
             * otherwise it was dragged to the right(bottom)
             */
            var itemsControl = ItemsControl.ItemsControlFromItemContainer(grip.LeftChild) as SplitterItemsControl;
            if (itemsControl == null)
                return;

            var gripOffset = GetGripOffset(grip);
            var newOffset = gripOffset + change;
            CoerceOffset(ref newOffset);

            var actualChange = Math.Abs(newOffset - gripOffset);
            var diffUnit = SplitterItemsControl.GetUnitForSize(itemsControl, actualChange);

            try
            {
                // we defer updating the panel, until we're done.
                itemsControl.DisallowPanelInvalidation = true;
                if (change < 0)
                {
                    grip.LeftChild.Length = new ItemLength(grip.LeftChild.Length.Value - diffUnit, grip.LeftChild.Length.UnitType);
                    grip.RightChild.Length = new ItemLength(grip.RightChild.Length.Value + diffUnit, grip.RightChild.Length.UnitType);
                }
                else
                {
                    grip.LeftChild.Length = new ItemLength(grip.LeftChild.Length.Value + diffUnit, grip.LeftChild.Length.UnitType);
                    grip.RightChild.Length = new ItemLength(grip.RightChild.Length.Value - diffUnit, grip.RightChild.Length.UnitType);
                }
            }
            finally
            {
                itemsControl.DisallowPanelInvalidation = false;
            }

            // we invalidate manually
            var panel = grip.ParentOfType<SplitterItemsPanel>();
            if (panel != null)
                panel.InvalidateMeasure();
        }
    }
}