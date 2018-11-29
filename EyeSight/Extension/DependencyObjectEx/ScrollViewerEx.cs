//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       ScrollViewerEx
//创建时间:     2015/9/21 星期一 15:38:38
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace EyeSight.Extension.DependencyObjectEx
{
    public static class ScrollViewerEx
    {
        private static readonly TimeSpan DefaultAnimatedScrollDuration = TimeSpan.FromSeconds(1.5);
        private static readonly EasingFunctionBase DefaultEasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

        #region AnimatedScrollHandler
        /// <summary>
        /// AnimatedScrollHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty AnimatedScrollHandlerProperty =
            DependencyProperty.RegisterAttached(
                "AnimatedScrollHandler",
                typeof(ScrollViewerAnimatedScrollHandler),
                typeof(ScrollViewerEx),
                new PropertyMetadata(null, OnAnimatedScrollHandlerChanged));

        /// <summary>
        /// Gets the AnimatedScrollHandler property. This dependency property 
        /// indicates the handler object that handles animated scrolling of the ScrollViewer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ScrollViewerAnimatedScrollHandler GetAnimatedScrollHandler(DependencyObject d)
        {
            if (null == d) return null;
            return (ScrollViewerAnimatedScrollHandler)d.GetValue(AnimatedScrollHandlerProperty);
        }

        /// <summary>
        /// Sets the AnimatedScrollHandler property. This dependency property 
        /// indicates the handler object that handles animated scrolling of the ScrollViewer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetAnimatedScrollHandler(DependencyObject d, ScrollViewerAnimatedScrollHandler value)
        {
            if (null == d) return;
            d.SetValue(AnimatedScrollHandlerProperty, value);
        }

        /// <summary>
        /// Handles changes to the AnimatedScrollHandler property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnAnimatedScrollHandlerChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewerAnimatedScrollHandler oldAnimatedScrollHandler = (ScrollViewerAnimatedScrollHandler)e.OldValue;
            ScrollViewerAnimatedScrollHandler newAnimatedScrollHandler = (ScrollViewerAnimatedScrollHandler)d.GetValue(AnimatedScrollHandlerProperty);

            var scrollViewer = d as ScrollViewer;

            Debug.Assert(scrollViewer != null);

            if (oldAnimatedScrollHandler != null)
                oldAnimatedScrollHandler.Detach();

            if (newAnimatedScrollHandler != null)
                newAnimatedScrollHandler.Attach(scrollViewer);
        }
        #endregion

        #region ScrollToHorizontalOffsetWithAnimation()
        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToHorizontalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToHorizontalOffset() does not update HorizontalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static async Task ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset)
        {
            await scrollViewer.ScrollToHorizontalOffsetWithAnimation(offset, DefaultAnimatedScrollDuration);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToHorizontalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToHorizontalOffset() does not update HorizontalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        public static async Task ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            double durationInSeconds)
        {
            await scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                offset,
                TimeSpan.FromSeconds(durationInSeconds),
                DefaultEasingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToHorizontalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToHorizontalOffset() does not update HorizontalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            double durationInSeconds,
            EasingFunctionBase easingFunction)
        {
            await scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                offset,
                TimeSpan.FromSeconds(durationInSeconds),
                easingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToHorizontalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToHorizontalOffset() does not update HorizontalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            TimeSpan duration)
        {
            await scrollViewer.ScrollToHorizontalOffsetWithAnimation(
                offset,
                duration,
                DefaultEasingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToHorizontalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToHorizontalOffset() does not update HorizontalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ScrollToHorizontalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            var handler = GetAnimatedScrollHandler(scrollViewer);

            if (handler == null)
            {
                handler = new ScrollViewerAnimatedScrollHandler();
                SetAnimatedScrollHandler(scrollViewer, handler);
            }

            await handler.ScrollToHorizontalOffsetWithAnimation(
                offset, duration, easingFunction);
        }
        #endregion

        #region ScrollToVerticalOffsetWithAnimation()
        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToVerticalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToVerticalOffset() does not update VerticalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static async Task ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset)
        {
            await scrollViewer.ScrollToVerticalOffsetWithAnimation(offset, DefaultAnimatedScrollDuration);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToVerticalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToVerticalOffset() does not update VerticalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        public static async Task ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            double durationInSeconds)
        {
            await scrollViewer.ScrollToVerticalOffsetWithAnimation(
                offset,
                TimeSpan.FromSeconds(durationInSeconds),
                DefaultEasingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToVerticalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToVerticalOffset() does not update VerticalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            double durationInSeconds,
            EasingFunctionBase easingFunction)
        {
            await scrollViewer.ScrollToVerticalOffsetWithAnimation(
                offset,
                TimeSpan.FromSeconds(durationInSeconds),
                easingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToVerticalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToVerticalOffset() does not update VerticalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            TimeSpan duration)
        {
            await scrollViewer.ScrollToVerticalOffsetWithAnimation(
                offset,
                duration,
                DefaultEasingFunction);
        }

        /// <summary>
        /// Scrolls to the specified offset using an animation instead of
        /// immediately jumping to that offset as with ScrollToVerticalOffset().
        /// </summary>
        /// <remarks>
        /// Note that calling ScrollToVerticalOffset() does not update VerticalOffset immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ScrollToVerticalOffsetWithAnimation(
            this ScrollViewer scrollViewer,
            double offset,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            var handler = GetAnimatedScrollHandler(scrollViewer);

            if (handler == null)
            {
                handler = new ScrollViewerAnimatedScrollHandler();
                SetAnimatedScrollHandler(scrollViewer, handler);
            }

            await handler.ScrollToVerticalOffsetWithAnimation(
                offset, duration, easingFunction);
        }
        #endregion

        #region ZoomToFactorWithAnimation()
        /// <summary>
        /// Zooms to the specified factor using an animation instead of
        /// immediately jumping to that value as with ZoomToFactor().
        /// </summary>
        /// <remarks>
        /// Note that calling ZoomToFactor() does not update ZoomFactor immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static async Task ZoomToFactorWithAnimation(
            this ScrollViewer scrollViewer,
            double factor)
        {
            await scrollViewer.ZoomToFactorWithAnimation(factor, DefaultAnimatedScrollDuration);
        }

        /// <summary>
        /// Zooms to the specified factor using an animation instead of
        /// immediately jumping to that value as with ZoomToFactor().
        /// </summary>
        /// <remarks>
        /// Note that calling ZoomToFactor() does not update ZoomFactor immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="factor"></param>
        /// <param name="durationInSeconds"></param>
        /// <returns></returns>
        public static async Task ZoomToFactorWithAnimation(
            this ScrollViewer scrollViewer,
            double factor,
            double durationInSeconds)
        {
            await scrollViewer.ZoomToFactorWithAnimation(
                factor,
                TimeSpan.FromSeconds(durationInSeconds),
                DefaultEasingFunction);
        }

        /// <summary>
        /// Zooms to the specified factor using an animation instead of
        /// immediately jumping to that value as with ZoomToFactor().
        /// </summary>
        /// <remarks>
        /// Note that calling ZoomToFactor() does not update ZoomFactor immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="factor"></param>
        /// <param name="durationInSeconds"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ZoomToFactorWithAnimation(
            this ScrollViewer scrollViewer,
            double factor,
            double durationInSeconds,
            EasingFunctionBase easingFunction)
        {
            await scrollViewer.ZoomToFactorWithAnimation(
                factor,
                TimeSpan.FromSeconds(durationInSeconds),
                easingFunction);
        }

        /// <summary>
        /// Zooms to the specified factor using an animation instead of
        /// immediately jumping to that value as with ZoomToFactor().
        /// </summary>
        /// <remarks>
        /// Note that calling ZoomToFactor() does not update ZoomFactor immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="factor"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task ZoomToFactorWithAnimation(
            this ScrollViewer scrollViewer,
            double factor,
            TimeSpan duration)
        {
            await scrollViewer.ZoomToFactorWithAnimation(
                factor,
                duration,
                DefaultEasingFunction);
        }

        /// <summary>
        /// Zooms to the specified factor using an animation instead of
        /// immediately jumping to that value as with ZoomToFactor().
        /// </summary>
        /// <remarks>
        /// Note that calling ZoomToFactor() does not update ZoomFactor immediately,
        /// so it is important to wait for it to change before calling this method.
        /// </remarks>
        /// <param name="scrollViewer"></param>
        /// <param name="factor"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public static async Task ZoomToFactorWithAnimation(
            this ScrollViewer scrollViewer,
            double factor,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            var handler = GetAnimatedScrollHandler(scrollViewer);

            if (handler == null)
            {
                handler = new ScrollViewerAnimatedScrollHandler();
                SetAnimatedScrollHandler(scrollViewer, handler);
            }

            await handler.ZoomToFactorWithAnimation(
                factor, duration, easingFunction);
        }
        #endregion

        #region ChangeView
        public static async Task ChangeViewEx(this ScrollViewer scrollViewer, double horizontalOffset, double verticalOffset, double factor)
        {
            var handler = GetAnimatedScrollHandler(scrollViewer);

            if (handler == null)
            {
                handler = new ScrollViewerAnimatedScrollHandler();
                SetAnimatedScrollHandler(scrollViewer, handler);
            }
            await handler.ChangeViewWithAnimation(factor, horizontalOffset, verticalOffset, TimeSpan.FromSeconds(0.8), new CircleEase() { EasingMode = EasingMode.EaseOut });
            //await handler.ZoomToFactorWithAnimation(
            //    factor, duration, easingFunction);
        }
        #endregion
    }

    public class ScrollViewerAnimatedScrollHandler : FrameworkElement
    {
        private ScrollViewer _scrollViewer;

        // Sliders are used as animation targets due to problems with custom property animation
        private Slider _sliderHorizontal;
        private Slider _sliderVertical;
        private Slider _sliderZoom;

        #region CTOR
        public ScrollViewerAnimatedScrollHandler()
        {
        }
        #endregion

        #region Attach()
        public void Attach(ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
            _sliderHorizontal = new Slider();
            _sliderHorizontal.SmallChange = 0.0000000001;
            _sliderHorizontal.Minimum = double.MinValue;
            _sliderHorizontal.Maximum = double.MaxValue;
            _sliderHorizontal.StepFrequency = 0.0000000001;
            _sliderHorizontal.ValueChanged += OnHorizontalOffsetChanged;
            _sliderVertical = new Slider();
            _sliderVertical.SmallChange = 0.0000000001;
            _sliderVertical.Minimum = double.MinValue;
            _sliderVertical.Maximum = double.MaxValue;
            _sliderVertical.StepFrequency = 0.0000000001;
            _sliderVertical.ValueChanged += OnVerticalOffsetChanged;
            _sliderZoom = new Slider();
            _sliderZoom.SmallChange = 0.0000000001;
            _sliderZoom.Minimum = double.MinValue;
            _sliderZoom.Maximum = double.MaxValue;
            _sliderZoom.StepFrequency = 0.0000000001;
            _sliderZoom.ValueChanged += OnZoomFactorChanged;
        }
        #endregion Attach()

        #region Detach()
        public void Detach()
        {
            _scrollViewer = null;

            if (_sliderHorizontal != null)
            {
                _sliderHorizontal.ValueChanged -= OnHorizontalOffsetChanged;
                _sliderHorizontal = null;
            }

            if (_sliderVertical != null)
            {
                _sliderVertical.ValueChanged -= OnHorizontalOffsetChanged;
                _sliderVertical = null;
            }

            if (_sliderZoom != null)
            {
                _sliderZoom.ValueChanged -= OnZoomFactorChanged;
                _sliderZoom = null;
            }
        }
        #endregion Detach()

        #region OnHorizontalOffsetChanged()
        private void OnHorizontalOffsetChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToHorizontalOffset(e.NewValue);
            }
        }
        #endregion

        #region OnVerticalOffsetChanged()
        private void OnVerticalOffsetChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollToVerticalOffset(e.NewValue);
            }
        }
        #endregion

        #region OnZoomFactorChanged()
        private void OnZoomFactorChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ZoomToFactor((float)e.NewValue);
            }
        }
        #endregion

        #region ScrollToHorizontalOffsetWithAnimation()
        internal async Task ScrollToHorizontalOffsetWithAnimation(
            double offset,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            var sb = new Storyboard();
            var da = new DoubleAnimation();
            da.EnableDependentAnimation = true;
            da.From = _scrollViewer.HorizontalOffset;
            da.To = offset;
            da.EasingFunction = easingFunction;
            da.Duration = duration;
            sb.Children.Add(da);
            Storyboard.SetTarget(sb, _sliderHorizontal);
            Storyboard.SetTargetProperty(da, "Value");
            await sb.BeginAsync();
        }
        #endregion

        #region ScrollToVerticalOffsetWithAnimation()
        internal async Task ScrollToVerticalOffsetWithAnimation(
            double offset,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            var sb = new Storyboard();
            var da = new DoubleAnimation();
            da.EnableDependentAnimation = true;
            da.From = _scrollViewer.VerticalOffset;
            da.To = offset;
            da.EasingFunction = easingFunction;
            da.Duration = duration;
            sb.Children.Add(da);
            Storyboard.SetTarget(sb, _sliderVertical);
            Storyboard.SetTargetProperty(da, "Value");
            await sb.BeginAsync();
        }
        #endregion

        #region ZoomToFactorWithAnimation()
        internal async Task ZoomToFactorWithAnimation(
            double factor,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            if (_scrollViewer == null) return;
            var sb = new Storyboard();
            var da = new DoubleAnimation();
            da.EnableDependentAnimation = true;
            da.From = _scrollViewer.ZoomFactor;
            da.To = factor;
            da.EasingFunction = easingFunction;
            da.Duration = duration;
            sb.Children.Add(da);
            Storyboard.SetTarget(sb, _sliderZoom);
            Storyboard.SetTargetProperty(da, "Value");
            await sb.BeginAsync();
        }
        #endregion

        #region ChangeViewWithAnimation()
        internal async Task ChangeViewWithAnimation(
            double factor, double horioffset, double vertioffset,
            TimeSpan duration,
            EasingFunctionBase easingFunction)
        {
            if (_scrollViewer == null) return;
            var sb = new Storyboard();
            var da = new DoubleAnimation();
            da.EnableDependentAnimation = true;
            da.From = _scrollViewer.ZoomFactor;
            da.To = factor;
            da.EasingFunction = easingFunction;
            da.Duration = duration;
            sb.Children.Add(da);
            Storyboard.SetTarget(da, _sliderZoom);
            Storyboard.SetTargetProperty(da, "Value");
            var daHori = new DoubleAnimation();
            daHori.EnableDependentAnimation = true;
            daHori.From = _scrollViewer.HorizontalOffset;
            daHori.To = horioffset;
            daHori.EasingFunction = easingFunction;
            daHori.Duration = duration;
            sb.Children.Add(daHori);
            Storyboard.SetTarget(daHori, _sliderHorizontal);
            Storyboard.SetTargetProperty(daHori, "Value");
            var daVerti = new DoubleAnimation();
            daVerti.EnableDependentAnimation = true;
            daVerti.From = _scrollViewer.VerticalOffset;
            daVerti.To = vertioffset;
            daVerti.EasingFunction = easingFunction;
            daVerti.Duration = duration;
            sb.Children.Add(daVerti);
            Storyboard.SetTarget(daVerti, _sliderVertical);
            Storyboard.SetTargetProperty(daVerti, "Value");
            await sb.BeginAsync();
        }
        #endregion
    }
}
