/*
 Copyright (c) 2015 Abdelkarim Sellamna (abdelkarim.se@gmail.com)
 Licensed under the MIT License. See the LICENSE.md file in the project root for full license information.
*/
using System;
using System.Windows;
using System.Windows.Data;

namespace Lib.Internals
{
    /// <summary>
    /// Helpers methods for defining bindings
    /// </summary>
    internal static class BindingHelpers
    {
        public static void DefineBinding(this DependencyObject source,
            DependencyProperty sourceProperty,
            DependencyObject target,
            DependencyProperty targetProperty)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (sourceProperty == null) throw new ArgumentNullException("sourceProperty");
            if (target == null) throw new ArgumentNullException("target");
            if (targetProperty == null) throw new ArgumentNullException("targetProperty");

            Binding binding = new Binding { Source = source, Path = new PropertyPath(sourceProperty)};
            BindingOperations.SetBinding(target, targetProperty, binding);
        }
    }
}
