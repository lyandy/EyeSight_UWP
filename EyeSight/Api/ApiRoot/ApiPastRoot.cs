//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Api.ApiRoot
//类名称:       ApiPastRoot
//创建时间:     2015/9/24 星期四 18:50:24
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Const;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EyeSight.Common.CommonEnum;

namespace EyeSight.Api.ApiRoot
{
    public class ApiPastRoot : ClassBase<ApiPastRoot>
    {
        public ApiPastRoot() : base() { }

        /// <summary>
        /// 往期分类-总分类请求链接
        /// </summary>
        public string CatrgoryUrl
        {
            get
            {
                return "http://baobab.wandoujia.com/api/v3/discovery?test=1";
            }
        }

        public string CatrgoryDetailUrl
        {
            get
            {
                var nextPageUrl = DicStore.GetValueOrDefault<string>(AppCommonConst.PAST_CATEGORY_DETAIL_NEXT_PAGE_URL, null);
                if (!string.IsNullOrEmpty(nextPageUrl))
                {
                    return nextPageUrl;
                }
                else
                {
                    string actionRealUrl = string.Empty;
                    CategorySubType catyegorySubType = DicStore.GetValueOrDefault<CategorySubType>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_ACTION_TYPE, CategorySubType.NONE);
                    int actionId = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_PAST_CATEGORY_DETAIL_ACTION_ID, (int)CategoryErrorType.ERROR);
                    if (catyegorySubType != CategorySubType.NONE && actionId != (int)CategoryErrorType.ERROR)
                    {
                        switch (catyegorySubType)
                        {
                            case CategorySubType.CAMPAIGN:
                                actionRealUrl = "http://baobab.wandoujia.com/api/v3/specialTopics?test=1";
                                break;
                            case CategorySubType.TAG:
                                actionRealUrl = "http://baobab.wandoujia.com/api/v3/tag/videos?num=20&tagId=" + actionId;
                                break;
                            case CategorySubType.CATEGORY:
                                actionRealUrl = "http://baobab.wandoujia.com/api/v3/videos?categoryId=" + actionId;
                                break;
                            default:
                                break;
                        }
                    }

                    return actionRealUrl + "&strategy=date";
                }
            }
        }
    }
}
