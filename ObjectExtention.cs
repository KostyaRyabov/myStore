
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace myStore
{
    public static class ObjectExtention
    {
        public static void Update<T>(this Collection<T> dict, string fieldName, Dictionary<string, object> source) where T: class
        {
            object tmp_obj;
            if (source.TryGetValue(fieldName, out tmp_obj))
            {
                var selected_items = (Collection<T>)tmp_obj;
                var missing_items = selected_items.Except(dict);

                foreach (var m in missing_items)
                {
                    dict.Add(m);
                }
            }
        }
    }
}
