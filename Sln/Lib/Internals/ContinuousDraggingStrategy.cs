﻿using System;
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
                min = grip.LeftChild.MinLength;
                max = grip.LeftChild.ActualHeight + (grip.RightChild.ActualHeight - grip.RightChild.MinLength);
            }
            else
            {
                min = grip.LeftChild.MinLength;
                max = grip.LeftChild.ActualWidth + (grip.RightChild.ActualWidth - grip.RightChild.MinLength);
            }
        }

        public void OnDragStarted(SplitterGrip splitterGrip, DragStartedEventArgs args)
        {
            ComputeMinMax(splitterGrip, out _min, out _max);
            args.Handled = true;
        }

        public void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args)
        {
            var itemsControl = grip.ParentOfType<SplitterItemsControl>();
            if (itemsControl == null)
                return;

            var isHorizontal = grip.Orientation == Orientation.Horizontal;
            var change = isHorizontal ? args.VerticalChange : args.HorizontalChange;

            var newLength = (isHorizontal ? grip.LeftChild.ActualHeight : grip.LeftChild.ActualWidth) + change;
            CoerceOffset(ref newLength);
            var actualChange = Math.Abs(newLength - (isHorizontal ? grip.LeftChild.ActualHeight : grip.LeftChild.ActualWidth));

            var diffUnit = SplitterItemsControl.GetUnitForSize(itemsControl, actualChange);
            try
            {
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

            args.Handled = true;
        }

        public void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs args)
        {
            args.Handled = true;
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