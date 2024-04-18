// <copyright file="WriteableBitmapExtensions.partial.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

#region Header

#endregion

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prodigy.WaveformControls.Extensions;

#if NETFX_CORE
namespace Windows.UI.Xaml.Media.Imaging
#else
namespace Prodigy.WaveformControls.Extensions
#endif
{
    /// <summary>
    /// Collection of extension methods for the WriteableBitmap class.
    /// </summary>
    public
    #if SILVERLIGHT
    #else
    unsafe 
    #endif
    static partial class WriteableBitmapExtensions
    {
        #region Fields
        
        internal const int SizeOfArgb = 4;
        
        #endregion
        
        #region Enums
        
        /// <summary>
        /// The interpolation method.
        /// </summary>
        public enum Interpolation
        {
            /// <summary>
            /// The nearest neighbor algorithm simply selects the color of the nearest pixel.
            /// </summary>
            NearestNeighbor = 0,
            
            /// <summary>
            /// Linear interpolation in 2D using the average of 3 neighboring pixels.
            /// </summary>
            Bilinear,
        }
        
        /// <summary>
        /// The mode for flipping.
        /// </summary>
        public enum FlipMode
        {
            /// <summary>
            /// Flips the image vertical (around the center of the y-axis).
            /// </summary>
            Vertical,
            
            /// <summary>
            /// Flips the image horizontal (around the center of the x-axis).
            /// </summary>
            Horizontal
        }
        
        #endregion
        
        #region Methods
        
        #region General
            
        private static int ConvertColor(Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24) |
                      ((byte)((color.R * a) >> 8) << 16) |
                      ((byte)((color.G * a) >> 8) << 8) |
                      ((byte)((color.B * a) >> 8));
            return col;
        }
        
        /// <summary>
        /// Fills the whole WriteableBitmap with a color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="color">The color used for filling.</param>
        public static void Clear(this WriteableBitmap bmp, Color color)
        {
            var col = ConvertColor(color);
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var len = w * SizeOfArgb;
                
                // Fill first line
                for (var x = 0; x < w; x++)
                {
                    pixels[x] = col;
                }
                
                // Copy first line
                var blockHeight = 1;
                var y = 1;
                while (y < h)
                {
                    Prodigy.WaveformControls.Extensions.BitmapContext.BlockCopy(context, 0, context, y * len, blockHeight * len);
                    y += blockHeight;
                    blockHeight = Math.Min(2 * blockHeight, h - y);
                }
            }
        }
        
        /// <summary>
        /// Fills the whole WriteableBitmap with an empty color (0).
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        public static void Clear(this WriteableBitmap bmp)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Clear();
            }
        }
        
        /// <summary>
        /// Clones the specified WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <returns>A copy of the WriteableBitmap.</returns>
        //public static WriteableBitmap Clone(this WriteableBitmap bmp)
        //{
        //    using (var srcContext = bmp.GetBitmapContext(Prodigy.WaveformAnalyser.Extensions.ReadWriteMode.ReadOnly))
        //    {
        //        var result = BitmapFactory.New(srcContext.Width, srcContext.Height);
        //        using (var destContext = result.GetBitmapContext())
        //        {
        //            Prodigy.WaveformAnalyser.Extensions.BitmapContext.BlockCopy(srcContext, 0, destContext, 0, srcContext.Length * SizeOfArgb);
        //        }
        //        return result;
        //    }
        //}
        
        #endregion
        
        #region ForEach
        
        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                int w = context.Width;
                int h = context.Height;
                int index = 0;
                    
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var color = func(x, y);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }
        
        /// <summary>
        /// Applies the given function to all the pixels of the bitmap in 
        /// order to set their color.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="func">The function to apply. With parameters x, y, source color and a color as a result</param>
        public static void ForEach(this WriteableBitmap bmp, Func<int, int, Color, Color> func)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var pixels = context.Pixels;
                var w = context.Width;
                var h = context.Height;
                var index = 0;
                    
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        var c = pixels[index];
                        
                        // Premultiplied Alpha!
                        var a = (byte)(c >> 24);
                        // Prevent division by zero
                        int ai = a;
                        if (ai == 0)
                        {
                            ai = 1;
                        }
                        // Scale inverse alpha to use cheap integer mul bit shift
                        ai = ((255 << 8) / ai);
                        var srcColor = Color.FromArgb(a,
                            (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                            (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                            (byte)((((c & 0xFF) * ai) >> 8)));
                    
                        var color = func(x, y, srcColor);
                        pixels[index++] = ConvertColor(color);
                    }
                }
            }
        }
        
        #endregion
        
        #region Get Pixel / Brightness
        
        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as integer.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y.</returns>
        public static int GetPixeli(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext())
            {
                return context.Pixels[y * context.Width + x];
            }
        }
        
        /// <summary>
        /// Gets the color of the pixel at the x, y coordinate as a Color struct.  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The color of the pixel at x, y as a Color struct.</returns>
        public static Color GetPixel(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext())
            {
                var c = context.Pixels[y * context.Width + x];
                var a = (byte)(c >> 24);
                
                // Prevent division by zero
                int ai = a;
                if (ai == 0)
                {
                    ai = 1;
                }
                
                // Scale inverse alpha to use cheap integer mul bit shift
                ai = ((255 << 8) / ai);
                return Color.FromArgb(a,
                    (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                    (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                    (byte)((((c & 0xFF) * ai) >> 8)));
            }
        }
        
        /// <summary>
        /// Gets the brightness / luminance of the pixel at the x, y coordinate as byte.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>The brightness of the pixel at x, y.</returns>
        public static byte GetBrightness(this WriteableBitmap bmp, int x, int y)
        {
            using (var context = bmp.GetBitmapContext(Prodigy.WaveformControls.Extensions.ReadWriteMode.ReadOnly))
            {
                // Extract color components
                var c = context.Pixels[y * context.Width + x];
                var r = (byte)(c >> 16);
                var g = (byte)(c >> 8);
                var b = (byte)(c);
            
                // Convert to gray with constant factors 0.2126, 0.7152, 0.0722
                return (byte)((r * 6966 + g * 23436 + b * 2366) >> 15);
            }
        }
        
        #endregion
        
        #region SetPixel
        
        #region Without alpha
        
        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }
        
        #endregion
        
        #region With alpha
        
        /// <summary>
        /// Sets the color of the pixel including the alpha value and using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel including the alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="r">The red value of the color.</param>
        /// <param name="g">The green value of the color.</param>
        /// <param name="b">The blue value of the color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, byte r, byte g, byte b)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }
        
        #endregion
        
        #region With System.Windows.Media.Color
        
        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = ConvertColor(color);
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = ConvertColor(color);
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel using an extra alpha value and a precalculated index (faster). 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[index] = (a << 24) |
                                        ((byte)((color.R * ai) >> 8) << 16) |
                                        ((byte)((color.G * ai) >> 8) << 8) |
                                        ((byte)((color.B * ai) >> 8));
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel using an extra alpha value. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="a">The alpha value of the color.</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, byte a, Color color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Add one to use mul and cheap bit shift for multiplicaltion
                var ai = a + 1;
                context.Pixels[y * context.Width + x] = (a << 24) |
                                                        ((byte)((color.R * ai) >> 8) << 16) |
                                                        ((byte)((color.G * ai) >> 8) << 8) |
                                                        ((byte)((color.B * ai) >> 8));
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel using a precalculated index (faster).  
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="index">The coordinate index.</param>
        /// <param name="color">The color.</param>
        public static void SetPixeli(this WriteableBitmap bmp, int index, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[index] = color;
            }
        }
        
        /// <summary>
        /// Sets the color of the pixel. 
        /// For best performance this method should not be used in iterative real-time scenarios. Implement the code directly inside a loop.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate (row).</param>
        /// <param name="y">The y coordinate (column).</param>
        /// <param name="color">The color.</param>
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, int color)
        {
            using (var context = bmp.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = color;
            }
        }
        
        #endregion
        
        #endregion
        
        #region Crop
        
        /// <summary>
        /// Creates a new cropped WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="x">The x coordinate of the rectangle that defines the crop region.</param>
        /// <param name="y">The y coordinate of the rectangle that defines the crop region.</param>
        /// <param name="width">The width of the rectangle that defines the crop region.</param>
        /// <param name="height">The height of the rectangle that defines the crop region.</param>
        /// <returns>A new WriteableBitmap that is a cropped version of the input.</returns>
        public static WriteableBitmap Crop(this WriteableBitmap bmp, int x, int y, int width, int height)
        {
            using (var srcContext = bmp.GetBitmapContext())
            {
                var srcWidth = srcContext.Width;
                var srcHeight = srcContext.Height;
                
                // If the rectangle is completly out of the bitmap
                if (x > srcWidth || y > srcHeight)
                {
                    return BitmapFactory.New(0, 0);
                }
                
                // Clamp to boundaries
                if (x < 0)
                {
                    x = 0;
                }
                if (x + width > srcWidth)
                {
                    width = srcWidth - x;
                }
                if (y < 0)
                {
                    y = 0;
                }
                if (y + height > srcHeight)
                {
                    height = srcHeight - y;
                }
                
                // Copy the pixels line by line using fast BlockCopy
                var result = BitmapFactory.New(width, height);
                using (var destContext = result.GetBitmapContext())
                {
                    for (var line = 0; line < height; line++)
                    {
                        var srcOff = ((y + line) * srcWidth + x) * SizeOfArgb;
                        var dstOff = line * width * SizeOfArgb;
                        BitmapContext.BlockCopy(srcContext, srcOff, destContext, dstOff, width * SizeOfArgb);
                    }
            
                    return result;
                }
            }
        }
        
        /// <summary>
        /// Creates a new cropped WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="region">The rectangle that defines the crop region.</param>
        /// <returns>A new WriteableBitmap that is a cropped version of the input.</returns>
        public static WriteableBitmap Crop(this WriteableBitmap bmp, Rect region)
        {
            return bmp.Crop((int)region.X, (int)region.Y, (int)region.Width, (int)region.Height);
        }
        
        #endregion
        
        #region Resize
        
        /// <summary>
        /// Creates a new resized WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new WriteableBitmap that is a resized version of the input.</returns>
        public static WriteableBitmap Resize(this WriteableBitmap bmp, int width, int height, Interpolation interpolation)
        {
            using (var srcContext = bmp.GetBitmapContext())
            {
                var pd = Resize(srcContext, srcContext.Width, srcContext.Height, width, height, interpolation);
                
                var result = BitmapFactory.New(width, height);
                using (var dstContext = result.GetBitmapContext())
                {
                    BitmapContext.BlockCopy(pd, 0, dstContext, 0, SizeOfArgb * pd.Length);
                }
                return result;
            }
        }
        
        /// <summary>
        /// Creates a new resized bitmap.
        /// </summary>
        /// <param name="srcContext">The source context.</param>
        /// <param name="widthSource">The width of the source pixels.</param>
        /// <param name="heightSource">The height of the source pixels.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new bitmap that is a resized version of the input.</returns>
        public unsafe static int[] Resize(BitmapContext srcContext, int widthSource, int heightSource, int width, int height, Interpolation interpolation)
        {
            return Resize(srcContext.Pixels, widthSource, heightSource, width, height, interpolation);
        }
        
        /// <summary>
        /// Creates a new resized bitmap.
        /// </summary>
        /// <param name="pixels">The source pixels.</param>
        /// <param name="widthSource">The width of the source pixels.</param>
        /// <param name="heightSource">The height of the source pixels.</param>
        /// <param name="width">The new desired width.</param>
        /// <param name="height">The new desired height.</param>
        /// <param name="interpolation">The interpolation method that should be used.</param>
        /// <returns>A new bitmap that is a resized version of the input.</returns>
        public unsafe static int[] Resize(int* pixels, int widthSource, int heightSource, int width, int height, Interpolation interpolation)
        {
            var pd = new int[width * height];
            var xs = (float)widthSource / width;
            var ys = (float)heightSource / height;
            
            float fracx, fracy, ifracx, ifracy, sx, sy, l0, l1, rf, gf, bf;
            int c, x0, x1, y0, y1;
            byte c1a, c1r, c1g, c1b, c2a, c2r, c2g, c2b, c3a, c3r, c3g, c3b, c4a, c4r, c4g, c4b;
            byte a, r, g, b;
            
            // Nearest Neighbor
            if (interpolation == Interpolation.NearestNeighbor)
            {
                var srcIdx = 0;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;
                
                        pd[srcIdx++] = pixels[y0 * widthSource + x0];
                    }
                }
            }
            // Bilinear
            else if (interpolation == Interpolation.Bilinear)
            {
                var srcIdx = 0;
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        sx = x * xs;
                        sy = y * ys;
                        x0 = (int)sx;
                        y0 = (int)sy;
                        
                        // Calculate coordinates of the 4 interpolation points
                        fracx = sx - x0;
                        fracy = sy - y0;
                        ifracx = 1f - fracx;
                        ifracy = 1f - fracy;
                        x1 = x0 + 1;
                        if (x1 >= widthSource)
                        {
                            x1 = x0;
                        }
                        y1 = y0 + 1;
                        if (y1 >= heightSource)
                        {
                            y1 = y0;
                        }
                        
                        // Read source color
                        c = pixels[y0 * widthSource + x0];
                        c1a = (byte)(c >> 24);
                        c1r = (byte)(c >> 16);
                        c1g = (byte)(c >> 8);
                        c1b = (byte)(c);
                        
                        c = pixels[y0 * widthSource + x1];
                        c2a = (byte)(c >> 24);
                        c2r = (byte)(c >> 16);
                        c2g = (byte)(c >> 8);
                        c2b = (byte)(c);
                        
                        c = pixels[y1 * widthSource + x0];
                        c3a = (byte)(c >> 24);
                        c3r = (byte)(c >> 16);
                        c3g = (byte)(c >> 8);
                        c3b = (byte)(c);
                        
                        c = pixels[y1 * widthSource + x1];
                        c4a = (byte)(c >> 24);
                        c4r = (byte)(c >> 16);
                        c4g = (byte)(c >> 8);
                        c4b = (byte)(c);
                        
                        // Calculate colors
                        // Alpha
                        l0 = ifracx * c1a + fracx * c2a;
                        l1 = ifracx * c3a + fracx * c4a;
                        a = (byte)(ifracy * l0 + fracy * l1);
                        
                        // Red
                        l0 = ifracx * c1r + fracx * c2r;
                        l1 = ifracx * c3r + fracx * c4r;
                        rf = ifracy * l0 + fracy * l1;
                        
                        // Green
                        l0 = ifracx * c1g + fracx * c2g;
                        l1 = ifracx * c3g + fracx * c4g;
                        gf = ifracy * l0 + fracy * l1;
                        
                        // Blue
                        l0 = ifracx * c1b + fracx * c2b;
                        l1 = ifracx * c3b + fracx * c4b;
                        bf = ifracy * l0 + fracy * l1;
                        
                        // Cast to byte
                        r = (byte)rf;
                        g = (byte)gf;
                        b = (byte)bf;
                    
                        // Write destination
                        pd[srcIdx++] = (a << 24) | (r << 16) | (g << 8) | b;
                    }
                }
            }
            return pd;
        }
        
        #endregion
        
        #region Rotate
        
        /// <summary>
        /// Rotates the bitmap in 90� steps clockwise and returns a new rotated WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="angle">The angle in degress the bitmap should be rotated in 90� steps clockwise.</param>
        /// <returns>A new WriteableBitmap that is a rotated version of the input.</returns>
        public unsafe static WriteableBitmap Rotate(this WriteableBitmap bmp, int angle)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Use refs for faster access (really important!) speeds up a lot!
                var w = context.Width;
                var h = context.Height;
                var p = context.Pixels;
                var i = 0;
                WriteableBitmap result = null;
                angle %= 360;
                    
                if (angle > 0 && angle <= 90)
                {
                    result = BitmapFactory.New(h, w);
                    using (var destContext = result.GetBitmapContext())
                    {
                        var rp = destContext.Pixels;
                        for (var x = 0; x < w; x++)
                        {
                            for (var y = h - 1; y >= 0; y--)
                            {
                                var srcInd = y * w + x;
                                rp[i] = p[srcInd];
                                i++;
                            }
                        }
                    }
                }
                else if (angle > 90 && angle <= 180)
                {
                    result = BitmapFactory.New(w, h);
                    using (var destContext = result.GetBitmapContext())
                    {
                        var rp = destContext.Pixels;
                        for (var y = h - 1; y >= 0; y--)
                        {
                            for (var x = w - 1; x >= 0; x--)
                            {
                                var srcInd = y * w + x;
                                rp[i] = p[srcInd];
                                i++;
                            }
                        }
                    }
                }
                else if (angle > 180 && angle <= 270)
                {
                    result = BitmapFactory.New(h, w);
                    using (var destContext = result.GetBitmapContext())
                    {
                        var rp = destContext.Pixels;
                        for (var x = w - 1; x >= 0; x--)
                        {
                            for (var y = 0; y < h; y++)
                            {
                                var srcInd = y * w + x;
                                rp[i] = p[srcInd];
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    result = bmp.Clone();
                }
                return result;
            }
        }
        
        /// <summary>
        /// Rotates the bitmap in any degree returns a new rotated WriteableBitmap.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="angle">Arbitrary angle in 360 Degrees (positive = clockwise).</param>
        /// <param name="crop">if true: keep the size, false: adjust canvas to new size</param>
        /// <returns>A new WriteableBitmap that is a rotated version of the input.</returns>
        public unsafe static WriteableBitmap RotateFree(this WriteableBitmap bmp, double angle, bool crop = true)
        {
            // rotating clockwise, so it's negative relative to Cartesian quadrants
            double cnAngle = -1.0 * (Math.PI / 180) * angle;
            
            // general iterators
            int i, j;
            // calculated indices in Cartesian coordinates
            int x, y;
            double fDistance, fPolarAngle;
            // for use in neighbouring indices in Cartesian coordinates
            int iFloorX, iCeilingX, iFloorY, iCeilingY;
            // calculated indices in Cartesian coordinates with trailing decimals
            double fTrueX, fTrueY;
            // for interpolation
            double fDeltaX, fDeltaY;

            // interpolated "top" pixels
            double fTopRed, fTopGreen, fTopBlue, fTopAlpha;

            // interpolated "bottom" pixels
            double fBottomRed, fBottomGreen, fBottomBlue, fBottomAlpha;

            // final interpolated colour components
            int iRed, iGreen, iBlue, iAlpha;
            
            int iCentreX, iCentreY;
            int iDestCentreX, iDestCentreY;
            int iWidth, iHeight, newWidth, newHeight;
            using (var bmpContext = bmp.GetBitmapContext())
            {
                iWidth = bmpContext.Width;
                iHeight = bmpContext.Height;
                    
                if (crop)
                {
                    newWidth = iWidth;
                    newHeight = iHeight;
                }
                else
                {
                    var rad = angle / (180 / Math.PI);
                    newWidth = (int)Math.Ceiling(Math.Abs(Math.Sin(rad) * iHeight) + Math.Abs(Math.Cos(rad) * iWidth));
                    newHeight = (int)Math.Ceiling(Math.Abs(Math.Sin(rad) * iWidth) + Math.Abs(Math.Cos(rad) * iHeight));
                }

                iCentreX = iWidth / 2;
                iCentreY = iHeight / 2;

                iDestCentreX = newWidth / 2;
                iDestCentreY = newHeight / 2;
                
                var bmBilinearInterpolation = BitmapFactory.New(newWidth, newHeight);
                    
                using (var bilinearContext = bmBilinearInterpolation.GetBitmapContext())
                {
                    var newp = bilinearContext.Pixels;
                    var oldp = bmpContext.Pixels;
                    var oldw = bmpContext.Width;
                    
                    // assigning pixels of destination image from source image
                    // with bilinear interpolation
                    for (i = 0; i < newHeight; ++i)
                    {
                        for (j = 0; j < newWidth; ++j)
                        {
                            // convert raster to Cartesian
                            x = j - iDestCentreX;
                            y = iDestCentreY - i;
                            
                            // convert Cartesian to polar
                            fDistance = Math.Sqrt(x * x + y * y);
                            if (x == 0)
                            {
                                if (y == 0)
                                {
                                    // centre of image, no rotation needed
                                    newp[i * newWidth + j] = oldp[iCentreY * oldw + iCentreX];
                                    continue;
                                }
                                if (y < 0)
                                {
                                    fPolarAngle = 1.5 * Math.PI;
                                }
                                else
                                {
                                    fPolarAngle = 0.5 * Math.PI;
                                }
                            }
                            else
                            {
                                fPolarAngle = Math.Atan2(y, x);
                            }
                            
                            // the crucial rotation part
                            // "reverse" rotate, so minus instead of plus
                            fPolarAngle -= cnAngle;
                            
                            // convert polar to Cartesian
                            fTrueX = fDistance * Math.Cos(fPolarAngle);
                            fTrueY = fDistance * Math.Sin(fPolarAngle);
                            
                            // convert Cartesian to raster
                            fTrueX = fTrueX + iCentreX;
                            fTrueY = iCentreY - fTrueY;
                            
                            iFloorX = (int)(Math.Floor(fTrueX));
                            iFloorY = (int)(Math.Floor(fTrueY));
                            iCeilingX = (int)(Math.Ceiling(fTrueX));
                            iCeilingY = (int)(Math.Ceiling(fTrueY));
                                
                            // check bounds
                            if (iFloorX < 0 || iCeilingX < 0 || iFloorX >= iWidth || iCeilingX >= iWidth || iFloorY < 0 ||
                                iCeilingY < 0 || iFloorY >= iHeight || iCeilingY >= iHeight)
                            {
                                continue;
                            }

                            fDeltaX = fTrueX - iFloorX;
                            fDeltaY = fTrueY - iFloorY;
                            
                            var clrTopLeft = oldp[iFloorY * oldw + iFloorX];
                            var clrTopRight = oldp[iFloorY * oldw + iCeilingX];
                            var clrBottomLeft = oldp[iCeilingY * oldw + iFloorX];
                            var clrBottomRight = oldp[iCeilingY * oldw + iCeilingX];
                            
                            fTopAlpha = (1 - fDeltaX) * ((clrTopLeft >> 24) & 0xFF) + fDeltaX * ((clrTopRight >> 24) & 0xFF);
                            fTopRed = (1 - fDeltaX) * ((clrTopLeft >> 16) & 0xFF) + fDeltaX * ((clrTopRight >> 16) & 0xFF);
                            fTopGreen = (1 - fDeltaX) * ((clrTopLeft >> 8) & 0xFF) + fDeltaX * ((clrTopRight >> 8) & 0xFF);
                            fTopBlue = (1 - fDeltaX) * (clrTopLeft & 0xFF) + fDeltaX * (clrTopRight & 0xFF);
                            
                            // linearly interpolate horizontally between bottom neighbours
                            fBottomAlpha = (1 - fDeltaX) * ((clrBottomLeft >> 24) & 0xFF) + fDeltaX * ((clrBottomRight >> 24) & 0xFF);
                            fBottomRed = (1 - fDeltaX) * ((clrBottomLeft >> 16) & 0xFF) + fDeltaX * ((clrBottomRight >> 16) & 0xFF);
                            fBottomGreen = (1 - fDeltaX) * ((clrBottomLeft >> 8) & 0xFF) + fDeltaX * ((clrBottomRight >> 8) & 0xFF);
                            fBottomBlue = (1 - fDeltaX) * (clrBottomLeft & 0xFF) + fDeltaX * (clrBottomRight & 0xFF);
                            
                            // linearly interpolate vertically between top and bottom interpolated results
                            iRed = (int)(Math.Round((1 - fDeltaY) * fTopRed + fDeltaY * fBottomRed));
                            iGreen = (int)(Math.Round((1 - fDeltaY) * fTopGreen + fDeltaY * fBottomGreen));
                            iBlue = (int)(Math.Round((1 - fDeltaY) * fTopBlue + fDeltaY * fBottomBlue));
                            iAlpha = (int)(Math.Round((1 - fDeltaY) * fTopAlpha + fDeltaY * fBottomAlpha));
                            
                            // make sure colour values are valid
                            if (iRed < 0)
                            {
                                iRed = 0;
                            }
                            if (iRed > 255)
                            {
                                iRed = 255;
                            }
                            if (iGreen < 0)
                            {
                                iGreen = 0;
                            }
                            if (iGreen > 255)
                            {
                                iGreen = 255;
                            }
                            if (iBlue < 0)
                            {
                                iBlue = 0;
                            }
                            if (iBlue > 255)
                            {
                                iBlue = 255;
                            }
                            if (iAlpha < 0)
                            {
                                iAlpha = 0;
                            }
                            if (iAlpha > 255)
                            {
                                iAlpha = 255;
                            }
                                                     
                            var a = iAlpha + 1;
                            newp[i * newWidth + j] = (iAlpha << 24) |
                                                     ((byte)((iRed * a) >> 8) << 16) |
                                                     ((byte)((iGreen * a) >> 8) << 8) |
                                                     ((byte)((iBlue * a) >> 8));
                        }
                    }
                    return bmBilinearInterpolation;
                }
            }
        }
        
        #endregion
        
        #region Flip
        
        /// <summary>
        /// Flips (reflects the image) eiter vertical or horizontal.
        /// </summary>
        /// <param name="bmp">The WriteableBitmap.</param>
        /// <param name="flipMode">The flip mode.</param>
        /// <returns>A new WriteableBitmap that is a flipped version of the input.</returns>
        public unsafe static WriteableBitmap Flip(this WriteableBitmap bmp, FlipMode flipMode)
        {
            using (var context = bmp.GetBitmapContext())
            {
                // Use refs for faster access (really important!) speeds up a lot!
                var w = context.Width;
                var h = context.Height;
                var p = context.Pixels;
                var i = 0;
                WriteableBitmap result = null;
                    
                if (flipMode == FlipMode.Horizontal)
                {
                    result = BitmapFactory.New(w, h);
                    using (var destContext = result.GetBitmapContext())
                    {
                        var rp = destContext.Pixels;
                        for (var y = h - 1; y >= 0; y--)
                        {
                            for (var x = 0; x < w; x++)
                            {
                                var srcInd = y * w + x;
                                rp[i] = p[srcInd];
                                i++;
                            }
                        }
                    }
                }
                else if (flipMode == FlipMode.Vertical)
                {
                    result = BitmapFactory.New(w, h);
                    using (var destContext = result.GetBitmapContext())
                    {
                        var rp = destContext.Pixels;
                        for (var y = 0; y < h; y++)
                        {
                            for (var x = w - 1; x >= 0; x--)
                            {
                                var srcInd = y * w + x;
                                rp[i] = p[srcInd];
                                i++;
                            }
                        }
                    }
                }
        
                return result;
            }
        }

        #endregion

        #endregion
    }
}