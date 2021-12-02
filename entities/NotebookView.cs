using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myStore.entities
{
    public class NotebookView : Accessored<NotebookView>, INotifyPropertyChanged
    {
        public int notebook_id { get; set; }
        public int price { get; set; }
        public string name { get; set; }
        private byte[] _image;
        public byte[] image
        {
            get
            {
                if (_image is null) return Properties.Resources.unloaded_image;
                else return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged("image");
            }
        }
        public long num_of_rates { get; set; }
        public decimal rate { get; set; }


        private bool removable = false;
        public bool Removable {
            get { return removable; }
            set
            {
                removable = value;
                OnPropertyChanged("Removable");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
