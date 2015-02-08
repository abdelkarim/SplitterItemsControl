using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Lib
{
    public class ItemLengthConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode typeCode = Type.GetTypeCode(sourceType);
            switch (typeCode)
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (InstanceDescriptor) || destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object sourceValue)
        {
            if (sourceValue != null)
            {
                if (sourceValue is string)
                {
                    return FromString((string)sourceValue, culture);
                }
                
                var value = Convert.ToDouble(sourceValue, culture);
                var unitType = ItemLengthUnitType.Pixel;

                return new ItemLength(value, unitType);
            }

            throw new InvalidOperationException();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && value is ItemLength)
            {
                var itemLength = (ItemLength) value;
                if (destinationType == typeof(string))
                {
                    return ToString(itemLength, culture);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo ci = typeof (ItemLength).GetConstructor(new[] {typeof (double), typeof (ItemLengthUnitType)});
                    return new InstanceDescriptor(ci, new object[] {itemLength.Value, itemLength.UnitType});
                }
            }

            throw new InvalidOperationException();
        }

        internal static string ToString(ItemLength itemLength, CultureInfo culture)
        {
            if (itemLength.UnitType == ItemLengthUnitType.Star)
            {
                return itemLength.Value == 1.0
                    ? "*"
                    : Convert.ToString(itemLength.Value, culture) + "*";
            }

            return Convert.ToString(itemLength.Value, culture);
        }

        internal static ItemLength FromString(string s, CultureInfo culture)
        {
            double value;
            var unitType = ItemLengthUnitType.Pixel;

            s = s.Trim().ToLowerInvariant();

            if (s.EndsWith("px"))
            {
                var pixelValueStr = s.Substring(0, s.Length - 2);
                value = Convert.ToDouble(pixelValueStr, culture);
            }
            else if (s.EndsWith("*"))
            {
                var starValueStr = s.Substring(0, s.Length - 1);
                value = Convert.ToDouble(starValueStr, culture);
                unitType = ItemLengthUnitType.Star;
            }
            else
            {
                value = Convert.ToDouble(s, culture);
            }

            return new ItemLength(value, unitType);
        }
    }
}