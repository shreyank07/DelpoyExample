using CommunityToolkit.Mvvm.DependencyInjection;
using Google.Protobuf.WellKnownTypes;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using Prodigy.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{

    public class ResultViewModel_I3C_Combined : ViewModelBase, IResetViewModel
    {
        private readonly ConfigurationViewModel configuration;
        private readonly List<IResultViewModel> protocolResults;
       // private readonly IDataProvider dataProvider;

        private bool isInitialized = false;
        private bool selectFrameOnce = true;
        public ResultViewModel_I3C_Combined(ConfigurationViewModel configuration)
        {

      //      ASyncCollection = new AsyncVirtualizingCollection<I3CListModel>
      //(new ItemsProvider<I3CListModel>(new Action<IList<I3CListModel>, int, int>(FetchRange))
      //, 200, 5000);

            this.configuration = configuration;

            this.protocolResults = new List<IResultViewModel>();
            var resultView = new CollectionViewSource();
            resultView.Source = this.ResultsI3C_Combined;

      

            SearchParameter = new SearchParameterI3C_Combined(resultView);
            SearchParameter.OnSelectionChanged += SearchParameter_SelectionChanged;
        }

  

        public void ResultAdded(IResultViewModel result)
        {
            protocolResults.Add(result);

            // All protocols are i3c
            (result as ResultViewModel_I3C).WareHouse.OnFramesDecoded += Result_I3C_OnFramesDecoded;
        }
     


        private bool isLoading = false;
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }
        private IList<IFrame> resultsI3C_Combined = new AsyncObservableCollection<IFrame>();

        /// <summary>
        /// Contains all I3C data combined - sorting is done on the view
        /// </summary>
        public IList<IFrame> ResultsI3C_Combined
        {
            get { return resultsI3C_Combined; }
            set
            {
                resultsI3C_Combined = value;
                RaisePropertyChanged(nameof(ResultsI3C_Combined));
            }
        }
        private I3CListModel selectedItem;
        public I3CListModel SelectedItem
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


        private IFrame selectedFrame = null;

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

                if (selectedFrame != null)
                {
                    var result = protocolResults.FirstOrDefault(p => p.Config.Name == selectedFrame.ProtocolName);
                    if (result != null)
                    {
                        //((ResultViewModel_I3C)result).DownloadedItems = ASyncCollection.GetDownloadedResults();
                        result.SelectedFrame = selectedFrame;
                    }

                    SearchParameter.SelectedFrame = selectedFrame;
                }
            }
        }

        private IFrame triggerPacket;

        public IFrame TriggerPacket
        {
            get { return triggerPacket; }
            set
            {
                triggerPacket = value;
                RaisePropertyChanged(nameof(TriggerPacket));
            }
        }

        private Command gotoTrigger;
        public Command GotoTrigger
        {
            get
            {
                if (gotoTrigger == null)
                    gotoTrigger = new Command(new Command.ICommandOnExecute(gotoTriggerMethod));
                return gotoTrigger;
            }
        }

        ResultModel_I3C wareHouse;
        public ResultModel_I3C WareHouse
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
        public  void gotoTriggerMethod()
        {
            if (TriggerPacket != null)
            {
                SelectedFrame = TriggerPacket;
            }
        }

        public double TriggerTime
        {
            get { return SessionConfiguration.TriggerTime; }
        }

        #region Search and filter

        public SearchParameterI3C_Combined SearchParameter { get; }

        private void SearchParameter_SelectionChanged(IFrame frame)
        {
            this.SelectedFrame = frame;
        }

        private ICommand rowLoadedCommand;

        public ICommand RowLoadedCommand
        {
            get
            {
                rowLoadedCommand ??= new CommunityToolkit.Mvvm.Input.RelayCommand<IFrame>(f =>
                    {
                        f.IsHighlighted = false;
                        // if search is enabled
                        if (SearchParameter.SearchMode == SearchMode.Search && SearchParameter.IsSearchActive)
                        {
                            f.IsHighlighted = SearchParameter.IsMatch(f);
                        }
                    });

                return rowLoadedCommand;
            }
        }

        public Command SearchCommandVisibility
        {
            get
            {

                return new Command(new Command.ICommandOnExecute(SearchVisibility));

            }
        }
        public void SearchVisibility()
        {
            if (BorderVisibility == false)
                BorderVisibility = true;

            else
                if (BorderVisibility == true)
                BorderVisibility = false;

        }
        private bool borderVisibility= false;
        public bool BorderVisibility
        {
            get
            {
                return borderVisibility;
            }
            set
            {
                borderVisibility = value;
                RaisePropertyChanged("BorderVisibility");
            }
        }

        #endregion

        private void Result_I3C_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            if (isInitialized)
            {
                ((AsyncObservableCollection<IFrame>)ResultsI3C_Combined).AddRange(frames);
          
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (selectFrameOnce && SearchParameter.ResultView.View.IsEmpty == false)
                    {
                        selectFrameOnce = false;
                        SelectedFrame = SearchParameter.ResultView.View.OfType<IFrame>().First();
                    }
                });
            }

            // Try set trigger packet
            if (TriggerPacket == null && SessionConfiguration.TriggerProtcolset && configuration.Trigger.TriggerType == eTriggerTypeList.Protocol)
            {
                var resultWithTriggerPacket = protocolResults.FirstOrDefault(r => r.TriggerPacketIndex != -1);

                if (resultWithTriggerPacket != null)
                {
                    var triggerPacket = resultWithTriggerPacket.ResultCollection.ElementAtOrDefault(resultWithTriggerPacket.TriggerPacketIndex);
                    if (triggerPacket != null)
                        TriggerPacket = triggerPacket;
                }
            }


            //if (TriggerPacket == null && SessionConfiguration.TriggerProtcolset && configuration.Trigger.TriggerType == eTriggerTypeList.Protocol)
            //{
            //    var resultWithTriggerPacket = protocolResults.FirstOrDefault(r => r.TriggerPacketIndex != -1);

            //    if (resultWithTriggerPacket != null)
            //    {
            //        var triggerPacket = resultWithTriggerPacket.ResultCollection.ElementAtOrDefault(resultWithTriggerPacket.TriggerPacketIndex);
            //        if (triggerPacket != null)
            //            TriggerPacket = triggerPacket;
            //    }
            //}
        }

        //private void FetchRange(IList<I3CListModel> arg1, int startIndex, int count)
        //{
        //    IsLoading = true;

        //    try
        //    {
        //        var list = GetPackets(startIndex, count);

        //        for (int i = 0; i < arg1.Count; i++)
        //        {
        //            if (i >= list.Count) break;

        //            arg1[i].Frame = list[i];
        //        }
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //    }
        //}

        //private List<IFrame> GetPackets(int startIndex, int count)
        //{
        //    return dataProvider.RequestFrames(configuration.ProtocolConfiguration[0].Name, startIndex, count);
        //}

        public void Initialize()
        {
        isInitialized = true; 
   // Refresh the data grid
            SearchParameter.ResultView.SortDescriptions.Add(new SortDescription(nameof(IFrame.StartTime), ListSortDirection.Ascending));
        }

        public void Reset()
        {
            isInitialized = false;

            foreach (var result in protocolResults)
            {
                result.Reset();

                if (configuration.ProtocolConfiguration.All(p => p is ConfigViewModel_I3C) && result is ResultViewModel_I3C)
                {
                    // All protocols are i3c
                    (result as ResultViewModel_I3C).WareHouse.OnFramesDecoded -= Result_I3C_OnFramesDecoded;
                }
            }

            SelectedFrame = null;
            ResultsI3C_Combined.Clear();
            protocolResults.Clear();

            // Refresh the data grid
            SearchParameter.Reset();
        }
    }

    public class SearchParameterI3C_Combined : SearchFilterViewModel_I3C
    {
        public SearchParameterI3C_Combined(CollectionViewSource source) : base(source) { }

        public SearchParameterI3C_Combined(System.Collections.IList collection) : base(collection) { }

        private string selectedBusName;
        public string SelectedBusName
        {
            get
            {
                return selectedBusName;
            }
            set
            {
                selectedBusName = value;
                OnPropertyChanged(nameof(SelectedBusName));
            }
        }

        public override bool IsMatch(IFrame frame)
        {
            bool success = base.IsMatch(frame);

            if (success == false)
                return false;

            if (!string.IsNullOrWhiteSpace(selectedBusName))
                success = frame.ProtocolName == selectedBusName;

            return success;
        }
    }
}