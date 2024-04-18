// <copyright file="DataTransform.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.Viewport
{
    using System;
    using System.Linq;

    internal class DataTransform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransform" /> class.
        /// </summary>
        private DataTransform()
        {
        }

        public static Prodigy.WaveformControls.Viewport.ViewportArea ViewportToData(
            Viewport.Viewport2D fromViewport,
            Prodigy.WaveformControls.Viewport.ViewportArea toViewport,
            double fromScaleX,
            double toScaleX)
        {
            toViewport.X1 = fromViewport.Visible.X1;
            toViewport.X2 = fromViewport.Visible.X2;
            toViewport.Y1 = toScaleX * 15d - ((fromScaleX * 15d - fromViewport.Visible.Y1) * (toScaleX / fromScaleX));
            toViewport.Y2 = toScaleX * 15d - ((fromScaleX * 15d - fromViewport.Visible.Y2) * (toScaleX / fromScaleX));

            return toViewport;
        }
    }
}