using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace myStore
{
    public class VerifyObject<T>: INotifyPropertyChanged where T : Accessored<T>, ICloneable, INotifyPropertyChanged, new()
    {
        public T obj { get; set; }
        private T old_obj;


        private bool isRemoving = false;
        public bool IsPreparedForRemoving {
            get => isRemoving;
            set {
                isRemoving = (bool)value;
                OnPropertyChanged("IsPreparedForRemoving");
            }
        }

        public void Load(T obj, params (string, object)[] args)
        {
            foreach(var arg in args)
            {
                obj[arg.Item1] = arg.Item2;
            }

            this.obj = obj;
            old_obj = obj.Clone() as T;
        }


        public void Reload()
        {
            var members = Accessored<T>.fieldNames();

            foreach (var m in members)
            {
                if (!Compare(obj[m], old_obj[m]))
                {
                    obj[m] = old_obj[m];
                }
            }
        }

        public VerifyObject() { }

        public VerifyObject(T obj, params (string, object)[] args)
        {
            Load(obj, args);
        }

        public VerifyObject(params (string, object)[] args)
        {
            Load(new T(), args);
        }

        private bool Compare(object x, object y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            if (x is IEnumerable a && y is IEnumerable b)
                return a.Cast<object>().SequenceEqual(b.Cast<object>());

            return x.Equals(y);
        }


        public Dictionary<string, object> GetChanges()
        {
            var dic = new Dictionary<string, object>();
            
            foreach (var m in Accessored<T>.fieldNames())
            {
                var new_val = obj[m];
                var old_val = old_obj[m];

                if (!Compare(new_val, old_val)) dic[m] = new_val;
            }

            return dic;
        }

        public void SaveChanges(Dictionary<string, object> dic)
        {
            foreach (var m in dic.Keys)
            {
                var new_val = obj[m];
                var old_val = old_obj[m];

                if (new_val is ICloneable cloneable)
                {
                    old_obj[m] = cloneable.Clone();
                }
                else
                {
                    old_obj[m] = new_val;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
