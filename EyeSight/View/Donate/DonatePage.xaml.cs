using EyeSight.Common;
using EyeSight.Config;
using GalaSoft.MvvmLight.Ioc;
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
using Brain.Animate;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EyeSight.View.Donate
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DonatePage : Page
    {
        private NavigationHelper navigationHelper;

        public DonatePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private  void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            

            this.borderWX.AnimateAsync(new FadeInDownAnimation() { Distance = 600, Duration = 0.5 });
            this.borderAliPay.AnimateAsync(new FadeInUpAnimation() { Distance = 600, Duration = 0.5 });
        }
    }
}
