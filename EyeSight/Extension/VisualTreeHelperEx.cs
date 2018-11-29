//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension
//类名称:       VisualTreeHelperEx
//创建时间:     2015/9/21 星期一 15:42:56
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace EyeSight.Extension
{
    public static class VisualTreeHelperEx
    {
        public static T FindVisualParentByName<T>(DependencyObject child, string name) where T : DependencyObject
        {
            try
            {
                var parent = VisualTreeHelper.GetParent(child);
                string controlName = parent.GetValue(Control.NameProperty) as string;
                if ((string.IsNullOrEmpty(name) || controlName == name) && parent is T)
                {
                    return parent as T;
                }
                else
                {
                    T result = FindVisualParentByName<T>(parent, name);
                    if (result != null)
                        return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static List<T> FindVisualChildrenByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            try
            {
                List<T> result = new List<T>();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is T)
                    {
                        result.Add(child as T);
                    }
                    else
                    {
                        List<T> subresult = FindVisualChildrenByName<T>(child, name);
                        if (subresult != null)
                            result = result.Concat(subresult).ToList();
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is T)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the specified DependencyObject is in visual tree.
        /// </summary>
        /// <remarks>
        /// Note that this might not work as expected if the object is in a popup.
        /// </remarks>
        /// <param name="dob">The DependencyObject.</param>
        /// <returns>
        ///   <c>true</c> if the specified dob is in visual tree ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInVisualTree(this DependencyObject dob)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return false;
            }

            //TODO: consider making it work with Popups too.
            if (Window.Current == null)
            {
                // This may happen when a picker or CameraCaptureUI etc. is open.
                return false;
            }

            return Window.Current.Content != null && dob.GetAncestors().Contains(Window.Current.Content);
        }

        /// <summary>
        /// Gets the ancestors.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject start)
        {
            var parent = VisualTreeHelper.GetParent(start);

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        /// <summary>
        /// Gets the descendants of the given type.
        /// </summary>
        /// <typeparam name="T">Type of descendants to return.</typeparam>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendants().OfType<T>();
        }

        /// <summary>
        /// Gets the descendants.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        {
            if (start == null)
            {
                yield break;
            }

            var queue = new Queue<DependencyObject>();

            var popup = start as Popup;

            if (popup != null)
            {
                if (popup.Child != null)
                {
                    queue.Enqueue(popup.Child);
                    yield return popup.Child;
                }
            }
            else
            {
                var count = VisualTreeHelper.GetChildrenCount(start);

                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(start, i);
                    queue.Enqueue(child);
                    yield return child;
                }
            }

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();

                popup = parent as Popup;

                if (popup != null)
                {
                    if (popup.Child != null)
                    {
                        queue.Enqueue(popup.Child);
                        yield return popup.Child;
                    }
                }
                else
                {
                    var count = VisualTreeHelper.GetChildrenCount(parent);

                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        yield return child;
                        queue.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the descendants.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        //public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        //{
        //    var queue = new Queue<DependencyObject>();
        //    var count = VisualTreeHelper.GetChildrenCount(start);

        //    for (int i = 0; i < count; i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(start, i);
        //        yield return child;
        //        queue.Enqueue(child);
        //    }

        //    while (queue.Count > 0)
        //    {
        //        var parent = queue.Dequeue();
        //        var count2 = VisualTreeHelper.GetChildrenCount(parent);

        //        for (int i = 0; i < count2; i++)
        //        {
        //            var child = VisualTreeHelper.GetChild(parent, i);
        //            yield return child;
        //            queue.Enqueue(child);
        //        }
        //    }
        //}
    }
}
