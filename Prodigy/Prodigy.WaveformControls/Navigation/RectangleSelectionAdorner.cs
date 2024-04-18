// <copyright file="RectangleSelectionAdorner.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Prodigy.WaveformControls.Navigation
{
    /// <summary>Helper class to draw semitransparent rectangle over the
    /// selection area</summary>
    public sealed class RectangleSelectionAdorner : Adorner
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                "Fill",
                typeof(Brush),
                typeof(RectangleSelectionAdorner),
                new FrameworkPropertyMetadata(
                    new SolidColorBrush(Color.FromArgb(60, 100, 100, 100)),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        private Rect? border = null;

        public RectangleSelectionAdorner(UIElement element) : base(element)
        {
            this.Pen = new Pen(Brushes.Red, 1.0);
        }

        public Rect? Border
        {
            get
            {
                return this.border;
            }
            set
            {
                this.border = value;
            }
        }

        public Brush Fill
        {
            get
            {
                return (Brush)this.GetValue(FillProperty);
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        public Pen Pen { get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.border.HasValue)
            {
                dc.DrawRectangle(this.Fill, this.Pen, this.border.Value);
            }
        }
    }
}