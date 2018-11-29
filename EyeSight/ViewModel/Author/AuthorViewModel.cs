//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModel.Author

//类名称:       AuthorViewModel

//创建时间:     2016/7/11 16:19:04

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.Model.Author;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.ViewModelAttribute.Author;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EyeSight.Common.CommonEnum;

namespace EyeSight.ViewModel.Author
{
    public class AuthorViewModel : AuthorViewModelAttribute
    {
        public async void GetAuthorCollection(ObservableCollection<AuthorData> collection, string url, string cacheFileName = null)
        {
            DispatcherHelper.RunAsync(async () =>
            {
                IsBusy = true;

                var backJson = await WebDataHelper.Instance.GetFromUrlWithAuthReturnString(url, null, 20);
                if (backJson != null)
                {
                    var result = JsonConvertHelper.Instance.DeserializeObject<AuthorListModel>(backJson);
                    if (result != null)
                    {
                        if (result.itemList.Count != 0)
                        {
                            result.itemList.ForEach(A =>
                            {
                                if (A.data.dataType.ToLower().Trim().Contains(AuthorDataType.BRIEFCARD.ToString().ToLower()) || A.data.dataType.ToLower().Trim().Contains(AuthorDataType.VIDEOCOLLECTIONWITHBRIE.ToString().ToLower()))
                                {
                                    bool isAlreadyExist = false;
                                    foreach (var authorData in collection)
                                    {
                                        if (authorData.id == A.data.id)
                                        {
                                            isAlreadyExist = true;
                                            break;
                                        }
                                    }

                                    if (!isAlreadyExist)
                                    {
                                        collection.Add(A.data);
                                    }
                                }
                            });

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
                            if (collection.Count == 0)
                            {
                                //这里使用反射
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.COLLECTION_ITEM_IS_ZERO, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
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
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<AuthorListModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.itemList.Count != 0)
                                    {
                                        localResult.itemList.ForEach(A =>
                                        {
                                            if (A.data.dataType.ToLower().Trim().Contains(AuthorDataType.BRIEFCARD.ToString().ToLower()) || A.data.dataType.ToLower().Trim().Contains(AuthorDataType.VIDEOCOLLECTIONWITHBRIE.ToString().ToLower()))
                                            {
                                                bool isAlreadyExist = false;
                                                foreach (var authorData in collection)
                                                {
                                                    if (authorData.id == A.data.id)
                                                    {
                                                        isAlreadyExist = true;
                                                        break;
                                                    }
                                                }

                                                if (!isAlreadyExist)
                                                {
                                                    collection.Add(A.data);
                                                }
                                            }
                                        });
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错
                                else
                                {
                                    //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                    if (AppEnvironment.IsInternetAccess)
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                                    }
                                    //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                    else
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                                    }
                                }
                            }
                            else
                            {
                                //如果此时还有网络，说明加载过程出错，提示信息为“加载数据出错，请重试。”
                                if (AppEnvironment.IsInternetAccess)
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                                }
                                //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                                else
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
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
                        if (AppEnvironment.IsInternetAccess)
                        {
                            RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWOTK_IS_ERROR, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                        }
                        //如果没有网络，说明数据加载失败是因为没有网络造成的。提示信息为“没有网络，请确认网络连接。”
                        else
                        {
                            //加载本地数据
                            var localJson = await FileHelper.Instance.ReadTextFromFile(CacheConfig.Instance.ListFileCacheRelativePath, cacheFileName);
                            if (localJson != null)
                            {
                                var localResult = JsonConvertHelper.Instance.DeserializeObject<AuthorListModel>(localJson);
                                if (localResult != null)
                                {
                                    //因为如果第一页获取到的数据为0条时不会写到本地的，所以，从本地获取到的数据条目数一定不为0
                                    if (localResult.itemList.Count != 0)
                                    {
                                        localResult.itemList.ForEach(A =>
                                        {
                                            if (A.data.dataType.ToLower().Trim().Contains(AuthorDataType.BRIEFCARD.ToString().ToLower()) || A.data.dataType.ToLower().Trim().Contains(AuthorDataType.VIDEOCOLLECTIONWITHBRIE.ToString().ToLower()))
                                            {
                                                bool isAlreadyExist = false;
                                                foreach (var authorData in collection)
                                                {
                                                    if (authorData.id == A.data.id)
                                                    {
                                                        isAlreadyExist = true;
                                                        break;
                                                    }
                                                }

                                                if (!isAlreadyExist)
                                                {
                                                    collection.Add(A.data);
                                                }
                                            }
                                        });
                                    }
                                    else
                                    {
                                        RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                                    }
                                }
                                //虽然错误的数据是不会写到本地的，但如果反序列化失败一样会出错，又没有网络
                                else
                                {
                                    RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                                }
                            }
                            else
                            {
                                RetryBox.Instance.ShowRetry(AppNetworkMessageConst.NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL, typeof(EyeSight.ViewModel.Author.AuthorViewModel), "GetAuthorCollection", new object[] { collection, url, cacheFileName });
                            }
                        }
                    }
                    //如果不是第一页就不用管了
                    else { }
                }

                //添加视频小分类集合 去除 专题和title为空的不知道名字(比如360全景居然没有title，type还换成了rectangleCard，暂时不屏蔽)的放心Squard 类型 的处理
                //collection.Where(A => (A.dataType.ToLower().Trim().Contains(AuthorDataType.BRIEFCARD.ToString().ToLower()))).ToList().ForEach(A =>
                //{
                //    AuthorSubCollection.Add(A);
                //});

                IsBusy = false;
            });
        }
    }
}
