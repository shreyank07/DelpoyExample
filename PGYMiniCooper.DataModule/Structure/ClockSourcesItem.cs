using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class ClockSourcesItem : ViewModelBase
    {
        public ClockSourcesItem()
        {
            clockSource = "Clock Source1";
            selectedClock = eChannles.CH1;
            selectedSampleEdge = eEdgeType.RISING_EDGE;
            maxfreqSupport = 12.5;
        }

        private string clockSource;

        public string ClockSource
        {
            get
            {
                return clockSource;
            }

            set
            {
                clockSource = value;
            }
        }

        public IEnumerable<eChannles> ChannelList
        {
            get
            {
                return Enum.GetValues(typeof(eChannles)).Cast<eChannles>();
            }
        }

        public eChannles SelectedClock
        {
            get
            {
                return selectedClock;
            }

            set
            {
                selectedClock = value;
            }
        }

        private eChannles selectedClock;

        public IEnumerable<eEdgeType> EdgeTypeList
        {
            get
            {
                return Enum.GetValues(typeof(eEdgeType)).Cast<eEdgeType>().Where(x => x != eEdgeType.NO_EDGE);
            }
        }

        public eEdgeType SelectedSampleEdge
        {
            get
            {
                return selectedSampleEdge;
            }

            set
            {
                selectedSampleEdge = value;
            }
        }

        public double MaxfreqSupport
        {
            get
            {
                return maxfreqSupport;
            }

            set
            {
                maxfreqSupport = value;
            }
        }

        private eEdgeType selectedSampleEdge;

        private double maxfreqSupport;
    }
}
