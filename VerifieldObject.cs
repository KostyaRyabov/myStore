using FastMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace myStore
{
    public class VerifieldObject<T> where T : class, new()
    {
        private static readonly TypeAccessor accessor = TypeAccessor.Create(typeof(T));
        private static readonly MemberSet members = accessor.GetMembers();

        public T obj { get; set; }
        private T old_obj;

        public void Load(T obj)
        {
            this.obj = obj;
            old_obj = (obj as ICloneable).Clone() as T;
        }

        public void Reload() => obj = (obj as ICloneable).Clone() as T;

        public VerifieldObject() { }

        public VerifieldObject(T obj)
        {
            Load(obj);
        }

        private bool Compare(object? x, object? y)
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
            
            foreach (var m in members)
            {
                var new_val = accessor[obj, m.Name];
                var old_val = accessor[old_obj, m.Name];

                if (!Compare(new_val, old_val))
                {
                    dic[m.Name] = new_val;


                    if (new_val is ICloneable cloneable)
                    {
                        accessor[old_obj, m.Name] = cloneable.Clone();
                    }
                    else
                    {
                        accessor[old_obj, m.Name] = new_val;
                    }
                }
            }

            return dic;
        }
    }
}
