using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class CategoryUIControl : UserControl
    {
        public CategoryUIControl()
        {
            this.InitializeComponent();
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //var grid = sender as Grid;
            //if (grid != null)
            //{
            //    await grid.AnimateAsync(new BounceOutLeftAnimation());
            //}
        }

        private async void Grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //var grid = sender as Grid;
            //if (grid != null)
            //{
            //    await Task.Yield();
            //    grid.AnimateAsync(new FadeInRightAnimation());
            //}
        }

        //private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

        //    if (rootPage != null)
        //    {
        //        Frame rootFrame = (Frame)rootPage.FindName("rootFrame");

        //        var grid = sender as Grid;
        //        if (grid != null)
        //        {
        //            var model = grid.DataContext as NavigationRootModel;
        //            if (model != null)
        //            {
        //                if (rootFrame.BackStack.Count == 0)
        //                {
        //                    if (rootFrame.SourcePageType == model.ClassType)
        //                    {
        //                        Debug.WriteLine("0我不导");
        //                        return;
        //                    }
        //                }
        //                else if (rootFrame.BackStack.Count > 0)
        //                {
        //                    if (rootFrame.BackStack[0].SourcePageType == model.ClassType)
        //                    {
        //                        Debug.WriteLine("非0我也不导");
        //                        return;
        //                    }
        //                }

        //                rootFrame.Navigate(model.ClassType, model.Title);
        //            }
        //        }
        //    }
        //}
    }
}
