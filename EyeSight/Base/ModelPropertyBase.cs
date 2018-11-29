//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Base
//类名称:       ModelPropertyBase
//创建时间:     2015/9/21 星期一 15:15:36
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Async;
using EyeSight.Config;
using EyeSight.Encrypt;
using EyeSight.Helper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static EyeSight.Common.CommonEnum;
using EyeSight.Extension.CommonObjectEx;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using Windows.UI.Xaml;

namespace EyeSight.Base
{
    [Table("VideoInfo")]
    public class ModelPropertyBase : ModelBase
    {
        [PrimaryKey, NotNull]
        public int id { get; set; }

        private bool _isAleadyDownload = false;
        [Ignore]
        public bool isAleadyDownload
        {
            get
            {
                return _isAleadyDownload;
            }
            set
            {
                if (_isAleadyDownload != value)
                {
                    _isAleadyDownload = value;

                    if (_isAleadyDownload)
                    {
                        downloadProgress = "已下载";
                    }
                    else
                    {
                        downloadProgress = "下载";

                        downloadPerformanceListProgress = "";
                    }

                    OnPropertyChanged();
                }
            }
        }

        private string _downloadProgress = "下载";
        [Ignore]
        public string downloadProgress
        {
            get
            {
                return _downloadProgress;
            }
            set
            {
                if (_downloadProgress != value)
                {
                    _downloadProgress = value;

                    downloadPerformanceListProgress = " /  " + _downloadProgress;

                    OnPropertyChanged();
                }
            }
        }

        private string _downloadPerformanceListProgress = "";
        [Ignore]
        public string downloadPerformanceListProgress
        {
            get
            {
                return _downloadPerformanceListProgress;
            }
            set
            {
                if (_downloadPerformanceListProgress != value)
                {
                    _downloadPerformanceListProgress = value;

                    OnPropertyChanged();
                }
            }
        }

