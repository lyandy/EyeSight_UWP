using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class AuthorDetailHeaderUIControl : UIControlBase
    {
        public AuthorDetailHeaderUIControl()
        {
            this.InitializeComponent();

            double with = 0;

            if (AppEnvironment.IsPhone)
            {
                with = Window.Current.Bounds.Width;
            }
            else
            {
                with = Window.Current.Bounds.Width - 48;

                this.ellipseAuthorPic.Width = this.ellipseAuthorPic.Height = 70;
            }

            this.grid.Width = with;
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            var with = e.NewSize.Width;

            this.grid.Width = with;
        }

        bool isLoaded = false;

        private async void grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //虚拟化会重新执行此方法绑定数据
            //await grid.AnimateAsync(new FadeInLeftAnimation());
            //await grid.AnimateAsync(new BounceInDownAnimation());

            var g = sender as Grid;
            if (g != null && !isLoaded)
            {
                await Task.Delay(500);

                var animationName = new FadeInDownAnimation();
                animationName.Distance = 100;
                g.AnimateAsync(animationName);

                isLoaded = true;
            }
        }
    }
}
