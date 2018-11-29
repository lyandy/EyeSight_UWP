//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Encrypt.CommandEx
//类名称:       ListViewCommandEx
//创建时间:     2015/9/21 星期一 15:27:49
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Locator;
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
    public class ListViewCommandEx
    {
        #region SelectionChangedCommand
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("SelectionChangedCommand", typeof(ICommand), typeof(ListViewCommandEx), new PropertyMetadata(null, OnSelectionChangedCommandPropertyChanged));

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
            var control = d as ListView;
            if (control != null)
            {
                control.IsItemClickEnabled = false;
                control.SelectionChanged -= control_SelectionChanged;
                control.SelectionChanged += control_SelectionChanged;
            }
        }

        static void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv != null)
            {
                var command = GetSelectionChangedCommand(lv);

                var paramter = GetCommandParameter(lv);

                object obj = null;

                if (paramter != null)
                {
                    obj = paramter;
                }
                else
                {
                    obj = lv.SelectedItems;
                }

                if (command != null && command.CanExecute(obj))
                    command.Execute(obj);
            }
        }
        #endregion

        #region LoadMoreCommand

        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.RegisterAttached("LoadMoreCommand", typeof(ICommand),
            typeof(ListViewCommandEx), new PropertyMetadata(null, OnLoadMoreCommandChanged));

        public static ICommand GetLoadMoreCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadMoreCommandProperty);
        }

        public static void SetLoadMoreCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadMoreCommandProperty, value);
        }

        private static void OnLoadMoreCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            element.Loaded -= element_Loaded;
            element.Loaded += element_Loaded;
        }

        private static bool next = false;

        //static ICommand listViewLoadMoreCommand;

        private static void element_Loaded(object sender, RoutedEventArgs e)
        {
            var d = sender as FrameworkElement;
            var scroll = VisualTreeHelperEx.FindVisualChildByName<ScrollViewer>(d, "ScrollViewer");

            if (scroll != null)
            {
                //var scrollBar = VisualTreeHelperEx.FindVisualChildByName<ScrollBar>(scroll, "VerticalScrollBar");
                //if (scrollBar != null)
                //{
                //    scrollBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 176, 14, 37));
                //    //Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                //}

                var command = GetLoadMoreCommand(d);
                if (command != null)
                {
                    //scroll.ViewChanged -= Scroll_ViewChanged;
                    scroll.ViewChanged += (s, a) =>
                    {
                        ViewModelLocator.Instance.ListViewScroll();
                        ViewModelLocator.Instance.FavoriteOrDownloadListViewScroll();

                        //Debug.WriteLine("scroll.VerticalOffset : " + scroll.VerticalOffset + "scroll.ScrollableHeight : " + scroll.ScrollableHeight +  "  scroll.ScrollableHeight - 0.5 : " + (scroll.ScrollableHeight - 0.5));

                        if (scroll.VerticalOffset >= scroll.ScrollableHeight)
                        {
                            //if (!next)
                            //{
                            //next = true;
                            if (command.CanExecute(null))
                                command.Execute(null);
                            // }
                            //else
                            //{
                            //next = false;
                            //}
                        }
                    };
                }
            }
        }

        //private static void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        //{
        //    var scroll = sender as ScrollViewer;
        //    if (scroll != null && listViewLoadMoreCommand != null)
        //    {
        //        if (scroll.VerticalOffset == scroll.ScrollableHeight)
        //        {
        //            if (!next)
        //            {
        //                next = true;
        //                if (listViewLoadMoreCommand.CanExecute(null))
        //                    listViewLoadMoreCommand.Execute(null);
        //            }
        //            else
        //            {
        //                next = false;
        //            }
        //        }
        //    }
        //}
        #endregion

        #region CommandParameter
        public static readonly DependencyProperty CommandParameterProperty =
           DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ListViewCommandEx), null);

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }
        #endregion

        #region LoadMoreCommand

        public static readonly DependencyProperty ScrollCommandProperty =
            DependencyProperty.RegisterAttached("ScrollCommand", typeof(ICommand),
            typeof(ListViewCommandEx), new PropertyMetadata(null, OnScrollCommandChanged));

        public static ICommand GetScrollCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ScrollCommandProperty);
        }

        public static void SetScrollCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ScrollCommandProperty, value);
        }

        private static void OnScrollCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            element.Loaded -= myElement_Loaded;
            element.Loaded += myElement_Loaded;
        }

        //static ICommand listViewLoadMoreCommand;

        private static void myElement_Loaded(object sender, RoutedEventArgs e)
        {
            var d = sender as FrameworkElement;
            var scroll = VisualTreeHelperEx.FindVisualChildByName<ScrollViewer>(d, "ScrollViewer");

            if (scroll != null)
            {
                var command = GetScrollCommand(d);
                if (command != null)
                {
                    //scroll.ViewChanged -= Scroll_ViewChanged;
                    scroll.ViewChanged += (s, a) =>
                    {
                        if (command.CanExecute(null))
                            command.Execute(null);
                    };
                }
            }
        }
        #endregion
    }
}
