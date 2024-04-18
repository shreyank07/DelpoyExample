// <copyright file="ITrendViewModel.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
namespace Prodigy.WaveformControls.Interfaces
{
    using Prodigy.Business;
    using Prodigy.WaveformControls;

    public interface ITrendViewModel
    {
        TrendValue[] Trend { get; set; }

        WfmEnum Channel { get; set; }

        TypeOfMeasurement MeasurementType { get; set; }

        double Max { get; }

        double Min { get; }

        bool IsDirty { get; set; }

        long WfmShowingStartIndex { get; set; }

        long WfmShowingStopIndex { get; set; }

        int StartPixel { get; set; }

        int StopPixel { get; set; }
    }
}