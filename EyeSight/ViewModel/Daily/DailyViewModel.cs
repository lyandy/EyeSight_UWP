//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel.Daily
//类名称:       DailyViewModel
//创建时间:     2015/9/21 星期一 19:21:43
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Daily.ViewModelAttribute;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using EyeSight.Model.Daily;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Download;
using EyeSight.Model.Download;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using EyeSight.Base;
using EyeSight.Encrypt;

namespace EyeSight.ViewModel.Daily
{
    public class DailyViewModel : DailyViewModelAttribute
    {
        public async void GetDaily(ObservableCollection<Videolist> collection, ObservableCollection<Videolist> flipViewCollection, string url, string cacheFileName = null, bool isRefresh = false)
        {
            //这里是为了异步，在无网络加载读取本地缓存文本的时候不会卡界面
            DispatcherHelper.RunAsync(async () =>
            {
                IsBusy = true;

                bool is_Campaign_Available = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_CAMPAIGN_AVAILABLE, false);
                string campaign_Image_Url = SettingsStore.GetValueOrDefault<string>(AppCommonConst.CAMPAIGN_IMAGE_URL, null);
                string campaign_Action_Url = SettingsStore.GetValueOrDefault<string>(AppCommonConst.CAMPAIGN_ACTION_URL, null);

                var backJson = await WebDataHelper.Instance.GetFromUrlWithAuthReturnString(url, null, 20);
                if (backJson != null)
                {
                    var result = JsonConvertHelper.Instance.DeserializeObject<DailyRootModel>(backJson);
                    if (result != null)
                    {
                        if (result.dailyList.Count != 0)
                        {
                            //如果是第一次加载数据或者刷新，则记录当前的刷新时间
                            if (collection.Count == 0 || isRefresh)
                            {
                                SettingsStore.AddOrUpdateValue<string>(AppCommonConst.LAST_UPDATE_TIME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }

                            if (isRefresh)
                            {
                                collection.Clear();
                                flipViewCollection.Clear();

                                List<Videolist> myCollection = new List<Videolist>();
                                List<Videolist> myFlipViewCollection = new List<Videolist>();

                                if (!AppEnvironment.IsPhone)
                                {
                                    result.dailyList[0].videoList.ForEach(B =>
                                    {
                                        myFlipViewCollection.Add(B);
                                    });
                                }
                                else
                                {
                                    result.dailyList.ForEach(A =>
                                    {
                                        if (myCollection.Count != 0)
                                        {
                                            Videolist vl = new Videolist();
                                            vl.today = A.today;
                                            myCollection.Add(vl);
                                        }
                                        else
                                        {
                                            if (is_Campaign_Available && campaign_Image_Url != null && campaign_Action_Url != null)
                                            {
                                                Videolist vlCam = new Videolist();
                                                //vlCam.is_Campaign_Available = is_Campaign_Available;
                                                vlCam.coverForDetail = campaign_Image_Url;
                                                vlCam.webUrl.raw = campaign_Action_Url;
                                                myCollection.Add(vlCam);
                                            }
                                        }

                                        A.videoList.ForEach(B =>
                                        {
                                            myCollection.Add(B);
                                            myFlipViewCollection.Add(B);
                                        });
                                        
                                    });
                                }

                                #region 处理 收藏 和 下载
                                if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                {
                                    SimpleIoc.Default.Register<CollectionViewModel>(false);
                                }
                                var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                if (cvm != null)
                                {
                                    myFlipViewCollection.ForEach(A =>
                                    {
                                        var model = from m in cvm.FavoriteCollection
                                                    where A.id == m.id
                                                    select A;
                                        if (model.Count() != 0)
                                        {
                                            A.isFavorite = true;
                                        }
                                    });
                                }

                                if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                {
                                    SimpleIoc.Default.Register<DownloadViewModel>(false);
                                }
                                var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

                                if (dlvm != null)
                                {
                                    myFlipViewCollection.ForEach(A =>
                                    {
                                        //先去检查下载队列里面有没有
                                        var downloadingModels = from m in dlvm.DownloadingList
                                                                where m.DownloadId == A.id
                                                                select m;
                                        //这里说明有正在下载的，那么应该把当前的Model添加到下载队列里面的DownloadModelList
                                        if (downloadingModels.Count() != 0)
                                        {
                                            var downloadModel = downloadingModels.FirstOrDefault() as DownloadingModel;
                                            if (downloadModel != null)
                                            {
                                                downloadModel.DownloadModelList.Add(A);
                                            }
                                        }
                                        else
                                        {
                                            //然后检查已下载集合里面有没有
                                            var hasDownloadModels = from m in dlvm.DownloadCollection
                                                                    where A.id == m.id
                                                                    select A;
                                            if (hasDownloadModels.Count() != 0)
                                            {
                                                A.isAleadyDownload = true;
                                            }
                                        }
                                    });
                                }

                                #endregion

                                DailyCollection = new ObservableCollection<Videolist>(myCollection);
                                DailyFlipViewCollection = new ObservableCollection<Videolist>(myFlipViewCollection);

                                myCollection = null;
                                myFlipViewCollection = null;
                            }
                            else
                            {
                                if (!AppEnvironment.IsPhone)
                                {
                                    result.dailyList[0].videoList.ForEach(B =>
                                    {
                                        flipViewCollection.Add(B);
                                    });
                                }
                                else
                                {
                                    result.dailyList.ForEach(A =>
                                    {
                                        //这个地方是当collection.Count == 0 的时候不添加日期头的处理
                                        if (collection.Count != 0)
                                        {
                                            Videolist vl = new Videolist();
                                            vl.today = A.today;
                                            collection.Add(vl);
                                        }
                                        else
                                        {
                                            if (is_Campaign_Available && campaign_Image_Url != null && campaign_Action_Url != null)
                                            {
                                                Videolist vlCam = new Videolist();
                                                //vlCam.is_Campaign_Available = is_Campaign_Available;
                                                vlCam.coverForDetail = campaign_Image_Url;
                                                vlCam.webUrl.raw = campaign_Action_Url;
                                                collection.Add(vlCam);
                                            }
                                        }

                                        A.videoList.ForEach(B =>
                                        {
                                            collection.Add(B);
                                            flipViewCollection.Add(B);
                                        });
                                    });
                                }

                                #region 处理收藏和下载
                                if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                {
                                    SimpleIoc.Default.Register<CollectionViewModel>(false);
                                }
                                var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                if (cvm != null)
                                {
                                    flipViewCollection.ForEach(A =>
                                    {
                                        var model = from m in cvm.FavoriteCollection
                                                    where A.id == m.id
                                                    select A;
                                        if (model.Count() != 0)
                                        {
                                            A.isFavorite = true;
                                        }
                                    });
                                }

                                if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                {
                                    SimpleIoc.Default.Register<DownloadViewModel>(false);
                                }
                                var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

                                if (dlvm != null)
                                {
                                    flipViewCollection.ForEach(A =>
                                    {
                                        //先去检查下载队列里面有没有
                                        var downloadingModels = from m in dlvm.DownloadingList
                                                                  where m.DownloadId == A.id
                                                                  select m;
                                        //这里说明有正在下载的，那么应该把当前的Model添加到下载队列里面的DownloadModelList
                                        if (downloadingModels.Count() != 0)
                                        {
                                            var downloadModel = downloadingModels.FirstOrDefault() as DownloadingModel;
                                            if (downloadModel != null)
                                            {
                                                downloadModel.DownloadModelList.Add(A);
                                            }
                                        }
                                        else
                                        {
                                            //然后检查已下载集合里面有没有
                                            var hasDownloadModels = from m in dlvm.DownloadCollection
                                                                    where A.id == m.id
                                                                    select A;
                                            if (hasDownloadModels.Count() != 0)
                                            {
                                                A.isAleadyDownload = true;
                                            }
                                        }
                                    });
                                }
                                #endregion

                                #region 首屏和磁贴
                                Task.Run(async () =>
                                {
                                    #region 存储每日精选的最新一天的图片。因为加入了动态磁贴，所以无论是手机还是pc都要去下载pc端今日的图片
                                    //if (!AppEnvironment.IsPhone)
                                    //{
                                    if (AppEnvironment.IsNeedToUpdateSplashImage)
                                    {
                                        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.PCSplashScreenImageCacheRelativePath, CreationCollisionOption.OpenIfExists);
                                        foreach (var file in await folder.GetFilesAsync())
                                        {
                                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                        }

                                        List<Task> taskList = new List<Task>();

                                        flipViewCollection.Take(8).ForEach(async A =>
                                        {
                                            try
                                            {
                                                var name = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(A.coverForDetail);

                                                Task task = Task.Run(async () =>
                                                {
                                                    await WebDownFileHelper.Instance.SaveAsyncWithHttp(A.coverForDetail, name, folder);
                                                });

                                                taskList.Add(task);
                                            }
                                            catch { }
                                        });

                                        Task.WhenAll(taskList).ContinueWith(o =>
                                        {
                                            Debug.WriteLine("我刚加载完首屏");

                                            #region 处理磁贴
                                            DicStore.AddOrUpdateValue<object>(AppCommonConst.CURRENT_TILE_COLLECTION, flipViewCollection.Take(5).ToList());
                                            if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_TILE_ACTIVE, true) == true)
                                            {
                                                TileHelper.Instance.UpdateTiles(flipViewCollection.Take(5).ToList());
                                            }
                                            else
                                            {
                                                TileHelper.Instance.CloseTiles();
                                            }
                                            #endregion

                                            Debug.WriteLine("我加载完磁贴了");
                                        });
                                    }

                                    //}
                                    #endregion
                                });

                                #region 处理磁贴
                                DicStore.AddOrUpdateValue<object>(AppCommonConst.CURRENT_TILE_COLLECTION, flipViewCollection.Take(5).ToList());
                                if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_TILE_ACTIVE, true) == true)
                                {
                                    TileHelper.Instance.UpdateTiles(flipViewCollection.Take(5).ToList());
                                }
                                else
                                {
                                    TileHelper.Instance.CloseTiles();
                                }
                                #endregion

                                #endregion
                            }

                            Debug.WriteLine("我的数据加载完了");

                            DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, false);

