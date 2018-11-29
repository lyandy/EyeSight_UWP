//=====================================================

//Copyright (C) 2013-2016 All rights reserved

//CLR版本:      4.0.30319.42000

//命名空间:     EyeSight.ViewModel.Gift

//类名称:       GiftViewModel

//创建时间:     2016/7/6 19:05:38

//作者：        Andy.Li

//作者站点:     http://www.cnblogs.com/lyandy

//======================================================

using EyeSight.Model.Gift;
using EyeSight.ViewModelAttribute.Gift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.ViewModel.Gift
{
    public class GiftViewModel : GiftViewModelAttribute
    {
        public void GetGiftCollection()
        {
            GiftCollection.Add(new GiftModel { AppImage = "ms-appx:///Assets/GiftImages/ITHome+.png", AppName = "微IT", AppDesc = "纯，只为你而来", AppProductUri = new Uri("ms-windows-store://pdp/?ProductId=9NBLGGH08TFH") });
        }
    }
}
