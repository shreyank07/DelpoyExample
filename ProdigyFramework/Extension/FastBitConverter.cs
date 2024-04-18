using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Extension
{
    public static class FastBitConverter
    {
        //
        // Summary:
        //     Returns a 16-bit unsigned integer converted from two bytes at a specified position
        //     in a byte array.
        //
        // Parameters:
        //   value:
        //     The array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 16-bit unsigned integer formed by two bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex equals the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        public static ushort ToUInt16Unsafe(byte[] value, int startIndex)
        {
            return (ushort)ToInt16Unsafe(value, startIndex);
        }

        //
        // Summary:
        //     Returns a 32-bit unsigned integer converted from four bytes at a specified position
        //     in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 32-bit unsigned integer formed by four bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex is greater than or equal to the length of value minus 3, and is less
        //     than or equal to the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        public static uint ToUInt32Unsafe(byte[] value, int startIndex)
        {
            return (uint)ToInt32Unsafe(value, startIndex);
        }

        //
        // Summary:
        //     Returns a 16-bit signed integer converted from two bytes at a specified position
        //     in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 16-bit signed integer formed by two bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex equals the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [SecuritySafeCritical]
        public unsafe static short ToInt16Unsafe(byte[] value, int startIndex)
        {
            fixed (byte* ptr = &value[startIndex])
            {
                if (startIndex % 2 == 0)
                {
                    return *(short*)ptr;
                }

                if (BitConverter.IsLittleEndian)
                {
                    return (short)(*ptr | (ptr[1] << 8));
                }

                return (short)((*ptr << 8) | ptr[1]);
            }
        }

        //
        // Summary:
        //     Returns a 32-bit signed integer converted from four bytes at a specified position
        //     in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 32-bit signed integer formed by four bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex is greater than or equal to the length of value minus 3, and is less
        //     than or equal to the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [SecuritySafeCritical]
        public unsafe static int ToInt32Unsafe(byte[] value, int startIndex)
        {
            fixed (byte* ptr = &value[startIndex])
            {
                if (startIndex % 4 == 0)
                {
                    return *(int*)ptr;
                }

                if (BitConverter.IsLittleEndian)
                {
                    return *ptr | (ptr[1] << 8) | (ptr[2] << 16) | (ptr[3] << 24);
                }

                return (*ptr << 24) | (ptr[1] << 16) | (ptr[2] << 8) | ptr[3];
            }
        }

        //
        // Summary:
        //     Returns a 64-bit signed integer converted from eight bytes at a specified position
        //     in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A 64-bit signed integer formed by eight bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex is greater than or equal to the length of value minus 7, and is less
        //     than or equal to the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [SecuritySafeCritical]
        public unsafe static long ToInt64_Unsafe(byte[] value, int startIndex)
        {
            fixed (byte* ptr = &value[startIndex])
            {
                if (startIndex % 8 == 0)
                {
                    return *(long*)ptr;
                }

                if (BitConverter.IsLittleEndian)
                {
                    int num = *ptr | (ptr[1] << 8) | (ptr[2] << 16) | (ptr[3] << 24);
                    int num2 = ptr[4] | (ptr[5] << 8) | (ptr[6] << 16) | (ptr[7] << 24);
                    return (uint)num | ((long)num2 << 32);
                }

                int num3 = (*ptr << 24) | (ptr[1] << 16) | (ptr[2] << 8) | ptr[3];
                int num4 = (ptr[4] << 24) | (ptr[5] << 16) | (ptr[6] << 8) | ptr[7];
                return (uint)num4 | ((long)num3 << 32);
            }
        }

        //
        // Summary:
        //     Returns a double-precision floating point number converted from eight bytes at
        //     a specified position in a byte array.
        //
        // Parameters:
        //   value:
        //     An array of bytes.
        //
        //   startIndex:
        //     The starting position within value.
        //
        // Returns:
        //     A double precision floating point number formed by eight bytes beginning at startIndex.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     startIndex is greater than or equal to the length of value minus 7, and is less
        //     than or equal to the length of value minus 1.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     startIndex is less than zero or greater than the length of value minus 1.
        [SecuritySafeCritical]
        public unsafe static double ToDoubleUnSafe(byte[] value, int startIndex)
        {
            long num = ToInt64_Unsafe(value, startIndex);
            return *(double*)(&num);
        }
    }
}
