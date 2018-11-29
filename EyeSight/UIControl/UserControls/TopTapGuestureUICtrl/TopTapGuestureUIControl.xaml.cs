using EyeSight.Extension;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.View;
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

namespace EyeSight.UIControl.UserControls.TopTapGuestureUICtrl
{
    public sealed partial class TopTapGuestureUIControl : UserControl
    {
        public TopTapGuestureUIControl()
        {
            this.InitializeComponent();

            //再 -60 是不要盖住顶部右侧的按钮
            this.grid.Width = Window.Current.Bounds.Width - 48 - 60;
            this.grid.Height = 30;
        }

        private void grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            if (rootPage != null)
            {
                Frame rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame != null)
                {
                    Page page = rootFrame.Content as Page;
                    if (page != null)
                    {
                        ListView szListView = page.FindName("szListView") as ListView;
                        if (szListView != null)
                        {
                            var scroll = VisualTreeHelperEx.FindVisualChildByName<ScrollViewer>(szListView, "ScrollViewer");
                            if (scroll != null)
                            {
                                scroll.ScrollToVerticalOffset(0);
                            }
                        }
                    }
                }
            }
        }
    }
}
