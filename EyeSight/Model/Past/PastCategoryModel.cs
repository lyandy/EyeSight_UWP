//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model.Past
//类名称:       CategoryModel
//创建时间:     2015/9/24 星期四 20:01:26
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Async;
using EyeSight.Base;
using EyeSight.Config;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using static EyeSight.Common.CommonEnum;

namespace EyeSight.Model.Past
{

    public class PastCategoryModel : ModelBase
    {
        /// <summary>
        /// 总的列表信息
        /// </summary>
        private List<CommonItemlist> _itemsList = new List<CommonItemlist>();
        public List<CommonItemlist> itemList
        {
            get
            {
                return _itemsList;
            }
            set
            {
                _itemsList = value;
            }
        }

        /// <summary>
        /// 列表信息一次要展示的个数 -- 暂时无用
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 总共的个数 -- 暂时无用
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 下一页信息 -- 暂时无用
        /// </summary>
        public object nextPageUrl { get; set; }
    }

    public class CommonItemlist
    {
        /// <summary>
        /// 列表条目的类型 分为 horizontalScrollCard（顶部水平滚动展示位）、rectangleCard（长方形展示位 例如360°全景视频）和 squareCard（方形视频类别普通展示位，这里可能会包含 专题、标签等等）
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 根据type 的不同 data会盛放不同的数据内容。
        /// </summary>
        public CommonItemListData data { get; set; }
    }

    public class CommonItemListData : ModelBase
    {
        /// <summary>
        /// 当 type 为 horizontalScrollCard（顶部水平滚动展示位）类型的时候，此处有数据 -- 暂时无用
        /// </summary>
        private List<SlideItemList> _itemList = new List<SlideItemList>();
        public List<SlideItemList> itemList
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

        /// <summary>
        /// 当 type 为 horizontalScrollCard（顶部水平滚动展示位）此处 是 水平滑动的条目个数。当 type 为 topPgc（优质作者展示位）此处是 优质作者要展示的个数 -- 暂时无用
        /// </summary>
        public int count { get; set; }

        /// <summary>
        ///  当 type 为 topPgc 这里显示的是 优质作者 标题 和 当 type 为 squareCard 这里显示的是 专题的标题 是带有 #号 的，当然自己要判断下才可以
        /// </summary>
        private string _title = string.Empty;
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                if (!_title.Trim().StartsWith("#"))
                {
                    convertName = "#" + _title;
                }
                else
                {
                    convertName = _title;
                }
            }
        }

        public string convertName { get; set; }

        /// <summary>
        /// 优质作者的信息 -- 暂时无用
        /// </summary>
        private List<AuthorPgclist> _pgcList = new List<AuthorPgclist>();
        public List<AuthorPgclist> pgcList
        {
            get
            {
                return _pgcList;
            }
            set
            {
                _pgcList = value;
            }
        }

        /// <summary>
        /// 跳转的 url，此处非常重要，要在这里解析的。分为 slide广告、专题、 tag标签、 优质作者和视频分类。其中slide广告、专题有自己的url。tag标签、优质作者、视频分类要分析actionUrl具体内容来决定请求的url
        /// </summary>
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
                if (string.IsNullOrEmpty(_actionUrl) || !_actionUrl.StartsWith("eyepetizer://"))
                {
                    actionId = (int)CategoryErrorType.ERROR;
                }
                else
                {
                    _actionUrl = _actionUrl.Replace("eyepetizer://", "");
                    string[] arr = _actionUrl.Split('/');
                    if (arr.Count() >= 2)
                    {
                        string vTypeStr = arr[0].ToLower().Trim();

                        int tempActionId = (int)CategoryErrorType.ERROR;
                        int.TryParse(arr[1], out tempActionId);
                        actionId = tempActionId;

                        if (vTypeStr.Contains(CategorySubType.CAMPAIGN.ToString().ToLower()))
                        {
                            categorySubType = CategorySubType.CAMPAIGN;
                        }
                        else if (vTypeStr.Contains(CategorySubType.TAG.ToString().ToLower()))
                        {
                            categorySubType = CategorySubType.TAG;
                        }
                        else if (vTypeStr.Contains(CategorySubType.CATEGORY.ToString().ToLower()))
                        {
                            categorySubType = CategorySubType.CATEGORY;
                        }
                        else
                        {
                            actionId = (int)CategoryErrorType.UNSUPPORT;
                        }
                    }
                    else
                    {
                        actionId = (int)CategoryErrorType.ERROR;
                    }

                    //对分类标题的单独处理
                    if (arr.Count() >= 3)
                    {
                        var titleStrOriginal = arr[2].ToString();
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
        /// 真正跳转时要用到的Id. 如果actionId == -1 的话说明解析出错，应当停止页面的跳转。如果actionId == -2的话，说明当前此类型暂未得到支持，也应当停止页面的跳转
        /// </summary>
        public int actionId { get; set; }

        /// <summary>
        /// 当前的视频类型：专题、tag标签视频、视频小分类
        /// </summary>
        public CategorySubType categorySubType { get; set; }

        //这里的数据类型 和 外侧的 type 类型是一样的，后期估计要换成这个
        public string dataType { get; set; }
        /// <summary>
        /// id，其实这里没用 -- 暂时无用
        /// </summary>
        public int id { get; set; }


        #region 视频类别、专题和tag标签的图片展示 处理
        private string _image = string.Empty;
        public string image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                if (!string.IsNullOrEmpty(_image))
                {
                    GetConvertImage();
                }
            }
        }

        private AsyncProperty<object> _convertImage;
        public AsyncProperty<object> convertImage
        {
            get
            {
                //解决集合数据不是通过反序列化生成导致图片资源无法显示的Bug，比如通过ForEach循环Add的数据集合。并且一定要判空，不然会死循环。
                if (_convertImage == null)
                {
                    if (!string.IsNullOrEmpty(image))
                    {
                        GetConvertImage();
                    }
                }

                return _convertImage;
            }
            set
            {
                if (_convertImage != value)
                {
                    _convertImage = value;

                    OnPropertyChanged();
                }
            }
        }

        public void GetConvertImage()
        {
            convertImage = new AsyncProperty<object>(async () =>
            {
                try
                {
                    var imageName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(image);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(imageName);
                    if (file != null)
                    {
                        return folder.Path + "\\" + imageName;
                    }
                    else
                    {
                        WebDownFileHelper.Instance.SaveAsyncWithHttp(image, imageName, folder);
                        return image;
                    }
                }
                catch
                {
                    return image;
                }
            }, null);
        }
        #endregion
    }

    /// <summary>
    ///  -- 暂时无用
    /// </summary>
    public class SlideItemList
    {
        public string type { get; set; }
        public SlideItemData data { get; set; }
    }

    /// <summary>
    /// -- 暂时无用
    /// </summary>
    public class SlideItemData
    {
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string actionUrl { get; set; }
        public object adTrack { get; set; }
    }

    /// <summary>
    /// 暂时无用
    /// </summary>
    public class AuthorPgclist
    {
        public int id { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string actionUrl { get; set; }
    }
}
