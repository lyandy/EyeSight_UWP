//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       CoreVisualTreeHelper
//创建时间:     2015/9/21 星期一 15:45:43
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EyeSight.Helper
{
    public class CoreVisualTreeHelper : ClassBase<CoreVisualTreeHelper>
    {
        public CoreVisualTreeHelper() : base() { }

        public T FindVisualParentByName<T>(DependencyObject child, string name) where T : DependencyObject
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

        public List<T> FindVisualChildrenByName<T>(DependencyObject parent, string name) where T : DependencyObject
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

        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
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
    }
}
