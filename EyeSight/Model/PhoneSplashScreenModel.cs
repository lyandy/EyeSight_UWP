//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model
//类名称:       PhoneSplashScreenModel
//创建时间:     2015/9/26 星期六 17:23:38
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Model
{
    public class PhoneSplashScreenModel
    {
        public Startpage startPage { get; set; }

        public campaignInFeed campaignInFeed { get; set; }
    }

    public class Startpage
    {
        public string imageUrl { get; set; }
    }

    public class campaignInFeed
    {
        public string imageUrl { get; set; }

        public bool available { get; set; }

        public string version { get; set; }

        public string actionUrl { get; set; }
    }
}
