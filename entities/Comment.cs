using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace myStore.entities
{
    public class Comment : Accessored<Comment>, INotifyPropertyChanged, ICloneable
    {
        public int comment_id { get; set; }
        public int notebook_id { get; set; }

        private string _plus_text;
        private string _minus_text;
        private string _review_text;

        private short _screen_rate;
        private short _power_rate;
        private short _work_duration_rate;


        public Comment() { }
        public Comment(int notebook_id) => this.notebook_id = notebook_id;


        public string plus_text
        {
            get
            {
                return _plus_text;
            }
            set
            {
                _plus_text = value;
                OnPropertyChanged("plus_text");
            }
        }
        public string minus_text
        {
            get
            {
                return _minus_text;
            }
            set
            {
                _minus_text = value;
                OnPropertyChanged("minus_text");
            }
        }
        public string review_text
        {
            get
            {
                return _review_text;
            }
            set
            {
                _review_text = value;
                OnPropertyChanged("review_text");
            }
        }

        public short screen_rate
        {
            get
            {
                return _screen_rate;
            }
            set
            {
                _screen_rate = value;
                OnPropertyChanged("screen_rate");
            }
        }
        public short power_rate
        {
            get
            {
                return _power_rate;
            }
            set
            {
                _power_rate = value;
                OnPropertyChanged("power_rate");
            }
        }
        public short work_duration_rate
        {
            get
            {
                return _work_duration_rate;
            }
            set
            {
                _work_duration_rate = value;
                OnPropertyChanged("work_duration_rate");
            }
        }

        public object Clone() => this.MemberwiseClone();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
