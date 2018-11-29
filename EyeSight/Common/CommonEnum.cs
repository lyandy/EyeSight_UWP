//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Common

//类名称:       CommonEnum

//创建时间:     2016/6/17 15:04:30

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Common
{
    /// <summary>
    /// 视频类型 枚举
    /// </summary>
    public class CommonEnum
    {
        public enum VideoType
        {
            NORMAL,
            PANORAMIC
        }

        /// <summary>
        /// 视频分类类型 枚举
        /// </summary>
        public enum CategoryType
        {
            //顶部水平滚动
            HORIZONTALSCROLLCARD,
            //优质作者
            TOPPGC,
            //方形视频小分类
            SQUARECARD,
            //方形视频，例如360°全景视频
            RECTANGLECARD
        }

        /// <summary>
        /// 视频小分类类型 枚举
        /// </summary>
        public enum CategorySubType
        {
            NONE,
            //专题
            CAMPAIGN,
            //标签视频
            TAG,
            //视频小分类
            CATEGORY
        }

        public enum CategoryErrorType
        {
            ERROR = -1,
            UNSUPPORT = -2
        }

        //作者dataType类型 枚举 BriefCard 代表 全部作者枚举类型 VideoCollectionWithBrie 代表 最近更新作者枚举类型
        public enum AuthorDataType
        {
            BRIEFCARD,
            //VideoCollectionWithBrie
            VIDEOCOLLECTIONWITHBRIE
        }
    }

}
