//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Encrypt.CommandEx
//类名称:       PivotCommandEx
//创建时间:     2015/9/21 星期一 15:28:33
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeSight.Extension.CommandEx
{
    public class PivotCommandEx
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand), typeof(PivotCommandEx), new PropertyMetadata(null, OnLoadedCommandPropertyChanged));

        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }


        private static void OnLoadedCommandPropertyChanged(DependencyObject d,
         DependencyPropertyChangedEventArgs e)
        {
            var control = d as Pivot;
            if (control != null)
            {
                control.PivotItemLoaded -= control_PivotItemLoaded;
                control.PivotItemLoaded += control_PivotItemLoaded;
            }
        }

        static void control_PivotItemLoaded(Pivot sender, PivotItemEventArgs args)
        {
            var pi = sender as Pivot;
            if (pi != null)
            {
                var command = GetLoadedCommand(pi);

                var paramter = GetCommandParameter(pi);

                object obj = null;

                if (paramter != null)
                {
                    obj = paramter;
                }
                else
                {
                    obj = pi;
                }

                if (command != null && command.CanExecute(obj))
                    command.Execute(obj);
            }
        }

        #region CommandParameter
        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(PivotCommandEx), null);

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }
        #endregion
    }
}
