using EyeSight.Common;
using EyeSight.Config;
using EyeSight.ViewModel.Gift;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EyeSight.View.Gift
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GiftPage : Page
    {
        private NavigationHelper navigationHelper;
        private GiftViewModel giftViewModel;

        public GiftPage()
        {
            this.InitializeComponent();

            if (giftViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<GiftViewModel>())
                {
                    SimpleIoc.Default.Register<GiftViewModel>();
                }

                giftViewModel = SimpleIoc.Default.GetInstance<GiftViewModel>();
            }

            this.DataContext = giftViewModel;

            this.navigationHelper = new NavigationHelper(this);

            //this.listView.ItemContainerStyle = AppEnvironment.IsPhone ? Application.Current.Resources["PhonePastCategoryListViewItemStyle"] as Style : Application.Current.Resources["PCPastCategoryListViewItemStyle"] as Style;
            //this.listView.Padding = AppEnvironment.IsPhone ? new Thickness(0, 0, -2, -2) : new Thickness(0, 0, 0, 3);

            this.Loaded += (ss, ee) =>
            {
                if (giftViewModel != null && giftViewModel.GiftCollection.Count == 0)
                {
                    giftViewModel.GetGiftCollection();
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
