//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Config
//类名称:       AppEnvironment
//创建时间:     2015/9/21 星期一 15:22:13
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Networking.Connectivity;
using Windows.UI.ViewManagement;

namespace EyeSight.Config
{
    public class AppEnvironment
    {
        //判断是不是手机
        public static bool IsPhone
        {
            get
            {
                return ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
            }
        }

        //判断手机是不是竖屏
        public static bool IsPortrait
        {
            get
            {
                return ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait;
            }
        }

        //记录竖屏屏幕宽度
        public static double ScreenPortraitWith;

        //设定桌面模式下的窗口最小大小
        public static Size DesktopSize
        {
            get
            {
                //其实这个地方设置了等于没设 ，因为只要Height高度设置不为0的数，它的最小高度都会变为666
                return new Size(800, 800);
            }
        }

        public static bool IsWlanOrInternet
        {
            get
            {
                var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                if (profile != null)
                {
                    var interfaceType = profile.NetworkAdapter.IanaInterfaceType;

                    // 71 is WiFi & 6 is Ethernet(LAN)
                    if (interfaceType == 71 || interfaceType == 6)
                    {
                        return true;
                    }
                    // 243 & 244 is 3G/Mobile
                    else if (interfaceType == 243 || interfaceType == 244)
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 是否具有网络连接
        /// </summary>
        public static bool IsInternetAccess
        {
            get
            {
                ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (internetConnectionProfile != null)
                {
                    if (internetConnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsNeedToUpdateSplashImage
        {
            get
            {
                //都要转换成东八区北京时间来计算
                var saveDate = SettingsStore.GetValueOrDefault<long>(AppCommonConst.DATE_HAS_SAVE, DateTime.Now.ToChinaStandardTime().AddDays(-1).ToUnixTime());
                if ((DateTime.Now.ToChinaStandardTime().Date - saveDate.ToDateTime().Date).Days >= 1)
                {
                    if (IsInternetAccess)
                    {
                        SettingsStore.AddOrUpdateValue<long>(AppCommonConst.DATE_HAS_SAVE, DateTime.Now.ToChinaStandardTime().ToUnixTime());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsMemoryLimitMoreThan185
        {
            get
            {
                var memoryLimit = Windows.System.MemoryManager.AppMemoryUsageLimit;
                memoryLimit = (memoryLimit / 1024) / 1024;
                Debug.WriteLine("Device Memory Limit: " + memoryLimit + "MB");
                if (memoryLimit > 200)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
