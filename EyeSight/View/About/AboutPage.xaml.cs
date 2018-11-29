using Brain.Animate;
using EyeSight.Common;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View.About
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private NavigationHelper navigationHelper;
        public AboutPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);

            var with = Window.Current.Bounds.Width * 2 / 3;

            if (with <= 320)
            {
                this.grid.Width = Window.Current.Bounds.Width;
            }
            else
            {
                this.grid.Width = with;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var with = Window.Current.Bounds.Width * 2 / 3;

            if (with <= 320)
            {
                this.grid.Width = Window.Current.Bounds.Width;
            }
            else
            {
                this.grid.Width = with;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            await this.pathMark.AnimateAsync(new FlipAnimation());

            //this.pathMark.AnimateAsync(new BreathingAnimation() { Forever = true });

            this.tbName.AnimateAsync(new FadeInRightAnimation() { Distance = 600, Duration = 0.25 });
            await this.tbVersion.AnimateAsync(new FadeInLeftAnimation() { Distance = 600, Duration = 0.25 });

            this.tbAuthor.AnimateAsync(new FlipInXAnimation());

            await this.btnReview.AnimateAsync(new FadeInDownAnimation());
            this.btnReview.AnimateAsync(new TadaAnimation() { Forever = true });

            this.tbBam.Text = "静水流深,沧笙踏歌";
            await tbBam.AnimateText(RandomAnimationHelper.Instance.GetAnimation(), 0.1);
            this.tbBam.Tapped += TbBam_Tapped;

            gs1.Color = gs11.Color = gs111.Color = ((SolidColorBrush)Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"]).Color;
            double from1 = 0.1;
            while (from1 < 1)
            {
                gs1.Offset = gs2.Offset = from1;
                from1 += 0.02;
                await Task.Delay(10);
            }
            double from11 = 0.1;
            while (from11 < 1)
            {
                gs11.Offset = gs22.Offset = from11;
                from11 += 0.02;
                await Task.Delay(10);
            }
            double from111 = 0.1;
            while (from111 < 1)
            {
                gs111.Offset = gs222.Offset = from111;
                from111 += 0.02;
                await Task.Delay(10);
            }

            //await this.cooperateImage.AnimateAsync(new FadeInUpAnimation());
        }

        private async void TbBam_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.cnblogs.com/lyandy/p/5435125.html"));

            //try
            //{
            //    var dataPackage = new DataPackage();
            //    dataPackage.SetText("513068538");
            //    Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            //    await new MessageDialog("群号已复制：513068538。", "提示").ShowAsyncQueue();
            //}
            //catch
            //{
            //    await new MessageDialog("群号复制过程发生错误，请重试。").ShowAsyncQueue();
            //}
        }

        //protected 
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

            this.tbBam.Tapped -= TbBam_Tapped;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonHelper.Instance.RateApp();
        }
        private async void cooperateImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ayywin://"));
        }

        private async void tbAuthor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/lyandy"));
        }
    }
}
