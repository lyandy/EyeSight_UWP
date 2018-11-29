//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Base
//类名称:       UIControlBase
//创建时间:     2015/9/21 星期一 15:15:54
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace EyeSight.Base
{
    public class UIControlBase : UserControl
    {
        NavigationRootPage rootPage = null;
        Frame rootFrame = null;
        public UIControlBase()
        {
            this.Loaded += (ss, ee) =>
            {
                this.PointerPressed += UIControlBase_PointerPressed;
                this.PointerExited += UIControlBase_PointerExited;
                this.PointerReleased += UIControlBase_PointerReleased;
                this.PointerCaptureLost += UIControlBase_PointerCaptureLost;
                this.Tapped += UIControlBase_Tapped;

                rootPage = Window.Current.Content as NavigationRootPage;
                if (rootPage != null)
                {
                    rootFrame = (Frame)rootPage.FindName("rootFrame");

                    if (rootFrame != null)
                    {
                        //rootFrame.SizeChanged -= RootFrame_SizeChanged;
                        rootFrame.SizeChanged += RootFrame_SizeChanged;
                    }
                }

            };
            this.Unloaded += (ss, ee) =>
            {
                this.PointerPressed -= UIControlBase_PointerPressed;
                this.PointerExited -= UIControlBase_PointerExited;
                this.PointerReleased -= UIControlBase_PointerReleased;
                this.PointerCaptureLost -= UIControlBase_PointerCaptureLost;
                this.Tapped -= UIControlBase_Tapped;

                if (rootPage != null)
                {
                    if (rootFrame != null)
                    {
                        rootFrame.SizeChanged -= RootFrame_SizeChanged;
                    }
                }
            };
        }

        #region 黑色遮罩操作 和 上划手势识别

        //Point startPoint;

        //int swipeCount = 0;

        //private void SwipeUpTimer_Tick(object sender, object e)
        //{
        //    swipeCount++;
        //}

        //private void SwipeRecognizer(Point endPoint)
        //{
        //    if (swipeUpTimer != null)
        //    {
        //        swipeUpTimer.Stop();
        //        swipeUpTimer = null;
        //    }
        //    if (startPoint.Y - endPoint.Y > 200 && swipeCount <= 200)
        //    {
        //        OnUIControlSwipeUp(null ,null);
        //    }
        //}

        bool isPress = false;

        DispatcherTimer timer = null;
        //DispatcherTimer swipeUpTimer = null;

        private void UIControlBase_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (!isPress)
            {
                OnUIControlBaseNeedNavigate(sender, null);
            }
        }

        private void UIControlBase_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            OnUIControlBaseNeedCaptureLost(sender, e);
            //SwipeRecognizer(e.GetCurrentPoint(this).Position);
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                isPress = false;
                OnUIControlBaseNeedReleaseHolding(sender, e);
                
            }
        }

        private void UIControlBase_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                //isPress = false;
                OnUIControlBaseNeedNavigate(sender, e);
            }
        }

        private void UIControlBase_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (isPress)
            {
                isPress = false;
                OnUIControlBaseNeedReleaseHolding(sender, e);
            }
        }

        private void UIControlBase_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //swipeCount = 0;
            //if (swipeUpTimer != null)
            //{
            //    swipeUpTimer.Stop();
            //    swipeUpTimer = null;
            //}

            //if (swipeUpTimer == null)
            //{
            //    swipeUpTimer = new DispatcherTimer();
            //}
            //swipeUpTimer.Interval = TimeSpan.FromMilliseconds(1);
            //swipeUpTimer.Tick += SwipeUpTimer_Tick;
            //startPoint.Y = e.GetCurrentPoint(this).Position.Y;
            //swipeUpTimer.Start();


            int count = 0;

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            if (timer == null)
            {
                timer = new DispatcherTimer();
            }

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += (ss, ee) =>
            {
                count++;

                if (count > 1)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                        isPress = true;
                    }
                    OnUIControlBaseNeedHolding(sender, e);
                }
            };
            timer.Start();
        }

        protected virtual void OnUIControlBaseNeedCaptureLost(object sender, PointerRoutedEventArgs e)
        {

        }

        protected virtual void OnUIControlSwipeUp(object sender, PointerRoutedEventArgs e)
        {
            //Debug.Write(str);
        }

        protected virtual void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            //Debug.Write(str);
        }

        protected virtual void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            //Debug.Write(str);
        }

        protected virtual void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            //Debug.Write(str);
        }
        #endregion

        //public string str = "ss";

        private void RootFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnRootFrameSizeChanged(sender, e);
            //Test();
            //Test1();
        }

        protected virtual void Test()
        {
            Debug.WriteLine("父类执行");
        }

        public void Test1()
        {
            Debug.WriteLine("父类执行Test1");
        }

        protected virtual void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Debug.Write(str);
        }

        ~UIControlBase()
        {
            rootPage = null;
            rootFrame = null;
        }
    }
}
