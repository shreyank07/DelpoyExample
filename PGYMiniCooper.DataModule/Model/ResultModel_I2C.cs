
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public class ResultModel_I2C : ResultModelBase
    {
        int indexer;

        public ResultModel_I2C()
        {
            resultCollection = new List<I2CFrame>();
            indexer = 0;
        }

        public override void Reset()
        {
            base.Reset();

            resultCollection.Clear();
            SelectedFrame = null;
            TriggerTime = 0;
            Triggerset = false;
            TriggerPacket = -1;
            indexer = 0;
        }

        public IFrame SelectedFrame
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
        private IFrame selectedFrame;
        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;
            resultCollection.Add((I2CFrame)frame);

            base.AddFrame(frame);
        }

        private List<I2CFrame> resultCollection;

        public List<I2CFrame> ResultCollection
        {
            get
            {
                return resultCollection;
            }
            set
            {
                resultCollection = value;
            }
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
        double triggerTime;


        #endregion
    }
}
