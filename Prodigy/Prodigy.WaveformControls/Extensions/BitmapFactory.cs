// <copyright file="BitmapFactory.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

#region Header

#endregion

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if NETFX_CORE
namespace Windows.UI.Xaml.Media.Imaging
#else
namespace Prodigy.WaveformControls.Extensions
#endif
{
    /// <summary>
    /// Cross-platform factory for WriteableBitmaps
    /// </summary>
    public static class BitmapFactory
    {
        /// <summary>
        /// Creates a new WriteableBitmap of the specified width and height
        /// </summary>
        /// <remarks>For WPF the default DPI is 96x96 and PixelFormat is Pbgra32</remarks>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <returns></returns>
        public static WriteableBitmap New(int pixelWidth, int pixelHeight)
        {
            //Throws system.Argument.Exception
            //Value does not fall within the expected range.
            if (pixelWidth == 0)
            {
                pixelWidth = 1;
            }
            if (pixelHeight == 0)
            {
                pixelHeight = 1;
            }
            
            #if SILVERLIGHT
         return new WriteableBitmap(pixelWidth, pixelHeight);
            #elif NETFX_CORE
         return new WriteableBitmap(pixelWidth, pixelHeight);
            #else
            return new WriteableBitmap(pixelWidth, pixelHeight, 96.0, 96.0, PixelFormats.Pbgra32, null);
            #endif
        }
        
        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
        
        public static int GetdpiX()
        {
            IntPtr hDc = NativeMethods.GetDC(IntPtr.Zero);
            if (hDc != IntPtr.Zero)
            {
                return NativeMethods.GetDeviceCaps(hDc, LOGPIXELSX);                
            }
            else
            {
                throw new ArgumentNullException("Failed to get DC.");
            }
        }
        
        public static int GetdpiY()
        {
            IntPtr hDc = NativeMethods.GetDC(IntPtr.Zero);
            if (hDc != IntPtr.Zero)
            {
                return NativeMethods.GetDeviceCaps(hDc, LOGPIXELSY);
            }
            else
            {
                throw new ArgumentNullException("Failed to get DC.");
            }
        }
        
        #if !SILVERLIGHT || !NETFX_CORE
        /// <summary>
        /// Converts the input BitmapSource to the Pbgra32 format WriteableBitmap which is internally used by the WriteableBitmapEx.
        /// </summary>
        /// <param name="source">The source bitmap.</param>
        /// <returns></returns>
        public static WriteableBitmap ConvertToPbgra32Format(BitmapSource source)
        {
            // Convert to Pbgra32 if it's a different format
            if (source.Format == PixelFormats.Pbgra32)
            {
                return new WriteableBitmap(source);
            }
            
            var formatedBitmapSource = new FormatConvertedBitmap();
            formatedBitmapSource.BeginInit();
            formatedBitmapSource.Source = source;
            formatedBitmapSource.DestinationFormat = PixelFormats.Pbgra32;
            formatedBitmapSource.EndInit();
            return new WriteableBitmap(formatedBitmapSource);
        }
        #endif
    }
}