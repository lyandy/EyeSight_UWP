//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.UIControl.UserControls.PullToRefreshUICtrl
//类名称:       PullToRefreshScrollViewer
//创建时间:     2015/9/21 星期一 16:03:40
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EyeSight.UIControl.UserControls.PullToRefreshUICtrl
{
    public class PullToRefreshScrollViewer : ListView
    {
        private const string ScrollViewerControl = "ScrollViewer";
        private const string ContainerGrid = "ContainerGrid";
        private const string ArrowGrid = "ArrowGrid";
        private const string TextGrid = "TextGrid";
        private const string PullToRefreshIndicator = "PullToRefreshIndicator";
        private const string VisualStateNormal = "Normal";
        private const string VisualStateReadyToRefresh = "ReadyToRefresh";

        private DispatcherTimer compressionTimer;
        public ScrollViewer scrollViewer;
        private DispatcherTimer timer;
        private Grid containerGrid;
        private Grid arrowGrid;
        private Border pullToRefreshIndicator;
        private bool isCompressionTimerRunning;
        private bool isReadyToRefresh;
        private bool isCompressedEnough;

        public event EventHandler RefreshContent;

        public static readonly DependencyProperty PullTextProperty = DependencyProperty.Register("PullText", typeof(string), typeof(PullToRefreshScrollViewer), new PropertyMetadata("下拉刷新..."));
        public static readonly DependencyProperty RefreshTextProperty = DependencyProperty.Register("RefreshText", typeof(string), typeof(PullToRefreshScrollViewer), new PropertyMetadata("松开开始刷新..."));
        public static readonly DependencyProperty RefreshTimeProperty = DependencyProperty.Register("RefreshTime", typeof(string), typeof(PullToRefreshScrollViewer), new PropertyMetadata("最后更新：" + SettingsStore.GetValueOrDefault<string>(AppCommonConst.LAST_UPDATE_TIME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))));
        public static readonly DependencyProperty RefreshHeaderHeightProperty = DependencyProperty.Register("RefreshHeaderHeight", typeof(double), typeof(PullToRefreshScrollViewer), new PropertyMetadata(40D));
        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(PullToRefreshScrollViewer), new PropertyMetadata(null));
        public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register("ArrowColor", typeof(Brush), typeof(PullToRefreshScrollViewer), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(200, 128, 128, 128))));

        //下拉到多少偏移量状态改变
        private double offsetTreshhold = 52;

        public PullToRefreshScrollViewer()
        {
            this.DefaultStyleKey = typeof(PullToRefreshScrollViewer);
            Loaded += PullToRefreshScrollViewer_Loaded;
        }
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public double RefreshHeaderHeight
        {
            get { return (double)GetValue(RefreshHeaderHeightProperty); }
            set { SetValue(RefreshHeaderHeightProperty, value); }
        }

        public string RefreshText
        {
            get { return (string)GetValue(RefreshTextProperty); }
            set { SetValue(RefreshTextProperty, value); }
        }

        public string RefreshTime
        {
            get { return (string)GetValue(RefreshTimeProperty); }
            set { SetValue(RefreshTimeProperty, value); }
        }

        public string PullText
        {
            get { return (string)GetValue(PullTextProperty); }
            set { SetValue(PullTextProperty, value); }
        }

        public Brush ArrowColor
        {
            get { return (Brush)GetValue(ArrowColorProperty); }
            set { SetValue(ArrowColorProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollViewer = (ScrollViewer)GetTemplateChild(ScrollViewerControl);
            scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
            scrollViewer.Margin = new Thickness(0, 0, 0, -RefreshHeaderHeight);
            var transform = new CompositeTransform();
            transform.TranslateY = -RefreshHeaderHeight;
            scrollViewer.RenderTransform = transform;

            containerGrid = (Grid)GetTemplateChild(ContainerGrid);

            arrowGrid = (Grid)GetTemplateChild(ArrowGrid);
            //textGrid.Loaded += (ss, ee) =>
            //{
            //    //Rect rect= textGrid.TransformToVisual(pullToRefreshIndicator).TransformBounds(new Rect(0.0, 0.0, pullToRefreshIndicator.ActualWidth, pullToRefreshIndicator.ActualHeight));

            //    //Debug.WriteLine(rect);

            //};

            pullToRefreshIndicator = (Border)GetTemplateChild(PullToRefreshIndicator);
            pullToRefreshIndicator.Loaded += (ss, ee) =>
            {
                arrowGrid.Margin = new Thickness(pullToRefreshIndicator.ActualWidth / 2 - 115, 0, 0, 0);
            };
            SizeChanged += OnSizeChanged;
        }

        private void PullToRefreshScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;

            compressionTimer = new DispatcherTimer();
            compressionTimer.Interval = TimeSpan.FromSeconds(0.01);
            compressionTimer.Tick += CompressionTimer_Tick;

            timer.Start();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (e.NextView.VerticalOffset == 0)
            {
                timer.Start();
            }
            else
            {
                if (timer != null)
                {
                    timer.Stop();
                }

                if (compressionTimer != null)
                {
                    compressionTimer.Stop();
                }

                isCompressionTimerRunning = false;
                isCompressedEnough = false;
                isReadyToRefresh = false;

                VisualStateManager.GoToState(this, VisualStateNormal, true);
            }
        }

        private async void CompressionTimer_Tick(object sender, object e)
        {
            if (isCompressedEnough)
            {
                VisualStateManager.GoToState(this, VisualStateReadyToRefresh, true);
                isReadyToRefresh = true;
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStateNormal, true);
                isCompressedEnough = false;
                compressionTimer.Stop();
                //哈哈哈哈哈哈哈哈，我是不是太聪明了，我日你的嘴微软！！ScrollViewer的回弹不会超过0.3秒，所以我延迟0.3秒就解决了问题。哈哈哈哈哈。
                await Task.Delay(300);
                isReadyToRefresh = false;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (containerGrid != null)
            {
                Rect elementBounds = pullToRefreshIndicator.TransformToVisual(containerGrid).TransformBounds(new Rect(0.0, 0.0, pullToRefreshIndicator.Height, RefreshHeaderHeight));
                var compressionOffset = elementBounds.Bottom;
                //Debug.WriteLine(compressionOffset);
                // Debug.WriteLine(isReadyToRefresh);

                if (compressionOffset > offsetTreshhold)
                {
                    if (isCompressionTimerRunning == false)
                    {
                        isCompressionTimerRunning = true;
                        compressionTimer.Start();
                    }

                    isCompressedEnough = true;
                }
                else if (compressionOffset <= 1 && isReadyToRefresh == true)
                {
                    isReadyToRefresh = false;
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Start();
                    }
                    InvokeRefresh();
                }
                else
                {
                    isCompressedEnough = false;
                    isCompressionTimerRunning = false;
                }
            }
        }

        private void InvokeRefresh()
        {
            isReadyToRefresh = false;
            VisualStateManager.GoToState(this, VisualStateNormal, true);

            if (RefreshContent != null)
            {
                RefreshContent(this, EventArgs.Empty);
            }

            var paramter = GetCommandParameter(this);

            if (RefreshCommand != null && RefreshCommand.CanExecute(paramter))
            {
                RefreshCommand.Execute(paramter);
            }

            //SettingsStore.AddOrUpdateValue<string>(AppCommonConst.LAST_UPDATE_TIME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            RefreshTime = "最后更新：" + SettingsStore.GetValueOrDefault<string>(AppCommonConst.LAST_UPDATE_TIME, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        #region RefeshCommandParameter
        public static readonly DependencyProperty RefeshCommandParameterProperty =
           DependencyProperty.RegisterAttached("RefreshCommandParameter", typeof(object), typeof(PullToRefreshScrollViewer), null);

        public object RefreshCommandParameter
        {
            get { return GetCommandParameter(this); }
            set { SetCommandParameter(this, value); }
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(RefeshCommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(RefeshCommandParameterProperty, value);
        }

        #endregion
    }
}
