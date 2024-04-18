using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.UARTStructure;
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

namespace PGYMiniCooper.DataModule.Model
{
    public class ResultModel_UART : ResultModelBase
    {
        int indexer;
        public ResultModel_UART()
        {
            uARTFrameCollection = new List<UARTFrame>();
            indexer = 0;
        }

        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            uARTFrameCollection.Add((UARTFrame)frame);

            base.AddFrame(frame);
        }

        public IList<UARTFrame> UARTFrameCollection
        {
            get
            {
                return uARTFrameCollection;
            }
            set
            {
                uARTFrameCollection = value;
            }
        }
        IList<UARTFrame> uARTFrameCollection;        

        public UARTFrame SelectedFrame
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
        private UARTFrame selectedFrame;

        public override void Reset()
        {
            base.Reset();

            UARTFrameCollection.Clear();
            TriggerTime = 0;
            selectedFrame = null;
            indexer = 0;
            TriggerPacket = -1;
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
        double triggerTime = -1;

        #endregion
    }
}
