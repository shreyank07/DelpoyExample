using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Prodigy.WaveformControls.View;

namespace Prodigy.WaveformControls.Plots.Collections
{
    [TypeConverterAttribute(typeof(PlotCollectionConverter))]
    public sealed class PlotCollection : Freezable, IFormattable,
    IList, ICollection, IEnumerable, IList<PlotView>, ICollection<Point>,
    IEnumerable<Point>
    {
        PlotView IList<PlotView>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        int ICollection<PlotView>.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ICollection<Point>.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int ICollection.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<PlotView>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<Point>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        void ICollection<Point>.Add(Point item)
        {
            throw new NotImplementedException();
        }

        void ICollection<PlotView>.Add(PlotView item)
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<Point>.Clear()
        {
            throw new NotImplementedException();
        }

        void ICollection<PlotView>.Clear()
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<PlotView>.Contains(PlotView item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<Point>.Contains(Point item)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<PlotView>.CopyTo(PlotView[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection<Point>.CopyTo(Point[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator<Point> IEnumerable<Point>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<PlotView> IEnumerable<PlotView>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        int IList<PlotView>.IndexOf(PlotView item)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList<PlotView>.Insert(int index, PlotView item)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<Point>.Remove(Point item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<PlotView>.Remove(PlotView item)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList<PlotView>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }
    }

    internal class PlotCollectionConverter
    {
    }
}
