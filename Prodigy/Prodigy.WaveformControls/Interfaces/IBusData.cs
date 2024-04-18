// <copyright file="IBusData.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
namespace Prodigy.WaveformControls.Interfaces
{
    using System;
    using System.Linq;
    using System.Windows.Media;

    public interface IBusData
    {
        Brush Brush { get; }

        double StartIndex { get; set; }

        double StopIndex { get; set; }

        string ToString();
    }
}