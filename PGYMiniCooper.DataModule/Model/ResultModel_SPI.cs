using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
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
    public class ResultModel_SPI : ResultModelBase
    {
        public ResultModel_SPI()
        {
            sPIFrameCollection = new List<SPIFrame>();
            indexer = 0;
        }

        int indexer;

        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            sPIFrameCollection.Add((SPIFrame)frame);

            base.AddFrame(frame);
        }        

        public List<SPIFrame> SPIFrameCollection
        {
            get
            {
                return sPIFrameCollection;
            }
            set
            {
                sPIFrameCollection = value;
            }
        }
        List<SPIFrame> sPIFrameCollection;

        public override void Reset()
        {
            base.Reset();

            SPIFrameCollection.Clear();
            TriggerTime = 0;
            Triggerset = false;
            TriggerPacket = -1;
            selectedFrame = null;
            indexer = 0;
        }

        public SPIFrame SelectedFrame
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
        private SPIFrame selectedFrame;

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

        private int _TriggerPacket;
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
