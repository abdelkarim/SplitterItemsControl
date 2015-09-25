/*
 Copyright (c) 2015 Abdelkarim Sellamna (abdelkarim.se@gmail.com)
 Licensed under the MIT License. See the LICENSE.md file in the project root for full license information.
*/

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
