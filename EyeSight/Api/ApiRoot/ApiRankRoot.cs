//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Api.ApiRoot
//类名称:       ApiRankRoot
//创建时间:     2015/9/25 星期五 19:31:12
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Api.ApiRoot
{
    public class ApiRankRoot : ClassBase<ApiRankRoot>
    {
        public ApiRankRoot() : base() { }

        public string RankUrl
        {
            get
            {
                var rankSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);
                string rankSelectedUrl = "http://baobab.wandoujia.com/api/v1/ranklist?strategy=weekly";
                switch (rankSelectedIndex)
                {
                    case 0:
                        rankSelectedUrl = "http://baobab.wandoujia.com/api/v1/ranklist?strategy=weekly";
                        break;
                    case 1:
                        rankSelectedUrl = "http://baobab.wandoujia.com/api/v1/ranklist?strategy=monthly";
                        break;
                    case 2:
                        rankSelectedUrl = "http://baobab.wandoujia.com/api/v1/ranklist?strategy=historical";
                        break;
                }

                return rankSelectedUrl;
            }
        }
    }
}
