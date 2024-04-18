using PGYMiniCooper.DataModule;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ChannelSelectionViewModel : ViewModelBase
    {
        public ChannelSelectionViewModel() { }  

        public ChannelSelectionViewModel(eChannles channel)
        {
            this.Channel = channel;
        }

        public eChannles Channel { get; set; }

        private bool isAvaialble = true;
        public bool IsAvailable
        {
            get { return isAvaialble; }
            set
            {
                isAvaialble = value;
            
                RaisePropertyChanged(nameof(IsAvailable));
            }
        }
    }
}
