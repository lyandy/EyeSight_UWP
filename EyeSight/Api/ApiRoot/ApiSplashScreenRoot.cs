//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Api.ApiRoot
//类名称:       ApiSplashScreenRoot
//创建时间:     2015/9/26 星期六 17:11:19
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Api.ApiRoot
{
    class ApiSplashScreenRoot : ClassBase<ApiSplashScreenRoot>
    {
        public ApiSplashScreenRoot() : base() { }

        /// <summary>
        /// 手机端自定义SplashScreenImage请求链接
        /// </summary>
        public string SplashScreenUrl
        {
            get
            {
                return "http://baobab.wandoujia.com/api/v1/configs?test=1";
            }
        }
    }
}
