using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prodigy.Framework.Interfaces;

namespace Prodigy.Framework.Collections
{
    /// <summary>
    /// Specialized list implementation that provides data virtualization. The collection is divided up into pages,
    /// and pages are dynamically fetched from the IItemsProvider when required. Stale pages are removed after a
    /// configurable period of time.
    /// Intended for use with large collections on a network or disk resource that cannot be instantiated locally
    /// due to memory consumption or fetch latency.
    /// </summary>
    /// <remarks>
    /// The IList implmentation is not fully complete, but should be sufficient for use as read only collection 
    /// data bound to a suitable ItemsControl.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class VirtualizingCollection<T> : IEnumerable, IList<T>, IList where T : DataWrapper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
        {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
            _pageTimeout = pageTimeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize)
        {
            _itemsProvider = itemsProvider;
            _pageSize = pageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        public VirtualizingCollection(IItemsProvider<T> itemsProvider)
        {
            _itemsProvider = itemsProvider;
        }

        #endregion

        #region ItemsProvider

        private readonly IItemsProvider<T> _itemsProvider;

        /// <summary>
        /// Gets the items provider.
        /// </summary>
        /// <value>The items provider.</value>
        public IItemsProvider<T> ItemsProvider
        {
            get { return _itemsProvider; }
        }

        #endregion

        #region PageSize

        private readonly int _pageSize = 100;

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return _pageSize; }
        }

        #endregion

        #region PageTimeout


        private readonly long _pageTimeout = 5000;

        /// <summary>
        /// Gets the page timeout.
        /// </summary>
        /// <value>The page timeout.</value>
        public long PageTimeout
        {
            get { return _pageTimeout; }
        }

        #endregion

        #region IList<T>, IList : T is DataWrapper

        #region Count

        private int _count = 0;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The first time this property is accessed, it will fetch the count from the IItemsProvider.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return _count;
            }
            protected set
            {
                _count = value;
            }
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets the item at the specified index. This property will fetch
        /// the corresponding page from the IItemsProvider if required.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                // determine which page and offset within page
                int pageIndex = index / PageSize;
                int pageOffset = index % PageSize;

                // request primary page
                RequestPage(pageIndex);

                // update previous page touch time if exists
                // update next page touch time if exists
                if (pageIndex > 0)
                    RefreshPage(pageIndex - 1);

                if (pageIndex < Count / PageSize)
                    RefreshPage(pageIndex + 1);

                //// User VirtualizingPanel.CacheLength
                //// if accessing upper 50% then request next page
                //if (pageOffset > PageSize / 2 && pageIndex < Count / PageSize)
                //    RequestPage(pageIndex + 1);

                ////// if accessing lower 50% then request prev page
                //if (pageOffset < PageSize / 2 && pageIndex > 0)
                //    RequestPage(pageIndex - 1);

                // remove stale pages
                CleanUpPages();

                // return requested item
                return _pages[pageIndex].Items[pageOffset];
            }
            set { throw new NotSupportedException(); }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region IEnumerator<DataWrapper<T>>, IEnumerator

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// This method should be avoided on large collections due to poor performance.
        /// </remarks>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Add

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Contains

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// Always false.
        /// </returns>
        public bool Contains(T item)
        {
            foreach (DataPage<T> page in _pages.Values)
            {
                if (page.Items.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Clear

        /// <summary>
        /// TODO
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IndexOf

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// TODO
        /// </returns>
        public int IndexOf(T item)
        {
            foreach (KeyValuePair<int, DataPage<T>> keyValuePair in _pages)
            {
                int indexWithinPage = keyValuePair.Value.Items.IndexOf(item);
                if (indexWithinPage != -1)
                {
                    return PageSize * keyValuePair.Key + indexWithinPage;
                }
            }
            return -1;
        }

        #endregion

        #region Insert

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region CopyTo

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>Always true.
        /// </returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>Always false.
        /// </returns>
        public bool IsFixedSize
        {
            get { return false; }
        }

        #endregion

        #endregion

        #region Paging

        protected Dictionary<int, DataPage<T>> _pages = new Dictionary<int, DataPage<T>>();

        /// <summary>
        /// Cleans up any stale pages that have not been accessed in the period dictated by PageTimeout.
        /// </summary>
        public void CleanUpPages()
        {
            lock (_pages)
            {
                int[] keys = _pages.Keys.ToArray();

                // datagrid after being idle for more than PageTimeout clears all pages
                // solution
                // skip first key as it will be accessed regularly
                // get last touch time from rest of the pages
                // use lastAccessed time instead of DateTime.Now
                DateTime lastAccessed = DateTime.Now;
                if (_pages.Skip(1).Count() > 0)
                {
                    lastAccessed = _pages.Skip(1).OrderBy(p => p.Value.TouchTime).Last().Value.TouchTime;
                }
                foreach (int key in keys)
                {
                    // page 0 is a special case, since WPF ItemsControl access the first item frequently
                    if (key != 0 && (lastAccessed - _pages[key].TouchTime).TotalMilliseconds > PageTimeout)
                    {
                        bool removePage = true;
                        DataPage<T> page;
                        if (_pages.TryGetValue(key, out page))
                        {
                            removePage = !page.IsInUse;
                        }

                        if (removePage)
                        {
                            _pages.Remove(key);
                            Trace.WriteLine("Removed Page: " + key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes a request for the specified page, creating the necessary slots in the dictionary,
        /// and updating the page touch time.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RequestPage(int pageIndex)
        {
            if (!_pages.ContainsKey(pageIndex))
            {
                // Create a page of empty data wrappers.
                int pageLength = Math.Min(this.PageSize, this.Count - pageIndex * this.PageSize);
                DataPage<T> page = new DataPage<T>(pageIndex * this.PageSize, pageLength);
                _pages.Add(pageIndex, page);
                Trace.WriteLine("Added page: " + pageIndex);
                LoadPage(pageIndex, pageLength);
            }
            else
            {
                // Create a page of empty data wrappers when new data is added
                int pageLength = Math.Min(this.PageSize, this.Count - pageIndex * this.PageSize);
                if (_pages[pageIndex].Items.Count != pageLength)
                {
                    DataPage<T> page = _pages[pageIndex];
                    page.UpdateItems(pageLength);
                    LoadPage(pageIndex, pageLength);
                }

                _pages[pageIndex].TouchTime = DateTime.Now;
            }
        }

        /// <summary>
        /// if previous page was accessed or next page is accessed,
        /// than update the page touch time if page exists.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        protected virtual void RefreshPage(int pageIndex)
        {
            if (_pages.ContainsKey(pageIndex))
            {
                // Create a page of empty data wrappers when new data is added
                int pageLength = Math.Min(this.PageSize, this.Count - pageIndex * this.PageSize);
                if (_pages[pageIndex].Items.Count != pageLength)
                {
                    DataPage<T> page = _pages[pageIndex];
                    page.UpdateItems(pageLength);
                    LoadPage(pageIndex, pageLength);
                }

                _pages[pageIndex].TouchTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Removes all cached pages. This is useful when the count of the 
        /// underlying collection changes.
        /// </summary>
        protected void EmptyCache()
        {
            _pages = new Dictionary<int, DataPage<T>>();
        }

        #endregion

        #region Load methods

        public virtual void UpdateCount(int newCount)
        {
            this.Count = newCount;
        }

        /// <summary>
        /// Loads the page of items.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageLength">Number of items in the page.</param>
        protected virtual void LoadPage(int pageIndex, int pageLength)
        {
            UpdatePage(pageIndex, pageLength);
        }

        #endregion

        #region Fetch methods
        protected void UpdatePage(int pageIndex, int pageLength)
        {
            ItemsProvider.UpdateRange(_pages[pageIndex].Items, pageIndex * PageSize, pageLength);            
        }

        #endregion

        #region Internally Datais stored in pages <DataPage<TPageType>> immplimentation
        protected class DataPage<TPage> where TPage : DataWrapper
        {
            public DataPage(int firstIndex, int pageLength)
            {
                this.firstIndex = firstIndex;
                this.Items = new List<TPage>(pageLength);
                for (int i = 0; i < pageLength; i++)
                {
                    var item = ((TPage)Activator.CreateInstance(typeof(TPage)));
                    item.Index = firstIndex + i;
                    this.Items.Add(item);
                }
                this.TouchTime = DateTime.Now;
            }

            private int firstIndex;

            public IList<TPage> Items { get; set; }

            public DateTime TouchTime { get; set; }

            public bool IsInUse
            {
                get { return this.Items.Any(wrapper => wrapper.IsInUse); }
            }

            public void UpdateItems(int pageLength)
            {
                int i = this.Items.Count;
                for (; i < pageLength; i++)
                {
                    var item = ((TPage)Activator.CreateInstance(typeof(TPage)));
                    item.Index = firstIndex + i;
                    this.Items.Add(item);
                }
            }
        }
        #endregion
    }
}
