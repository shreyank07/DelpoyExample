using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ProdigyFramework.Helpers
{
    public static class UiDispatcherHelper
    {
        public static Dispatcher UiDispatcher { get; private set; }

        /// <summary>
        /// This method should be called once on the UI thread to ensure that
        /// the property is initialized.
        /// <para>In WPF, call this method on the static App() constructor.</para>
        /// </summary>
        public static void Initialize()
        {
            if (UiDispatcher != null && UiDispatcher.Thread.IsAlive)
                return;

            UiDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Executes an action on the UI thread.
        /// The action will be enqueued on the UI thread's
        /// dispatcher and executed asynchronously.
        /// </summary>
        /// <param name="action">The action will
        /// be enqueued on the UI thread's dispatcher
        /// and executed asynchronously..
        /// </param>
        public static void BeginInvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.InvokeAsync(action, DispatcherPriority.Input);
        }

        /// <summary>
        /// Executes an action on the UI thread. The action will be enqueued
        /// on the UI thread's dispatcher and executed synchronously.
        /// </summary>
        /// <param name="action">
        /// The action that will be executed on the UI thread synchronously.
        /// </param>
        public static void InvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.Invoke(action);
        }

        /// <summary>
        /// Executes an action on the UI thread. The action will be enqueued on the
        /// UI thread's dispatcher with the specified priority and executed asynchronously.
        /// </summary>
        /// <param name="action">
        /// The action that will be executed on the UI thread.</param>
        /// <param name="priority"></param>

        public static DispatcherOperation InvokeOnUiAsync(Action action, DispatcherPriority priority)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return UiDispatcher.InvokeAsync(action, priority);
        }
    }
}
