//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModel.Past
//类名称:       PastDetailViewModel
//创建时间:     2015/9/25 星期五 13:59:46
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using EyeSight.Model.Past;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModelAttribute.Past;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Download;
using EyeSight.Model.Download;

namespace EyeSight.ViewModel.Past
{
    public class PastDetailViewModel : PastDetailViewModelAttribute
    {
        public async void GetPastCategoryDetail(ObservableCollection<VideoDetailData> collection, string url)
        {
            DispatcherHelper.RunAsync(async () =>
            {
                IsBusy = true;

                var backJson = await WebDataHelper.Instance.GetFromUrlWithAuthReturnString(url, null, 20);
                if (backJson != null)
                {
                    var result = JsonConvertHelper.Instance.DeserializeObject<PastCategoryDetailModel>(backJson);
                    if (result != null)
                    {
                        if (result.itemList.Count != 0)
                        {
                            result.itemList.ForEach(A =>
                            {
                                collection.Add(A.data);
                            });

                            #region 处理收藏和下载
                            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                            {
                                SimpleIoc.Default.Register<CollectionViewModel>(false);
                            }
                            var cvm = SimpleIoc.Default.GetInstance<CollectionViewModel>();

                            if (cvm != null)
                            {
                                collection.ForEach(A =>
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
                                collection.ForEach(A =>
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
                            //判断是不是分页索引第一页，如果是第一页并且获取到的条目个数为0个，则此时要求再次获取。此时提示“没有获取到数据，请重试”
                            if (collection.Count == 0)
                            {
                                //这里使用反射
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.COLLECTION_ITEM_IS_ZERO, typeof(EyeSight.ViewModel.Past.PastDetailViewModel), "GetPastCategoryDetail", new object[] { collection, url });
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
                            //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                            if (AppEnvironment.IsInternetAccess)
                            {
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Past.PastDetailViewModel), "GetPastCategoryDetail", new object[] { collection, url });
                            }
                            //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                            else
                            {
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Past.PastDetailViewModel), "GetPastCategoryDetail", new object[] { collection, url });
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
                        if (AppEnvironment.IsInternetAccess)
                        {
                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Past.PastDetailViewModel), "GetPastCategoryDetail", new object[] { collection, url });
                        }
                        //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                        else
                        {
                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Past.PastDetailViewModel), "GetPastCategoryDetail", new object[] { collection, url });
                        }
                    }
                    //如果不是第一页就不用管了
                    else { }
                }

                IsBusy = false;
            });
        }

        #region 清理ViewModel
        public override void Cleanup()
        {
            base.Cleanup();

            CategoryDetailCollection.ForEach(o =>
            {
                o.Dispose();
            });

            CategoryDetailCollection.Clear();

        }
        #endregion
    }
}
