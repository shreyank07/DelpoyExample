using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Extension
{
    /// <summary>
    /// This class can compress / decompress the data
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Method to compress the data
        /// </summary>
        /// <param name="input">The data to compress</param>
        /// <param name="offset">The byte offset to start the compression from</param>
        /// <param name="count">The byte count to compress </param>
        /// <param name="output">The buffer to write the compressed data. The size of buffer should be more then or equal to the size of compressed data.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="NotSupportedException">Exception is thown when the buffer is less then the compressed data</exception>
        public static long Compress(byte[] input, int offset, int count, ref byte[] output, CompressionLevel level = CompressionLevel.Optimal)
        {
            long compressedDataLength = 0;
            using (MemoryStream outputStream = new MemoryStream(output))
            {
                compressedDataLength = Compress(input, offset, count, outputStream, level);
            }
            return compressedDataLength;
        }

        /// <summary>
        /// Method to compress the data
        /// </summary>
        /// <param name="input">The data to compress</param>
        /// <param name="offset">The byte offset to start the compression from</param>
        /// <param name="count">The byte count to compress </param>
        /// <param name="outputStream">The stream to write the compressed data.</param>
        /// <param name="level">Compression level to use</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        public static long Compress(byte[] input, int offset, int count, Stream outputStream, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (DeflateStream dstream = new DeflateStream(outputStream, level, true))
            {
                dstream.Write(input, offset, count);
            }
            return outputStream.Position;
        }

        /// <summary>
        /// Method to compress the data
        /// </summary>
        /// <param name="input">The data to compress</param>
        /// <param name="offset">The byte offset to start the compression from</param>
        /// <param name="count">The byte count to compress </param>
        /// <param name="outputStream">The stream to write the compressed data.</param>
        /// <param name="level">Compression level to use</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        public static long Compress(IList<byte[]> inputBufferList, Stream outputStream, CompressionLevel level = CompressionLevel.Optimal)
        {
            using (DeflateStream dstream = new DeflateStream(outputStream, level, true))
            {
                for (int i = 0; i < inputBufferList.Count; i++)
                {
                    byte[] input = inputBufferList[i];
                    dstream.Write(input, 0, input.Length);
                }
            }
            return outputStream.Position;
        }

        /// <summary>
        /// Method to decompress the data
        /// </summary>
        /// <param name="data">The data to decompress</param>
        /// <param name="offset">The byte offset to start the decompression from</param>
        /// <param name="count">The byte count to decompress </param>
        /// <param name="output">The buffer to write the decompressed data. The size of buffer should be more then or equal to the size of decompressed data.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="NotSupportedException">Exception is thown when the buffer is less then the decompressed data</exception>
        public static long Decompress(Stream inputStream, ref byte[] output)
        {
            int decompressedDataLength = 0;
            using (DeflateStream dstream = new DeflateStream(inputStream, CompressionMode.Decompress, true))
            {
                // Read 512KB 
                int bytesRead;
                while (inputStream.Position < inputStream.Length && (bytesRead = dstream.Read(output, decompressedDataLength, 524288)) != 0)
                {
                    decompressedDataLength += bytesRead;
                }
            }
            return decompressedDataLength;
        }

        /// <summary>
        /// Method to decompress the data
        /// </summary>
        /// <param name="input">The data to decompress</param>
        /// <param name="offset">The byte offset to start the decompression from</param>
        /// <param name="count">The byte count to decompress </param>
        /// <param name="output">The buffer to write the decompressed data. The size of buffer should be more then or equal to the size of decompressed data.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="NotSupportedException">Exception is thown when the buffer is less then the decompressed data</exception>
        public static long Decompress(byte[] input, int offset, int count, ref byte[] output)
        {
            using (MemoryStream inputStream = new MemoryStream(input, offset, count))
            { 
                return Decompress(inputStream, ref output);
            }
        }

        /// <summary>
        /// Method to compress the data
        /// </summary>
        /// <param name="input">The data to compress</param>
        /// <param name="output">The buffer to write the compressed data. The size of buffer should be more then or equal to the size of compressed data.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="NotSupportedException">Exception is thown when the buffer is less then the compressed data</exception>
        public static long Compress(byte[] input, ref byte[] output)
        {
            return Compress(input, 0, input.Length, ref output);
        }

        /// <summary>
        /// Method to decompress the data
        /// </summary>
        /// <param name="input">The data to decompress</param>
        /// <param name="output">The buffer to write the decompressed data. The size of buffer should be more then or equal to the size of decompressed data.</param>
        /// <returns>The number of bytes written to the buffer.</returns>
        /// <exception cref="NotSupportedException">Exception is thown when the buffer is less then the decompressed data</exception>
        public static long Decompress(byte[] input, ref byte[] output)
        {
            return Decompress(input, 0, input.Length, ref output);
        }
    }
}
