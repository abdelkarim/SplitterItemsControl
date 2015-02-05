using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Lib.Internals;
using Validation;

namespace Lib.Primitives
{
    public class SplitterItemsPanel : VirtualizingPanel
    {
        #region "Fields"

        private IList<SplitterGrip> _generatedSplitterGrips;
        private IDictionary<SplitterItem, Size> _measures = new Dictionary<SplitterItem, Size>();
        private DispatcherOperation CurrentOperation;

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

        #region Orientation

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(SplitterItemsPanel),
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsArrange));

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

        #region IsVirtualizing

        /// <summary>
        /// IsVirtualizing Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            VirtualizingStackPanel.IsVirtualizingProperty.AddOwner(typeof(SplitterItemsPanel),
                new FrameworkPropertyMetadata(
                    VirtualizingStackPanel.IsVirtualizingProperty.DefaultMetadata.DefaultValue,
                    FrameworkPropertyMetadataOptions.None,
                    (o, args) =>
                    {
                        var panel = (SplitterItemsPanel)o;
                        panel.InvalidateContainers();
                    }));

        /// <summary>
        /// Gets or sets the IsVirtualizing property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsVirtualizing
        {
            get { return (bool)GetValue(IsVirtualizingProperty); }
            set { SetValue(IsVirtualizingProperty, value); }
        }

        #endregion

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
            if (CurrentOperation != null)
                CurrentOperation.Abort();

            Action action = delegate
            {
                CurrentOperation = null;
                GenerateContainers();
            };

            CurrentOperation = Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
        }

        private void GenerateContainers()
        {
            var internalChildren = InternalChildren;
            var generator = ItemContainerGenerator;
            var count = ItemsCount;

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

            var actualLength = isVertical
                ? Math.Max(0.0, availableSize.Width - (_generatedSplitterGrips.Count*6.0))
                : Math.Max(0.0, availableSize.Height - (_generatedSplitterGrips.Count*6.0));

            var splitterItems = InternalChildren.OfType<SplitterItem>().ToList();
            var allSizes = splitterItems.Sum(si => si.Size);
            var uniformSize = actualLength / allSizes;

            foreach (UIElement si in InternalChildren)
            {
                if (si is SplitterGrip)
                {
                    si.Measure(availableSize);
                    continue;
                }

                var splitterItem = (SplitterItem)si;
                var itemSize = splitterItem.Size * uniformSize;
                var childAvailableSize = isVertical
                    ? new Size(itemSize, availableSize.Height)
                    : new Size(availableSize.Width, itemSize);

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

        private void Cleanup(IItemContainerGenerator generator, IEnumerable<UIElement> children)
        {
            foreach (var child in children)
            {
                var childIndex = InternalChildren.IndexOf(child);
                if (child is SplitterGrip)
                {
                    RemoveInternalChildRange(childIndex, 1);
                    _generatedSplitterGrips.Remove((SplitterGrip)child);
                    continue;
                }

                VirtualizeContainer(generator, child, childIndex);
            }
        }

        private void VirtualizeContainer(IItemContainerGenerator generator, UIElement container, int indexOf)
        {
            Requires.NotNull(generator, "generator");
            Requires.NotNull(container, "container");

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

        private SplitterGrip GenerateSplitterGrip()
        {
            var splitterGrip = new SplitterGrip();
            this.AddInternalChild(splitterGrip);
            _generatedSplitterGrips.Add(splitterGrip);

            // set orientation binding to the panel
            this.DefineBinding(OrientationProperty, splitterGrip, SplitterGrip.OrientationProperty);
            return splitterGrip;
        }

        /// <summary>
        /// Will generate a container and measure it. This method might generate <see cref="SplitterItem"/> containers
        /// or <see cref="SplitterGrip"/> items, depending on the index.
        /// </summary>
        private SplitterItem GenerateChild(IItemContainerGenerator generator, int index)
        {
            var position = generator.GeneratorPositionFromIndex(index);
            using (generator.StartAt(position, GeneratorDirection.Forward, false))
            {
                var container = (SplitterItem)generator.GenerateNext();
                container.SetValue(IndexForItemContainerProperty, index);
                this.AddInternalChild(container);
                generator.PrepareItemContainer(container);
                return container;
            }
        }

        #endregion

        #region "Properties"

        #region IndexForItemContainer

        private static readonly DependencyProperty IndexForItemContainerProperty = DependencyProperty.RegisterAttached(
            "IndexForItemContainer",
            typeof(int),
            typeof(SplitterItemsPanel),
            new FrameworkPropertyMetadata(-1));

        #endregion

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

        #endregion
    }
}