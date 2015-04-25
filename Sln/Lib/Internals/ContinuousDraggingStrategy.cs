using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Lib.Primitives;

namespace Lib.Internals
{
    internal class ContinuousDraggingStrategy : IDraggingStrategy
    {
        #region "Fields"

        private double _min;
        private double _max;

        private ItemLength _leftChildLength;
        private ItemLength _rightChildLength;

        #endregion

        public void ComputeMinMax(SplitterGrip grip, out double min, out double max)
        {
            if (grip.Orientation == Orientation.Horizontal)
            {
                min = grip.LeftChild.MinLength;
                max = grip.LeftChild.ActualHeight + (grip.RightChild.ActualHeight - grip.RightChild.MinLength);
            }
            else
            {
                min = grip.LeftChild.MinLength;
                max = grip.LeftChild.ActualWidth + (grip.RightChild.ActualWidth - grip.RightChild.MinLength);
            }
        }

        public void OnDragStarted(SplitterGrip grip, DragStartedEventArgs args)
        {
            ComputeMinMax(grip, out _min, out _max);

            // save the current lentgs
            _leftChildLength = grip.LeftChild.Length;
            _rightChildLength = grip.RightChild.Length;

            args.Handled = true;
        }

        public void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args)
        {
            var itemsControl = grip.ParentOfType<SplitterItemsControl>();
            if (itemsControl == null)
                return;

            MoveGrip(grip, itemsControl, args.VerticalChange, args.HorizontalChange);
            args.Handled = true;
        }

        private void MoveGrip(SplitterGrip grip, SplitterItemsControl itemsControl, double verticalChange, double horizontalChange)
        {
            var isHorizontal = grip.Orientation == Orientation.Horizontal;
            var change = isHorizontal ? verticalChange : horizontalChange;

            var newLength = (isHorizontal ? grip.LeftChild.ActualHeight : grip.LeftChild.ActualWidth) + change;
            CoerceOffset(ref newLength);

            var coercedChange = newLength - (isHorizontal ? grip.LeftChild.ActualHeight : grip.LeftChild.ActualWidth);
            coercedChange = Math.Abs(coercedChange);

            var diffUnit = SplitterItemsControl.GetUnitForSize(itemsControl, coercedChange);

            var isLeftAbsolute = grip.LeftChild.IsFixed;
            var isRightAbsolute = grip.RightChild.IsFixed;

            try
            {
                itemsControl.DisallowPanelInvalidation = true;

                if (isLeftAbsolute || isRightAbsolute)
                {
                    if (isLeftAbsolute)
                        if (change < 0)
                            grip.LeftChild.Length -= coercedChange;
                        else
                            grip.LeftChild.Length += coercedChange;

                    if (isRightAbsolute)
                        if (change < 0)
                            grip.RightChild.Length += coercedChange;
                        else
                            grip.RightChild.Length -= coercedChange;
                }
                else // both use star based measurement system
                {
                    if (change < 0)
                    {
                        grip.LeftChild.Length -= diffUnit;
                        grip.RightChild.Length += diffUnit;
                    }
                    else
                    {
                        grip.LeftChild.Length += diffUnit;
                        grip.RightChild.Length -= diffUnit;
                    }
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

        public void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs e)
        {
            _leftChildLength = ItemLength.Empty;
            _rightChildLength = ItemLength.Empty;

            e.Handled = true;
        }

        public void Cancel()
        {
            var grip = Keyboard.FocusedElement as SplitterGrip;
            if (grip == null || !grip.IsDragging || (_leftChildLength == ItemLength.Empty || _rightChildLength == ItemLength.Empty))
                return;

            var itemsControl = grip.ParentOfType<SplitterItemsControl>();
            if (itemsControl == null)
            {
                return;
            }

            // restore the previous position
            try
            {
                itemsControl.DisallowPanelInvalidation = true;
                grip.LeftChild.Length = _leftChildLength;
                grip.RightChild.Length = _rightChildLength;
                grip.CancelDrag();
            }
            finally
            {
                itemsControl.DisallowPanelInvalidation = false;
            }

            var panel = grip.ParentOfType<SplitterItemsPanel>();
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }

        public void Move(double horizontalChange, double verticalChange)
        {
            var grip = Keyboard.FocusedElement as SplitterGrip;
            if (grip == null)
            {
                return;
            }

            var itemsControl = grip.ParentOfType<SplitterItemsControl>();
            if (itemsControl == null)
            {
                return;
            }

            ComputeMinMax(grip, out _min, out _max);
            MoveGrip(grip, itemsControl, verticalChange, horizontalChange);
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