//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Model
//类名称:       NavigationRootModel
//创建时间:     2015/9/21 星期一 16:49:00
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.Model
{
    public class NavigationRootModel : ModelBase
    {
        /// <summary>
        /// 类别前面的图标
        /// </summary>
        public BitmapImage IconBitmap { get; set; }

        /// <summary>
        /// 类别名称/页面名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 跳转页面类型
        /// </summary>
        public Type ClassType { get; set; }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
