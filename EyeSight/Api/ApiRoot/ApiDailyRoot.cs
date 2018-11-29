//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Api.ApiRoot
//类名称:       ApiDailyRoot
//创建时间:     2015/9/21 星期一 17:47:19
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Api.ApiRoot
{
    public class ApiDailyRoot : ClassBase<ApiDailyRoot>
    {
        public ApiDailyRoot() : base() { }

        /// <summary>
        /// 每日精选网络请求链接
        /// </summary>
        public string DailyUrl
        {
            get
            {
                var nextPageUrl = DicStore.GetValueOrDefault<string>(AppCommonConst.DAILY_NEXT_PAGE_URL, null);
                if (!string.IsNullOrEmpty(nextPageUrl))
                {
                    return nextPageUrl;
                }
                else
                {
                    return "http://baobab.wandoujia.com/api/v1/feed?num=10&date=" + DateTime.Now.ToChinaStandardTime().ToString("yyyyMMdd"); 
                }
            }
        }
    }
}
