using FastMember;
using System.Linq;

namespace myStore
{
    public abstract class Accessored<T>
    {
        private static TypeAccessor accessor = TypeAccessor.Create(typeof(T));
        private static string[] _fieldNames = accessor.GetMembers().Where(m => m.Name != "Item").Select(m => m.Name).ToArray();
        public static string[] fieldNames() => _fieldNames;

        public virtual object this[string member]
        {
            get => accessor[this, member];
            set => accessor[this, member] = value;
        }
    }
}
