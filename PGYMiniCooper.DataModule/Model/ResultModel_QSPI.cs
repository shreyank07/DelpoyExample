using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;

using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections;
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
   public class ResultModel_QSPI : ResultModelBase
    {
        int indexer;
        public ResultModel_QSPI()
        {
            QSPIFrameCollection = new List<QSPIFrame>();
            indexer = 0;
        }


        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            qSPIFrameCollection.Add((QSPIFrame)frame);

            base.AddFrame(frame);
        }        
        
        public override void Reset()
        {
            base.Reset();

            QSPIFrameCollection.Clear();
            SelectedFrame= null;
            TriggerTime = 0;
            Triggerset = false;
          
            TriggerPacket = -1;
            indexer = 0;
       
        }

        public List<QSPIFrame> QSPIFrameCollection
        {
            get
            {
                return qSPIFrameCollection;
            }
            set
            {
                qSPIFrameCollection = value;
            }
        }
        List<QSPIFrame> qSPIFrameCollection;


        public QSPIFrame SelectedFrame
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
        private QSPIFrame selectedFrame;
       
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

    }
}

       
          