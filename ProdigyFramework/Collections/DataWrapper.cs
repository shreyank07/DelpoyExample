using ProdigyFramework.ComponentModel;

namespace Prodigy.Framework.Collections
{
    public abstract class DataWrapper : ViewModelBase
    {
        private int index;
        public int Index
        {
            get { return this.index; }
            set
            {
                this.index = value;
                RaisePropertyChanged("Index");
            }
        }

        private bool isLoading = true;
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }

        private bool isInUse;
        public bool IsInUse
        {
            get
            {
                return isInUse;
            }

            set
            {
                isInUse = value;
                RaisePropertyChanged("IsInUse");
            }
        }

        private int hash = int.MinValue;
        public override int GetHashCode()
        {
            if (hash == int.MinValue)
                hash = (this.GetType().GUID.ToString() + Index).GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                return (this.Index == ((DataWrapper)obj).Index) && (GetHashCode() == obj.GetHashCode());
            }

            return false;
        }
    }
}
