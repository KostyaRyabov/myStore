using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myStore.entities
{
    public class Gpu : INotifyPropertyChanged
    {
        private int _gpu_id;
        private string _video_adapter;
        private string _video_card;
        private string _video_memory_type;
        private short? _video_memory_size;
        private short? _num_of_cores;


        public int gpu_id
        {
            get
            {
                return _gpu_id;
            }
            set
            {
                _gpu_id = value;
                OnPropertyChanged("gpu_id");
            }
        }
        public string video_adapter
        {
            get
            {
                return _video_adapter;
            }
            set
            {
                _video_adapter = value;
                OnPropertyChanged("video_adapter");
            }
        }
        public string video_card
        {
            get
            {
                return _video_card;
            }
            set
            {
                _video_card = value;
                OnPropertyChanged("video_card");
            }
        }
        public string video_memory_type
        {
            get
            {
                return _video_memory_type;
            }
            set
            {
                _video_memory_type = value;
                OnPropertyChanged("video_memory_type");
            }
        }
        public short? video_memory_size
        {
            get
            {
                return _video_memory_size;
            }
            set
            {
                _video_memory_size = value;
                OnPropertyChanged("video_memory_size");
            }
        }
        public short? num_of_cores
        {
            get
            {
                return _num_of_cores;
            }
            set
            {
                _num_of_cores = value;
                OnPropertyChanged("num_of_cores");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
