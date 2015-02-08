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
        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="SplitterItem"/> class.
        /// </summary>
        public SplitterItem()
        {
            
        } 

        #endregion

        #region "Properties"

        #region Length

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length",
            typeof(ItemLength),
            typeof(SplitterItem),
            new FrameworkPropertyMetadata(
                new ItemLength(1.0, ItemLengthUnitType.Star),
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
                    var length = (ItemLength) value;
                    
                    if (length.Value < 0.0)
                        length.Value = 0.0;

                    return length;
                }));

        /// <summary>
        /// Gets or sets the Length property. This dependency property 
        /// indicates ....
        /// </summary>
        /// <remarks>
        /// Leave it empty for the value to be set automatically.
        /// </remarks>
        public ItemLength Length
        {
            get { return (ItemLength)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        #endregion

        #region MinLength

        /// <summary>
        /// Identifies the <see cref="MinLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register(
            "MinLength",
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
        [Bindable(true)]
        public double MinLength
        {
            get { return (double)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        #endregion

        internal bool IsFixed
        {
            get { return this.Length.UnitType == ItemLengthUnitType.Pixel; }
        }

        #endregion
    }
}
