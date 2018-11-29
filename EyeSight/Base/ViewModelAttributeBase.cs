//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Base
//类名称:       ViewModelAttributeBase
//创建时间:     2015/9/21 星期一 15:16:43
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.ProgressUICtrl;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using EyeSight.View;
using EyeSight.View.Daily;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeSight.Base
{
    public class ViewModelAttributeBase : ViewModelBase
    {
        private ModelPropertyBase _modelPropertyBase;
        public ModelPropertyBase curModelPropertyBase
        {
            get
            {
                return _modelPropertyBase;
            }
            set
            {
                if (_modelPropertyBase != value)
                {
                    _modelPropertyBase = value;
                    try
                    {
                        RaisePropertyChanged();
                    }
                    catch(Exception ex)
                    {
                        string s = ex.Message;
                    }
                }
            }
        }

        #region IsBusy
        /// <summary>
        /// 只是是否正在加载数据
        /// </summary>
        private bool _IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }

            set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;

                    if (_IsBusy)
                    {
                        //及时隐藏数据加载不成功的提示
                        RetryBox.Instance.HideRetry();

                        //这里主要是兼容对自定义SplashScreenImage欢迎屏幕的控制。让主页面DailyPage的Progress在自己的页面上显示
                        var rootPage = Window.Current.Content as NavigationRootPage;
                        Frame rootFrame = (Frame)rootPage.FindName("rootFrame");
                        if (rootFrame != null)
                        {
                            if (rootFrame.Content != null && rootFrame.Content.GetType() != typeof(DailyPage))
                            {
                                ProgressBox.Instance.ShowProgress();
                            }
                        }
                    }
                    else
                    {
                        ProgressBox.Instance.HideProgress();
                    }

                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
