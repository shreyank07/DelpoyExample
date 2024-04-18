using CommunityToolkit.Mvvm.ComponentModel;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using Prodigy.Business;
using ProdigyFramework.Behavior;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Input;

namespace PGYMiniCooper.CoreModule.ViewModel.Interfaces
{
    public enum SearchMode { Search, Filter_In, Filter_Out };

    public abstract class SearchFilterViewModelBase<T> : ObservableObject where T : class
    {
        protected readonly CollectionViewSource source;

        public SearchFilterViewModelBase(CollectionViewSource source) 
        {
            this.source = source;
        }

        public SearchFilterViewModelBase(IList collection)
        {
            this.source = new CollectionViewSource();
            this.source.Source = collection;
            this.source.View.Refresh();
        }

        public event Action<T> OnSelectionChanged;

        public abstract bool IsMatch(T frame);

        private SearchMode searchMode;

        public SearchMode SearchMode
        {
            get { return searchMode; }
            set
            {
                searchMode = value;
                OnPropertyChanged(nameof(SearchMode));
            }
        }

        private bool isSearchActive;

        public bool IsSearchActive
        {
            get { return  isSearchActive; }
            set
            {
                isSearchActive = value;
                OnPropertyChanged(nameof(IsSearchActive));
            }
        }

        private bool isFilterActive;

        public bool IsFilterActive
        {
            get { return isFilterActive; }
            set
            {
                isFilterActive = value;
                OnPropertyChanged(nameof(IsFilterActive));
            }
        }

        private T selectedFrame;

        public T SelectedFrame
        {
            get { return selectedFrame; }
            set
            {
                selectedFrame = value;
                OnPropertyChanged(nameof(SelectedFrame));
            }
        }

        public CollectionViewSource ResultView => source;

        private ICommand searchCommand;

        public ICommand SearchCommand
        {
            get
            {
                return searchCommand ??= new Command(new Command.ICommandOnExecute(Search));
            }
        }

        private ICommand previousCommand;

        public ICommand PreviousCommand
        {
            get
            {
                return previousCommand ??= new Command(new Command.ICommandOnExecute(SearchPrevious), new Command.ICommandOnCanExecute((p) =>
                {
                    return SearchMode == SearchMode.Search;
                }));
            }
        }

        private ICommand nextCommand;

        public ICommand NextCommand
        {
            get
            {
                return nextCommand ??= new Command(new Command.ICommandOnExecute(SearchNext), new Command.ICommandOnCanExecute((p) =>
                {
                    return SearchMode == SearchMode.Search;
                }));
            }
        }

        private ICommand resetCommand;

        public ICommand ResetCommand
        {
            get
            {
                return resetCommand ??= new Command(new Command.ICommandOnExecute(Reset));
            }
        }

        public void Search()
        {
            IsSearchActive = false;
            IsFilterActive = false;

            if (SearchMode == SearchMode.Search)
            {
                IsSearchActive = true;
                SelectedFrame = source.View.OfType<T>().FirstOrDefault();
            }
            else
            {
                AddFilter();
            }

            source.View.Refresh();
        }

        private void SearchPrevious()
        {
            bool success = false;
            var remainingResultsReversed = source.View.OfType<T>().Reverse<T>().SkipWhile(f => f != selectedFrame);

            if (remainingResultsReversed.Any())
            {
                foreach (var frame in remainingResultsReversed.Skip(1))
                {
                    if (this.IsMatch(frame))
                    {
                        SelectedFrame = frame;
                        success = true;
                        OnSelectionChanged?.Invoke(frame);
                        break;
                    }
                }
            }

            if (success == false)
            {
                // no more results
            }
        }

        public void SearchNext()
        {
            bool success = false;
            var remainingResults = source.View.OfType<T>().SkipWhile(d => d != selectedFrame);

            if (remainingResults.Any())
            {
                foreach (var frame in source.View.OfType<T>().SkipWhile(d => d != selectedFrame).Skip(1))
                {
                    if (this.IsMatch(frame))
                    {
                        SelectedFrame = frame;
                        OnSelectionChanged?.Invoke(frame);
                        success = true;
                        break;
                    }
                }
            }


            if (success == false)
            {
                // display no more results
            }

            source.View.Refresh();
        }

        private void AddFilter()
        {
            IsFilterActive = true;
            source.Filter += FilterEvent;
        }

        private void RemoveFilter()
        {
            if (IsFilterActive)
                source.Filter -= FilterEvent;

            IsFilterActive = false;
        }

        private void FilterEvent(object sender, FilterEventArgs e)
        {
            bool isMatch = this.IsMatch((T)e.Item);

            // Filter out - add unmatched item
            // Filter in - add matched items

            e.Accepted = false;
            if (isMatch)
            {
                if (searchMode == SearchMode.Filter_In)
                {
                    e.Accepted = true;
                }
            }
            else if (searchMode == SearchMode.Filter_Out)
            {
                e.Accepted = true;
            }
        }

        public void Reset()
        {
            RemoveFilter();

            IsSearchActive = false;
            IsFilterActive = false;
            SelectedFrame = null;

            source.View.Refresh();
        }
    }
}
