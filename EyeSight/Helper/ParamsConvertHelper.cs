//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       ParamsConvertHelper
//创建时间:     2015/9/21 星期一 15:48:08
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Helper
{
    public class ParamsConvertHelper : ClassBase<ParamsConvertHelper>
    {
        public ParamsConvertHelper() : base() { }

        public string Combine(IDictionary<string, string> parameters)
        {
            StringBuilder _apiParamsData = new StringBuilder();
            foreach (var pair in parameters)
            {
                _apiParamsData.Append(string.Format("{0}={1}&", pair.Key, pair.Value.ToString()));
            }
            return _apiParamsData.ToString();
        }
    }
}
