using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public class ResultModel_I3C : ResultModelBase
    {
        private int indexer;

        public ResultModel_I3C()
        {
            resultCollection = new List<FramePattern>();
            indexer = 0;
        }

        public override void Reset()
        {
            base.Reset();

            resultCollection.Clear();
            SelectedFrame = null;
            //Status = "";
            TriggerTime = 0;
            Triggerset = false;
            TriggerPacket = -1;
            selectedFrame = null;
            indexer = 0;
        }

        public override void AddFrame(IFrame frame)
        {
            frame.FrameIndex = indexer++;

            resultCollection.Add((FramePattern)frame);

            base.AddFrame(frame);
        }

        public FramePattern SelectedFrame
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
        private FramePattern selectedFrame;
        public IMessage SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                RaisePropertyChanged("SelectedItem");
            }
        }
        private IMessage selectedItem;

        List<FramePattern> resultCollection;

        public List<FramePattern> FrameCollection
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

