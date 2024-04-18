using PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdigyFramework.Behavior;
using System.Windows.Input;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using ExtendedXmlSerializer;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows;
using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.Collections;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Business.Extensions;
using System.Windows.Data;
using ExtendedXmlSerializer.Core.Sources;
using System.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ResultsViewModel : ViewModelBase, IResetViewModel
    {
        private readonly ConfigurationViewModel configuration;
        private  bool isCombinedViewActive = false;

        public event Action<IResultViewModel> ResultAdded;

        /// <summary>
        /// Only used for design time object creation
        /// </summary>
        public ResultsViewModel() : this(new ConfigurationViewModel()) { }

        public ResultsViewModel(ConfigurationViewModel configuration)
        {
            this.configuration = configuration;
            protocolResults = new ObservableCollection<IResultViewModel>();           
        }

        private void ResultsViewModel_OnProtocolResultAdded(DataModule.Interface.IConfigModel config, object resultModel)
        {
            IResultViewModel result = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                IConfigViewModel configViewModel = this.configuration.ProtocolConfiguration.FirstOrDefault(c => c.Model == config);
            switch (config)
            {
                case ConfigModel_CAN configModel_CAN:
                    result = new ResultViewModel_CAN((dynamic)configViewModel, (dynamic)resultModel,configuration.Trigger);
                    break;
                case ConfigModel_I2C configModel_I2C:
                    result = new ResultViewModel_I2C((dynamic)configViewModel, (dynamic)resultModel, configuration.Trigger);
                    break;
                case ConfigModel_I3C configModel_I3C:
                    result = new ResultViewModel_I3C((dynamic)configViewModel, (dynamic)resultModel, configuration.Trigger, isCombinedViewActive);
                    break;
                case ConfigModel_QSPI configModel_QSPI:
                    result = new ResultViewModel_QSPI((dynamic)configViewModel, (dynamic)resultModel, configuration.Trigger);
                    break;
                case ConfigModel_RFFE configModel_RFFE:
                    result = new ResultViewModel_RFFE((dynamic)configViewModel, (dynamic)resultModel,configuration.Trigger);
                    break;
                case ConfigModel_SPI configModel_SPI:
                    result = new ResultViewModel_SPI((dynamic)configViewModel, (dynamic)resultModel, configuration.Trigger);
                    break;
                case ConfigModel_SPMI configModel_SPMI:
                    result = new ResultViewModel_SPMI((dynamic)configViewModel, (dynamic)resultModel,configuration.Trigger);
                    break;
                case ConfigModel_UART configModel_UART:
                    result = new ResultViewModel_UART((dynamic)configViewModel, (dynamic)resultModel, configuration.Trigger);
                    break;
            }

           
                ProtocolResults.Add(result);             
            });

            if (SelectedResult == null)
                SelectedResult = result;

            ResultAdded?.Invoke(result);
        }

        ResultViewModel_I3C_Combined wareHouse;
        public ResultViewModel_I3C_Combined WareHouse
        {
            get
            {
                return wareHouse;
            }
            set
            {
                wareHouse = value;
                RaisePropertyChanged("WareHouse");
            }
        }

        private ObservableCollection<IResultViewModel> protocolResults;

        public ObservableCollection<IResultViewModel> ProtocolResults 
        { 
            get => protocolResults;
            set
            {
                protocolResults = value;
                RaisePropertyChanged(nameof(ProtocolResults));
            }
        }

        private IResultViewModel selectedResult;

        public IResultViewModel SelectedResult
        {
            get
            {
                return selectedResult;
            }
            set
            {
                selectedResult = value;
                RaisePropertyChanged("SelectedResult");
               
            }
        }

        public void Serach(eProtocol protocolType)
        {
            foreach (var result in protocolResults.Where(r => r.Config.ProtocolType == protocolType))
            {
                result.Serach();
            }
        }

        public void Initialize()
        {
           // isCombinedViewActive = configuration.ConfigurationMode != eConfigMode.LA_Mode && configuration.ProtocolConfiguration.All(p => p.ProtocolType == eProtocol.I3C);
            Ioc.Default.GetService<IDataProvider>().OnProtocolResultAdded += ResultsViewModel_OnProtocolResultAdded;

            //if (isCombinedViewActive)
            //{
            //    WareHouse = new ResultViewModel_I3C_Combined(configuration);            
            //    ResultAdded += wareHouse.ResultAdded;
            //    WareHouse.Initialize();
            //}
        }

        public void Reset()
        {
            Ioc.Default.GetService<IDataProvider>().OnProtocolResultAdded -= ResultsViewModel_OnProtocolResultAdded;

            if (wareHouse != null)
            {
                wareHouse.Reset();
                ResultAdded -= wareHouse.ResultAdded;

                WareHouse = null;
            }

            // Remove all results
            protocolResults.Clear();
        }


        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; RaisePropertyChanged("Status"); }
        }
    }
}
