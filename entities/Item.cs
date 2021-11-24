using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myStore.entities
{
    public class Item : INotifyPropertyChanged
    {
        public bool editable { get; set; } = false;
        
        private string _value;
        public string value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("value");
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
