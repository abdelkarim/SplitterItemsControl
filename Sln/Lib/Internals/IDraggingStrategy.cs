using System.Windows.Controls.Primitives;
using Lib.Primitives;

namespace Lib.Internals
{
    internal interface IDraggingStrategy
    {
        void ComputeMinMax(SplitterGrip grip, out double min, out double max);
        void OnDragStarted(SplitterGrip splitterGrip, DragStartedEventArgs args);
        void OnDragDelta(SplitterGrip grip, DragDeltaEventArgs args);
        void OnDragCompleted(SplitterGrip grip, DragCompletedEventArgs args);
    }
}
