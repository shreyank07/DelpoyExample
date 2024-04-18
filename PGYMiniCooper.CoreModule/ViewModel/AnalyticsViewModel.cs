using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class AnalyticsViewModel : ViewModelBase
    {
        public AnalyticsViewModel()
        {
            statistics = StatisticsRepository.GetInstance();
        }

        private StatisticsRepository statistics;
        public StatisticsRepository Statistics
        {
            get
            {
                return statistics;
            }
            set
            {
                statistics = value;
                RaisePropertyChanged("Statistics");
            }
        }
    }
}
