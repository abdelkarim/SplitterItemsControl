﻿/*
 Copyright (c) 2015 Abdelkarim Sellamna (abdelkarim.se@gmail.com)
 Licensed under the MIT License. See the LICENSE.md file in the project root for full license information.
*/
namespace Lib
{
    /// <summary>
    /// The <see cref="ItemLengthUnitType"/> is used to indicate thd kind of value a <see cref="ItemLength"/>
    /// instance is holding.
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