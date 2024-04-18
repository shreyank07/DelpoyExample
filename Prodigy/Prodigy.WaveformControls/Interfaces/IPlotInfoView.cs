// <copyright file="IPlotInfoView.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
namespace Prodigy.WaveformControls.Interfaces
{
    using System;
    using System.Linq;
    using Prodigy.Business;
    using Prodigy.WaveformControls;
    using Prodigy.WaveformControls;

    public interface IPlotInfoView
    {
        event RemovePlotEventHandler OnRemove;

        TypeOfChart ChartType { get; }

        bool IsDirty { get; set; }

        bool IsShowing { get; set; }

        bool IsCursorVisible { get; set; }

        bool IsEnabled { get; }

        WfmEnum Channel { get; }

        double SampleRate { get; }

        string Source { get; }

        double VerticalScale { get; set; }

        double HorizontalScale { get; }

        double VerticalOffset { get; set; }

        double VCursor1Position { get; set; }

        double VCursor2Position { get; set; }

        double TriggerPosition { get; }

        double StartWfmIndex { get; set; }

        double StopWfmIndex { get; set; }

        long WfmLength { get; }

        void Remove();

        void SetIsShowing();
    }

    public interface IDigitalPlotInfoView : IPlotInfoView { }
}