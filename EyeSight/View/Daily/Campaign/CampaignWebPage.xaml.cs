using Brain.Animate;
using EyeSight.Base;
using EyeSight.Common;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.ProgressUICtrl;
using EyeSight.UIControl.UserControls.WPTostUICtrl;
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

namespace EyeSight.View.Daily.Campaign
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CampaignWebPage : Page
    {
        private NavigationHelper navigationHelper;

        //WebView wv = null;

        public CampaignWebPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);

            //if (wv == null)
            //{
            //    wv = new WebView();
                
            //    wv.VerticalAlignment = VerticalAlignment.Stretch;
            //    wv.HorizontalAlignment = HorizontalAlignment.Stretch;
            //}

            this.Loaded -= CampaignWebPage_Loaded;
            this.Loaded += CampaignWebPage_Loaded; 
        }

        private void CampaignWebPage_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBox.Instance.ShowProgress();

            var model = DicStore.GetValueOrDefault<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, null);
            if (model != null)
            {
                var url = model.webUrl.raw;
                if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                {
                    this.wv.NavigationCompleted -= Wv_NavigationCompleted;
                    this.wv.NavigationCompleted += Wv_NavigationCompleted;
                    this.wv.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    WPToastBox.Instance.ShowWPToastNotice(AppNetworkMessageConst.IS_WEBVIEW_NAVIAGATION_ERROR);
                }
            }
            else
            {
                WPToastBox.Instance.ShowWPToastNotice(AppNetworkMessageConst.IS_WEBVIEW_NAVIAGATION_ERROR);
            }
        }

        private async void Wv_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            ProgressBox.Instance.HideProgress();

            //如果导航未成功，说明导航错误，网页加载出错。
            if (!e.IsSuccess)
            {
                this.Opacity = 0.0;
                WPToastBox.Instance.ShowWPToastNotice(AppNetworkMessageConst.IS_WEBVIEW_NAVIAGATION_ERROR);
            }
            else
            {
                //this.AnimationGrid.Children.Add(this.wv);

                this.wv.Opacity = 1.0;

                try
                {
                    //2、直接执行js代码
                    var replaceTarget = "var as = document.getElementsByTagName('div');" +
                                                      "for(i = 0;i < as.length;i++){" +
                                                      " if (as[i].id  == 'footer'){ as[i].style.display= 'none';break;}}";

                    this.wv.InvokeScriptAsync("eval", new[] { replaceTarget });

                    var replaceTarget1 = "var as = document.getElementsByTagName('div');" +
                                                      "for(i = 0;i < as.length;i++){" +
                                                      " if (as[i].className  == 'divider-h'){ as[i].style.display= 'none';break;}}";

                    this.wv.InvokeScriptAsync("eval", new[] { replaceTarget1 });

                    var replaceTarget2 = "var as = document.getElementsByTagName('div');" +
                                                      "for(i = 0;i < as.length;i++){" +
                                                      " if (as[i].id  == 'download-area'){ as[i].style.display= 'none';break;}}";

                    await this.wv.InvokeScriptAsync("eval", new[] { replaceTarget2 });

                    var replaceTarget3 = "var as = document.getElementsByTagName('div');" +
                                                      "for(i = 0;i < as.length;i++){" +
                                                      " if (as[i].id  == 'promotion-bar-container'){ as[i].style.display= 'none';break;}}";

                    this.wv.InvokeScriptAsync("eval", new[] { replaceTarget3 });
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }

                await AnimationGrid.AnimateAsync(new FadeInDownAnimation() { Distance = 200 });
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);

            ProgressBox.Instance.HideProgress();

            this.Loaded -= CampaignWebPage_Loaded;
            this.wv.NavigationCompleted -= Wv_NavigationCompleted;

            this.AnimationGrid.Children.Remove(this.wv);

            this.wv = null;
        }
    }
}
