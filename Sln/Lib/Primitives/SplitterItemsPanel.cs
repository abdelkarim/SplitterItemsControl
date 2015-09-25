using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Lib.Internals;

namespace Lib.Primitives
{
    public class SplitterItemsPanel : VirtualizingPanel
    {
        #region "Fields"

        private DispatcherOperation _currentOperation;
        private IList<SplitterGrip> _generatedSplitterGrips;
        private IDictionary<SplitterItem, Size> _measures = new Dictionary<SplitterItem, Size>();

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="SplitterItemsPanel"/> class.
        /// </summary>
        static SplitterItemsPanel()
        {

        }

        /// <summary>
        /// Initializes instance members of the <see cref="SplitterItemsPanel"/> class.
        /// </summary>
        public SplitterItemsPanel()
        {
            _generatedSplitterGrips = new List<SplitterGrip>();
        }

        #endregion

        #region "Properties"

        #region Length

        /// <summary>
        /// Length Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.RegisterAttached(
            "Length",
            typeof(ItemLength),
            typeof(SplitterItemsPanel),
            new FrameworkPropertyMetadata(new ItemLength(1, ItemLengthUnitType.Star),
                (o, args) =>
                {
                    // we should invalidate the arrange of the parent
                    var item = (SplitterItem)o;

                    var itemsControl = ItemsControl.ItemsControlFromItemContainer(item) as SplitterItemsControl;
                    if (itemsControl != null && itemsControl.DisallowPanelInvalidation)
                        return;

                    var panel = Lib.SplitterItemsControl.PanelFromContainer(item);
                    if (panel != null)
                        panel.InvalidateMeasure();
                },
                (o, value) =>
                {
                    var length = (ItemLength)value;

                    if (length.Value < 0.0)
                        length.Value = 0.0;

                    return length;
                }));

        /// <summary>
        /// Gets the Length property. This dependency property 
        /// indicates ....
        /// </summary>
        public static ItemLength GetLength(DependencyObject d)
        {
            return (ItemLength)d.GetValue(LengthProperty);
        }

        /// <summary>
        /// Sets the Length property. This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetLength(DependencyObject d, ItemLength value)
        {
            d.SetValue(LengthProperty, value);
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(SplitterItemsPanel),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets the Orientation property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region IndexForItemContainer

        private static readonly DependencyProperty IndexForItemContainerProperty = DependencyProperty.RegisterAttached(
            "IndexForItemContainer",
            typeof(int),
            typeof(SplitterItemsPanel),
            new FrameworkPropertyMetadata(-1));

        #endregion

        #endregion

        #region "Internal Properties"

        public SplitterItemsControl SplitterItemsControl
        {
            get { return TemplatedParent as SplitterItemsControl; }
        }

        private ItemsControl ItemsOwner
        {
            get { return ItemsControl.GetItemsOwner(this); }
        }

        private int ItemsCount
        {
            get
            {
                var itemsOwner = this.ItemsOwner;
                return itemsOwner == null ? 0 : itemsOwner.Items.Count;
            }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                var basedLogicalChildren = base.LogicalChildren;
                var children = new List<object>();

                // enumerate the old items
                if (basedLogicalChildren != null)
                {
                    while (basedLogicalChildren.MoveNext())
                    {
                        children.Add(basedLogicalChildren.Current);
                    }
                }

                // insert the new items
                children.AddRange(_generatedSplitterGrips);
                return children.GetEnumerator();
            }
        }

        #endregion

        #region "Methods"

        protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
        {
            base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
            InvalidateContainers();
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            InvalidateContainers();
        }

        private void InvalidateContainers()
        {
            if (_currentOperation != null)
                _currentOperation.Abort();

            Action action = delegate
            {
                _currentOperation = null;
                GenerateContainers();
            };

            _currentOperation = Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
        }

        private void GenerateContainers()
        {
            var internalChildren = InternalChildren;
            var generator = ItemContainerGenerator;
            var count = ItemsCount;

            if (count == 0)
            {
                return;
            }


            Cleanup(generator, InternalChildren.Cast<UIElement>().ToList());

            int index = -1;
            var previousContainer = GenerateChild(generator, ++index);
            while (index < (count - 1))
            {
                var splitter = GenerateSplitterGrip();
                var nextContainer = GenerateChild(generator, ++index);

                splitter.LeftChild = previousContainer;
                splitter.RightChild = nextContainer;
                previousContainer = nextContainer;
            }
        }

        private void Cleanup(IItemContainerGenerator generator, IEnumerable<UIElement> children)
        {
            foreach (var child in children)
            {
                var childIndex = InternalChildren.IndexOf(child);
                if (child is SplitterGrip)
                {
                    RemoveInternalChildRange(childIndex, 1);
                    RemoveLogicalChild(child);
                    _generatedSplitterGrips.Remove((SplitterGrip)child);
                    continue;
                }

                VirtualizeContainer(generator, child, childIndex);
            }
        }

