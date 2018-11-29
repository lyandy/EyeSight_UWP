//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel
//类名称:       NavigationRootViewModel
//创建时间:     2015/9/21 星期一 16:56:19
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Helper;
using EyeSight.Model;
using EyeSight.View.About;
using EyeSight.View.Author;
using EyeSight.View.Collection;
using EyeSight.View.Daily;
using EyeSight.View.Download;
using EyeSight.View.Gift;
using EyeSight.View.Donate;
using EyeSight.View.Past;
using EyeSight.View.Rank;
using EyeSight.View.Setting;
using EyeSight.ViewModelAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModel
{
    public class NavigationRootViewModel : NavigationRootViewModelAttribute
    {
        public async Task InitCategoryPngs()
        {
            DailyPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Daily" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            PastPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Past" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            AuthorPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Author" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            RankPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Rank" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            CollectionPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Collection" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            DownloadPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Download" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            DonatePNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Donate" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            GiftPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Gift" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            SettingPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/Setting" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
            AboutPNG = await CommonHelper.Instance.LoadImageSource("ms-appx:///Assets/Category/About" + (AppEnvironment.IsPhone ? "_Phone" : "") + ".png");
        }

        public void GetEyeSightCommonCollection()
        {
            EyeSightCommonCollection.Clear();
            // 添加项列表
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = DailyPNG, Title = "每日精选", ClassType = typeof(DailyPage), IsSelected = true });
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = PastPNG, Title = "精彩发现", ClassType = typeof(PastPage), IsSelected = false });
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = AuthorPNG, Title = "优质作者", ClassType = typeof(AuthorPage), IsSelected = false });
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = RankPNG, Title = "火热排行", ClassType = typeof(RankPage), IsSelected = false });
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = CollectionPNG, Title = "我的收藏", ClassType = typeof(CollectionPage), IsSelected = false });
            EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = DownloadPNG, Title = "我的下载", ClassType = typeof(DownloadPage), IsSelected = false });
            if (AppEnvironment.IsPhone == false)
            {
                EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = DonatePNG, Title = "更新动力", ClassType = typeof(DonatePage), IsSelected = false });
            }
            //if (AppEnvironment.IsPhone)
            //{
            //    EyeSightCommonCollection.Add(new NavigationRootModel { IconBitmap = GiftPNG, Title = "应用推荐", ClassType = typeof(GiftPage), IsSelected = false });
            //}
        }

        public void GetEyeSightBottomCollection()
        {
            EyeSightBottomCollection.Clear();
            // 添加项列表
            EyeSightBottomCollection.Add(new NavigationRootModel { IconBitmap = SettingPNG, Title = "设置", ClassType = typeof(SettingPage), IsSelected = false });
            EyeSightBottomCollection.Add(new NavigationRootModel { IconBitmap = AboutPNG, Title = "关于", ClassType = typeof(AboutPage), IsSelected = false });
        }

        #region 清理ViewModel
        public override void Cleanup()
        {
            CleanApp();
            base.Cleanup();
        }

        public void CleanApp()
        {
            IsBusy = false;
        }
        #endregion
    }
}
