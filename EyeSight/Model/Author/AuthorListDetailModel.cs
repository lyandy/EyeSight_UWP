//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Model.Author

//类名称:       AuthorListDetailModel

//创建时间:     2016/7/11 16:25:17

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
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.Model.Author
{

    public class AuthorListDetailModel
    {
        private List<VideoList> _itemList = new List<VideoList>();
        public List<VideoList> itemList
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
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.AUTHOR_DETAIL_HAS_NEXT_PAGE, false);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.AUTHOR_DETAIL_NEXT_PAGE_URL, null);
                }
                else
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.AUTHOR_DETAIL_HAS_NEXT_PAGE, true);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.AUTHOR_DETAIL_NEXT_PAGE_URL, _nextPageUrl);
                }
            }
        }
        public Pgcinfo pgcInfo { get; set; }
    }

    public class Pgcinfo : ModelBase
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
                try
                {
                    var iconName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(icon);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(iconName);
                    if (file != null)
                    {
                        return folder.Path + "\\" + iconName;
                    }
                    else
                    {

                        WebDownFileHelper.Instance.SaveAsyncWithHttp(icon, iconName, folder);

                        return icon;
                    }
                }
                catch
                {
                    return icon;
                }
            }, null);
        }
        #endregion
        public string title { get; set; }
        public string name { get; set; }
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
                _actionUrl = value.ToLower().Trim();

                _actionUrl = _actionUrl.Replace("eyepetizer://", "");
                string[] arr = _actionUrl.Split('/');

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

        public void Dispose()
        {
            convertIcon = null;
        }
    }

    public class VideoList
    {
        public string type { get; set; }
        public VideoData data { get; set; }
    }

    public class VideoData : ModelPropertyBase
    {
 
    }
}
