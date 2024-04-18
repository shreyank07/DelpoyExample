// <copyright file="ViewportRestrictions.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.Viewport
{
    using System;
    using System.Linq;
    using System.Windows;

    internal class ViewportRestrictions
    {
        public ViewportRestrictions(ViewportArea restriction)
        {
            this.Restriction = restriction;
        }

        public ViewportRestrictions()
        {
        }

        /// <summary>
        /// Gets or sets the viewport visible restriction.
        /// </summary>
        /// <value>The restriction.</value>
        public ViewportArea Restriction { get; set; }

        /// <summary>
        /// Applies the specified visible.
        /// </summary>
        /// <param name="visible">The visible.</param>
        /// <returns></returns>
        public ViewportArea Apply(ViewportArea visible)
        {
            double tmp = 0;
            if (visible.X1 < this.Restriction.X1)
            {
                tmp = this.Restriction.X1 - visible.X1;
                visible.X1 += tmp;
                visible.X2 += tmp;
            }
            if (visible.X2 > this.Restriction.X2)
            {
                tmp = visible.X2 - this.Restriction.X2;
                visible.X1 -= tmp;
                visible.X2 -= tmp;
            }

            if (visible.Y1 > this.Restriction.Y1)
            {
                tmp = visible.Y1 - this.Restriction.Y1;
                visible.Y1 -= tmp;
                visible.Y2 -= tmp;
            }
            if (visible.Y2 < this.Restriction.Y2)
            {
                tmp = this.Restriction.Y2 - visible.Y2;
                visible.Y1 += tmp;
                visible.Y2 += tmp;
            }

            if (visible.X1 < this.Restriction.X1)
            {
                visible.X1 = this.Restriction.X1;
            }
            if (visible.X2 > this.Restriction.X2)
            {
                visible.X2 = this.Restriction.X2;
            }
            if (visible.Y1 > this.Restriction.Y1)
            {
                visible.Y1 = this.Restriction.Y1;
            }
            if (visible.Y2 < this.Restriction.Y2)
            {
                visible.Y2 = this.Restriction.Y2;
            }

            return visible;
        }

        /// <summary>
        /// Applies the specified output.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        public ViewportArea Apply(ViewportArea rect, Viewport.Viewport2D viewport)
        {
            Point p1 = viewport.CursorScreenToData(new Point(rect.X1, rect.Y1));
            Point p2 = viewport.CursorScreenToData(new Point(rect.X2, rect.Y2));

            double tmp = 0;
            if (rect.X1 < this.Restriction.X1)
            {
                tmp = this.Restriction.X1 - rect.X1;
                rect.X1 += tmp;
                rect.X2 += tmp;
            }
            if (rect.X2 > this.Restriction.X2)
            {
                tmp = rect.X2 - this.Restriction.X2;
                rect.X1 -= tmp;
                rect.X2 -= tmp;
            }

            if (rect.Y1 < this.Restriction.Y1)
            {
                tmp = this.Restriction.Y1 - rect.Y1;
                rect.Y1 += tmp;
                rect.Y2 += tmp;
            }
            if (rect.Y2 > this.Restriction.Y2)
            {
                tmp = rect.Y2 - this.Restriction.Y2;
                rect.Y1 -= tmp;
                rect.Y2 -= tmp;
            }

            return rect;
        }
    }
}