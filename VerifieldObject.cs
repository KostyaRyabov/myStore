using FastMember;
using System;
using System.Collections.Generic;

namespace myStore
{
    public class VerifieldObject<T> where T : class, new()
    {
        private readonly TypeAccessor accessor;
        private readonly MemberSet members;

        public T obj { get; set; }
        private T old_obj;

        public void Load(T obj)
        {
            this.obj = obj;
            old_obj = (obj as ICloneable).Clone() as T;
        }

        public VerifieldObject()
        {
            accessor = TypeAccessor.Create(typeof(T));
            members = accessor.GetMembers();
        }

        public Dictionary<string, object> GetChanges()
        {
            var dic = new Dictionary<string, object>();

            foreach (var m in members)
            {
                var val = accessor[obj, m.Name];

                if (val.Equals(accessor[old_obj, m.Name]))
                {
                    dic[m.Name] = val;
                }
            }

            return dic;
        }
    }
}
