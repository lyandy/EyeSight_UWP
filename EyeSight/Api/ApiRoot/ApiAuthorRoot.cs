//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.Api.ApiRoot

//类名称:       ApiAuthorRoot

//创建时间:     2016/7/11 17:20:31

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
using static EyeSight.Common.CommonEnum;

namespace EyeSight.Api.ApiRoot
{
    public class ApiAuthorRoot : ClassBase<ApiAuthorRoot>
    {
        public ApiAuthorRoot() : base() { }

        public string AuthorListUrl
        {
            get
            {
                var nextPageUrl = DicStore.GetValueOrDefault<string>(AppCommonConst.AUTHOR_NEXT_PAGE_URL, null);
                if (!string.IsNullOrEmpty(nextPageUrl))
                {
                    return nextPageUrl;
                }
                else
                {
                    return "http://baobab.wandoujia.com/api/v3/tabs/pgcs?test=1";
                }
            }
        }

        public string AuthorDetailUrl
        {
            get
            {
                var nextPageUrl = DicStore.GetValueOrDefault<string>(AppCommonConst.AUTHOR_DETAIL_NEXT_PAGE_URL, null);
                if (!string.IsNullOrEmpty(nextPageUrl))
                {
                    return nextPageUrl;
                }
                else
                {
                    string actionRealUrl = string.Empty;
                    int actionId = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_AUTHOR_DETAIL_ACTION_ID, (int)CategoryErrorType.ERROR);
                    if (actionId != (int)CategoryErrorType.ERROR)
                    {
                        actionRealUrl = "http://baobab.wandoujia.com/api/v3/pgc/videos?pgcId=" + actionId;
                    }

                    return actionRealUrl + "&start=0&strategy=date";
                }
            }
        }
    }
}
