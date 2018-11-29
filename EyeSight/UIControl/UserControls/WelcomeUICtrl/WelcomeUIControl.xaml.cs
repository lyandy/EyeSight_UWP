using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.LeftSliderGuestureUICtrl;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.WelcomeUICtrl
{
    public sealed partial class WelcomeUIControl : UIControlBase
    {
        public WelcomeUIControl()
        {
            this.InitializeComponent();

            this.txtGrid.Margin = AppEnvironment.IsPhone ? new Thickness(0, 0, 0, 90) : new Thickness(0, 0, 0, 150);

            this.grid.Width = Window.Current.Bounds.Width;
            this.grid.Height = Window.Current.Bounds.Height;
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            this.grid.Width = Window.Current.Bounds.Width;
            this.grid.Height = Window.Current.Bounds.Height;
        }

        private async void grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.converGrid.AnimateAsync(new FadeOutAnimation() { Duration = AppEnvironment.IsPhone ? 1 : 0.2 });

            //this.animationImage.ImageUrl = DicStore.GetValueOrDefault<BitmapImage>(AppCommonConst.SPLASH_BITMAPIMAGE, null);

            await Task.Delay(1000);

            if (AppEnvironment.IsPhone)
            {
                this.pathMark.AnimateAsync(new FadeOutUpAnimation());

                await Task.Delay(150);

                this.tbName.AnimateAsync(new FadeOutUpAnimation());

                await Task.Delay(150);

                await this.tbSummary.AnimateAsync(new FadeOutUpAnimation());
            }
            else
            {
                this.pathMark.AnimateAsync(new FadeOutUpAnimation() { Distance = 300 });

                await Task.Delay(150);

                this.tbName.AnimateAsync(new FadeOutUpAnimation() { Distance = 300 });

                await Task.Delay(150);

                await this.tbSummary.AnimateAsync(new FadeOutUpAnimation() { Distance = 300 });
            }
            await Task.Delay(200);

            await this.grid.AnimateAsync(new FadeOutAnimation());

            var popup = this.Parent as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
                popup.Child = null;
                popup = null;
            }
            if (AppEnvironment.IsPhone)
            {
                LeftSliderGuestureBox.Instance.ShowLeftSliderGuestureUIControl();
            }
        }
    }
}
