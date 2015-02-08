namespace Lib
{
    /// <summary>
    /// The ItemLengthUnitType is used to indicate what kind of value the <see cref="ItemLength"/>
    /// is holding.
    /// </summary>
    public enum ItemLengthUnitType
    {
        /// <summary>
        /// The value is expressed as pixels.
        /// </summary>
        Pixel,

        /// <summary>
        /// The value is expressed as a weighted proportion of the available space.
        /// </summary>
        Star
    }
}