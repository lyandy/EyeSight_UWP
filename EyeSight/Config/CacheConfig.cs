//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Config
//类名称:       CacheConfig
//创建时间:     2015/9/21 星期一 15:23:10
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Config
{
    public class CacheConfig : ClassBase<CacheConfig>
    {
        public string ListFileCacheRelativePath
        {
            get
            {
                return "Cache\\Txt";
            }
        }

        public string ImageFileCacheRelativePath
        {
            get
            {
                return "Cache\\Image";
            }
        }

        public string VideoFileDownloadRelativePath
        {
            get
            {
                return "Cache\\Video";
            }
        }

        public string PhoneSplashScreenImageCacheRelativePath
        {
            get
            {
                return "Cache\\SplashScreenImage\\Phone";
            }
        }

        public string PCSplashScreenImageCacheRelativePath
        {
            get
            {
                return "Cache\\SplashScreenImage\\PC";
            }
        }

        public string EyeSightFavoriteDatabaseCacheRelativePath
        {
            get
            {
                return "Cache\\Database\\";
            }
        }
    }
}
