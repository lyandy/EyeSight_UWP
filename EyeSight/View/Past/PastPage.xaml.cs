using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.TopTapGuestureUICtrl;
using EyeSight.ViewModel.Past;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View.Past
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PastPage : Page
    {
        private NavigationHelper navigationHelper;
        private PastViewModel pastViewModel;

        public PastPage()
        {
            this.InitializeComponent();

            if (pastViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<PastViewModel>())
                {
                    SimpleIoc.Default.Register<PastViewModel>();
                }

                pastViewModel = SimpleIoc.Default.GetInstance<PastViewModel>();
            }

            this.DataContext = pastViewModel;

            this.navigationHelper = new NavigationHelper(this);

            this.szListView.ItemContainerStyle = AppEnvironment.IsPhone ? Application.Current.Resources["PhonePastCategoryListViewItemStyle"] as Style : Application.Current.Resources["PCPastCategoryListViewItemStyle"] as Style;
            this.szListView.Padding = AppEnvironment.IsPhone ? new Thickness(0, 0, -2.5, -2) : new Thickness(0, 0, 0, 3);
            this.szListView.Margin = AppEnvironment.IsPhone ? new Thickness(0, 0, -2.5, 0) : new Thickness(0, 0, -1, 0);

            this.Loaded += (ss, ee) =>
            {
                if (pastViewModel != null && pastViewModel.CategoryCollection.Count == 0)
                {
                    pastViewModel.GetPastCategory(pastViewModel.CategoryCollection, ApiPastRoot.Instance.CatrgoryUrl, AppCacheNewsFileNameConst.CACHE_PAST_CATEGORY_FILENAME);
                }

                if (AppEnvironment.IsPhone)
                {
                    TopTapGuestureBox.Instance.ShowTopTapGuestureUIControl();
                }
            };

            this.Unloaded += (ss, ee) =>
            {
                if (AppEnvironment.IsPhone)
                {
                    //TopTapGuestureBox.Instance.HideTopTapGuestureUIControl();
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            //及时将当前PastCategoryDetail的请求链接置为null，以便下次访问的时候能够正确设置好请求链接
            DicStore.AddOrUpdateValue<string>(AppCommonConst.PAST_CATEGORY_DETAIL_NEXT_PAGE_URL, null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
