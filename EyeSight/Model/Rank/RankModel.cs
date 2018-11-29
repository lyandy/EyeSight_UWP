//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model.Rank
//类名称:       RankModel
//创建时间:     2015/9/25 星期五 19:29:01
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Model.Rank
{

    public class RankModel
    {
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

    }
}
