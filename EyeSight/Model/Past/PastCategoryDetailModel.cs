//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model.Past
//类名称:       PasrCategoryDetailModel
//创建时间:     2015/9/25 星期五 12:35:55
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

namespace EyeSight.Model.Past
{
    public class PastCategoryDetailModel
    {
        private List<Itemlist> _itemList = new List<Itemlist>();
        public List<Itemlist> itemList
        {
            get
            {
                return _itemList;
            }
            set
            {
                _itemList = value;
            }
        }
        public int count { get; set; }
        public int total { get; set; }

        private string _nextPageUrl = null;
        public string nextPageUrl
        {
            get
            {
                return _nextPageUrl;
            }
            set
            {
                _nextPageUrl = value;
                if (string.IsNullOrEmpty(_nextPageUrl))
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.PAST_CATEGORY_DETAIL_HAS_NEXT_PAGE, false);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.PAST_CATEGORY_DETAIL_NEXT_PAGE_URL, null);
                }
                else
                {
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.PAST_CATEGORY_DETAIL_HAS_NEXT_PAGE, true);
                    DicStore.AddOrUpdateValue<string>(AppCommonConst.PAST_CATEGORY_DETAIL_NEXT_PAGE_URL, _nextPageUrl);
                }
            }
        }
    }

    public class Itemlist
    {
        public string type { get; set; }
        public VideoDetailData data { get; set; }
    }

    public class VideoDetailData : ModelPropertyBase
    {
    }
}
