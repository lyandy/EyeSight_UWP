using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Helper;
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

namespace EyeSight.UIControl.UserControls.ProgressUICtrl
{
    public sealed partial class ProgressUIControl : UIControlBase
    {
        public ProgressUIControl()
        {
            this.InitializeComponent();

            //重新绘制此控件，其容器宽高应当是整个页面的容器宽高各自减去48.因为左侧和上部各有48像素的距离
            //if (!AppEnvironment.IsPhone)
            //{
            //    this.Width = Window.Current.Bounds.Width - 48;
            //}
            //else
            //{
            //    this.Width = Window.Current.Bounds.Width;
            //}
            //this.Height = Window.Current.Bounds.Height - 48;

            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //这里不用减去，因为e.NewSize拿到的本来就是NavigationRootPage的rootFrame的宽高大小
            //if (AppEnvironment.IsPhone)
            //{
            //    if (!AppEnvironment.IsPortrait)
            //    {
            //        this.Width = e.NewSize.Width + 48;
            //    }
            //    else
            //    {
            //        this.Width = e.NewSize.Width;
            //    }
            //}
            //else
            //{
            this.Width = Window.Current.Bounds.Width;
            //}
            this.Height = Window.Current.Bounds.Height;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.pro.IsActive = true;
        }

        private void Grid_Unloaded(object sender, RoutedEventArgs e)
        {
            this.pro.IsActive = false;
        }
    }
}
