using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Helper;
using EyeSight.Model.Gift;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class GiftUIControl : UIControlBase
    {
        public GiftUIControl()
        {
            this.InitializeComponent();

            var phoneWith = Window.Current.Bounds.Width;
            var PCWith = Window.Current.Bounds.Width - 48;

            if (AppEnvironment.IsPhone)
            {
                this.grid.Width = phoneWith - 6;

                this.grid.Height = this.grid.Width / 2;

                this.imgApp.Width = 60;
                this.imgApp.Height = 60;
            }
            else
            {
                this.imgApp.Width = 100;
                this.imgApp.Height = 100;

                if (PCWith >= 700)
                {
                    this.grid.Width = (PCWith - 4 * 3) / 3;
                }
                else
                {
                    this.grid.Width = (PCWith - 3 * 3) / 2;
                }

                this.grid.Height = this.grid.Width;
            }

            this.Loaded += async (ss, ee) =>
            {
                this.spApp.Opacity = 1;
                await Task.Delay(300);
                this.imgApp.AnimateAsync(new FadeInDownAnimation() { Distance = 100 });
                await this.tbAppName.AnimateAsync(new FadeInUpAnimation() { Distance = 100 });

                //只有当model.AppDesc有值得时候才进行model.AppDesc的动画显示
                var model = this.DataContext as GiftModel;
                if (model != null && !string.IsNullOrEmpty(model.AppDesc))
                {
                    await Task.Delay(200);

                    this.imgApp.AnimateAsync(new FadeOutUpAnimation() { Distance = 100 });
                    await this.tbAppName.AnimateAsync(new FadeOutDownAnimation() { Distance = 100 });
                    this.spApp.Opacity = 0;

                    this.tbBam.Text = model.AppDesc;
                    await tbBam.AnimateText(RandomAnimationHelper.Instance.GetAnimation(), 0.1);
                    await this.tbBam.AnimateAsync(new FadeOutAnimation() { Duration = 0.3, Delay = 0.3 });

                    this.imgApp.AnimateAsync(new ResetAnimation());
                    await this.tbAppName.AnimateAsync(new ResetAnimation());
                    await this.spApp.AnimateAsync(new FadeInAnimation() { Duration = 0.2 });
                }
            };

            this.Unloaded += (ss, ee) =>
            {
                this.spApp.AnimateAsync(new ResetAnimation());
                this.imgApp.AnimateAsync(new ResetAnimation());
                this.tbAppName.AnimateAsync(new ResetAnimation());
                this.tbBam.AnimateAsync(new ResetAnimation());
            };
        }

        protected override void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedReleaseHolding(sender, e);
        }

        protected override void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedHolding(sender, e);
        }

        protected override async void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedNavigate(sender, e);

            var control = sender as FrameworkElement;
            if (control != null)
            {
                var model = control.DataContext as GiftModel;
                if (model != null)
                {
                    await Launcher.LaunchUriAsync(model.AppProductUri);
                }
            }
        }
        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            var with = e.NewSize.Width;

            if (!AppEnvironment.IsPhone)
            {

                if (with >= 700)
                {
                    this.grid.Width = (with - 4 * 3) / 3;
                }
                else
                {
                    this.grid.Width = (with - 3 * 3) / 2;
                }

                this.grid.Height = this.grid.Width;
            }
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
                await Task.Delay(100);

                var animationName = new FadeInAnimation();
                //animationName.Distance = 150;
                g.AnimateAsync(animationName);

                isLoaded = true;
            }
        }
    }
}
