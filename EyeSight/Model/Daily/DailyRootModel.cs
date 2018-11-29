//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model.Daily
//类名称:       DailyModel
//创建时间:     2015/9/21 星期一 17:58:33
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Async;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EyeSight.Model.Daily
{

    public class DailyRootModel
    {
        /// <summary>
        /// 获取到的视频列表
        /// </summary>
        private List<Dailylist> _dailyList = new List<Dailylist>();
        public List<Dailylist> dailyList
        {
            get
            {
                return _dailyList;
            }
            set
            {
                _dailyList = value;
            }
        }

        /// <summary>
        /// 下一页的链接
        /// </summary>
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
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.DAILY_HAS_NEXT_PAGE, false);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.DAILY_NEXT_PAGE_URL, null);
                }
                else
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.DAILY_HAS_NEXT_PAGE, true);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.DAILY_NEXT_PAGE_URL, _nextPageUrl);
                }
            }
        }
    }

    public class Dailylist
    {
        private string _today = DateTime.Now.ToString();
        public string today
        {
            get
            {
                try
                {
                    var dt = date.ToDateTime();
                    var arr = dt.ToString("r", DateTimeFormatInfo.InvariantInfo).Split(' ');
                    if (arr.Count() >= 3)
                    {
                        _today = "- " + arr[2] + ". " + arr[1] + " -";
                    }
                }
                catch
                {
                    var now = DateTime.Now;
                    var arrCatch = now.ToString("r", DateTimeFormatInfo.InvariantInfo).Split(' ');
                    if (arrCatch.Count() >= 3)
                    {
                        _today = "- " + arrCatch[2] + ". " + arrCatch[1] + " -";
                    }
                }

                return _today;
            }
            set
            {
                _today = value;
            }
        }

        /// <summary>
        /// 视频日期列表
        /// </summary>
        public long date { get; set; }

        private List<Videolist> _videoList = new List<Videolist>();
        public List<Videolist> videoList
        {
            get
            {
                return _videoList;
            }
            set
            {
                _videoList = value;
            }
        }
    }

    public class Videolist : ModelPropertyBase
    {
        //public string campaign_ImageUrl { get; set; }

        //public bool is_Campaign_Available { get; set; }

        //public string campaign_Version { get; set; }

        //public string campaign_ActionUrl { get; set; }
    }
}
