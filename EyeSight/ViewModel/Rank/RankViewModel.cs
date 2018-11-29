//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel.Rank
//类名称:       RankViewModel
//创建时间:     2015/9/25 星期五 19:27:42
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using EyeSight.Model.Rank;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModelAttribute.Rank;
using GalaSoft.MvvmLight.Threading;
using System;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Download;
using EyeSight.Model.Download;

namespace EyeSight.ViewModel.Rank
{
    public class RankViewModel : RankViewModelAttribute
    {
        public async void GetRank(ObservableCollection<Videolist> performmanceCollection, ObservableCollection<Videolist> realCollection, string url, string cacheFileName = null)
        {
            DispatcherHelper.RunAsync(async () =>
            {
                IsBusy = true;

                var backJson = await WebDataHelper.Instance.GetFromUrlWithAuthReturnString(url, null, 20);

                performmanceCollection.Clear();
                realCollection.Clear();

                if (backJson != null)
                {
                    var result = JsonConvertHelper.Instance.DeserializeObject<RankModel>(backJson);
                    if (result != null)
                    {
                        if (result.videoList.Count != 0)
                        {
                            result.videoList.ForEach(A =>
                            {
                                performmanceCollection.Add(A);
                                realCollection.Add(A);

                            });

                            #region 处理收藏和下载
                            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                            {
                                SimpleIoc.Default.Register<CollectionViewModel>(false);
                            }
                            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                            if (cvm != null)
                            {
                                performmanceCollection.ForEach(A =>
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
                                performmanceCollection.ForEach(A =>
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
                            if (realCollection.Count == 0)
                            {
                                //这里使用反射
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.COLLECTION_ITEM_IS_ZERO, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                            }
                            //如果不是第一页，就不用去管
                            else { }
                        }
                    }
                    else
                    {
                        //判断是不是分页索引第一页，如果是第一页的话则会弹出提示，重新加载。因为第一页没有加载成功，本条目下就没数据，此时就要弹出一个东西让其重新加载数据。如果不是第一页就不用去管了
                        if (realCollection.Count == 0)
                        {
                            //加载本地数据
                            var localJson = await FileHelper.Instance.ReadTextFromFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName);
                            if (localJson != null)
                            {
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<RankModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.videoList.Count != 0)
                                    {
                                        localResult.videoList.ForEach(A =>
                                        {
                                            performmanceCollection.Add(A);
                                            realCollection.Add(A);
                                        });

                                        #region 处理收藏和下载
                                        if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                        {
                                            SimpleIoc.Default.Register<CollectionViewModel>(false);
                                        }
                                        var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                        if (cvm != null)
                                        {
                                            performmanceCollection.ForEach(A =>
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
                                            performmanceCollection.ForEach(A =>
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
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错
                                else
                                {
                                    //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                    if (AppEnvironment.IsInternetAccess)
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                    }
                                    //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                    else
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                    }
                                }
                            }
                            else
                            {
                                //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                if (AppEnvironment.IsInternetAccess)
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                }
                                //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                else
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                }
                            }
                        }
                    }
                }
                else
                {
                    //判断是不是分页索引第一页，如果是第一页的话则会弹出提示，重新加载。因为第一页没有加载成功，本条目下就没数据，此时就要弹出一个东西让其重新加载数据。如果不是第一页就不用去管了
                    if (realCollection.Count == 0)
                    {
                        //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                        if (AppEnvironment.IsInternetAccess)
                        {
                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                        }
                        //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                        else
                        {
                            //加载本地数据
                            var localJson = await FileHelper.Instance.ReadTextFromFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName);
                            if (localJson != null)
                            {
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<RankModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.videoList.Count != 0)
                                    {
                                        localResult.videoList.ForEach(A =>
                                        {
                                            performmanceCollection.Add(A);
                                            realCollection.Add(A);
                                        });

                                        #region 处理收藏和下载
                                        if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                                        {
                                            SimpleIoc.Default.Register<CollectionViewModel>(false);
                                        }
                                        var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                                        if (cvm != null)
                                        {
                                            performmanceCollection.ForEach(A =>
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
                                            performmanceCollection.ForEach(A =>
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
                                    }
                                    else
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错，又没有网络
                                else
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
                                }
                            }
                            else
                            {
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Rank.RankViewModel), "GetRank", new object[] { performmanceCollection, realCollection, url, cacheFileName });
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
