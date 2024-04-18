
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using Prodigy.Framework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I3CStructure
{
    public class I3CListModel : DataWrapper
    {
        public I3CListModel()
        {
        }

        private IFrame frame;

        public IFrame Frame
        {
            get => frame;
            set
            {
                if (value != null)
                    IsLoading = false;

                frame = value;
                RaisePropertyChanged(nameof(Frame));
            }
        }


       
    }
}
