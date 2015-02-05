using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Lib.Primitives;

namespace Lib.Internals
{
    internal class ContinuousDraggingStrategy : IDraggingStrategy
    {
        #region "Fields"

        private double _min;
        private double _max;

        #endregion

        public void ComputeMinMax(SplitterGrip grip, out double min, out double max)
        {
            if (grip.Orientation == Orientation.Horizontal)
            {
                min = grip.LeftChild.MinSize;
                max = grip.LeftChild.ActualHeight + (grip.RightChild.ActualHeight - grip.RightChild.MinSize);
            }
            else
            {
                min = grip.LeftChild.MinSize;
                max = grip.LeftChild.ActualWidth + (grip.RightChild.ActualWidth - grip.RightChild.MinSize);
            }
        }

        public void OnDragStarted(SplitterGrip splitterGrip, DragStartedEventArgs args)
        {
            ComputeMinMax(splitterGrip, out _min, out _max);
        }

        public void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args)
        {
            var itemsControl = grip.ParentOfType<SplitterItemsControl>();
            if (itemsControl == null)
                return;

            var change = grip.Orientation == Orientation.Horizontal ? args.VerticalChange : args.HorizontalChange;


            var newWidth = grip.LeftChild.ActualWidth + change;
            CoerceOffset(ref newWidth);
            var actualChange = Math.Abs(newWidth - grip.LeftChild.ActualWidth);

            var diffUnit = SplitterItemsControl.GetUnitForSize(itemsControl, actualChange);
            try
            {
                itemsControl.DisallowPanelInvalidation = true;
                if (change < 0)
                {
                    grip.LeftChild.Size -= diffUnit;
                    grip.RightChild.Size += diffUnit;
                }
                else
                {
                    grip.LeftChild.Size += diffUnit;
                    grip.RightChild.Size -= diffUnit;
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

        public void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs args)
        {
            
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
    }
}