        private bool _isFavorite = false;
        [Ignore]
        public bool isFavorite
        {
            get
            {
                return _isFavorite;
            }
            set
            {
                if (_isFavorite != value)
                {
                    _isFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isEditing = false;
        [Ignore]
        public bool isEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                if (_isEditing != value)
                {
                    if (!AppEnvironment.IsPhone)
                    {
                        _isEditing = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public string date { get; set; }

        //今天的时期
        public string today { get; set; }

        //视频标题
        public string title { get; set; }
        //视频描述
        public string description { get; set; }

        //视频分类
        public string _category = "";
        public string category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                if (!_category.Trim().StartsWith("#"))
                {
                    convertCategory = "#" + _category;
                }
                else
                {
                    convertCategory = _category;
                }
            }
        }

        [Ignore]
        public string convertCategory { get; set; }

        [Ignore]
        public Provider provider { get; set; }

        private string _videoDuration = "";
        [Ignore]
        public string videoDurtion
        {
            get
            {
                if (duration > 0)
                {
                    if (duration != null)
                    {
                        var minute = ((int)duration / 60).ToString("00");
                        var second = ((int)duration % 60).ToString("00");

                        _videoDuration = "  /  " + minute + "'" + second + "''";
                    }
                    else
                    {
                        _videoDuration = "  /  00'00''";
                    }
                }

                return _videoDuration;
            }
            set
            {
                _videoDuration = value;
            }
        }

        private string _videoDurationForCommon = "";
        [Ignore]
        public string videoDurationForCommon
        {
            get
            {
                if (duration > 0)
                {
                    if (duration != null)
                    {
                        var minute = ((int)duration / 60).ToString("00");
                        var second = ((int)duration % 60).ToString("00");

                        _videoDurationForCommon = minute + "'" + second + "''";
                    }
                    else
                    {
                        _videoDurationForCommon = "00'00''";
                    }
                }

                return _videoDurationForCommon;
            }
            set
            {
                _videoDurationForCommon = value;
            }
        }

        //时长
        public int? duration { get; set; }
        public string coverForFeed { get; set; }

        #region 视频大图的处理
        private string _coverForDetail = string.Empty;
        public string coverForDetail
        {
            get
            {
                return _coverForDetail;
            }
            set
            {
                _coverForDetail = value;

                if (!string.IsNullOrEmpty(_coverForDetail))
                {
                    GetConvertCoverForDetailImage();
                }
            }
        }

        private AsyncProperty<object> _convertCoverForDetail;
        [Ignore]
        public AsyncProperty<object> convertCoverForDetail
        {
            get
            {
                //解决集合数据不是通过反序列化生成导致图片资源无法显示的Bug，比如通过ForEach循环Add的数据集合。并且一定要判空，不然会死循环。
                if (_convertCoverForDetail == null)
                {
                    if (string.IsNullOrEmpty(_coverForDetail) && cover != null)
                    {
                        _coverForDetail = cover.detail;
                    }

                    if (!string.IsNullOrEmpty(_coverForDetail))
                    {
                        GetConvertCoverForDetailImage();
                    }
                }

                return _convertCoverForDetail;
            }
            set
            {
                if (_convertCoverForDetail != value)
                {
                    _convertCoverForDetail = value;

                    OnPropertyChanged();
                }
            }
        }

        public void GetConvertCoverForDetailImage()
        {
            convertCoverForDetail = new AsyncProperty<object>(async () =>
            {
                try
                {
                    var imageName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(coverForDetail);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(imageName);
                    if (file != null)
                    {
                        return folder.Path + "\\" + imageName;
                    }
                    else
                    {
                        WebDownFileHelper.Instance.SaveAsyncWithHttp(coverForDetail, imageName, folder);
                        return coverForDetail;
                    }
                }
                catch
                {
                    return coverForDetail;
                }
            }, null);
        }
        #endregion

        #region 视频磨砂图处理

        private string _coverBlurred = string.Empty;
        public string coverBlurred
        {
            get
            {
                return _coverBlurred;
            }
            set
            {
                _coverBlurred = value;

                if (!string.IsNullOrEmpty(_coverBlurred))
                {
                    GetConvertCoverBlurredImage();
                }
            }
        }

        private AsyncProperty<object> _convertCoverBlurred;
        [Ignore]
        public AsyncProperty<object> convertCoverBlurred
        {
            get
            {
                //解决集合数据不是通过反序列化生成导致图片资源无法显示的Bug，比如通过ForEach循环Add的数据集合。并且一定要判空，不然会死循环。
                if (_convertCoverBlurred == null)
                {
                    if (string.IsNullOrEmpty(_coverBlurred) && cover != null)
                    {
                        _coverBlurred = cover.blurred;
                    }

                    if (!string.IsNullOrEmpty(_coverBlurred))
                    {
                        GetConvertCoverBlurredImage();
                    }
                }

                return _convertCoverBlurred;
            }
            set
            {
                if (_convertCoverBlurred != value)
                {
                    _convertCoverBlurred = value;

                    OnPropertyChanged();
                }
            }
        }

        public void GetConvertCoverBlurredImage()
        {
            convertCoverBlurred = new AsyncProperty<object>(async () =>
            {
                try
                {
                    var imageName = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(coverBlurred);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.ImageFileCacheRelativePath, CreationCollisionOption.OpenIfExists);
                    var file = await folder.TryGetItemAsync(imageName);
                    if (file != null)
                    {
                        return folder.Path + "\\" + imageName;
                    }
                    else
                    {
                        WebDownFileHelper.Instance.SaveAsyncWithHttp(coverBlurred, imageName, folder);
                        return coverBlurred;
                    }
                }
                catch
                {
                    return coverBlurred;
                }
            }, null);
        }

        [Ignore]
        public Cover cover { get; set; }

        #endregion
        public string coverForSharing { get; set; }
        public string playUrl { get; set; }

        private List<Playinfo> _playInfo = new List<Playinfo>();
        [Ignore]
        public List<Playinfo> playInfo
        {
            get
            {
                return _playInfo;
            }
            set
            {
                _playInfo = value;
            }
        }

        [Ignore]
        public WebUrl webUrl { get; set; }

        private string _rawWebUrl = string.Empty;
        public string rawWebUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_rawWebUrl) && webUrl != null)
                {
                    _rawWebUrl = webUrl.raw;
                }
                return _rawWebUrl;
            }
            set
            {
                _rawWebUrl = value;
            }
        }
        [Ignore]
        public Consumption consumption { get; set; }

        private List<VideoTag> _tags = new List<VideoTag>();
        [Ignore]
        public List<VideoTag> tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;

                if (AppEnvironment.IsPhone)
                {
                    if (_tags != null)
                    {
                        //if (_tags.Count >= 5)
                        //{
                            _tags.Take(4).ForEach(A =>
                            {
                                _convertTags.Add(A);
                            });
                            //_convertTags.Add(new VideoTag { name = ". . .", actionUrl = null, tempTag = _tags });
                        //}
                        //else
                        //{
                        //    _convertTags = _tags;
                        //}
                    }
                }
                else
                {
                    _convertTags = _tags;
                }
            }
        }

        private List<VideoTag> _convertTags = new List<VideoTag>();
        [Ignore]
        public List<VideoTag> convertTags
        {
            get
            {
                return _convertTags;
            }
            set
            {
                _convertTags = value;
            } 
        }

        //因为牵扯到数据库保存最原始的数据，所以这里不可以对原始字段做处理。如果要使用数据库，字段必须得使用get;set;
        //对于已经存在的数据库，不应该再次增加数据库字段，会插入失败的。除非每次都去CreateTablesAsync
        public string type { get; set; }

        [Ignore]
        public string convertVideoType
        {
            get
            {
                string temp = string.Empty;

                if (type != null && type.ToLower().Contains(VideoType.PANORAMIC.ToString().ToLower()))
                {
                    temp = "360° 全景";
                }
                else
                {
                    temp = string.Empty;
                }
                return temp;
            }
        }

        public void Dispose()
        {
            convertCoverBlurred = null;
            convertCoverForDetail = null;
        }
    }

    public class VideoTag
    {
        private int _parentId = 0;
        public int parentId
        {
            get
            {
                return _parentId;
            }
            set
            {
                _parentId = value;

                uniqueId = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(id + parentId + "");
            }
        }

        public int id { get; set; }

        [PrimaryKey, NotNull]
        public string uniqueId { get; set; }


        private string _name = string.Empty;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (!_name.StartsWith("#"))
                {
                    convertName = "#" + _name;
                }
                else
                {
                    convertName = _name;
                }
                
            }
        }

        [Ignore]
        public string convertName { get; set; }

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
                }
            }
        }

        [Ignore]
        public CategorySubType categorySubType { get; set; }

        [Ignore]
        public int actionId { get; set; }

        [Ignore]
        public object tempTag { get; set; }
    }


    public class Cover
    {
        [PrimaryKey, NotNull]
        public int parentId { get; set; }

        public string feed { get; set; }
        public string detail { get; set; }
        public string blurred { get; set; }
        public string sharing { get; set; }
    }

    public class WebUrl
    {
        [PrimaryKey, NotNull]
        public int parentId { get; set; }

        public string raw { get; set; }
        public string forWeibo { get; set; }
    }

    public class Provider
    {
        [PrimaryKey, NotNull]
        public int parentId { get; set; }

        public string name { get; set; }
        public string alias { get; set; }
        public string icon { get; set; }
    }

    public class Consumption
    {
        [PrimaryKey, NotNull]
        public int parentId { get; set; }

        private int? _colectionCount = 0;
        public int? collectionCount
        {
            get
            {
                return _colectionCount;
            }
            set
            {
                _colectionCount = value;
            }
        }
        public int? shareCount { get; set; }
        public int? playCount { get; set; }
        public int? replyCount { get; set; }
    }

    public class Playinfo
    { 
        [PrimaryKey, NotNull]
        public string uniqueId { get; set; }
        
        public int parentId { get; set; }

        public string name { get; set; }
        public string type { get; set; }

        private string _url = string.Empty;
        public string url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
                uniqueId = EyeSight.Encrypt.MD5Core.Instance.ComputeMD5(_url);
            }
        }
    }
}
