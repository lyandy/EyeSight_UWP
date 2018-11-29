//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Model.Author

//类名称:       AuthorListModel

//创建时间:     2016/7/11 16:24:59

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Async;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Encrypt;
using EyeSight.Helper;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using static EyeSight.Common.CommonEnum;

namespace EyeSight.Model.Author
{
    public class AuthorListModel
    {
        private List<AuthorsList> _itemList = new List<AuthorsList>();
        public List<AuthorsList> itemList
        {
            get
            {
                return _itemList;
            }
            set
            {
                _itemList = value;
            }
        }
        public int count { get; set; }
        public int total { get; set; }

        private string _nextPageUrl = null;
        public string nextPageUrl
        {
            get
            {
                return _nextPageUrl;
            }
            set
            {
                _nextPageUrl = value;
                if (string.IsNullOrEmpty(_nextPageUrl))
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.AUTHOR_HAS_NEXT_PAGE, false);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.AUTHOR_NEXT_PAGE_URL, null);
                }
                else
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.AUTHOR_HAS_NEXT_PAGE, true);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.AUTHOR_NEXT_PAGE_URL, _nextPageUrl);
                }
            }
        }
    }
    public class AuthorsList
    {
        public string type { get; set; }
        public AuthorData data { get; set; }
    }

    public class AuthorData : ModelBase
    {
        public string dataType { get; set; }

        public int id { get; set; }

        #region 作者头像图片的处理
        private string _icon = string.Empty;
        public string icon
        {
            get
            {
                return _icon;

            }
            set
            {
                _icon = value;
                if (!string.IsNullOrEmpty(_icon))
                {
                    GetConvertIcon();
                }
            }
        }

        private AsyncProperty<object> _convertIcon;
        public AsyncProperty<object> convertIcon
        {
            get
            {
                //解决集合数据不是通过反序列化生成导致图片资源无法显示的Bug，比如通过ForEach循环Add的数据集合。并且一定要判空，不然会死循环。
                if (_convertIcon == null)
                {
                    if (!string.IsNullOrEmpty(icon))
                    {
                        GetConvertIcon();
                    }
                }

                return _convertIcon;
            }
            set
            {
                if (_convertIcon != value)
                {
                    _convertIcon = value;

                    OnPropertyChanged();
                }
            }
        }

        public void GetConvertIcon()
        {
            convertIcon = new AsyncProperty<object>(async () =>
            {
                BitmapImage img = null;

                try
                {
                    var iconName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(icon);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(iconName);
                    if (file != null)
                    {
                        await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
                        {
                            try
                            {
                                var resultFile = await folder.GetFileAsync(iconName);

                                img = new BitmapImage();
                                //降低解析度降低内存占用
                                img.DecodePixelHeight = 100;
                                using (var stream = await resultFile.OpenAsync(FileAccessMode.Read))
                                    await img.SetSourceAsync(stream);
                            }
                            catch (Exception ex)
                            {
                                img = null;
                                convertIcon = null;
                            }
                        });
                    }
                    else
                    {
                        await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
                        {
                            try
                            {
                                img = new BitmapImage();
                                //降低解析度降低内存占用
                                img.DecodePixelHeight = 100;
                                if (Uri.IsWellFormedUriString(icon, UriKind.Absolute))
                                {
                                    img.UriSource = new Uri(icon);
                                }
                            }
                            catch(Exception ex)
                            {
                                img = null;
                                convertIcon = null;
                            }
                        });

                        WebDownFileHelper.Instance.SaveAsyncWithHttp(icon, iconName, folder);
                    }

                    SpinWait.SpinUntil(() =>
                    {
                        if (img == null) return false;
                        else return true;
                    }, 200);

                    return img;
                }
                catch
                {
                    return icon;
                }
            }, null);
        }
        #endregion

        public string title { get; set; }
        public string subTitle { get; set; }
        public string description { get; set; }

        private string _actionUrl = string.Empty;
        public string actionUrl
        {
            get
            {
                return _actionUrl;
            }
            set
            {
                if (value != null)
                {
                    _actionUrl = value.ToLower().Trim();
                }

                if (string.IsNullOrEmpty(_actionUrl) || !_actionUrl.StartsWith("eyepetizer://"))
                {
                    actionId = (int)CategoryErrorType.ERROR;
                }
                else
                {
                    _actionUrl = _actionUrl.Replace("eyepetizer://", "");
                    string[] arr = _actionUrl.Split('/');
                    if (arr.Count() >= 3)
                    {
                        int tempActionId = (int)CategoryErrorType.ERROR;
                        int.TryParse(arr[2], out tempActionId);
                        actionId = tempActionId;
                    }
                    else
                    {
                        actionId = (int)CategoryErrorType.ERROR;
                    }

                    //对分类标题的单独处理
                    if (arr.Count() >= 4)
                    {
                        var titleStrOriginal = arr[3].ToString();
                        var titleStrArr = titleStrOriginal.Split('=');
                        if (titleStrArr.Count() >= 2)
                        {
                            title = WebUtility.UrlDecode(titleStrArr[1]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 真正跳转时要用到的Id
        /// </summary>
        public int actionId { get; set; }

        private Header _header = null;
        public Header header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                if (_header != null)
                {
                    id = _header.id;
                    icon = _header.icon;
                    title = _header.title;
                    subTitle = _header.subTitle;
                    description = _header.description;
                    actionUrl = _header.actionUrl;
                }
            }
        }
    }

    public class Header
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string description { get; set; }
        public string actionUrl { get; set; }
        public object adTrack { get; set; }
    }

}
