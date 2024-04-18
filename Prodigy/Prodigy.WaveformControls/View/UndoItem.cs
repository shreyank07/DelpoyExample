// <copyright file="UndoItem.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;

    internal class UndoItem
    {
        public UndoItem(double x1, double x2, double y1, double y2, double offset, double scale, double dataOffset, double dataScale)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;
            this.Offset = offset;
            this.Scale = scale;
            DataOffset = dataOffset;
            DataScale = dataScale;
        }

        public double X1 { get; set; }

        public double X2 { get; set; }

        public double Y1 { get; set; }

        public double Y2 { get; set; }

        public double Offset { get; set; }

        public double Scale { get; set; }

        public double DataOffset { get; set; }

        public double DataScale { get; set; }

        public bool Equals(UndoItem undoItem)
        {
            if (this.X1 == undoItem.X1 &&
                this.X2 == undoItem.X2 &&
                this.Y1 == undoItem.Y1 &&
                this.Y2 == undoItem.Y2 &&
                this.Offset == undoItem.Offset &&
                this.Scale == undoItem.Scale &&
                this.DataOffset == undoItem.DataOffset &&
                this.DataScale == undoItem.DataScale)
            {
                return true;
            }

            return false;
        }
    }
}