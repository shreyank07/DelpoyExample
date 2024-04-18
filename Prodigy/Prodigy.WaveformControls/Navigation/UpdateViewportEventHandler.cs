// <copyright file="UpdateViewportEventHandler.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

using System.Windows;
using Prodigy.WaveformControls.View;

namespace Prodigy.WaveformControls.Navigation
{
    public delegate void UpdateViewportEventHandler(PlotEvents plotEvent, Point visibleStart, Point visibleEnd);
}