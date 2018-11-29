//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Const
//类名称:       AppNetworkMessageConst
//创建时间:     2015/9/21 星期一 15:25:05
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Const
{
    public class AppNetworkMessageConst
    {
        public const string COLLECTION_ITEM_IS_ZERO = "没有获取到数据，请重试。";

        public const string NETWOTK_IS_ERROR = "数据获取发生错误，请重试。";

        public const string NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_ERROR = "无网络连接且本地缓存读取出错，请连接网络后重试。";

        public const string NETWORK_IS_OFFLINE_LOCAL_CACHE_IS_NULL = "无网络连接且本地无缓存，请连接网络后重试。";

        public const string NETWORK_IS_OFFLINEL = "无网络连接，请连接网络后重试。";

        public const string DATA_PRE_COMBINE_IS_ERROR = "数据预处理发生错误，请重试。";

        public const string DATA_UNSUPPORT = "此类型暂不支持，应用下次版本更新即可支持，请耐心等待。";

        public const string WEB_IS_ERROR = "网页加载错误，请重试。";

        public const string NETWORK_IS_NOT_WIFI_OR_LAN_TO_DOWNLOAD = "当前不是WiFi或宽带网络，下载将会消耗你的手机流量。确定要下载吗？";

        public const string NETWORK_IS_NOT_WIFI_OR_LAN_TO_PLAY = "当前不是WiFi或宽带网络，播放视频将会消耗你的手机流量。确定要播放吗？";

        public const string VIDEO_URL_IS_ERROR = "视频获取发生错误，请稍后再试。";

        public const string IS_WEBVIEW_NAVIAGATION_ERROR = "网页加载出错，请重试。";
    }
}
