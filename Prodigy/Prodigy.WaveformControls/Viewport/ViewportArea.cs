// <copyright file="ViewportArea.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
namespace Prodigy.WaveformControls.Viewport
{
    public class ViewportArea
    {
        public double X1;
        public double X2;
        public double Y1;
        public double Y2;

        public ViewportArea(double x1, double x2, double y1, double y2)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;
        }

        public double Width
        {
            get
            {
                return this.X2 - this.X1;
            }
        }

        public double Height
        {
            get
            {
                return this.Y2 - this.Y1;
            }
        }

        public bool IsValid()
        {
            if (this.Height < 0 &&
                this.Width > 0)
            {
                return true;
            }

            return false;
        }
    }
}