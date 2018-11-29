//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Const
//类名称:       AppCommonMsgConst
//创建时间:     2015/9/21 星期一 15:24:13
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
    public class AppCommonMsgConst
    {
        public const string IS_GET_NET_DATA = "数据更新中，请稍后...";

        public const string IS_GET_NET_DATA_ERROR = "网路错误，获取数据失败，请重试...";

        public const string IS_LOCAL_DATA_NULL = "网路错误，且本地无缓存数据。";

        public const string IS_NET_DISCONNECTED = "网络连接已断开。";

        public const string IS_MANIPULATE_DATA_ERROR = "数据处理出错，请重试...";
    }
}
