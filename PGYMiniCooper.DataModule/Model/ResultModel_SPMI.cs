using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;
using PGYMiniCooper.DataModule.Structure.SPMIStructure;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PGYMiniCooper.DataModule.Model
{
    public class ResultModel_SPMI : ResultModelBase
    {
        int indexer;
        public ResultModel_SPMI()
        {
            FrameCollection = new List<SPMIFrameStructure>();
            indexer = 0;
        }

        public override void Reset()
        {
            base.Reset();

            FrameCollection.Clear();
            //Status = "";
            TriggerTime = 0;
            Triggerset = false;
            TriggerPacket = -1;
            indexer = 0;
            selectedFrame = null;

        }


        public SPMIFrameStructure SelectedFrame
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
        private SPMIFrameStructure selectedFrame;

        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            FrameCollection.Add((SPMIFrameStructure)frame);

            base.AddFrame(frame);
        }



        public List<SPMIFrameStructure> FrameCollection
        {
            get
            {
                return _ResultCollection;
            }
            set
            {
                _ResultCollection = value;
            }
        }
        List<SPMIFrameStructure> _ResultCollection;     

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
        double triggerTime=-1;

        #endregion
    }
}
