//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.CommandEx
//类名称:       FlipViewCommandEx
//创建时间:     2015/10/9 19:26:32
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
    public class FlipViewCommandEx
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("SelectionChangedCommand", typeof(ICommand), typeof(FlipViewCommandEx), new PropertyMetadata(null, OnSelectionChangedCommandPropertyChanged));

        public static void SetSelectionChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static ICommand GetSelectionChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }


        private static void OnSelectionChangedCommandPropertyChanged(DependencyObject d,
         DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlipView;
            if (control != null)
            {
                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = -1;
                control.SelectionChanged += control_SelectionChanged;
            }
        }

        static void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fv = sender as FlipView;
            if (fv != null)
            {
                var command = GetSelectionChangedCommand(fv);

                object obj = fv.SelectedItem;


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
