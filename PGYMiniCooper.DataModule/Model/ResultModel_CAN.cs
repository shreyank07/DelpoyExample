using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Threading;
using System.Windows.Threading;

namespace PGYMiniCooper.DataModule.Model
{
    public class ResultModel_CAN : ResultModelBase
    {
        int indexer;

        public ResultModel_CAN()
        {
            canFrameCollection = new List<CANFrame>();
            indexer = 0;
        }

        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            canFrameCollection.Add((CANFrame)frame);

            base.AddFrame(frame);
        }

        public List<CANFrame> CANFrameCollection
        {
            get
            {
                return canFrameCollection;
            }
            set
            {
                canFrameCollection = value;
            }
        }
        List<CANFrame> canFrameCollection;

        public CANFrame SelectedFrame
        {
            get
            {
                return selectedFrame;
            }
            set
            {
                selectedFrame = value;
                RaisePropertyChanged("SelectedFrame");
            }
        }
        private CANFrame selectedFrame;

        public override void Reset()
        {
            base.Reset();

            CANFrameCollection.Clear();
            TriggerTime = 0;
            selectedFrame = null;
            indexer = 0;
            TriggerPacket = -1;

            SelectedFrame=null;
        }

        #region Trigger



        private bool triggerset = false;
        public bool Triggerset
        {
            get
            {
                return triggerset;
            }
            set
            {
                triggerset = value;
                RaisePropertyChanged("Triggerset");
            }
        }

        private int _TriggerPacket = -1;
        public int TriggerPacket
        {
            get
            {
                return _TriggerPacket;
            }
            set
            {
                _TriggerPacket = value;
                RaisePropertyChanged("TriggerPacket");
            }
        }

        public double TriggerTime
        {
            get
            {
                return triggerTime;
            }
            set
            {
                triggerTime = value;
                RaisePropertyChanged("TriggerTime");
            }
        }
        double triggerTime = 0;
        #endregion



        [NonSerialized]
        private bool brsSelected;
        public bool BRSselectedFreq
        {
            get
            {
                return brsSelected;
            }
            set
            {
                 brsSelected = value;
                RaisePropertyChanged("BRSselectedFreq");
            }
        }

        [NonSerialized]
        private bool canWbrs;

        public bool CanWbrs
        {
            get
            {
                return canWbrs;
            }
            set
            {
                canWbrs = value;
                RaisePropertyChanged("CanWbrs");
            }
        }
    }


   
}

