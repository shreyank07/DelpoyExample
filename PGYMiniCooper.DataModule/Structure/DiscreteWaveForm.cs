using ProdigyFramework.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PGYMiniCooper.DataModule.Structure
{
    public class DiscreteWaveFormCollection : IList<DiscreteWaveForm>, IDisposable
    {
        public const int DEFAULT_CAPACITY = (int)12.8e6;

        private static int capacity = DEFAULT_CAPACITY;
        private static BufferManager BufferManagerTimestamp;
        private static BufferManager BufferManagerData;
        private byte[] buffer_Timestamp;
        private byte[] buffer_Data;

        /// <summary>
        /// Initialize the interbal buffer.
        /// </summary>
        /// <param name="maxCapacity">Maximum number of <see cref="DiscreteWaveForm"/> points that can be stored.</param>
        public static void InitializeBuffer(int maxCapacity = DEFAULT_CAPACITY, int numberOfBuffers = 12)
        {
            capacity = maxCapacity;

            BufferManagerTimestamp?.Clear();
            BufferManagerData?.Clear();

            BufferManagerTimestamp = BufferManager.CreateBufferManager(numberOfBuffers, capacity * 8);
            BufferManagerData = BufferManager.CreateBufferManager(numberOfBuffers, capacity * 2);
        }

        public DiscreteWaveFormCollection() 
        {
            if (BufferManagerTimestamp == null)
                InitializeBuffer();

            buffer_Timestamp = BufferManagerTimestamp.TakeBuffer(capacity * 8);
            buffer_Data = BufferManagerData.TakeBuffer(capacity * 2);
        }

        public DiscreteWaveFormCollection(DiscreteWaveFormCollection collection) : this()
        {
            Buffer.BlockCopy(collection.buffer_Timestamp, 0, buffer_Timestamp, 0, collection.count * 8);
            Buffer.BlockCopy(collection.buffer_Data, 0, buffer_Data, 0, collection.count * 2);

            this.count = collection.count;
        }

        public DiscreteWaveForm this[int index] 
        {
            get
            {
                if (index >= count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return GetData(index);
            }
            set => throw new NotImplementedException(); 
        }

        private DiscreteWaveForm GetData(int index)
        {
            double timestamp = FastBitConverter.ToDoubleUnSafe(buffer_Timestamp, index * 8);
            UInt16 data = FastBitConverter.ToUInt16Unsafe(buffer_Data, index * 2);

            return new DiscreteWaveForm(timestamp, data);
        }

        private int count = 0;

        public int Count => count;

        public bool IsReadOnly => true;

        public void Add(DiscreteWaveForm item)
        {
            // Check if buffer is available
            if (count + 1 > capacity)
                throw new InternalBufferOverflowException();

            var timestamp_Bytes = BitConverter.GetBytes(item.TimeStamp);
            var data_Bytes = BitConverter.GetBytes(item.ChannelValue);

            Buffer.BlockCopy(timestamp_Bytes, 0, buffer_Timestamp, count * 8, timestamp_Bytes.Length);
            Buffer.BlockCopy(data_Bytes, 0, buffer_Data, count * 2, data_Bytes.Length);

            Interlocked.Exchange(ref count, count + 1);
        }

        public void Clear()
        {
            Interlocked.Exchange(ref count, 0);
        }

        public bool Contains(DiscreteWaveForm item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(DiscreteWaveForm[] array, int arrayIndex)
        {
            if ((arrayIndex + array.Length) > count) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < array.Length; i++)
                array[i] = GetData(arrayIndex + i);
        }

        public IEnumerator<DiscreteWaveForm> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        public int IndexOf(DiscreteWaveForm item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, DiscreteWaveForm item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(DiscreteWaveForm item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            // Return memory 
            BufferManagerTimestamp.ReturnBuffer(buffer_Timestamp);
            BufferManagerData.ReturnBuffer(buffer_Data);
        }
    }

    public struct DiscreteWaveForm
    {
        public DiscreteWaveForm(double timeStamp, UInt16 symbol)
        {
            this.TimeStamp = timeStamp;
            this.ChannelValue = symbol;
        }

        public double TimeStamp;

        public UInt16 ChannelValue;

        public int GetChannelState(int index)
        {
            return (this.ChannelValue >> index) & 0x01;
        }

        public int GetChannelState(eChannles channel)
        {
            return (this.ChannelValue >> (int)channel) & 0x01;
        }
    }
}
