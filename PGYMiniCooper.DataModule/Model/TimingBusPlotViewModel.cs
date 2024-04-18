using Prodigy.Business;
using Prodigy.WaveformControls;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PGYMiniCooper.DataModule.Model
{
    public class TimingBusPlotViewModel : ViewModelBase, IPlotInfoView
    {
        private bool _IsShowing;
        private double _SampleRate = 0;
        private double _VerticalOffset;
        private double _StartWfmIndex;
        private double _StopWfmIndex;
        private long _WfmLength;
        private ObservableCollection<IBusData> _BusFullData;
        private IEnumerable<IBusData> _BusData;

        public TimingBusPlotViewModel()
        {
            BusFullData = new ObservableCollection<IBusData>();
            _verticalScale = 2.8;
        }

        public event RemovePlotEventHandler OnRemove;


        public bool IsDirty
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    _BusData = BusFullData.Where(b => b.StartIndex > StartWfmIndex && b.StopIndex < StopWfmIndex);
                    RaisePropertyChanged("BusFullData");
                    RaisePropertyChanged("BusData");
                    RaisePropertyChanged("IsDirty");
                }
            }
        }
        private string _tag;
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                this.RaisePropertyChanged("Tag");
            }
        }

        public bool IsShowing
        {
            get
            {
                return _IsShowing;
            }
            set
            {
                _IsShowing = value;
                RaisePropertyChanged("IsShowing");
            }
        }

        public bool IsCursorVisible
        {
            get
            {
                return false;
            }
            set
            {
                ////////////////
            }
        }

        public bool IsEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        WfmEnum wfmType;
        public WfmEnum Channel
        {
            get
            {
                return this.wfmType;
            }
            set
            {
                wfmType = value;
                RaisePropertyChanged("Channel");
            }
        }

        public double SampleRate
        {
            get
            {
                return _SampleRate;
            }
            set
            {
                _SampleRate = value;
                RaisePropertyChanged("SampleRate");
            }
        }

        public string Source
        {
            get
            {
                return "";
            }
        }

        private double _verticalScale = 2.8;
        public double VerticalScale
        {
            get
            {
                return _verticalScale;
            }
            set
            {
                _verticalScale = value;
                RaisePropertyChanged("VerticalScale");
            }
        }

        public double HorizontalScale
        {
            get
            {
                return 1;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return _VerticalOffset;
            }
            set
            {
                _VerticalOffset = value;
                RaisePropertyChanged("VerticalOffset");
            }
        }

        public double VCursor1Position
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public double VCursor2Position
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public double StartWfmIndex
        {
            get
            {
                return _StartWfmIndex;
            }
            set
            {
                _StartWfmIndex = value;
                RaisePropertyChanged("StartWfmIndex");
            }
        }

        public double StopWfmIndex
        {
            get
            {
                return _StopWfmIndex;
            }
            set
            {
                _StopWfmIndex = value;
                RaisePropertyChanged("StopWfmIndex");
            }
        }

        public long WfmLength
        {
            get
            {
                return _WfmLength;
            }
            set
            {
                _WfmLength = value;
            }
        }

        public ObservableCollection<IBusData> BusFullData
        {
            get
            {
                return _BusFullData;
            }
            set
            {
                _BusFullData = value;
                BusData = value;
                RaisePropertyChanged("BusFullData");
            }
        }

        public IEnumerable<IBusData> BusData
        {
            get
            {
                return _BusData;
            }
            set
            {
                _BusData = value;
                RaisePropertyChanged("BusData");
            }
        }

        public TypeOfChart ChartType
        {
            get
            {
                return TypeOfChart.BUS;
            }
        }

        private Brush _StartBitBrush = Brushes.Red;

        public Brush StartBitBrush
        {
            get
            {
                return _StartBitBrush;
            }
            set
            {
                _StartBitBrush = value;
                RaisePropertyChanged("StartBitBrush");
            }
        }

        private Brush _StopBitBrush = Brushes.Red;
        public Brush StopBitBrush
        {
            get
            {
                return _StopBitBrush;
            }
            set
            {
                _StopBitBrush = value;
                RaisePropertyChanged("StopBitBrush");
            }
        }
        private Brush _AddressBrush = Brushes.DimGray;
        public Brush AddressBrush
        {
            get
            {
                return _AddressBrush;
            }
            set
            {
                _AddressBrush = value;
                RaisePropertyChanged("AddressBrush");
            }
        }
        private Brush _DataBrush = Brushes.LightBlue;
        public Brush DataBrush
        {
            get
            {
                return _DataBrush;
            }
            set
            {
                _DataBrush = value;
                RaisePropertyChanged("DataBrush");
            }
        }
        private Brush _RWBrush = Brushes.Pink;
        public Brush RWBrush
        {
            get
            {
                return _RWBrush;
            }
            set
            {
                _RWBrush = value;
                RaisePropertyChanged("RWBrush");
            }
        }

        public double TriggerPosition
        {
            get
            {
                return 0;
            }
        }

        public void Remove()
        {
            IsEnabled = false;
            IsShowing = false;
            if (OnRemove != null)
            {
                OnRemove(this, new Prodigy.WaveformControls.RemovePlotEventArgs(this));
                OnRemove = null;
            }
        }

        public void SetIsShowing()
        {
            IsShowing = true;
        }
    }
}
