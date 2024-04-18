using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public class TrendInfoViewModel : ViewModelBase
    {
        private WfmPlotViewModel trend;

        public TrendInfoViewModel()
        {
            trend = new WfmPlotViewModel();
        }

        public WfmPlotViewModel Trend
        {
            get
            {
                return this.trend;
            }
        }
    }
}
