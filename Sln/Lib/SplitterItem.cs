using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class SplitterItem : ContentControl
    {
        #region "Properties"

        #region Size

        /// <summary>
        /// Identifies the <see cref="Size"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size",
            typeof(double),
            typeof(SplitterItem),
            new FrameworkPropertyMetadata(
                1.0,
                (o, args) =>
                {
                    // we should invalidate the arrange of the parent
                    var item = (SplitterItem)o;

                    var itemsControl = ItemsControl.ItemsControlFromItemContainer(item) as SplitterItemsControl;
                    if (itemsControl != null && itemsControl.DisallowPanelInvalidation)
                        return;

                    var panel = SplitterItemsControl.PanelFromContainer(item);
                    if (panel != null)
                        panel.InvalidateMeasure();
                },
                (o, value) =>
                {
                    var desiredSize = (double)value;

                    if (desiredSize < 0.0)
                        desiredSize = 0.0;

                    return desiredSize;
                }));

        /// <summary>
        /// Gets or sets the Size property. This dependency property 
        /// indicates ....
        /// </summary>
        /// <remarks>
        /// Leave it empty for the value to be set automatically.
        /// </remarks>
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region MinSize

        /// <summary>
        /// Identifies the <see cref="MinSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinSizeProperty = DependencyProperty.Register(
            "MinSize",
            typeof(double),
            typeof(SplitterItem),
            new FrameworkPropertyMetadata(
                0.0,
                null,
                (o, value) =>
                {
                    var currentValue = (double)value;
                    return currentValue < 0 ? 0 : value;
                }));

        /// <summary>
        /// Gets or sets a value(in pixels) that indicates the minimum size of this <see cref="SplitterItem"/>. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public double MinSize
        {
            get { return (double)GetValue(MinSizeProperty); }
            set { SetValue(MinSizeProperty, value); }
        }

        #endregion

        #endregion
    }
}
