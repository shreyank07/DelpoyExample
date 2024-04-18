// <copyright file="RemovePlotEventHandler.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

using System;
using Prodigy.WaveformControls.Interfaces;

namespace Prodigy.WaveformControls
{
    public delegate void RemovePlotEventHandler(object sender, RemovePlotEventArgs e);

    public class RemovePlotEventArgs : EventArgs
    {
        private IPlotInfoView plot;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovePlotEventArgs" /> class.
        /// </summary>
        /// <param name="plot">The plot.</param>
        public RemovePlotEventArgs(IPlotInfoView plot)
        {
            this.plot = plot;
        }

        public IPlotInfoView Plot
        {
            get
            {
                return plot;
            }
        }
    }
}