using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using System.Windows.Media;

namespace myStore
{
    public static class DependencySearcher
    {
        public static List<Tuple<Binding, DependencyObject>> GetBindingObjects(DependencyObject element)
        {
            List<Tuple<Binding, DependencyObject>> bindings = new List<Tuple<Binding, DependencyObject>>();
            List<DependencyProperty> dpList = new List<DependencyProperty>();
            dpList.AddRange(GetDependencyProperties(element));
            dpList.AddRange(GetAttachedProperties(element));

            foreach (DependencyProperty dp in dpList)
            {
                Binding b = BindingOperations.GetBinding(element, dp);
                if (b != null)
                {
                    bindings.Add(new Tuple<Binding, DependencyObject>(b, element));
                }
            }

            return bindings;
        }

        public static List<DependencyProperty> GetDependencyProperties(DependencyObject element)
        {
            //List<DependencyProperty> properties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                return markupObject.Properties.Where(mp => mp.DependencyProperty != null).Select(mp => mp.DependencyProperty).ToList();
                
                //foreach (MarkupProperty mp in markupObject.Properties)
                //{
                //    if (mp.DependencyProperty != null)
                //    {
                //        properties.Add(mp.DependencyProperty);
                //    }
                //}
            }

            return new List<DependencyProperty>();
        }

        public static List<DependencyProperty> GetAttachedProperties(DependencyObject element)
        {
            List<DependencyProperty> attachedProperties = new List<DependencyProperty>();
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            if (markupObject != null)
            {
                foreach (MarkupProperty mp in markupObject.Properties)
                {
                    if (mp.IsAttached)
                    {
                        attachedProperties.Add(mp.DependencyProperty);
                    }
                }
            }

            return attachedProperties;
        }

        public static void GetBindingsRecursive(DependencyObject dObj, List<Tuple<Binding, DependencyObject>> bindingList)
        {
            bindingList.AddRange(GetBindingObjects(dObj));

            foreach (var child in LogicalTreeHelper.GetChildren(dObj)){
                try
                {
                    DependencyObject d = (DependencyObject)child;
                    GetBindingsRecursive(d, bindingList);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
