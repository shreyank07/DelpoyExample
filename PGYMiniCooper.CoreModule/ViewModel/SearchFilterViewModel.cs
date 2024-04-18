using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class SearchFilterViewModel : ViewModelBase, IResetViewModel
    {
        #region Constructor

        public SearchFilterViewModel()
        {          
            I2CSearchList = new List<eI2CSearchAtList>();
            I2CSearchList.Add(eI2CSearchAtList.Start);
            I2CSearchList.Add(eI2CSearchAtList.Address);
            I2CSearchList.Add(eI2CSearchAtList.Data);
            I2CSearchList.Add(eI2CSearchAtList.Address_Data);
            I2CSearchList.Add(eI2CSearchAtList.Ack);
            I2CSearchList.Add(eI2CSearchAtList.Nack);
            I2CSearchList.Add(eI2CSearchAtList.Repeated_Start);
            I2CSearchList.Add(eI2CSearchAtList.Stop);

            ComparisonList = new List<eComparisonList>();
            ComparisonList.Add(eComparisonList.Equal_to);
            ComparisonList.Add(eComparisonList.Not_equal_to);

            PatternList = new List<ePatternFormat>();
            PatternList.Add(ePatternFormat.Binary);
            PatternList.Add(ePatternFormat.Decimal);
            PatternList.Add(ePatternFormat.Hex);
            PatternList.Add(ePatternFormat.Octal);

            UARTTypeList = new List<eUARTTypeList>();
            UARTTypeList.Add(eUARTTypeList.TX);
            UARTTypeList.Add(eUARTTypeList.RX);
        }
        #endregion

        #region Data Members
        bool searchSelected;
        public bool SearchSelected
        {
            get
            {
                return searchSelected;
            }
            set
            {
                searchSelected = value;
                RaisePropertyChanged("SearchSelected");
            }
        }

        bool filterInSelected;
        public bool FilterInSelected
        {
            get
            {
                return filterInSelected;
            }
            set
            {
                filterInSelected = value;
                RaisePropertyChanged("FilterInSelected");
            }
        }

        bool filterOutSelected;
        public bool FilterOutSelected
        {
            get
            {
                return filterOutSelected;
            }
            set
            {
                filterOutSelected = value;
                RaisePropertyChanged("FilterOutSelected");
            }
        }

        bool i2CSelected;
        public bool I2CSelected
        {
            get
            {
                return i2CSelected;
            }
            set
            {
                i2CSelected = value;
                RaisePropertyChanged("I2CSelected");
            }
        }

        bool sPISelected;
        public bool SPISelected
        {
            get
            {
                return sPISelected;
            }
            set
            {
                sPISelected = value;
                RaisePropertyChanged("SPISelected");
            }
        }

        bool uARTSelected;
        public bool UARTSelected
        {
            get
            {
                return uARTSelected;
            }
            set
            {
                uARTSelected = value;
                RaisePropertyChanged("UARTSelected");
            }
        }

        private List<eI2CSearchAtList> i2CSearchList;

        public List<eI2CSearchAtList> I2CSearchList
        {
            get 
            { 
                return i2CSearchList; 
            }
            set 
            { 
                i2CSearchList = value; 
                RaisePropertyChanged("I2CSearchList"); 
            }
        }

        private List<eComparisonList> comparisonList;

        public List<eComparisonList> ComparisonList
        { 
            get 
            { 
                return comparisonList; 
            } 
            set 
            {
                comparisonList = value; 
                RaisePropertyChanged("ComparisonList"); 
            } 
        }

        private List<ePatternFormat> patternList;

        public List<ePatternFormat> PatternList
        {
            get 
            { 
                return patternList; 
            }
            set
            {
                patternList = value; 
                RaisePropertyChanged("PatternList");
            }
        }

        private List<eUARTTypeList> uARTTypeList;

        public List<eUARTTypeList> UARTTypeList
        {
            get 
            { 
                return uARTTypeList; 
            }
            set
            {
                uARTTypeList = value; 
                RaisePropertyChanged("UARTTypeList");
            }
        }

        private eI2CSearchAtList i2CSearchSelected;

        public eI2CSearchAtList I2CSearchSelected
        {
            get 
            { 
                return i2CSearchSelected; 
            }
            set 
            { 
                i2CSearchSelected = value; 
                RaisePropertyChanged("I2CSearchSelected"); 
            }
        }

        private eComparisonList mOSIComparison;

        public eComparisonList MOSIComparison
        {
            get 
            { 
                return mOSIComparison; 
            }
            set
            { 
                mOSIComparison = value; 
                RaisePropertyChanged("MOSIComparison"); 
            }
        }

        private eComparisonList mISOComparison;

        public eComparisonList MISOComparison
        {
            get 
            { 
                return mISOComparison; 
            }
            set
            { 
                mISOComparison = value; 
                RaisePropertyChanged("MISOComparison"); 
            }
        }

        private eComparisonList uARTComparison;

        public eComparisonList UARTComparison
        {
            get 
            { 
                return uARTComparison; 
            }
            set
            {
                uARTComparison = value; 
                RaisePropertyChanged("UARTComparison");
            }
        }

        private ePatternFormat mOSIPattern;

        public ePatternFormat MOSIPattern
        {
            get 
            { 
                return mOSIPattern; 
            }
            set
            {
                mOSIPattern = value; 
                RaisePropertyChanged("MOSIPattern");
            }
        }

        private ePatternFormat mISOPattern;

        public ePatternFormat MISOPattern
        {
            get 
            { 
                return mISOPattern; 
            }
            set
            {
                mISOPattern = value; 
                RaisePropertyChanged("MISOPattern");
            }
        }

        private eUARTTypeList uARTType;

        public eUARTTypeList UARTType
        {
            get 
            { 
                return uARTType; 
            }
            set
            {
                uARTType = value; 
                RaisePropertyChanged("UARTType");
            }
        }

        private bool mOSIDataSelected;

        public bool MOSIDataSelected
        {
            get
            {
                return mOSIDataSelected;
            }
            set
            {
                mOSIDataSelected = value;
                RaisePropertyChanged("MOSIDataSelected");
            }
        }

        private bool mISODataSelected;

        public bool MISODataSelected
        {
            get
            {
                return mISODataSelected;
            }
            set
            {
                mISODataSelected = value;
                RaisePropertyChanged("MISODataSelected");
            }
        }

        private string mOSISearchData;

        public string MOSISearchData
        {
            get
            {
                return mOSISearchData;
            }
            set
            {
                mOSISearchData = value;
                RaisePropertyChanged("MOSISearchData");
            }
        }

        private string mISOSearchData;

        public string MISOSearchData
        {
            get
            {
                return mISOSearchData;
            }
            set
            {
                mISOSearchData = value;
                RaisePropertyChanged("MISOSearchData");
            }
        }

        private string uARTSearchData;

        public string UARTSearchData
        {
            get
            {
                return uARTSearchData;
            }
            set
            {
                uARTSearchData = value;
                RaisePropertyChanged("UARTSearchData");
            }
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            // Initialize search and filter
        }

        public void Reset()
        {
            SearchSelected = true;
            FilterInSelected = false;
            FilterOutSelected = false;
            I2CSelected = true;
            SPISelected = false;
            UARTSelected = false;

            I2CSearchSelected = eI2CSearchAtList.Start;
            MOSIComparison = eComparisonList.Equal_to;
            MISOComparison = eComparisonList.Equal_to;
            UARTComparison = eComparisonList.Equal_to;
            MOSIPattern = ePatternFormat.Hex;
            MISOPattern = ePatternFormat.Hex;
            UARTType = eUARTTypeList.TX;
            MOSIDataSelected = true;
            MISODataSelected = false;
            MOSISearchData = "0x00";
            MISOSearchData = "0x00";
            UARTSearchData = "0x00";
        }
        #endregion
    }
}
