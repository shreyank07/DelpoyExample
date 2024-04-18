using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using System.Windows.Input;


using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Behavior;


namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public sealed class ProtocolActivityViewModel : ViewModelBase
    {
        public ProtocolActivityViewModel(ConfigurationViewModel configuration)
        {
            protocolActivity = new ProtocolActivityModel(configuration.Config);
        }

        

        ProtocolActivityModel protocolActivity;
        public ProtocolActivityModel ProtocolActivity
        {
            get
            {
                return protocolActivity;
            }
            set
            {
                protocolActivity = value;
                RaisePropertyChanged(" ProtocolActivity");
            }
        }

        public event Action<ProtocolActivityHolder> OnSelectedActivityChanged;

        private double triggerTime;

        public double TriggerTime
        {
            get { return triggerTime; }
            set { triggerTime = value; RaisePropertyChanged("TriggerTime"); }
        }



        private ProtocolActivityHolder selectedFrame;

        public ProtocolActivityHolder SelectedFrame
        {
            get
            {
                return selectedFrame;
            }
            set
            {
                selectedFrame = value;
                RaisePropertyChanged("SelectedFrame");
                OnSelectedActivityChanged?.Invoke(selectedFrame);
            }
        }

        public void Initialize()
        {
            protocolActivity.Initialize();
        }

        public void Reset()
        {
            // Reset view model
            SelectedFrame = null;

            protocolActivity.Reset();
        }
    }
}
