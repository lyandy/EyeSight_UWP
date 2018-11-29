//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Model.Gift

//类名称:       GiftModel

//创建时间:     2016/7/6 19:02:04

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Model.Gift
{
    public class GiftModel
    {
        public string BgColor { get; set; }

        public string AppImage { get; set; }

        public string AppName { get; set; }

        private string _AppNameForground = "#FFFFFF";
        public string AppNameForground
        {
            get
            {
                return _AppNameForground;
            }
            set
            {
                _AppNameForground = value;
            }
        }

        public string AppDesc { get; set; }

        public Uri AppProductUri { get; set; }
    }
}
