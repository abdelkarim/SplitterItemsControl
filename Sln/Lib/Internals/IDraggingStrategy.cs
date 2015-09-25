/*
 Copyright (c) 2015 Abdelkarim Sellamna (abdelkarim.se@gmail.com)
 Licensed under the MIT License. See the LICENSE.md file in the project root for full license information.
*/
using System.Windows.Controls.Primitives;
using Lib.Primitives;

namespace Lib.Internals
{
    internal interface IDraggingStrategy
    {
        void ComputeMinMax(SplitterGrip grip, out double min, out double max);
        void OnDragStarted(SplitterGrip splitterGrip, DragStartedEventArgs args);
        void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args);
        void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs e);
        void Cancel();
        void Move(double horizontalChange, double verticalChange);
    }
}
