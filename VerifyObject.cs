using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace myStore
{
    public class VerifyObject<T> where T : Accessored<T>, ICloneable
    {
        public T obj { get; set; }
        private T old_obj;

        public void Load(T obj)
        {
            this.obj = obj;
            old_obj = obj.Clone() as T;
        }

        public void Reload() => obj = obj.Clone() as T;

        public VerifyObject() { }

        public VerifyObject(T obj)
        {
            Load(obj);
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

        public Dictionary<string, object> GetAndSaveChanges()
        {
            var dic = new Dictionary<string, object>();
            var members = Accessored<T>.fieldNames();

            foreach (var m in members)
            {
                var new_val = obj[m];
                var old_val = old_obj[m];

                if (!Compare(new_val, old_val))
                {
                    dic[m] = new_val;


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

            return dic;
        }
    }
}
