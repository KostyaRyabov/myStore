using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myStore.entities
{
    public class Cpu : INotifyPropertyChanged
    {
        private int _cpu_id;
        private string _processor;
        private string _producer;
        private short? _frequency_mhz;
        private short? _max_frequency_mhz;
        private short? _num_of_cores;



        public int cpu_id
        {
            get
            {
                return _cpu_id;
            }
            set
            {
                _cpu_id = value;
                OnPropertyChanged("cpu_id");
            }
        }
        public string processor
        {
            get
            {
                return _processor;
            }
            set
            {
                _processor = value;
                OnPropertyChanged("processor");
            }
        }
        public string producer
        {
            get
            {
                return _producer;
            }
            set
            {
                _producer = value;
                OnPropertyChanged("producer");
            }
        }
        public short? frequency_mhz
        {
            get
            {
                return _frequency_mhz;
            }
            set
            {
                _frequency_mhz = value;
                OnPropertyChanged("frequency_mhz");
            }
        }
        public short? max_frequency_mhz
        {
            get
            {
                return _max_frequency_mhz;
            }
            set
            {
                _max_frequency_mhz = value;
                OnPropertyChanged("max_frequency_mhz");
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