                            //只存第一页的数据
                            if (cacheFileName != null)
                            {
                                //将第一页数据缓存到本地
                                FileHelper.Instance.SaveTextToFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName, backJson);
                            }
                        }
                        else
                        {
                            //判断是不是分页索引第一页，如果是第一页并且获取到的条目个数为0个，则此时要求再次获取。此时提示“没有获取到数据，请重试”
                            if (collection.Count == 0 && !isRefresh)
                            {
                                //这里使用反射
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.COLLECTION_ITEM_IS_ZERO, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                            }
                            //如果不是第一页，就不用去管
                            else { }
                        }
                    }
                    else
                    {
                        //判断是不是分页索引第一页，如果是第一页的话则会弹出提示，重新加载。因为第一页没有加载成功，本条目下就没数据，此时就要弹出一个东西让其重新加载数据。如果不是第一页就不用去管了
                        if (collection.Count == 0)
                        {
                            //加载本地数据
                            var localJson = await FileHelper.Instance.ReadTextFromFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName);
                            if (localJson != null)
                            {
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<DailyRootModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.dailyList.Count != 0)
                                    {
                                        //不是刷新说明是第一次加载。如果是刷新的话，此处无动作。因为如果数据加载成功的话前面会正确处理，如果数据加载失败了刷新还到这一步的话添加也无用。
                                        if (!isRefresh)
                                        {
                                            if (!AppEnvironment.IsPhone)
                                            {
                                                localResult.dailyList[0].videoList.ForEach(B =>
                                                {
                                                    flipViewCollection.Add(B);
                                                });
                                            }
                                            else
                                            {
                                                localResult.dailyList.ForEach(A =>
                                                {
                                                    if (collection.Count != 0)
                                                    {
                                                        Videolist vl = new Videolist();
                                                        vl.today = A.today;
                                                        collection.Add(vl);
                                                    }
                                                    else
                                                    {
                                                        if (is_Campaign_Available && campaign_Image_Url != null && campaign_Action_Url != null)
                                                        {
                                                            Videolist vlCam = new Videolist();
                                                            //vlCam.is_Campaign_Available = is_Campaign_Available;
                                                            vlCam.coverForDetail = campaign_Image_Url;
                                                            vlCam.webUrl.raw = campaign_Action_Url;
                                                            collection.Add(vlCam);
                                                        }
                                                    }

                                                    A.videoList.ForEach(B =>
                                                    {
                                                        collection.Add(B);
                                                        flipViewCollection.Add(B);
                                                    });
                                                });
                                            }

                                            #region 处理收藏和下载
                                            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                            {
                                                SimpleIoc.Default.Register<CollectionViewModel>(false);
                                            }
                                            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                            if (cvm != null)
                                            {
                                                flipViewCollection.ForEach(A =>
                                                {
                                                    var model = from m in cvm.FavoriteCollection
                                                                where A.id == m.id
                                                                select A;
                                                    if (model.Count() != 0)
                                                    {
                                                        A.isFavorite = true;
                                                    }
                                                });
                                            }

                                            if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                            {
                                                SimpleIoc.Default.Register<DownloadViewModel>(false);
                                            }
                                            var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

                                            if (dlvm != null)
                                            {
                                                flipViewCollection.ForEach(A =>
                                                {
                                                    //先去检查下载队列里面有没有
                                                    var downloadingModels = from m in dlvm.DownloadingList
                                                                            where m.DownloadId == A.id
                                                                            select m;
                                                    //这里说明有正在下载的，那么应该把当前的Model添加到下载队列里面的DownloadModelList
                                                    if (downloadingModels.Count() != 0)
                                                    {
                                                        var downloadModel = downloadingModels.FirstOrDefault() as DownloadingModel;
                                                        if (downloadModel != null)
                                                        {
                                                            downloadModel.DownloadModelList.Add(A);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //然后检查已下载集合里面有没有
                                                        var hasDownloadModels = from m in dlvm.DownloadCollection
                                                                                where A.id == m.id
                                                                                select A;
                                                        if (hasDownloadModels.Count() != 0)
                                                        {
                                                            A.isAleadyDownload = true;
                                                        }
                                                    }
                                                });
                                            }
                                            #endregion

                                            #region 处理磁贴
                                            DicStore.AddOrUpdateValue<object>(AppCommonConst.CURRENT_TILE_COLLECTION, flipViewCollection.Take(5).ToList());
                                            if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_TILE_ACTIVE, true) == true)
                                            {
                                                TileHelper.Instance.UpdateTiles(flipViewCollection.Take(5).ToList());
                                            }
                                            else
                                            {
                                                TileHelper.Instance.CloseTiles();
                                            }
                                            #endregion
                                        }

                                        DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, false);
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错
                                else
                                {
                                    if (!isRefresh)
                                    {
                                        //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                        if (AppEnvironment.IsInternetAccess)
                                        {
                                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                        }
                                        //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                        else
                                        {
                                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!isRefresh)
                                {
                                    //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                    if (AppEnvironment.IsInternetAccess)
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                    }
                                    //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                    else
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //判断是不是分页索引第一页，如果是第一页的话则会弹出提示，重新加载。因为第一页没有加载成功，本条目下就没数据，此时就要弹出一个东西让其重新加载数据。如果不是第一页就不用去管了
                    if (collection.Count == 0)
                    {
                        //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                        if (AppEnvironment.IsInternetAccess && !isRefresh)
                        {
                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                        }
                        //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                        else
                        {
                            //加载本地数据
                            var localJson = await FileHelper.Instance.ReadTextFromFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName);
                            if (localJson != null)
                            {
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<DailyRootModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.dailyList.Count != 0)
                                    {
                                        //不是刷新说明是第一次加载。如果是刷新的话，此处无动作。因为如果数据加载成功的话前面会正确处理，如果数据加载失败了刷新还到这一步的话添加也无用。
                                        if (!isRefresh)
                                        {
                                            if (!AppEnvironment.IsPhone)
                                            {
                                                localResult.dailyList[0].videoList.ForEach(B =>
                                                {
                                                    flipViewCollection.Add(B);
                                                });
                                            }
                                            else
                                            {
                                                localResult.dailyList.ForEach(A =>
                                                {
                                                    if (collection.Count != 0)
                                                    {
                                                        Videolist vl = new Videolist();
                                                        vl.today = A.today;
                                                        collection.Add(vl);
                                                    }
                                                    else
                                                    {
                                                        if (is_Campaign_Available && campaign_Image_Url != null && campaign_Action_Url != null)
                                                        {
                                                            Videolist vlCam = new Videolist();
                                                            //vlCam.is_Campaign_Available = is_Campaign_Available;
                                                            vlCam.coverForDetail = campaign_Image_Url;
                                                            vlCam.webUrl.raw = campaign_Action_Url;
                                                            collection.Add(vlCam);
                                                        }
                                                    }

                                                    A.videoList.ForEach(B =>
                                                    {
                                                        collection.Add(B);
                                                        flipViewCollection.Add(B);
                                                    });
                                                });
                                            }

                                            #region 处理收藏和下载
                                            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                            {
                                                SimpleIoc.Default.Register<CollectionViewModel>(false);
                                            }
                                            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                            if (cvm != null)
                                            {
                                                flipViewCollection.ForEach(A =>
                                                {
                                                    var model = from m in cvm.FavoriteCollection
                                                                where A.id == m.id
                                                                select A;
                                                    if (model.Count() != 0)
                                                    {
                                                        A.isFavorite = true;
                                                    }
                                                });
                                            }

                                            if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                                            {
                                                SimpleIoc.Default.Register<DownloadViewModel>(false);
                                            }
                                            var dlvm = SimpleIoc.Default.GetInstance<DownloadViewModel>();

                                            if (dlvm != null)
                                            {
                                                flipViewCollection.ForEach(A =>
                                                {
                                                    //先去检查下载队列里面有没有
                                                    var downloadingModels = from m in dlvm.DownloadingList
                                                                            where m.DownloadId == A.id
                                                                            select m;
                                                    //这里说明有正在下载的，那么应该把当前的Model添加到下载队列里面的DownloadModelList
                                                    if (downloadingModels.Count() != 0)
                                                    {
                                                        var downloadModel = downloadingModels.FirstOrDefault() as DownloadingModel;
                                                        if (downloadModel != null)
                                                        {
                                                            downloadModel.DownloadModelList.Add(A);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //然后检查已下载集合里面有没有
                                                        var hasDownloadModels = from m in dlvm.DownloadCollection
                                                                                where A.id == m.id
                                                                                select A;
                                                        if (hasDownloadModels.Count() != 0)
                                                        {
                                                            A.isAleadyDownload = true;
                                                        }
                                                    }
                                                });
                                            }
                                            #endregion

                                            #region 处理磁贴
                                            DicStore.AddOrUpdateValue<object>(AppCommonConst.CURRENT_TILE_COLLECTION, flipViewCollection.Take(5).ToList());
                                            if (SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_TILE_ACTIVE, true) == true)
                                            {
                                                TileHelper.Instance.UpdateTiles(flipViewCollection.Take(5).ToList());
                                            }
                                            else
                                            {
                                                TileHelper.Instance.CloseTiles();
                                            }
                                            #endregion
                                        }

                                        DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, false);
                                    }
                                    else
                                    {
                                        if (!isRefresh)
                                        {
                                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                        }
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错，又没有网络
                                else
                                {
                                    if (!isRefresh)
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                    }
                                }
                            }
                            else
                            {
                                if (!isRefresh)
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Daily.DailyViewModel), "GetDaily", new object[] { collection, flipViewCollection, url, cacheFileName, isRefresh });
                                }
                            }
                        }
                    }
                    //如果不是第一页就不用管了
                    else { }
                }

                IsBusy = false;
            });
        }
    }
}
