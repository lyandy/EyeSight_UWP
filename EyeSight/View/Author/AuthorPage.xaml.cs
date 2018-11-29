using EyeSight.Api.ApiRoot;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.TopTapGuestureUICtrl;
using EyeSight.ViewModel.Author;
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

namespace EyeSight.View.Author
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AuthorPage : Page
    {
        private NavigationHelper navigationHelper;
        private AuthorViewModel authorViewModel;

        public AuthorPage()
        {
            this.InitializeComponent();
            if (authorViewModel == null)
            {
                if (!SimpleIoc.Default.IsRegistered<AuthorViewModel>())
                {
                    SimpleIoc.Default.Register<AuthorViewModel>();
                }

                authorViewModel = SimpleIoc.Default.GetInstance<AuthorViewModel>();
            }

            this.DataContext = authorViewModel;

            this.navigationHelper = new NavigationHelper(this);

            //this.listView.ItemContainerStyle = AppEnvironment.IsPhone ? Application.Current.Resources["PhonePastCategoryListViewItemStyle"] as Style : Application.Current.Resources["PCPastCategoryListViewItemStyle"] as Style;
            this.szListView.Padding = AppEnvironment.IsPhone ? new Thickness(0, 0, 0, 0) : new Thickness(0, 0, -10, 0);
            this.szListView.Margin = AppEnvironment.IsPhone ? new Thickness(0, -3, 0, -1) : new Thickness(0, 0, -1, -1);

            this.Loaded += (ss, ee) =>
            {
                if (authorViewModel != null && authorViewModel.AuthorCollection.Count == 0)
                {
                    authorViewModel.GetAuthorCollection(authorViewModel.AuthorCollection, ApiAuthorRoot.Instance.AuthorListUrl, AppCacheNewsFileNameConst.CACHE_AUTHOR_FILENAME);
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

            //及时将当前AuthorDetail的请求链接置为null，以便下次访问的时候能够正确设置好请求链接
            DicStore.AddOrUpdateValue<string>(AppCommonConst.AUTHOR_DETAIL_NEXT_PAGE_URL, null);


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
