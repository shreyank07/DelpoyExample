// <copyright file="BitmapContext.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

#region Header

#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prodigy.WaveformControls.Extensions;

#if NETFX_CORE
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Concurrent;

namespace Windows.UI.Xaml.Media.Imaging
#else
namespace Prodigy.WaveformControls.Extensions
#endif
{
    /// <summary>
    /// A disposable cross-platform wrapper around a WriteableBitmap, allowing a common API for Silverlight + WPF with locking + unlocking if necessary
    /// </summary>
    /// <remarks>Attempting to put as many preprocessor hacks in this file, to keep the rest of the codebase relatively clean</remarks>
    public
    #if SILVERLIGHT
    #else
    unsafe
    #endif
    struct BitmapContext : IDisposable
    {
        private readonly ReadWriteMode mode;
        #if SILVERLIGHT
        #elif NETFX_CORE
      private readonly static IDictionary<WriteableBitmap, int> UpdateCountByBmp = new ConcurrentDictionary<WriteableBitmap, int>();
      private readonly static IDictionary<WriteableBitmap, int[]> PixelCacheByBmp = new ConcurrentDictionary<WriteableBitmap, int[]>();
      private int length;
      private int[] pixels;
        #else
        private readonly static IDictionary<WriteableBitmap, int> UpdateCountByBmp = new System.Collections.Concurrent.ConcurrentDictionary<WriteableBitmap, int>();

        #endif
        
        /// <summary>
        /// The Bitmap
        /// </summary>
        public WriteableBitmap WriteableBitmap { get; private set; }
        
        /// <summary>
        /// Width of the bitmap
        /// </summary>
        public int Width
        {
            get
            {
                return this.WriteableBitmap.PixelWidth;
            }
        }
        
        /// <summary>
        /// Height of the bitmap
        /// </summary>
        public int Height
        {
            get
            {
                return this.WriteableBitmap.PixelHeight;
            }
        }
        
        /// <summary>
        /// Creates an instance of a BitmapContext, with default mode = ReadWrite
        /// </summary>
        /// <param name="writeableBitmap"></param>
        public BitmapContext(WriteableBitmap writeableBitmap) : this(writeableBitmap, ReadWriteMode.ReadWrite)
        {
        }
        
        /// <summary>
        /// Creates an instance of a BitmapContext, with specified ReadWriteMode
        /// </summary>
        /// <param name="writeableBitmap"></param>
        /// <param name="mode"></param>
        public BitmapContext(WriteableBitmap writeableBitmap, ReadWriteMode mode) : this()
        {
            this.WriteableBitmap = writeableBitmap;
            this.mode = mode;
            
            #if NETFX_CORE
         // Ensure the bitmap is in the dictionary of mapped Instances
         if (!UpdateCountByBmp.ContainsKey(writeableBitmap))
         {
            // Set UpdateCount to 1 for this bitmap 
            UpdateCountByBmp.Add(writeableBitmap, 1);
            length = writeableBitmap.PixelWidth * writeableBitmap.PixelHeight;
            pixels = new int[length];
            CopyPixels();
            PixelCacheByBmp.Add(writeableBitmap, pixels);
         }
         else
         {
            // For previously contextualized bitmaps increment the update count
            IncrementRefCount(writeableBitmap);
            pixels = PixelCacheByBmp[writeableBitmap];
            length = pixels.Length;
         }
            #elif SILVERLIGHT
            #else
            // Check if it's the Pbgra32 pixel format
            if (writeableBitmap.Format != PixelFormats.Pbgra32)
            {
                throw new ArgumentException("The input WriteableBitmap needs to have the Pbgra32 pixel format. Use the BitmapFactory.ConvertToPbgra32Format method to automatically convert any input BitmapSource to the right format accepted by this class.", "writeableBitmap");
            }
            // Mode is used to invalidate the bmp at the end of the update if mode==ReadWrite
            mode = ReadWriteMode.ReadWrite;
            // Ensure the bitmap is in the dictionary of mapped Instances
            if (!UpdateCountByBmp.ContainsKey(writeableBitmap))
            {
                // Set UpdateCount to 1 for this bitmap 
                UpdateCountByBmp.Add(writeableBitmap, 1);
                // Lock the bitmap
                writeableBitmap.Lock();
            }
            else
            {
                // For previously contextualized bitmaps increment the update count
                IncrementRefCount(writeableBitmap);
            }
            this.Pixels = (int*)writeableBitmap.BackBuffer;
            #endif
        }
        
        #if NETFX_CORE
      private unsafe void CopyPixels()
      {
         var data = writeableBitmap.PixelBuffer.ToArray();
         fixed (byte* srcPtr = data)
         {
            fixed (int* dstPtr = pixels)
            {
               for (var i = 0; i < length; i++)
               {
                  dstPtr[i] = (srcPtr[i * 4 + 3] << 24) | (srcPtr[i * 4 + 2] << 16) | (srcPtr[i * 4 + 1] << 8) | srcPtr[i * 4 + 0];
               }
            }
         }
      }
        #endif

        #if SILVERLIGHT
        
      /// <summary>
      /// Gets the Pixels array 
      /// </summary>        
      public int[] Pixels { get { return writeableBitmap.Pixels; } }
        
      /// <summary>
      /// Gets the length of the Pixels array 
      /// </summary>
      public int Length { get { return writeableBitmap.Pixels.Length; } }
        
      /// <summary>
      /// Performs a Copy operation from source BitmapContext to destination BitmapContext
      /// </summary>
      /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      public static void BlockCopy(BitmapContext src, int srcOffset, BitmapContext dest, int destOffset, int count)
      {
         Buffer.BlockCopy(src.Pixels, srcOffset, dest.Pixels, destOffset, count);
      }
        
      /// <summary>
      /// Performs a Copy operation from source Array to destination BitmapContext
      /// </summary>
      /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      public static void BlockCopy(Array src, int srcOffset, BitmapContext dest, int destOffset, int count)
      {
         Buffer.BlockCopy(src, srcOffset, dest.Pixels, destOffset, count);
      }
        
      /// <summary>
      /// Performs a Copy operation from source BitmapContext to destination Array
      /// </summary>
      /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
      public static void BlockCopy(BitmapContext src, int srcOffset, Array dest, int destOffset, int count)
      {
         Buffer.BlockCopy(src.Pixels, srcOffset, dest, destOffset, count);
      }
        
      /// <summary>
      /// Clears the BitmapContext, filling the underlying bitmap with zeros
      /// </summary>
      public void Clear()
      {
         var pixels = writeableBitmap.Pixels;
         Array.Clear(pixels, 0, pixels.Length);
      }
        
      /// <summary>
      /// Disposes this instance if the underlying platform needs that.
      /// </summary>
      public void Dispose()
      {
         // For silverlight, do nothing
      }
        #else
        /// <summary>
        /// The pixels as ARGB integer values, where each channel is 8 bit.
        /// </summary>
        public int* Pixels { get; private set; }
        
        /// <summary>
        /// The number of pixels.
        /// </summary>
        public int Length
        {
            [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
            get
            {
                double pixelWidth = this.WriteableBitmap.BackBufferStride / WriteableBitmapExtensions.SizeOfArgb;
                double pixelHeight = this.WriteableBitmap.PixelHeight;
                return (int)(pixelWidth * pixelHeight);
            }
        }
        
        /// <summary>
        /// Performs a Copy operation from source Bto destination BitmapContext
        /// </summary>
        /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static unsafe void BlockCopy(BitmapContext src, int srcOffset, BitmapContext dest, int destOffset, int count)
        {
            NativeMethods.CopyUnmanagedMemory((byte*)src.Pixels, srcOffset, (byte*)dest.Pixels, destOffset, count);
        }
        
        /// <summary>
        /// Performs a Copy operation from source Array to destination BitmapContext
        /// </summary>
        /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static unsafe void BlockCopy(int[] src, int srcOffset, BitmapContext dest, int destOffset, int count)
        {
            fixed (int* srcPtr = src)
            {
                NativeMethods.CopyUnmanagedMemory((byte*)srcPtr, srcOffset, (byte*)dest.Pixels, destOffset, count);
            }
        }
        
        /// <summary>
        /// Performs a Copy operation from source Array to destination BitmapContext
        /// </summary>
        /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static unsafe void BlockCopy(byte[] src, int srcOffset, BitmapContext dest, int destOffset, int count)
        {
            fixed (byte* srcPtr = src)
            {
                NativeMethods.CopyUnmanagedMemory(srcPtr, srcOffset, (byte*)dest.Pixels, destOffset, count);
            }
        }
        
        /// <summary>
        /// Performs a Copy operation from source BitmapContext to destination Array
        /// </summary>
        /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static unsafe void BlockCopy(BitmapContext src, int srcOffset, byte[] dest, int destOffset, int count)
        {
            fixed (byte* destPtr = dest)
            {
                NativeMethods.CopyUnmanagedMemory((byte*)src.Pixels, srcOffset, destPtr, destOffset, count);
            }
        }
        
        /// <summary>
        /// Performs a Copy operation from source BitmapContext to destination Array
        /// </summary>
        /// <remarks>Equivalent to calling Buffer.BlockCopy in Silverlight, or native memcpy in WPF</remarks>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public static unsafe void BlockCopy(BitmapContext src, int srcOffset, int[] dest, int destOffset, int count)
        {
            fixed (int* destPtr = dest)
            {
                NativeMethods.CopyUnmanagedMemory((byte*)src.Pixels, srcOffset, (byte*)destPtr, destOffset, count);
            }
        }
        
        /// <summary>
        /// Clears the BitmapContext, filling the underlying bitmap with zeros
        /// </summary>
        [System.Runtime.TargetedPatchingOptOut("Candidate for inlining across NGen boundaries for performance reasons")]
        public void Clear()
        {
            NativeMethods.SetUnmanagedMemory(this.WriteableBitmap.BackBuffer, 0, this.WriteableBitmap.BackBufferStride * this.WriteableBitmap.PixelHeight);
        }
        
        /// <summary>
        /// Disposes the BitmapContext, unlocking it and invalidating if WPF
        /// </summary>
        public void Dispose()
        {
            // Decrement the update count. If it hits zero
            if (DecrementRefCount(this.WriteableBitmap) == 0)
            {
                // Remove this bitmap from the update map 
                UpdateCountByBmp.Remove(this.WriteableBitmap);
                
                // Invalidate the bitmap if ReadWrite mode
                if (this.mode == ReadWriteMode.ReadWrite)
                {
                    this.WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, this.WriteableBitmap.PixelWidth, this.WriteableBitmap.PixelHeight));
                }
                
                // Unlock the bitmap
                this.WriteableBitmap.Unlock();
            }
        }

        #endif
        
        #if SILVERLIGHT
        #else
        private static void IncrementRefCount(WriteableBitmap target)
        {
            UpdateCountByBmp[target]++;
        }
        
        private static int DecrementRefCount(WriteableBitmap target)
        {
            int current = UpdateCountByBmp[target];
            current--;
            UpdateCountByBmp[target] = current;
            return current;
        }
        #endif
    }
}