        private void VirtualizeContainer(IItemContainerGenerator generator, UIElement container, int indexOf)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("generator");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            var index = (int)container.GetValue(IndexForItemContainerProperty);
            if (index == -1)
                return;

            container.ClearValue(IndexForItemContainerProperty);

            // remove from the visual tree
            RemoveInternalChildRange(indexOf, 1);

            // remove from the generator
            var trackPosition = generator.GeneratorPositionFromIndex(index);
            generator.Remove(trackPosition, 1);
        }

        private SplitterGrip GenerateSplitterGrip(int index = -1)
        {
            var splitterGrip = new SplitterGrip();
            if (index == -1)
            {
                this.AddInternalChild(splitterGrip);
            }
            else
            {
                InsertInternalChild(index, splitterGrip);
            }

            AddLogicalChild(splitterGrip);
            _generatedSplitterGrips.Add(splitterGrip);

            // set orientation binding to the panel
            this.DefineBinding(OrientationProperty, splitterGrip, SplitterGrip.OrientationProperty);
            this.SplitterItemsControl.DefineBinding(Lib.SplitterItemsControl.SplitterGripStyleProperty, splitterGrip, StyleProperty);
            return splitterGrip;
        }

        /// <summary>
        /// Will generate a container and measure it.
        /// </summary>
        private SplitterItem GenerateChild(IItemContainerGenerator generator, int index, int uiIndex = -1)
        {
            var position = generator.GeneratorPositionFromIndex(index);
            using (generator.StartAt(position, GeneratorDirection.Forward, false))
            {
                var container = (SplitterItem)generator.GenerateNext();
                container.SetValue(IndexForItemContainerProperty, index);

                if (uiIndex == -1)
                {
                    this.AddInternalChild(container);
                }
                else
                {
                    InsertInternalChild(uiIndex, container);
                }

                generator.PrepareItemContainer(container);
                return container;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _measures.Clear();
            var isVertical = Orientation == Orientation.Vertical;
            var count = ItemsCount;

            if (count == 0)
                return base.MeasureOverride(availableSize);

            // we only accept size with fixed width/height, based on the orientation.
            if (isVertical && double.IsPositiveInfinity(availableSize.Width))
                return base.MeasureOverride(availableSize);

            if (!isVertical && double.IsPositiveInfinity(availableSize.Height))
                return base.MeasureOverride(availableSize);

            var desiredSize = new Size();

            #region "Measure the Containers"

            var gripSize = SplitterItemsControl.GripLength;
            var splitterItems = InternalChildren.OfType<SplitterItem>().ToList();

            // the available space excluding the grips.
            var actualLength = isVertical
                ? Math.Max(0.0, availableSize.Width - (_generatedSplitterGrips.Count * gripSize))
                : Math.Max(0.0, availableSize.Height - (_generatedSplitterGrips.Count * gripSize));

            // space reserved for fixed size items.
            var fixedLength = splitterItems.Where(si => si.IsFixed)
                .Sum(si => si.Length.Value);

            actualLength = Math.Max(0.0, actualLength - fixedLength);


            var allSizes = splitterItems.Where(si => !si.IsFixed).Sum(si => si.Length.Value);
            var uniformSize = actualLength / allSizes;

            foreach (UIElement si in InternalChildren)
            {
                if (si is SplitterGrip)
                {
                    si.Measure(availableSize);
                    continue;
                }

                var splitterItem = (SplitterItem)si;
                var itemLength = splitterItem.IsFixed
                    ? splitterItem.Length.Value
                    : splitterItem.Length.Value * uniformSize;

                var childAvailableSize = isVertical
                    ? new Size(itemLength, availableSize.Height)
                    : new Size(availableSize.Width, itemLength);

                _measures.Add(splitterItem, childAvailableSize);
                splitterItem.Measure(childAvailableSize);
                var childDesiredSize = splitterItem.DesiredSize;

                if (isVertical)
                {
                    if (desiredSize.Height < childDesiredSize.Height)
                        desiredSize.Height = childDesiredSize.Height;
                }
                else
                {
                    if (desiredSize.Width < childDesiredSize.Width)
                        desiredSize.Width = childDesiredSize.Width;
                }
            }

            if (isVertical)
                desiredSize.Width = availableSize.Width;
            else
                desiredSize.Height = availableSize.Height;

            #endregion

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_measures.Count == 0)
                return base.ArrangeOverride(finalSize);

            bool isVertical = Orientation == Orientation.Vertical;
            var rect = new Rect(finalSize);
            foreach (UIElement child in this.InternalChildren)
            {
                var desiredSize = child is SplitterGrip ? child.DesiredSize : _measures[(SplitterItem)child];
                if (isVertical)
                {
                    rect.Width = desiredSize.Width;
                    child.Arrange(rect);
                    rect.X += desiredSize.Width;
                }
                else
                {
                    rect.Height = desiredSize.Height;
                    child.Arrange(rect);
                    rect.Y += desiredSize.Height;
                }
            }

            return finalSize;
        }

        #endregion
    }
}
