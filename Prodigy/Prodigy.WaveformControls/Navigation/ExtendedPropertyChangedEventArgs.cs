// <copyright file="ExtendedPropertyChangedEventArgs.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;

namespace Prodigy.WaveformControls.Navigation
{
    public class ExtendedPropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }

        public static ExtendedPropertyChangedEventArgs FromDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            return new ExtendedPropertyChangedEventArgs
            {
                PropertyName = e.Property.Name,
                NewValue = e.NewValue,
                OldValue = e.OldValue
            };
        }
    }
}