using CommunityToolkit.Mvvm.ComponentModel;
using Prodigy.WaveformControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class PolygonBusData : ObservableObject, IBusData
    {
        private double _StartIndex;
        private double _StopIndex;
        private string _Data;
        private System.Windows.Media.Brush _Brush = System.Windows.Media.Brushes.White;

        public double StartIndex
        {
            get
            {
                return _StartIndex;
            }
            set
            {
                _StartIndex = value;
            }
        }

        public double StopIndex
        {
            get
            {
                return _StopIndex;
            }
            set
            {
                _StopIndex = value;
            }
        }

        public string Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        public System.Windows.Media.Brush Brush
        {
            get
            {
                return _Brush;
            }
            set
            {
                _Brush = value;
                OnPropertyChanged("Brush");
            }
        }

        public override string ToString()
        {
            return Data;
        }
    }
}
