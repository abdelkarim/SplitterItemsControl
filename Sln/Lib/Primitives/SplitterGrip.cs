/*
  The MIT License (MIT)
  
  Copyright (c) 2014 Abdelkarim Sellamna (abdelkarim.se@gmail.com)
  
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:
  
  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.
  
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Lib.Internals;

namespace Lib.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class SplitterGrip : Thumb
    {
        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="SplitterGrip"/> class.
        /// </summary>
        static SplitterGrip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrip), new FrameworkPropertyMetadata(typeof(SplitterGrip)));
            FocusableProperty.OverrideMetadata(typeof(SplitterGrip), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes instance members of the <see cref="SplitterGrip"/> class.
        /// </summary>
        public SplitterGrip()
        {

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
            typeof(SplitterGrip),
            new FrameworkPropertyMetadata(Orientation.Vertical));

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

        /// <summary>
        /// Gets the <see cref="System.Windows.Controls.Primitives.Popup"/>
        /// </summary>
        internal Popup Popup { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal SplitterItem LeftChild { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal SplitterItem RightChild { get; set; }

        

        #endregion

        #region "Methods"

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Keyboard.Focus(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Popup = GetTemplateChild("PART_Popup") as Popup;
        }

        #endregion
    }
}
