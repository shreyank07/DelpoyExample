using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace PGYMiniCooper.DataModule.Model
{
    public abstract class ResultModelBase : ViewModelBase, IResultModel
    {
        private bool notificationTaskRunning = false;
        private readonly object LockObject = new object();
        private List<IFrame> pendingFrames = new List<IFrame>();

        public event EventHandler<IReadOnlyList<IFrame>> OnFramesDecoded;

        private void Notify()
        {
            List<IFrame> framesToNotify;
            lock (LockObject)
            {
                // Stop recursive task
                if (pendingFrames.Count == 0)
                {
                    notificationTaskRunning = false;
                    return;
                }

                framesToNotify = pendingFrames;
                pendingFrames = new List<IFrame>();
            }

            Task.Run(async () =>
            {
                OnFramesDecoded?.Invoke(this, framesToNotify);

                // Allow UI to breathe
                await Task.Delay(1000);

                Notify();
            });
        }

        public virtual void AddFrame(IFrame frame)
        {
            lock (LockObject)
            {
                pendingFrames.Add(frame);
            }

            // Called only once
            if (notificationTaskRunning == false)
            {
                notificationTaskRunning = true;
                Notify();
            }
        }

        public virtual void Reset()
        {
            lock (LockObject)
            {
                pendingFrames = new List<IFrame>();
            }
            notificationTaskRunning = false;
        }
    }
}

