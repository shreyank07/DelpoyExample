// <copyright file="IPlot.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using Prodigy.Business;

    public interface IPlot
    {
        double StartIndex { get; set; }

        double StopIndex { get; set; }

        WfmEnum Channel { get; set; }

        bool IsShowing { get; set; }

        double Offset { get; set; }

        bool IsDirty { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        Viewport.Viewport2D Viewport { get; set; }

        double Scale { get; set; }

        double MinIndex { get; set; }

        double MaxIndex { get; set; }

        void OnSizeChanged(object sender, SizeChangedEventArgs e);

        void UpdateViewport(PlotEvents plotEvent, Point visibleStart, Point visibleEnd);

        void DoUndo();

        void UndoClear();
    }
}