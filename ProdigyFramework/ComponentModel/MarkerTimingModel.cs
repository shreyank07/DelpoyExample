using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.ComponentModel
{
    public class MarkerTimingModel : ViewModelBase
    {

        private double difference;
        public double Difference
        {
            get
            {
                return difference;
            }
            set
            {
                difference = value;
                RaisePropertyChanged("Difference");
            }
        }

        //SelectedItem for Combobox Marker0
        private Markerdetails start;
        public Markerdetails Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
                RaisePropertyChanged("Start");
                OnMarkerChanged();
            }
        }

        //SelectedItem for Combobox Marker1
        private Markerdetails stop;
        public Markerdetails Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
                RaisePropertyChanged("Stop");
                OnMarkerChanged();
            }
        }


        //Selectionchanged method
        void OnMarkerChanged()
        {
            if (Start != null && Stop != null)
            {
                Difference = Math.Abs(Stop.Timestamp - Start.Timestamp);
            }
            else
            {
                Difference = 0;
            }
        }
    }
}
