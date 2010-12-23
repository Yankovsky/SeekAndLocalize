using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SeekAndLocalize.Core
{
    public static class VisualTreeUtils
    {
        public static ChildItem FindVisualChild<ChildItem>(DependencyObject obj) where ChildItem : class
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildItem)
                    return child as ChildItem;
                else
                {
                    ChildItem childOfChild = FindVisualChild<ChildItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static IEnumerable<ChildItem> FindVisualChildren<ChildItem>(DependencyObject obj) where ChildItem : class
        {
            List<ChildItem> children = new List<ChildItem>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildItem)
                    children.Add(child as ChildItem);

                children.AddRange(FindVisualChildren<ChildItem>(child));
            }
            return children;
        }

        public static T GetParent<T>(this DependencyObject obj) where T : class
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent != null)
                return parent is T ? parent as T : GetParent<T>(parent);
            return null;
        }

        public static T GetParent<T>(this DependencyObject depObj, string name) where T : FrameworkElement
        {
            T obj = depObj.GetParent<T>();
            while (obj.Name != name || obj == null)
            {
                obj = obj.GetParent<T>();
            }
            return obj;
        }
    }
}
