// <copyright file="Viewport2D.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.Viewport
{
    using System;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// TODO: The view port should contain the data offset and scale.
    /// Data area - has offset and scale that can be changed from the data
    /// </summary>
    public class Viewport2D : GalaSoft.MvvmLight.ViewModelBase
    {
        ViewportArea _Visible;
        private ViewportArea _Output;

        public Viewport2D(ViewportArea visible, ViewportArea output)
        {
            this._Visible = visible;
            this._Output = output;
        }

        private double _DataVerticalScale = 1.0;
        public double DataVerticalScale 
        {
            get
            {
                return this._DataVerticalScale;
            }
            set
            {
                this._DataVerticalScale = value;
                RaisePropertyChanged(nameof(DataVerticalScale));
            }
        }

        private double _DataVerticalOffset = 1.0;
        public double DataVerticalOffset 
        {
            get
            {
                return _DataVerticalOffset;
            }
            set
            {
                _DataVerticalOffset = value;
                RaisePropertyChanged(nameof(DataVerticalOffset));
            }
        }

        /// <summary>
        /// Gets or sets the Actual Data.
        /// </summary>
        /// <value>The visible.</value>
        public ViewportArea Visible
        {
            get
            {
                return this._Visible;
            }
            set
            {
                this._Visible = value;
                this.RaisePropertyChanged("Visible");
            }
        }

        /// <summary>
        /// Gets or sets the output Screen Coordinates.
        /// </summary>
        /// <value>The output.</value>
        public ViewportArea Output
        {
            get
            {
                return this._Output;
            }
            set
            {
                this._Output = value;
                this.RaisePropertyChanged("Output");
            }
        }

        /// <summary>
        /// Data to screen point.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public Point DataToScreen(Point data)
        {
            double x = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Output.X1, this.Output.X2, this.Visible.X1, this.Visible.X2, data.X);
            double y = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(
                this.Output.Y1, 
                this.Output.Y2, 
                this.Visible.Y1, 
                this.Visible.Y2, 
                this.DataVerticalOffset + this.DataVerticalScale * data.Y);

            return new Point(x, y);
        }

        public Point ScreenToData(Point point)
        {
            double x = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Visible.X1, this.Visible.X2, this.Output.X1, this.Output.X2, point.X);

            // Remove scale and offset to obtain actual data
            double y = (Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Visible.Y1, this.Visible.Y2, this.Output.Y1, this.Output.Y2, point.Y)
                - this.DataVerticalOffset) / this.DataVerticalScale;

            return new Point(x, y);
        }

        public Point CursorScreenToData(Point point)
        {
            double x = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Visible.X1, this.Visible.X2, this.Output.X1, this.Output.X2, point.X);

            // Remove scale and offset to obtain actual data
            double y = (Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Visible.Y1, this.Visible.Y2, this.Output.Y1, this.Output.Y2, point.Y));

            return new Point(x, y);
        }

        public Point DataToCursorScreen(Point data)
        {
            double x = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Output.X1, this.Output.X2, this.Visible.X1, this.Visible.X2, data.X);
            double y = Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(this.Output.Y1, this.Output.Y2, this.Visible.Y1, this.Visible.Y2, data.Y);

            return new Point(x, y);
        }
    }
}