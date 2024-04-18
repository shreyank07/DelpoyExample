using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class GenericSelection : ViewModelBase
    {
        public GenericSelection()
        {
            isStaticChannels = false;
            groupName = "Group";
            selectedClock = eChannles.CH1;
            Items = new ObservableCollection<eChannles>();
            foreach (var item in Enum.GetValues(typeof(eChannles)).Cast<eChannles>())
                Items.Add(item);
            SelectedItems = new ObservableCollection<eChannles>();
            SelectedItems.Add(eChannles.CH1);
        }

        private ObservableCollection<eChannles> _items;
        private ObservableCollection<eChannles> _selectedItems;


        public ObservableCollection<eChannles> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public ObservableCollection<eChannles> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                NotifyPropertyChanged("SelectedItems");
            }
        }

        private eChannles selectedItem;
        
        private string groupName;

        public string GroupName
        {
            get
            {
                return groupName;
            }

            set
            {
                groupName = value;
            }
        }

        public bool IsStaticChannels
        {
            get
            {
                return isStaticChannels;
            }

            set
            {
                isStaticChannels = value;
            }
        }

        public eChannles SelectedItem
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

        private bool isStaticChannels;

        private eChannles selectedClock;
    }
}
