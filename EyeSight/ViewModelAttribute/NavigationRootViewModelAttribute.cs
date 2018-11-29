//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.ViewModelAttribute
//类名称:       NavigationRootViewModelAttribute
//创建时间:     2015/9/21 星期一 16:47:28
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using EyeSight.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace EyeSight.ViewModelAttribute
{
    public class NavigationRootViewModelAttribute : ViewModelAttributeBase
    {
        #region 左侧类别数据集合
        public ObservableCollection<NavigationRootModel> _EyeSightCommonCollection = new ObservableCollection<NavigationRootModel>();
        public ObservableCollection<NavigationRootModel> EyeSightCommonCollection
        {
            get
            {
                return _EyeSightCommonCollection;
            }
            set
            {
                if (_EyeSightCommonCollection != value)
                {
                    _EyeSightCommonCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 底部分割线一下关于集合
        public ObservableCollection<NavigationRootModel> _EyeSightBottomCollection = new ObservableCollection<NavigationRootModel>();
        public ObservableCollection<NavigationRootModel> EyeSightBottomCollection
        {
            get
            {
                return _EyeSightBottomCollection;
            }
            set
            {
                if (_EyeSightBottomCollection != value)
                {
                    _EyeSightBottomCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region 左侧图片集合
        public BitmapImage DailyPNG
        {
            get;
            set;
        }

        public BitmapImage PastPNG
        {
            get;
            set;
        }

        public BitmapImage AuthorPNG
        {
            get;
            set;
        }

        public BitmapImage RankPNG
        {
            get;
            set;
        }

        public BitmapImage CollectionPNG
        {
            get;
            set;
        }

        public BitmapImage DownloadPNG
        {
            get;
            set;
        }

        public BitmapImage DonatePNG
        {
            get;
            set;
        }

        public BitmapImage GiftPNG
        {
            get;
            set;
        }

        public BitmapImage SettingPNG
        {
            get;
            set;
        }

        public BitmapImage AboutPNG
        {
            get;
            set;
        }
        #endregion
    }
}
