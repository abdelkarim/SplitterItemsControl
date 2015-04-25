using System;
using System.ComponentModel;
using System.Globalization;

namespace Lib
{
    /// <summary>
    /// The ItemLength is used to express the length of
    /// the <see cref="SplitterItem"/> <see cref="SplitterItem.Length"/> property.
    /// </summary>
    [TypeConverter(typeof(ItemLengthConverter))]
    public struct ItemLength : IEquatable<ItemLength>
    {
        #region "Fields"

        private double _value;
        private ItemLengthUnitType _type;
        private static readonly ItemLength _empty;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="ItemLength"/> class.
        /// </summary>
        static ItemLength()
        {
            _empty = new ItemLength(0, ItemLengthUnitType.Star, false);
        }

        /// <summary>
        /// Initializes instance members of the <see cref="ItemLength"/> class.
        /// </summary>
        public ItemLength(double pixels)
            : this(pixels, ItemLengthUnitType.Pixel)
        {
            
        }

        /// <summary>
        /// Initializes instance members of the <see cref="ItemLength"/> class.
        /// </summary>
        public ItemLength(double value, ItemLengthUnitType type)
            : this(value, type, true)
        {
        }

        internal ItemLength(double value, ItemLengthUnitType type, bool validateParams)
        {
            if (validateParams)
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentException("");
                }

                if (double.IsInfinity(value))
                {
                    throw new ArgumentException("");
                }
            }

            _value = value;
            _type = type;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// 
        /// </summary>
        public bool IsAbsolute
        {
            get { return _type == ItemLengthUnitType.Pixel; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStar
        {
            get { return _type == ItemLengthUnitType.Star; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Value
        {
            get { return _value; }
            internal set { _value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ItemLengthUnitType UnitType
        {
            get { return _type; }
        }

        internal static ItemLength Empty
        {
            get { return _empty; }
        }

        #endregion

        #region "Methods"

        public static bool operator ==(ItemLength length1, ItemLength length2)
        {
            return (length1.UnitType == length2.UnitType) && (length1.Value == length2.Value);
        }

        public static bool operator !=(ItemLength length1, ItemLength length2)
        {
            return (length1.UnitType != length2.UnitType) || (length1.Value != length2.Value);
        }

        public bool Equals(ItemLength other)
        {
            return this == other;
        }

        public override bool Equals(object other)
        {
            if (other is ItemLength)
            {
                var length = (ItemLength) other;
                return this == length;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)Value + (int)UnitType;
        }

        public override string ToString()
        {
            return ItemLengthConverter.ToString(this, CultureInfo.InvariantCulture);
        }

        #endregion

        #region "Operators"

        public static ItemLength operator +(ItemLength leftHandSide, double rightSide)
        {
            return new ItemLength(leftHandSide.Value + rightSide, leftHandSide.UnitType);
        }

        public static ItemLength operator -(ItemLength leftHandSide, double rightSide)
        {
            return new ItemLength(leftHandSide.Value - rightSide, leftHandSide.UnitType);
        }

        #endregion
    }
}