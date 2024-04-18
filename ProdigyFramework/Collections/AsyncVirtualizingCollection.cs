using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Prodigy.Framework.Interfaces;

namespace Prodigy.Framework.Collections
{
    /// <summary>
    /// Derived VirtualizatingCollection, performing loading asychronously.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class AsyncVirtualizingCollection<T> : VirtualizingCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : DataWrapper
    {
        private TaskFactory _taskFactory;
        private Prodigy.Framework.Helpers.StackTaskScheduler _taskSchedular;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncVirtualizingCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="itemsProvider">The items provider.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageTimeout">The page timeout.</param>
        public AsyncVirtualizingCollection(IItemsProvider<T> itemsProvider, int pageSize, int pageTimeout)
            : base(itemsProvider, pageSize, pageTimeout)
        {
            _synchronizationContext = SynchronizationContext.Current;
            _taskSchedular = new Prodigy.Framework.Helpers.StackTaskScheduler(5);
            _taskFactory = new TaskFactory(_taskSchedular);
        }
        #region SynchronizationContext

        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Gets the synchronization context used for UI-related operations. This is obtained as
        /// the current SynchronizationContext when the AsyncVirtualizingCollection is created.
        /// </summary>
        /// <value>The synchronization context.</value>
        protected SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
        }

        #endregion

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raises the <see cref="E:CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            //NotifyCollectionChangedEventHandler h = CollectionChanged;
            //if (h != null)
            //    h(this, e);

            // Recommended is to avoid reentry 
            // in collection changed event while collection
            // is getting changed on other thread.
            NotifyCollectionChangedEventHandler eventHandler =
                  this.CollectionChanged;
            if (eventHandler == null)
            {
                return;
            }

            // Walk thru invocation list.
            Delegate[] delegates = eventHandler.GetInvocationList();

            foreach
            (NotifyCollectionChangedEventHandler handler in delegates)
            {
                // If the subscriber is a DispatcherObject and different thread.
                DispatcherObject dispatcherObject
                     = handler.Target as DispatcherObject;

                if (dispatcherObject != null
                       && !dispatcherObject.CheckAccess())
                {
                    // Invoke handler in the target dispatcher's thread... 
                    // asynchronously for better responsiveness.
                    dispatcherObject.Dispatcher.BeginInvoke
                          (DispatcherPriority.DataBind, handler, this, e);
                }
                else
                {
                    // Execute handler as is.
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Fires the collection reset event.
        /// </summary>
        private void FireCollectionReset()
        {
            NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(e);
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler h = PropertyChanged;
            if (h != null)
                h(this, e);
        }

        /// <summary>
        /// Fires the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            OnPropertyChanged(e);
        }

        #endregion

        #region Threads Avaliable

        private bool threadsAvaliable = true;

        public bool ThreadsAvaliable
        {
            get
            {
                return threadsAvaliable;
            }
            set
            {
                threadsAvaliable = value;
                FirePropertyChanged("ThreadsAvaliable");
            }
        }

        #endregion

        #region Load overrides

        public override void UpdateCount(int newCount)
        {
            if (SynchronizationContext == null)
                TakeNewCount(newCount);
            else
                SynchronizationContext.Send(TakeNewCount, newCount);
        }

        private void TakeNewCount(object newCount)
        {
            this.TakeNewCount((int)newCount);
        }

        private void TakeNewCount(int newCount)
        {
            if (newCount != this.Count)
            {
                this.Count = newCount;
                //don't empty the cache because data is static
                if (newCount == 0)
                {
                    // empty the catch
                    this.EmptyCache();

                    // kill all running threads
                    _taskSchedular.CleanUpTasks();
                }
                FireCollectionReset();
            }
        }

        /// <summary>
        /// Asynchronously loads the page.
        /// </summary>
        /// <param name="index">The index.</param>
        protected override void LoadPage(int pageIndex, int pageLength)
        {
            this.ThreadsAvaliable = !_taskSchedular.MaxThreadsReached;
            _taskFactory.StartNew(()=>
                {                    
                    LoadPageWork(pageIndex, pageLength);
                    this.ThreadsAvaliable = !_taskSchedular.MaxThreadsReached;
                });
            //ThreadPool.QueueUserWorkItem(LoadPageWork, new int[] { pageIndex, pageLength });
        }

        /// <summary>
        /// Performed on background thread.
        /// </summary>
        /// <param name="args">Index of the page to load.</param>
        private void LoadPageWork(int pageIndex, int pageLength)
        {
            // collection empty called/ page has been removed
            if (_pages.ContainsKey(pageIndex))
                UpdatePage(pageIndex, pageLength);
        }

        ///// <summary>
        ///// Performed on background thread.
        ///// </summary>
        ///// <param name="args">Index of the page to load.</param>
        //private void LoadPageWork(object state)
        //{
        //    int[] args = (int[])state;
        //    int pageIndex = args[0];
        //    int pageLength = args[1];
        //    // collection empty called/ page has been removed
        //    if (_pages.ContainsKey(pageIndex))
        //        UpdatePage(pageIndex, pageLength);
        //}

        #endregion
    }
}
