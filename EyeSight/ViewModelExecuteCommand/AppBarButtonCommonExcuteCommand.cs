//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelExecuteCommand
//类名称:       AppBarButtonCommonExcuteCommand
//创建时间:     2015/10/15 16:53:08
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Model.Daily;
using EyeSight.Model.Download;
using EyeSight.UIControl.UserControls.CopyUICtrl;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.UIControl.UserControls.WinToastUICtrl;
using EyeSight.View;
using EyeSight.ViewModel.Author;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Daily;
using EyeSight.ViewModel.Download;
using EyeSight.ViewModel.Past;
using EyeSight.ViewModel.Rank;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace EyeSight.ViewModelExecuteCommand
{
    public class AppBarButtonCommonExcuteCommand
    {
        /// <summary>
        /// 收藏AppBarButton点击操作
        /// </summary>
        private RelayCommand<ModelPropertyBase> _AppBarButtonFavoriteCommand;
        public RelayCommand<ModelPropertyBase> AppBarButtonFavoriteCommand
        {
            get
            {
                return _AppBarButtonFavoriteCommand
                    ?? (_AppBarButtonFavoriteCommand = new RelayCommand<ModelPropertyBase>(async o =>
                    {
                        if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                        {
                            SimpleIoc.Default.Register<CollectionViewModel>();
                        }
                        var collectionViewModel = SimpleIoc.Default.GetInstance<CollectionViewModel>();
                        if (collectionViewModel != null)
                        {
                            if (o.isFavorite)
                            {
                                var isSuccess = await DatabaseHelper.Instance.DeleteBySingle(o);
                                if (isSuccess)
                                {
                                    #region 每日精选
                                    if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                                    {
                                        SimpleIoc.Default.Register<DailyViewModel>();
                                    }
                                    var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                                    if (dailyViewModel != null)
                                    {
                                        var dailyModels = from m in dailyViewModel.DailyFlipViewCollection
                                                          where m.id == o.id
                                                          select m;
                                        var dailyModel = dailyModels.FirstOrDefault() as ModelPropertyBase;
                                        if (dailyModel != null)
                                        {
                                            dailyModel.isFavorite = false;
                                        }
                                    }
                                    #endregion

                                    #region 收藏集合处理
                                    var models = from m in collectionViewModel.FavoriteCollection
                                                where m.id == o.id
                                                select m;
                                    var model = models.FirstOrDefault() as ModelPropertyBase;
                                    if (model != null)
                                    {
                                        collectionViewModel.FavoriteCollection.Remove(model);
                                    }
                                    #endregion

                                    #region 往期分类详细集合处理
                                    if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                                    {
                                        SimpleIoc.Default.Register<PastDetailViewModel>();
                                    }
                                    var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                                    if (pastDetailViewModel != null)
                                    {
                                        var pastDetailModels = from m in pastDetailViewModel.CategoryDetailCollection
                                                     where m.id == o.id
                                                     select m;
                                        var pastDetailModel = pastDetailModels.FirstOrDefault() as ModelPropertyBase;
                                        if (pastDetailModel != null)
                                        {
                                            pastDetailModel.isFavorite = false;
                                        }
                                    }
                                    #endregion

                                    #region 排行榜
                                    if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                                    {
                                        SimpleIoc.Default.Register<RankViewModel>();
                                    }
                                    var rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();
                                    if (rankViewModel != null)
                                    {
                                        #region Performance
                                        var rankPerformanceModels = from m in rankViewModel.RankPerformanceCollection
                                                         where m.id == o.id
                                                         select m;
                                        var rankPerformanceModel = rankPerformanceModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankPerformanceModel != null)
                                        {
                                            rankPerformanceModel.isFavorite = false;
                                        }
                                        #endregion

                                        #region Week
                                        var rankWeekModels = from m in rankViewModel.WeekCollection
                                                                    where m.id == o.id
                                                                    select m;
                                        var rankWeekModel = rankWeekModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankWeekModel != null)
                                        {
                                            rankWeekModel.isFavorite = false;
                                        }
                                        #endregion

                                        #region Month
                                        var rankMonthModels = from m in rankViewModel.MonthCollection
                                                             where m.id == o.id
                                                             select m;
                                        var rankMonthModel = rankMonthModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankMonthModel != null)
                                        {
                                            rankMonthModel.isFavorite = false;
                                        }
                                        #endregion

                                        #region All
                                        var rankAllModels = from m in rankViewModel.RankAllCollection
                                                              where m.id == o.id
                                                              select m;
                                        var rankAllModel = rankAllModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankAllModel != null)
                                        {
                                            rankAllModel.isFavorite = false;
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 我的下载
                                    if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                    {
                                        SimpleIoc.Default.Register<DownloadViewModel>();
                                    }
                                    var downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();
                                    if (downloadViewModel != null)
                                    {
                                        var downloadyModels = from m in downloadViewModel.DownloadCollection
                                                          where m.id == o.id
                                                          select m;
                                        var downloadModel = downloadyModels.FirstOrDefault() as ModelPropertyBase;
                                        if (downloadModel != null)
                                        {
                                            downloadModel.isFavorite = false;
                                        }
                                    }
                                    #endregion

                                    #region 优质作者
                                    if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                                    {
                                        SimpleIoc.Default.Register<AuthorDetailViewModel>();
                                    }
                                    var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();
                                    if (authorDetailViewModel != null)
                                    {
                                        var authorDetailModels = from m in authorDetailViewModel.AuthorDetailCollection
                                                              where m.id == o.id
                                                              select m;
                                        var authorDetailModel = authorDetailModels.FirstOrDefault() as ModelPropertyBase;
                                        if (authorDetailModel != null)
                                        {
                                            authorDetailModel.isFavorite = false;
                                        }
                                    }
                                    #endregion

                                    o.isFavorite = false;
                                }
                            }
                            else
                            {
                                var isSuccess = await DatabaseHelper.Instance.InsertBySingle(o);
                                if (isSuccess)
                                {
                                    #region 收藏集合处理
                                    collectionViewModel.FavoriteCollection.Add(o);
                                    #endregion

                                    #region 每日精选
                                    if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                                    {
                                        SimpleIoc.Default.Register<DailyViewModel>();
                                    }
                                    var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                                    if (dailyViewModel != null)
                                    {
                                        var dailyModels = from m in dailyViewModel.DailyFlipViewCollection
                                                               where m.id == o.id
                                                               select m;
                                        var dailyModel = dailyModels.FirstOrDefault() as ModelPropertyBase;
                                        if (dailyModel != null)
                                        {
                                            dailyModel.isFavorite = true;
                                        }
                                    }
                                    #endregion

                                    #region 往期分类详细集合处理
                                    if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                                    {
                                        SimpleIoc.Default.Register<PastDetailViewModel>();
                                    }
                                    var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                                    if (pastDetailViewModel != null)
                                    {
                                        var pastDetailModels = from m in pastDetailViewModel.CategoryDetailCollection
                                                               where m.id == o.id
                                                               select m;
                                        var pastDetailModel = pastDetailModels.FirstOrDefault() as ModelPropertyBase;
                                        if (pastDetailModel != null)
                                        {
                                            pastDetailModel.isFavorite = true;
                                        }
                                    }
                                    #endregion

                                    #region 排行榜
                                    if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                                    {
                                        SimpleIoc.Default.Register<RankViewModel>();
                                    }
                                    var rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();
                                    if (rankViewModel != null)
                                    {
                                        #region Performance
                                        var rankPerformanceModels = from m in rankViewModel.RankPerformanceCollection
                                                                    where m.id == o.id
                                                                    select m;
                                        var rankPerformanceModel = rankPerformanceModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankPerformanceModel != null)
                                        {
                                            rankPerformanceModel.isFavorite = true;
                                        }
                                        #endregion

                                        #region Week
                                        var rankWeekModels = from m in rankViewModel.WeekCollection
                                                             where m.id == o.id
                                                             select m;
                                        var rankWeekModel = rankWeekModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankWeekModel != null)
                                        {
                                            rankWeekModel.isFavorite = true;
                                        }
                                        #endregion

                                        #region Month
                                        var rankMonthModels = from m in rankViewModel.MonthCollection
                                                              where m.id == o.id
                                                              select m;
                                        var rankMonthModel = rankMonthModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankMonthModel != null)
                                        {
                                            rankMonthModel.isFavorite = true;
                                        }
                                        #endregion

                                        #region All
                                        var rankAllModels = from m in rankViewModel.RankAllCollection
                                                            where m.id == o.id
                                                            select m;
                                        var rankAllModel = rankAllModels.FirstOrDefault() as ModelPropertyBase;
                                        if (rankAllModel != null)
                                        {
                                            rankAllModel.isFavorite = true;
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region 我的下载
                                    if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                    {
                                        SimpleIoc.Default.Register<DownloadViewModel>();
                                    }
                                    var downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();
                                    if (downloadViewModel != null)
                                    {
                                        var downloadyModels = from m in downloadViewModel.DownloadCollection
                                                              where m.id == o.id
                                                              select m;
                                        var downloadModel = downloadyModels.FirstOrDefault() as ModelPropertyBase;
                                        if (downloadModel != null)
                                        {
                                            downloadModel.isFavorite = true;
                                        }
                                    }
                                    #endregion

                                    #region 优质作者
                                    if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                                    {
                                        SimpleIoc.Default.Register<AuthorDetailViewModel>();
                                    }
                                    var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();
                                    if (authorDetailViewModel != null)
                                    {
                                        var authorDetailModels = from m in authorDetailViewModel.AuthorDetailCollection
                                                                 where m.id == o.id
                                                                 select m;
                                        var authorDetailModel = authorDetailModels.FirstOrDefault() as ModelPropertyBase;
                                        if (authorDetailModel != null)
                                        {
                                            authorDetailModel.isFavorite = true;
                                        }
                                    }
                                    #endregion

                                    o.isFavorite = true;

                                    if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_WHEN_FAVORITE_VIDEO, false))
                                    {
                                        if (this.AppBarButtonDownloadCommand.CanExecute(o))
                                        {
                                            this.AppBarButtonDownloadCommand.Execute(o);
                                        }
                                    }
                                }
                            }

                            WinToastBox.Instance.ShowWinToastNotice("已" + (o.isFavorite ? "添加" : "取消") + "收藏", o.isFavorite, 1);
                        }

                    }, o => { return o == null ? false : true; }
                ));
            }
        }

        /// <summary>
        /// 下载AppBarButton点击操作
        /// </summary>
        private RelayCommand<ModelPropertyBase> _AppBarButtonDownloadCommand;
        public RelayCommand<ModelPropertyBase> AppBarButtonDownloadCommand
        {
            get
            {
                return _AppBarButtonDownloadCommand
                    ?? (_AppBarButtonDownloadCommand = new RelayCommand<ModelPropertyBase>(async o =>
                    {
                        if (AppEnvironment.IsInternetAccess)
                        {
                            bool isDownloadAccepted = false;
                            if (AppEnvironment.IsWlanOrInternet)
                            {
                                isDownloadAccepted = true;
                            }
                            else
                            {
                                var messageDialog = new MessageDialog(AppNetworkMessageConst.NETWORK_IS_NOT_WIFI_OR_LAN_TO_DOWNLOAD, "下载提示");

                                messageDialog.Commands.Add(new UICommand("继续", new UICommandInvokedHandler(uc =>
                                {
                                    isDownloadAccepted = true;
                                })));

                                messageDialog.Commands.Add(new UICommand("取消"));

                                messageDialog.DefaultCommandIndex = 1;

                                await messageDialog.ShowAsyncQueue();
                            }

                            if (isDownloadAccepted)
                            {
                                if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                {
                                    SimpleIoc.Default.Register<DownloadViewModel>();
                                }
                                var downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();
                                if (downloadViewModel != null)
                                {
                                    var list = from l in downloadViewModel.DownloadingList
                                               where l.DownloadId == o.id
                                               select l;

                                    var d = list.FirstOrDefault() as DownloadingModel;
                                    if (d == null)
                                    {
                                        if (!o.isAleadyDownload)
                                        {
                                            var downloadUrl = CommonHelper.Instance.DecideDownloadUrl(o);
                                            if (!string.IsNullOrEmpty(downloadUrl))
                                            {
                                                DownloadingModel dlm = new DownloadingModel();
                                                dlm.DownloadId = o.id;

                                                //下面用来通知已存在的集合要去显示下载东西的进度了
                                                #region 每日精选
                                                if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                                                {
                                                    SimpleIoc.Default.Register<DailyViewModel>();
                                                }
                                                var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                                                if (dailyViewModel != null)
                                                {
                                                    var dailyModels = from m in dailyViewModel.DailyFlipViewCollection
                                                                      where m.id == o.id
                                                                      select m;
                                                    var dailyModel = dailyModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (dailyModel != null)
                                                    {
                                                        dailyModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(dailyModel);
                                                    }
                                                }
                                                #endregion

                                                #region 收藏集合处理
                                                if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                                {
                                                    SimpleIoc.Default.Register<CollectionViewModel>();
                                                }
                                                var collectionViewModel = SimpleIoc.Default.GetInstance<CollectionViewModel>();
                                                if (dailyViewModel != null)
                                                {
                                                    var collectionModels = from m in collectionViewModel.FavoriteCollection
                                                                           where m.id == o.id
                                                                           select m;
                                                    var collectionModel = collectionModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (collectionModel != null)
                                                    {
                                                        //由于没有将isFavorite存入数据库，所以可以弥补的方法就是查看收藏集合里有没有该条目，有的话的手动将isFavorite置为true。此时不能采用改数据库字段的方式去处理，因为收藏数据库在之前的几个版本已经定下结构，如果此时更改了数据库的结构会导致数据存储失败导致应用崩溃。所以，在以后的数据库字段的设计中要尽量想的周全，多几个冗余字段是没问题的。
                                                        o.isFavorite = true;
                                                        o.downloadProgress = "开始下载";
                                                        collectionModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(collectionModel);
                                                    }
                                                    else
                                                    {
                                                        o.isFavorite = false;
                                                    }
                                                }
                                                #endregion

                                                #region 往期分类详细集合处理
                                                if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                                                {
                                                    SimpleIoc.Default.Register<PastDetailViewModel>();
                                                }
                                                var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                                                if (pastDetailViewModel != null)
                                                {
                                                    var pastDetailModels = from m in pastDetailViewModel.CategoryDetailCollection
                                                                           where m.id == o.id
                                                                           select m;
                                                    var pastDetailModel = pastDetailModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (pastDetailModel != null)
                                                    {
                                                        pastDetailModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(pastDetailModel);
                                                    }
                                                }
                                                #endregion

                                                #region 排行榜
                                                if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                                                {
                                                    SimpleIoc.Default.Register<RankViewModel>();
                                                }
                                                var rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();
                                                if (rankViewModel != null)
                                                {
                                                    #region Performance
                                                    var rankPerformanceModels = from m in rankViewModel.RankPerformanceCollection
                                                                                where m.id == o.id
                                                                                select m;
                                                    var rankPerformanceModel = rankPerformanceModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (rankPerformanceModel != null)
                                                    {
                                                        rankPerformanceModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(rankPerformanceModel);
                                                    }
                                                    #endregion

                                                    #region Week
                                                    var rankWeekModels = from m in rankViewModel.WeekCollection
                                                                         where m.id == o.id
                                                                         select m;
                                                    var rankWeekModel = rankWeekModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (rankWeekModel != null)
                                                    {
                                                        rankWeekModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(rankWeekModel);
                                                    }
                                                    #endregion

                                                    #region Month
                                                    var rankMonthModels = from m in rankViewModel.MonthCollection
                                                                          where m.id == o.id
                                                                          select m;
                                                    var rankMonthModel = rankMonthModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (rankMonthModel != null)
                                                    {
                                                        rankMonthModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(rankMonthModel);
                                                    }
                                                    #endregion

                                                    #region All
                                                    var rankAllModels = from m in rankViewModel.RankAllCollection
                                                                        where m.id == o.id
                                                                        select m;
                                                    var rankAllModel = rankAllModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (rankAllModel != null)
                                                    {
                                                        rankAllModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(rankAllModel);
                                                    }
                                                    #endregion
                                                }
                                                #endregion

                                                #region 优质作者
                                                if (!SimpleIoc.Default.IsRegistered<AuthorDetailViewModel>())
                                                {
                                                    SimpleIoc.Default.Register<AuthorDetailViewModel>();
                                                }
                                                var authorDetailViewModel = SimpleIoc.Default.GetInstance<AuthorDetailViewModel>();
                                                if (authorDetailViewModel != null)
                                                {
                                                    var authorDetailModels = from m in authorDetailViewModel.AuthorDetailCollection
                                                                           where m.id == o.id
                                                                           select m;
                                                    var authorDetailModel = authorDetailModels.FirstOrDefault() as ModelPropertyBase;
                                                    if (authorDetailModel != null)
                                                    {
                                                        authorDetailModel.downloadProgress = "开始下载";
                                                        dlm.DownloadModelList.Add(authorDetailModel);
                                                    }
                                                }
                                                #endregion

                                                dlm.StartDownload(downloadUrl, async isDownloadSuccess =>
                                                {
                                                    //下载完成后将其从下载队列中移除
                                                    downloadViewModel.DownloadingList.Remove(dlm);

                                                    //这里的垃圾回收是回收以下载到内存的视频，不及时清理会占用内存时间比较长
                                                    GC.Collect();

                                                    if (isDownloadSuccess)
                                                    {
                                                        //向下载数据库中添加条目
                                                        var isDatabaseOperateSuccess = await DatabaseHelper.Instance.InsertBySingle(o, true);
                                                        if (isDatabaseOperateSuccess)
                                                        {
                                                            await DispatcherHelper.RunAsync(() =>
                                                            {
                                                                //确认当前条目的标记状态
                                                                NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;
                                                                if (rootPage != null)
                                                                {
                                                                    o.isEditing = rootPage.isDownloadEditing;
                                                                }

                                                                //下载完成后将下载完毕的条目添加到已下载的集合
                                                                downloadViewModel.DownloadCollection.Add(o);

                                                                if (downloadViewModel.DownloadCollection.Count == 0)
                                                                {
                                                                    downloadViewModel.isEmptyShow = true;
                                                                }
                                                                else
                                                                {
                                                                    downloadViewModel.isEmptyShow = false;
                                                                }
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                });

                                                downloadViewModel.DownloadingList.Add(dlm);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                        }

                    }, o => { return o == null ? false : true; }
                ));
            }
        }

        /// <summary>
        /// 复制链接AppBarButton点击操作
        /// </summary>
        private RelayCommand<ModelPropertyBase> _AppBarButtonCopyCommand;
        public RelayCommand<ModelPropertyBase> AppBarButtonCopyCommand
        {
            get
            {
                return _AppBarButtonCopyCommand
                    ?? (_AppBarButtonCopyCommand = new RelayCommand<ModelPropertyBase>( o =>
                    {
                        try
                        {
                            var dataPackage = new DataPackage();
                            dataPackage.SetText(o.rawWebUrl);
                            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

                            CopyBox.Instance.ShowCopyNotice(1);
                        }
                        catch { }
                    }, o => { return o == null ? false : true; }
                ));
            }
        }
    }
}
