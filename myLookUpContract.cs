using BlackPearl.Controls.Contract;
using System.Windows;

namespace myStore
{
    public class myLookUpContract : ILookUpContract
    {
        public bool SupportsNewObjectCreation => true;

        public object CreateObject(object sender, string searchString) => searchString;

        public bool IsItemEqualToString(object sender, object item, string seachString)
        {
            if (!(item is string str))
            {
                return false;
            }

            return string.Compare(seachString, str, System.StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public bool IsItemMatchingSearchString(object sender, object item, string searchString)
        {
            if (!(item is string str))
            {
                return false;
            }

            if (string.IsNullOrEmpty(searchString))
            {
                return true;
            }

            return str.ToLower()?.Contains(searchString?.ToLower()) == true;
        }
    }
}
