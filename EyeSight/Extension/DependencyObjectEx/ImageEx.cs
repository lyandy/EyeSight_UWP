//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       ImageEx
//创建时间:     2015/9/21 星期一 15:34:44
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
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;

namespace EyeSight.Extension.DependencyObjectEx
{
    public enum ImageLoadedTransitionTypes
    {
        /// <summary>
        /// Image fades in when it loads.
        /// </summary>
        FadeIn,
        /// <summary>
        /// Image slides up when it loads.
        /// </summary>
        SlideUp,
        /// <summary>
        /// Image slides left when it loads.
        /// </summary>
        SlideLeft,
        /// <summary>
        /// Image slides down when it loads.
        /// </summary>
        SlideDown,
        /// <summary>
        /// Image slides right when it loads.
        /// </summary>
        SlideRight,
        /// <summary>
        /// Image uses a random transition from all available ones when it loads.
        /// </summary>
        Random
    }

    /// <summary>
    /// Attached properties that extend the Image control class.
    /// </summary>
    public static class ImageEx
    {
        #region FadeInCustomOnLoaded

        /// <summary>
        /// FadeInCustomOnLoaded Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FadeInCustomOnLoadedProperty =
            DependencyProperty.RegisterAttached(
                "FadeInCustomOnLoaded",
                typeof(bool),
                typeof(ImageEx),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the FadeInCustomOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static bool GetFadeInCustomOnLoaded(DependencyObject d)
        {
            return (bool)d.GetValue(FadeInCustomOnLoadedProperty);
        }

        /// <summary>
        /// Sets the FadeInCustomOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static void SetFadeInCustomOnLoaded(DependencyObject d, bool value)
        {
            d.SetValue(FadeInCustomOnLoadedProperty, value);
        }

        #endregion

        #region FadeInOnLoaded
        /// <summary>
        /// FadeInOnLoaded Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FadeInOnLoadedProperty =
            DependencyProperty.RegisterAttached(
                "FadeInOnLoaded",
                typeof(bool),
                typeof(ImageEx),
                new PropertyMetadata(false, OnFadeInOnLoadedChanged));

        /// <summary>
        /// Gets the FadeInOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static bool GetFadeInOnLoaded(DependencyObject d)
        {
            return (bool)d.GetValue(FadeInOnLoadedProperty);
        }

        /// <summary>
        /// Sets the FadeInOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static void SetFadeInOnLoaded(DependencyObject d, bool value)
        {
            d.SetValue(FadeInOnLoadedProperty, value);
        }

        /// <summary>
        /// Handles changes to the FadeInOnLoaded property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnFadeInOnLoadedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newFadeInOnLoaded = (bool)d.GetValue(FadeInOnLoadedProperty);
            var image = (Image)d;

            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (newFadeInOnLoaded)
            {
                var handler = new FadeInOnLoadedHandler((Image)d);
                SetFadeInOnLoadedHandler(d, handler);

#pragma warning disable 4014
                image.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => image.SetBinding(
                        SourceProperty,
                        new Binding
                        {
                            Path = new PropertyPath("Source"),
                            Source = image
                        }));
#pragma warning restore 4014
            }
            else
            {
                var handler = GetFadeInOnLoadedHandler(d);
                SetFadeInOnLoadedHandler(d, null);
                handler.Detach();
                image.SetValue(
                    ImageEx.SourceProperty,
                    null);
            }
        }
        #endregion

        #region FadeInOnLoadedHandler
        /// <summary>
        /// FadeInOnLoadedHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FadeInOnLoadedHandlerProperty =
            DependencyProperty.RegisterAttached(
                "FadeInOnLoadedHandler",
                typeof(FadeInOnLoadedHandler),
                typeof(ImageEx),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets the FadeInOnLoadedHandler property. This dependency property 
        /// indicates the handler for the FadeInOnLoaded property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static FadeInOnLoadedHandler GetFadeInOnLoadedHandler(DependencyObject d)
        {
            return (FadeInOnLoadedHandler)d.GetValue(FadeInOnLoadedHandlerProperty);
        }

        /// <summary>
        /// Sets the FadeInOnLoadedHandler property. This dependency property 
        /// indicates the handler for the FadeInOnLoaded property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetFadeInOnLoadedHandler(DependencyObject d, FadeInOnLoadedHandler value)
        {
            d.SetValue(FadeInOnLoadedHandlerProperty, value);
        }
        #endregion

        #region FadeAnimationType
        // Using a DependencyProperty as the backing store for FadeAnimationType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FadeAnimationTypeProperty =
            DependencyProperty.Register("FadeAnimationType", typeof(FadeAnimationEnum), typeof(ImageEx), new PropertyMetadata(FadeAnimationEnum._3DFlip));


        #endregion

        #region ImageLoadedTransitionType
        /// <summary>
        /// ImageLoadedTransitionType Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImageLoadedTransitionTypeProperty =
            DependencyProperty.RegisterAttached(
                "ImageLoadedTransitionType",
                typeof(ImageLoadedTransitionTypes),
                typeof(ImageEx),
                new PropertyMetadata(ImageLoadedTransitionTypes.FadeIn));

        /// <summary>
        /// Gets the ImageLoadedTransitionType property. This dependency property 
        /// indicates the type of transition to use when the image loads.
        /// </summary>
        public static ImageLoadedTransitionTypes GetImageLoadedTransitionType(DependencyObject d)
        {
            return (ImageLoadedTransitionTypes)d.GetValue(ImageLoadedTransitionTypeProperty);
        }

        /// <summary>
        /// Sets the ImageLoadedTransitionType property. This dependency property 
        /// indicates the type of transition to use when the image loads.
        /// </summary>
        public static void SetImageLoadedTransitionType(DependencyObject d, ImageLoadedTransitionTypes value)
        {
            d.SetValue(ImageLoadedTransitionTypeProperty, value);
        }
        #endregion

        #region Source
        /// <summary>
        /// Source Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached(
                "Source",
                typeof(object),
                typeof(ImageEx),
                new PropertyMetadata(null, OnSourceChanged));

        /// <summary>
        /// Gets the Source property. This dependency property 
        /// indicates the Image.Source that supports property change handling.
        /// </summary>
        public static object GetSource(DependencyObject d)
        {
            return (object)d.GetValue(SourceProperty);
        }

        /// <summary>
        /// Sets the Source property. This dependency property 
        /// indicates the Image.Source that supports property change handling.
        /// </summary>
        public static void SetSource(DependencyObject d, object value)
        {
            d.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool fadeInOnLoaded = GetFadeInOnLoaded(d);

            if (fadeInOnLoaded)
            {
                var handler = GetFadeInOnLoadedHandler(d);

                if (handler != null)
                    handler.Detach();

                SetFadeInOnLoadedHandler(d, new FadeInOnLoadedHandler((Image)d));
            }
        }
        #endregion

        #region CustomSource
        /// <summary>
        /// CustomSource Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CustomSourceProperty =
            DependencyProperty.RegisterAttached(
                "CustomSource",
                typeof(string),
                typeof(ImageEx),
                new PropertyMetadata(null, OnCustomSourceChanged));

        /// <summary>
        /// Gets the CustomSource property. This dependency property 
        /// indicates the location of the image file to be used as a source of the Image.
        /// </summary>
        /// <remarks>
        /// This property can be used to use custom loading and tracing code for the Source,
        /// though it currently requires modifying the toolkit code.
        /// </remarks>
        public static string GetCustomSource(DependencyObject d)
        {
            return (string)d.GetValue(CustomSourceProperty);
        }

        /// <summary>
        /// Sets the CustomSource property. This dependency property 
        /// indicates the location of the image file to be used as a source of the Image.
        /// </summary>
        /// <remarks>
        /// This property can be used to use custom loading and tracing code for the Source,
        /// though it currently requires modifying the toolkit code.
        /// </remarks>
        public static void SetCustomSource(DependencyObject d, string value)
        {
            d.SetValue(CustomSourceProperty, value);
        }

        /// <summary>
        /// Handles changes to the CustomSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static async void OnCustomSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string newCustomSource = (string)d.GetValue(CustomSourceProperty);

            Debug.Assert(d is Image);

            var image = (Image)d;

            //DC.Trace("ImageCreate: " + newCustomSource);
            //var bi = await BitmapImageLoadExtensions.LoadAsync(
            //    Package.Current.InstalledLocation, newCustomSource);
            //image.Source = bi;
            image.Source = new BitmapImage(new Uri("ms-appx:///" + newCustomSource));

            await image.WaitForUnloadedAsync();
            //DC.Trace("ImageDispose: " + newCustomSource);
            image.Source = null;

            GC.Collect();
        }
        #endregion
    }

    /// <summary>
    /// Handles fade in animations on mage controls.
    /// </summary>
    public class FadeInOnLoadedHandler
    {
        private static Random _random;
        private static Random Random { get { return _random ?? (_random = new Random()); } }
        private Image _image;
        private BitmapImage _source;
        private double _targetOpacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeInOnLoadedHandler" /> class.
        /// </summary>
        /// <param name="image">The image.</param>
        public FadeInOnLoadedHandler(Image image)
        {
            Attach(image);
        }

        /// <summary>
        /// Attaches the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        private void Attach(Image image)
        {
            _image = image;
            _source = image.Source as BitmapImage;

            if (_source != null)
            {
                if (_source.PixelWidth > 0)
                {
                    image.Opacity = 1;
                    _image = null;
                    _source = null;
                    return;
                }

                _source.ImageOpened += OnSourceImageOpened;
                _source.ImageFailed += OnSourceImageFailed;
            }

            image.Unloaded += OnImageUnloaded;

            _targetOpacity = image.Opacity == 0.0 ? 1.0 : image.Opacity;
            image.Opacity = 0;
        }

        private void OnSourceImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var source = (BitmapImage)sender;
            Debug.WriteLine("Failed: " + source.UriSource);
            source.ImageOpened -= OnSourceImageOpened;
            source.ImageFailed -= OnSourceImageFailed;
        }

        private async void OnSourceImageOpened(object sender, RoutedEventArgs e)
        {
            var source = (BitmapImage)sender;

            source.ImageOpened -= OnSourceImageOpened;
            source.ImageFailed -= OnSourceImageFailed;

            var transitionType = ImageEx.GetImageLoadedTransitionType(_image);

            if (transitionType == ImageLoadedTransitionTypes.Random)
            {
                transitionType = (ImageLoadedTransitionTypes)FadeInOnLoadedHandler.Random.Next(0, (int)ImageLoadedTransitionTypes.Random);
            }

            switch (transitionType)
            {
                case ImageLoadedTransitionTypes.FadeIn:
                    await _image.FadeInCustom(TimeSpan.FromSeconds(0.5), null, _targetOpacity);
                    break;
                case ImageLoadedTransitionTypes.SlideUp:
                default:
                    SlideIn(transitionType);
                    break;
            }


            //#pragma warning disable 4014
            //            if ((bool)_image.GetValue(ImageEx.FadeInCustomOnLoadedProperty))
            //                _image.FadeInCustom(TimeSpan.FromSeconds(1), null, _targetOpacity);
            //            else
            //            {
            //                var animationType = (FadeAnimationEnum)_image.GetValue(ImageEx.FadeAnimationTypeProperty);
            //                switch (animationType)
            //                {
            //                    case FadeAnimationEnum.FadeIn:
            //                        _image.FadeIn(TimeSpan.FromSeconds(1));
            //                        break;
            //                    case FadeAnimationEnum._3DFlip:
            //                        _image.Swipe3D(TimeSpan.FromSeconds(1), null, _targetOpacity);
            //                        break;
            //                    case FadeAnimationEnum.PopuUp:
            //                        _image.PopUp(TimeSpan.FromSeconds(1), null, _targetOpacity);
            //                        break;
            //                    default:
            //                        break;
            //                }

            //            }
            //#pragma warning restore 4014
        }

        private async void SlideIn(ImageLoadedTransitionTypes transitionType)
        {
            _image.Opacity = _targetOpacity;

            // Built-in animations are nice, but not very customizable. Leaving this for posterity.
            ////var animation = new RepositionThemeAnimation
            ////{
            ////    FromVerticalOffset = _image.ActualHeight,
            ////    Duration = TimeSpan.FromSeconds(2)
            ////};

            ////Storyboard.SetTarget(animation, _image);

            var oldTransform = _image.RenderTransform;
            var tempTransform = new TranslateTransform();
            _image.RenderTransform = tempTransform;
            DoubleAnimation animation = null;

            switch (transitionType)
            {
                case ImageLoadedTransitionTypes.SlideUp:
                    animation = new DoubleAnimation
                    {
                        From = _image.ActualHeight,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTargetProperty(animation, "Y");
                    break;
                case ImageLoadedTransitionTypes.SlideDown:
                    animation = new DoubleAnimation
                    {
                        From = -_image.ActualHeight,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTargetProperty(animation, "Y");
                    break;
                case ImageLoadedTransitionTypes.SlideRight:
                    animation = new DoubleAnimation
                    {
                        From = -_image.ActualWidth,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTargetProperty(animation, "X");
                    break;
                case ImageLoadedTransitionTypes.SlideLeft:
                    animation = new DoubleAnimation
                    {
                        From = _image.ActualWidth,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(1),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    Storyboard.SetTargetProperty(animation, "X");
                    break;
            }

            Storyboard.SetTarget(animation, tempTransform);
            var sb = new Storyboard();
            sb.Duration = animation.Duration;
            sb.Children.Add(animation);
            var clippingParent = _image.Parent as FrameworkElement;

            RectangleGeometry clip = null;

            if (clippingParent != null)
            {
                clip = clippingParent.Clip;
                var transformToParent = _image.TransformToVisual(clippingParent);
                var topLeft = transformToParent.TransformPoint(new Point(0, 0));
                topLeft = new Point(Math.Max(0, topLeft.X), Math.Max(0, topLeft.Y));
                var bottomRight = transformToParent.TransformPoint(new Point(_image.ActualWidth, _image.ActualHeight));
                bottomRight = new Point(Math.Min(clippingParent.ActualWidth, bottomRight.X), Math.Min(clippingParent.ActualHeight, bottomRight.Y));
                clippingParent.Clip =
                    new RectangleGeometry
                    {
                        Rect = new Rect(
                            topLeft,
                            bottomRight)
                    };
            }

            await sb.BeginAsync();

            if (_image == null)
            {
                return;
            }

            if (clippingParent != null)
            {
                _image.Clip = clip;
            }

            _image.RenderTransform = oldTransform;
        }

        private void OnImageUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        internal void Detach()
        {
            if (_source != null)
            {
                _source.ImageOpened -= OnSourceImageOpened;
                _source.ImageFailed -= OnSourceImageFailed;
            }

            if (_image != null)
            {
                _image.Unloaded -= OnImageUnloaded;
                _image.CleanUpPreviousFadeStoryboard();
                _image.Opacity = _targetOpacity;
                _image = null;
            }
        }
    }

    /// <summary>
    /// Author:wangrz 20131106
    /// </summary>
    public enum FadeAnimationEnum
    {
        FadeIn,
        _3DFlip,
        PopuUp
    }

    #region Image动画辅助类
    internal static class UIElementAnimationEx
    {
        #region AttachedFadeStoryboard
        /// <summary>
        /// AttachedFadeStoryboard Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty AttachedFadeStoryboardProperty =
            DependencyProperty.RegisterAttached(
                "AttachedFadeStoryboard",
                typeof(Storyboard),
                typeof(UIElementAnimationEx),
                new PropertyMetadata(null, OnAttachedFadeStoryboardChanged));

        /// <summary>
        /// Gets the AttachedFadeStoryboard property. This dependency property 
        /// indicates the currently running custom fade in/out storyboard.
        /// </summary>
        private static Storyboard GetAttachedFadeStoryboard(DependencyObject d)
        {
            return (Storyboard)d.GetValue(AttachedFadeStoryboardProperty);
        }

        /// <summary>
        /// Sets the AttachedFadeStoryboard property. This dependency property 
        /// indicates the currently running custom fade in/out storyboard.
        /// </summary>
        private static void SetAttachedFadeStoryboard(DependencyObject d, Storyboard value)
        {
            d.SetValue(AttachedFadeStoryboardProperty, value);
        }

        /// <summary>
        /// Handles changes to the AttachedFadeStoryboard property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnAttachedFadeStoryboardChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Storyboard oldAttachedFadeStoryboard = (Storyboard)e.OldValue;
            Storyboard newAttachedFadeStoryboard = (Storyboard)d.GetValue(AttachedFadeStoryboardProperty);
        }
        #endregion

        #region FadeIn()
        /// <summary>
        /// Fades the element in using the FadeInThemeAnimation.
        /// </summary>
        /// <remarks>
        /// Opacity property of the element is not affected.<br/>
        /// The duration of the visible animation itself is not affected by the duration parameter. It merely indicates how long the Storyboard will run.<br/>
        /// If FadeOutThemeAnimation was not used on the element before - nothing will happen.<br/>
        /// </remarks>
        /// <param name="element"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task FadeIn(this UIElement element, TimeSpan? duration = null)
        {
            ((FrameworkElement)element).Visibility = Visibility.Visible;
            var fadeInStoryboard = new Storyboard();
            var fadeInAnimation = new FadeInThemeAnimation();

            if (duration != null)
            {
                fadeInAnimation.Duration = duration.Value;
            }

            Storyboard.SetTarget(fadeInAnimation, element);
            fadeInStoryboard.Children.Add(fadeInAnimation);
            await fadeInStoryboard.BeginAsync();
        }
        #endregion

        #region FadeOut()
        /// <summary>
        /// Fades the element out using the FadeOutThemeAnimation.
        /// </summary>
        /// <remarks>
        /// Opacity property of the element is not affected.<br/>
        /// The duration of the visible animation itself is not affected by the duration parameter. It merely indicates how long the Storyboard will run.<br/>
        /// If FadeOutThemeAnimation was already run before and FadeInThemeAnimation was not run after that - nothing will happen.<br/>
        /// </remarks>
        /// <param name="element"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static async Task FadeOut(this UIElement element, TimeSpan? duration = null)
        {
            var fadeOutStoryboard = new Storyboard();
            var fadeOutAnimation = new FadeOutThemeAnimation();

            if (duration != null)
            {
                fadeOutAnimation.Duration = duration.Value;
            }

            Storyboard.SetTarget(fadeOutAnimation, element);
            fadeOutStoryboard.Children.Add(fadeOutAnimation);
            await fadeOutStoryboard.BeginAsync();
        }
        #endregion

        #region FadeInCustom()
        /// <summary>
        /// Fades the element in using a custom DoubleAnimation of the Opacity property.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"> </param>
        /// <returns></returns>
        public static async Task FadeInCustom(this UIElement element, TimeSpan? duration = null, EasingFunctionBase easingFunction = null, double targetOpacity = 1.0)
        {
            CleanUpPreviousFadeStoryboard(element);

            var fadeInStoryboard = new Storyboard();
            var fadeInAnimation = new DoubleAnimation();

            if (duration == null)
                duration = TimeSpan.FromSeconds(0.4);

            fadeInAnimation.Duration = duration.Value;
            fadeInAnimation.To = targetOpacity;
            fadeInAnimation.EasingFunction = easingFunction;

            Storyboard.SetTarget(fadeInAnimation, element);
            Storyboard.SetTargetProperty(fadeInAnimation, "Opacity");
            fadeInStoryboard.Children.Add(fadeInAnimation);
            SetAttachedFadeStoryboard(element, fadeInStoryboard);

            await fadeInStoryboard.BeginAsync();
            element.Opacity = targetOpacity;
            fadeInStoryboard.Stop();
        }

        public static async Task PopUp(this UIElement element, TimeSpan? duration = null, EasingFunctionBase easingFunction = null, double targetOpacity = 1.0)
        {
            CleanUpPreviousFadeStoryboard(element);

            var fadeInStoryboard = AnimationHelper.PopOutAnimation(element);
            SetAttachedFadeStoryboard(element, fadeInStoryboard);

            await fadeInStoryboard.BeginAsync();
            element.Opacity = targetOpacity;
            fadeInStoryboard.Stop();
        }

        public static async Task Swipe3D(this UIElement element, TimeSpan? duration = null, EasingFunctionBase easingFunction = null, double targetOpacity = 1.0)
        {
            CleanUpPreviousFadeStoryboard(element);

            var fadeInStoryboard = AnimationHelpers.Swipe3D(element as FrameworkElement);
            SetAttachedFadeStoryboard(element, fadeInStoryboard);

            await fadeInStoryboard.BeginAsync();
            element.Opacity = targetOpacity;
            fadeInStoryboard.Stop();
        }
        #endregion

        #region FadeOutCustom()
        /// <summary>
        /// Fades the element out using a custom DoubleAnimation of the Opacity property.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="duration"></param>
        /// <param name="easingFunction"> </param>
        /// <returns></returns>
        public static async Task FadeOutCustom(this UIElement element, TimeSpan? duration = null, EasingFunctionBase easingFunction = null)
        {
            CleanUpPreviousFadeStoryboard(element);

            var fadeOutStoryboard = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation();

            if (duration == null)
                duration = TimeSpan.FromSeconds(0.4);

            fadeOutAnimation.Duration = duration.Value;
            fadeOutAnimation.To = 0.0;
            fadeOutAnimation.EasingFunction = easingFunction;

            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, "Opacity");
            fadeOutStoryboard.Children.Add(fadeOutAnimation);
            SetAttachedFadeStoryboard(element, fadeOutStoryboard);
            await fadeOutStoryboard.BeginAsync();
            element.Opacity = 0.0;
            fadeOutStoryboard.Stop();
        }
        #endregion

        #region CleanUpPreviousFadeStoryboard()
        public static void CleanUpPreviousFadeStoryboard(this UIElement element)
        {
            var attachedFadeStoryboard = GetAttachedFadeStoryboard(element);

            if (attachedFadeStoryboard != null)
            {
                attachedFadeStoryboard.Stop();
            }
        }
        #endregion
    }

    internal static class AnimationHelper
    {
        /// <summary>
        /// Animates the value of a Color property between two target values using linear interpolation over a specified Duration. 
        /// </summary>
        /// <param name="element">The target element</param>
        /// <param name="property">The target property</param>
        /// <param name="toColor">Animation ending color</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static void ColorAnimation(DependencyObject element, string property, Color toColor, double duration)
        {
            Storyboard storyboard = new Storyboard();

            ColorAnimation animation = new ColorAnimation();
            animation.To = toColor;
            animation.Duration = TimeSpan.FromMilliseconds(duration);

            Storyboard.SetTargetProperty(animation, property);

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        /// <summary>
        /// Animates the value of a Color property between two target values using linear interpolation over a specified Duration. 
        /// </summary>
        /// <param name="element">The target element</param>
        /// <param name="property">The target property</param>
        /// <param name="fromColor">Animation starting color</param>
        /// <param name="toColor">Animation ending color</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static void ColorAnimation(DependencyObject element, string property, Color fromColor, Color toColor, double duration)
        {
            Storyboard storyboard = new Storyboard();

            ColorAnimation animation = new ColorAnimation();
            animation.From = fromColor;
            animation.To = toColor;
            animation.Duration = TimeSpan.FromMilliseconds(duration);

            Storyboard.SetTargetProperty(animation, property);

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        /// <summary>
        /// Animates the value of an object property along a single key frame for the specified duration
        /// </summary>
        /// <param name="element">The target element</param>
        /// <param name="property">The target property</param>
        /// <param name="value">Animation value</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static void ObjectAnimationUsingKeyFrames(DependencyObject element, string property, object value, double duration)
        {
            Storyboard storyboard = new Storyboard();

            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();

            DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame();
            keyFrame.Value = value;
            keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration));
            animation.KeyFrames.Add(keyFrame);

            Storyboard.SetTargetProperty(animation, property);

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        /// <summary>
        /// Pointer down animation for a user action that taps an item or element
        /// </summary>
        /// <param name="element">The target element</param>
        public static void PointerDownThemeAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();

            PointerDownThemeAnimation animation = new PointerDownThemeAnimation();

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        /// <summary>
        /// Pointer up animation for a user action that taps an item or element
        /// </summary>
        /// <param name="element">The target element</param>
        public static void PointerUpThemeAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();

            PointerUpThemeAnimation animation = new PointerUpThemeAnimation();

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        /// <summary>
        /// Applies multiple transform operations to an object
        /// </summary>
        /// <param name="element">The target element</param>
        /// <param name="animateTo">Y coordinate</param>
        /// <param name="duration">Animation duration in milliseconds</param>
        public static void CompositeTransformTranslateY(UIElement element, double animateTo, double duration)
        {
            element.RenderTransform = new CompositeTransform();

            Storyboard storyboard = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.To = animateTo;

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            storyboard.Begin();
        }



        public static void PopOutThemeAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();

            PopOutThemeAnimation animation = new PopOutThemeAnimation();

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        public static void PopInThemeAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();

            PopInThemeAnimation animation = new PopInThemeAnimation();

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        public static void FadeOutThemeAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();

            FadeOutThemeAnimation animation = new FadeOutThemeAnimation();

            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);

            storyboard.Begin();
        }

        public static Storyboard PopOutAnimation(DependencyObject element)
        {
            Storyboard storyboard = new Storyboard();
            QuinticEase qe = new QuinticEase();
            qe.EasingMode = EasingMode.EaseOut;
            //透明度
            DoubleAnimation OpacityAnimation = new DoubleAnimation();
            OpacityAnimation.From = 0.2;
            OpacityAnimation.To = 1;
            OpacityAnimation.EasingFunction = qe;
            OpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(OpacityAnimation, element);
            Storyboard.SetTargetProperty(OpacityAnimation, "UIElement.Opacity");
            storyboard.Children.Add(OpacityAnimation);

            ////角度
            //DoubleAnimation AngleAnimation = new DoubleAnimation();
            //AngleAnimation.From = 70;
            //AngleAnimation.To = 0;
            //AngleAnimation.EasingFunction = qe;
            //AngleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.2));
            //Storyboard.SetTarget(AngleAnimation, element);
            //Storyboard.SetTargetProperty(AngleAnimation, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(CompositeTransform.Rotation)");
            //storyboard.Children.Add(AngleAnimation);
            //CompositeTransform e; e.ScaleX; e.ScaleY;e.

            ////X轴
            DoubleAnimation ScaleXAnimation = new DoubleAnimation();
            ScaleXAnimation.From = 0;
            ScaleXAnimation.To = 1;
            ScaleXAnimation.EasingFunction = qe;
            ScaleXAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.2));
            Storyboard.SetTarget(ScaleXAnimation, element);
            Storyboard.SetTargetProperty(ScaleXAnimation, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(CompositeTransform.ScaleX)");
            storyboard.Children.Add(ScaleXAnimation);

            ////Y轴
            DoubleAnimation ScaleYAnimation = new DoubleAnimation();
            ScaleYAnimation.From = 0;
            ScaleYAnimation.To = 1;
            ScaleYAnimation.EasingFunction = qe;
            ScaleYAnimation.Duration = new Duration(TimeSpan.FromSeconds(1.2));
            Storyboard.SetTarget(ScaleYAnimation, element);
            Storyboard.SetTargetProperty(ScaleYAnimation, "(UIElement.RenderTransform).(TransformGroup.Children)[1].(CompositeTransform.ScaleY)");
            storyboard.Children.Add(ScaleYAnimation);

            return storyboard;
        }
    }

    internal static class AnimationHelpers
    {
        public static Storyboard Entrance(IEnumerable<UIElement> targets, double staggerOffset, TimeSpan staggerDuration, TimeSpan slideDuration)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan time = TimeSpan.FromSeconds(0.0);
            using (IEnumerator<UIElement> enumerator = targets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FrameworkElement target = (FrameworkElement)enumerator.Current;
                    target.EnsureCompositeTransform();
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), staggerOffset, null);
                    doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(time, staggerOffset, null);
                    doubleAnimationUsingKeyFrames.AddSplineDoubleKeyFrame(time.Add(slideDuration), 0.0, KeySplines.EntranceTheme);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames, target, TargetProperty.RenderTransformTranslateX);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), 0.0, null);
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(time, 0.0, null);
                    doubleAnimationUsingKeyFrames2.AddSplineDoubleKeyFrame(time.Add(TimeSpan.FromMilliseconds(170.0)), 1.0, KeySplines.EntranceTheme);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames2, target, TargetProperty.Opacity);
                    time = time.Add(staggerDuration);
                }
            }
            return storyboard;
        }
        public static Storyboard PopUp(FrameworkElement target, double YOffset)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan.FromSeconds(0.0);
            target.EnsureCompositeTransform();
            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), YOffset, null);
            doubleAnimationUsingKeyFrames.AddSplineDoubleKeyFrame(TimeSpan.FromMilliseconds(367.0), 0.0, KeySplines.Popup);
            storyboard.AddTimeline(doubleAnimationUsingKeyFrames, target, TargetProperty.RenderTransformTranslateY);
            DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames();
            doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), 0.0, null);
            doubleAnimationUsingKeyFrames2.AddSplineDoubleKeyFrame(TimeSpan.FromMilliseconds(83.0), 1.0, KeySplines.EntranceTheme);
            storyboard.AddTimeline(doubleAnimationUsingKeyFrames2, target, TargetProperty.Opacity);
            return storyboard;
        }

        public static Storyboard Swipe3D(FrameworkElement e, double fromX = 678.0)
        {
            return AnimationHelpers.Swipe3DList(new List<UIElement>
            {
                e
            }, fromX, -80.0, -558.0);
        }

        public static Storyboard Swipe3DListReturn(IEnumerable<DependencyObject> l, double fromX = 678.0, double projY = 80.0, double offZ = 300.0, TimeSpan ts = new TimeSpan(), bool isModify = false)
        {
            return AnimationHelpers.Swipe3DList(l, fromX, projY, offZ, ts, isModify);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * 2.0 * 3.1415926535897931 / 360.0;
        }

        public static Storyboard Swipe3DList(IEnumerable<DependencyObject> l, double fromX = 678.0, double projY = -80.0, double offZ = -558.0, TimeSpan ts = new TimeSpan(), bool isModify = false)
        {
            Storyboard storyboard = new Storyboard();
            KeySpline arg_0B_0 = KeySplines.EntranceTheme;
            TimeSpan time = TimeSpan.FromSeconds(0.0);
            if (!isModify)
                ts = TimeSpan.FromMilliseconds(80.0);
            using (IEnumerator<DependencyObject> enumerator = l.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FrameworkElement frameworkElement = (FrameworkElement)enumerator.Current;
                    double num = Window.Current.Bounds.Width - 200.0;
                    bool flag = frameworkElement.ActualWidth > num;
                    double num2 = flag ? -120.0 : projY;
                    double num3 = offZ;
                    if (flag)
                    {
                        num3 = -1.0 * (Math.Tan(AnimationHelpers.DegreesToRadians(num2)) * frameworkElement.ActualWidth);
                        num3 += frameworkElement.ActualWidth;
                    }
                    double value = num3;
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), fromX, null);
                    doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(time, fromX, null);
                    doubleAnimationUsingKeyFrames.AddSplineDoubleKeyFrame(time.Add(TimeSpan.FromSeconds(1.5)), 0.0, KeySplines.EntranceTheme);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames, frameworkElement, TargetProperty.RenderTransformTranslateX);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), num2, null);
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(time, num2, null);
                    doubleAnimationUsingKeyFrames2.AddSplineDoubleKeyFrame(time.Add(TimeSpan.FromSeconds(1.0)), 0.0, KeySplines.EntranceTheme);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames2, frameworkElement, TargetProperty.ProjectionRotationY);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames3 = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames3.AddEasingDoubleKeyFrame(TimeSpan.FromSeconds(0.0), value, null);
                    doubleAnimationUsingKeyFrames3.AddEasingDoubleKeyFrame(time, value, null);
                    doubleAnimationUsingKeyFrames3.AddSplineDoubleKeyFrame(time.Add(TimeSpan.FromSeconds(1.0)), 0.0, KeySplines.EntranceTheme);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames3, frameworkElement, TargetProperty.ProjectionGlobalOffsetZ);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames4 = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames4.AddEasingDoubleKeyFrame(TimeSpan.FromMilliseconds(0.0), 0.0, null);
                    doubleAnimationUsingKeyFrames4.AddEasingDoubleKeyFrame(time, 0.0, null);
                    doubleAnimationUsingKeyFrames4.AddEasingDoubleKeyFrame(time.Add(TimeSpan.FromMilliseconds(1.0)), 1.0, null);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames4, frameworkElement, TargetProperty.Opacity);
                    time = time.Add(ts);
                }
            }
            return storyboard;
        }

        public static Storyboard SlideOutList(IEnumerable<DependencyObject> l, bool slideLeft = true, double xtranslation = 1000.0, TimeSpan ts = new TimeSpan(), bool isModify = false)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan time = TimeSpan.FromMilliseconds(0.0);
            if (!isModify) ts = TimeSpan.FromMilliseconds(75.0);
            TimeSpan ts2 = TimeSpan.FromMilliseconds(500.0);
            TimeSpan ts3 = TimeSpan.FromMilliseconds(125.0);
            double num = slideLeft ? (-xtranslation) : xtranslation;
            CubicEase cubicEase = new CubicEase();
            cubicEase.EasingMode = EasingMode.EaseIn;
            EasingFunctionBase ease = cubicEase;
            using (IEnumerator<DependencyObject> enumerator = l.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FrameworkElement target = (FrameworkElement)enumerator.Current;
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames.AddEasingDoubleKeyFrame(time.Add(ts2), num, ease);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames, target, TargetProperty.RenderTransformTranslateX);
                    DoubleAnimationUsingKeyFrames doubleAnimationUsingKeyFrames2 = new DoubleAnimationUsingKeyFrames();
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(time, 1.0, null);
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(time.Add(ts2).Subtract(ts3), 1.0, null);
                    doubleAnimationUsingKeyFrames2.AddEasingDoubleKeyFrame(time.Add(ts2), 0.0, null);
                    storyboard.AddTimeline(doubleAnimationUsingKeyFrames2, target, TargetProperty.Opacity);
                    time = time.Add(ts);
                    num += -200.0;
                }
            }
            return storyboard;
        }
    }

    internal static class Extensions
    {
        public static void ApplyStoryboardOptions(this Storyboard storyboard, StoryboardOptions options)
        {
            if (options != null)
            {
                storyboard.AutoReverse = options.AutoReverse.HasValue && options.AutoReverse.Value;
                storyboard.SpeedRatio = options.SpeedRatio.HasValue ? options.SpeedRatio.Value : 1.0;
                if (options.RepeatForever.HasValue && options.RepeatForever.Value)
                {
                    storyboard.RepeatBehavior = (new RepeatBehavior
                    {
                        Type = RepeatBehaviorType.Forever
                    });
                    return;
                }
                if (options.RepeatCount.HasValue)
                {
                    storyboard.RepeatBehavior = (new RepeatBehavior
                    {
                        Count = (double)options.RepeatCount.Value,
                        Type = RepeatBehaviorType.Count
                    });
                }
            }
        }

        public static void ResetTarget(this Storyboard storyboard, FrameworkElement target)
        {
            if (storyboard != null && storyboard.Children != null)
            {
                storyboard.Stop();
                foreach (var item in storyboard.Children)
                {
                    Storyboard.SetTarget(item, target);
                    Storyboard.SetTargetName(item, target.Name);
                    Storyboard.SetTargetProperty(item, Storyboard.GetTargetProperty(item));
                }
            }
        }

        public static void SetDuration(this Storyboard storyboard, double duration)
        {
            var timeSpan = TimeSpan.FromSeconds(duration);
            foreach (var item in storyboard.Children) item.Duration = timeSpan;
        }

        public static string AddToWindowResources(this Storyboard storyboard, string resourceName)
        {
            FrameworkElement frameworkElement = Window.Current.Content as FrameworkElement;
            string text = Base.GenerateUniqueID() + resourceName;
            frameworkElement.Resources.Add(text, storyboard);
            return text;
        }
        public static void AddDoubleAnimation(this Storyboard storyboard, DoubleAnimation doubleAnimation, string targetProperty)
        {
            storyboard.AddDoubleAnimation(doubleAnimation, null, targetProperty);
        }
        public static void AddDoubleAnimation(this Storyboard storyboard, DoubleAnimation doubleAnimation, FrameworkElement target, string targetProperty)
        {
            if (target != null)
            {
                Storyboard.SetTarget(doubleAnimation, target);
                Storyboard.SetTargetName(doubleAnimation, target.Name);
            }
            Storyboard.SetTargetProperty(doubleAnimation, targetProperty);
            storyboard.Children.Add(doubleAnimation);
        }
        public static void AddNewDoubleAnimation(this Storyboard storyboard, string targetProperty, double? _from, double? _to, TimeSpan? beginTime, TimeSpan _duration, EasingFunctionBase _easingMode)
        {
            storyboard.AddNewDoubleAnimation(null, targetProperty, _from, _to, beginTime, _duration, _easingMode);
        }
        public static void AddNewDoubleAnimation(this Storyboard storyboard, FrameworkElement target, string targetProperty, double? _from, double? _to, TimeSpan? beginTime, TimeSpan _duration, EasingFunctionBase _easingMode)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = (_from);
            doubleAnimation.To = (_to);
            doubleAnimation.Duration = (new Duration(_duration));
            doubleAnimation.EasingFunction = (_easingMode);
            doubleAnimation.BeginTime = (beginTime);
            DoubleAnimation doubleAnimation2 = doubleAnimation;
            if (target != null)
            {
                target.EnsureCompositeTransform();
                storyboard.AddDoubleAnimation(doubleAnimation2, target, targetProperty);
            }
            else storyboard.AddDoubleAnimation(doubleAnimation2, targetProperty);
        }
        public static void ClearLocalStoryboards(this FrameworkElement _target)
        {
            ResourceDictionary resources = _target.Resources;
            List<object> list = new List<object>();
            foreach (KeyValuePair<object, object> current in resources)
            {
                if (current.Value.ToString() == "Windows.UI.Xaml.Media.Animation.Storyboard")
                {
                    Storyboard storyboard = _target.Resources[current.Key] as Storyboard;
                    storyboard.Stop();
                    list.Add(current.Key);
                }
            }
            foreach (object current2 in list)
            {
                _target.Resources.Remove(current2);
            }
        }
        public static void SetCompositeTransformCentre(this FrameworkElement _target, double X, double Y)
        {
            CompositeTransform compositeTransform = _target.RenderTransform as CompositeTransform;
            if (compositeTransform == null)
            {
                _target.EnsureCompositeTransform();
                compositeTransform = (_target.RenderTransform as CompositeTransform);
            }
            compositeTransform.CenterX = (X);
            compositeTransform.CenterY = (Y);
            _target.RenderTransformOrigin = (new Point(X, Y));
        }
        public static void EnsureCompositeTransform(this FrameworkElement _target)
        {
            if (!(_target.RenderTransform is CompositeTransform))
            {
                TranslateTransform translateTransform = _target.RenderTransform as TranslateTransform;
                if (translateTransform != null)
                {
                    double x = translateTransform.X;
                    double y = translateTransform.Y;
                    CompositeTransform compositeTransform = new CompositeTransform();
                    _target.RenderTransform = (compositeTransform);
                    compositeTransform.TranslateX = (x);
                    compositeTransform.TranslateY = (y);
                }
                else
                {
                    _target.RenderTransform = (new CompositeTransform());
                }
                _target.SetCompositeTransformCentre(0.5, 0.5);
            }
        }
        public static void EnsurePlaneProjection(this FrameworkElement _target)
        {
            Base.GetPlaneProjection(_target);
        }
        public static Color? ConvertHexColorToColor(string color)
        {
            if (color.Length == 9)
            {
                byte a = Convert.ToByte(color.Substring(1, 2), 16);
                byte r = Convert.ToByte(color.Substring(3, 2), 16);
                byte g = Convert.ToByte(color.Substring(5, 2), 16);
                byte b = Convert.ToByte(color.Substring(7, 2), 16);
                return new Color?(Color.FromArgb(a, r, g, b));
            }
            if (color.Length == 7)
            {
                byte r = Convert.ToByte(color.Substring(1, 2), 16);
                byte g = Convert.ToByte(color.Substring(3, 2), 16);
                byte b = Convert.ToByte(color.Substring(5, 2), 16);
                return new Color?(Color.FromArgb(255, r, g, b));
            }
            return null;
        }
        public static void SetPoints(this KeySpline keySpline, double x1, double y1, double x2, double y2)
        {
            keySpline.ControlPoint1 = (new Point(x1, y1));
            keySpline.ControlPoint2 = (new Point(x2, y2));
        }
    }

    internal class StoryboardOptions
    {
        public bool? AutoReverse
        {
            get;
            set;
        }
        public int? RepeatCount
        {
            get;
            set;
        }
        public bool? RepeatForever
        {
            get;
            set;
        }
        public double? SpeedRatio
        {
            get;
            set;
        }
    }

    internal static class Keyframes
    {
        public static void AddTimeline(this Storyboard storyboard, Timeline timeline, FrameworkElement target, string targetProperty)
        {
            if (targetProperty == TargetProperty.RenderTransformRotation || targetProperty == TargetProperty.RenderTransformScaleX || targetProperty == TargetProperty.RenderTransformScaleY || targetProperty == TargetProperty.RenderTransformSkewX || targetProperty == TargetProperty.RenderTransformSkewY || targetProperty == TargetProperty.RenderTransformTranslateX || targetProperty == TargetProperty.RenderTransformTranslateY)
            {
                Base.GetCompositeTransform(target);
            }
            else
            {
                if (targetProperty == TargetProperty.ProjectionGlobalOffsetX || targetProperty == TargetProperty.ProjectionGlobalOffsetY || targetProperty == TargetProperty.ProjectionGlobalOffsetZ || targetProperty == TargetProperty.ProjectionRotationX || targetProperty == TargetProperty.ProjectionRotationY || targetProperty == TargetProperty.ProjectionRotationZ)
                {
                    Base.GetPlaneProjection(target);
                }
            }
            Storyboard.SetTarget(timeline, target);
            Storyboard.SetTargetName(timeline, target.Name);
            Storyboard.SetTargetProperty(timeline, targetProperty);
            storyboard.Children.Add(timeline);
        }
        public static void AddEasingDoubleKeyFrame(this DoubleAnimationUsingKeyFrames doubleAnimation, TimeSpan time, double value, EasingFunctionBase ease = null)
        {
            ICollection<DoubleKeyFrame> arg_27_0 = doubleAnimation.KeyFrames;
            EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
            easingDoubleKeyFrame.KeyTime = (KeyTime.FromTimeSpan(time));
            easingDoubleKeyFrame.Value = (value);
            easingDoubleKeyFrame.EasingFunction = (ease);
            arg_27_0.Add(easingDoubleKeyFrame);
        }
        public static void AddEasingColorKeyFrame(this ColorAnimationUsingKeyFrames colorAnimation, TimeSpan time, Color value, EasingFunctionBase ease = null)
        {
            ICollection<ColorKeyFrame> arg_27_0 = colorAnimation.KeyFrames;
            EasingColorKeyFrame easingColorKeyFrame = new EasingColorKeyFrame();
            easingColorKeyFrame.KeyTime = (KeyTime.FromTimeSpan(time));
            easingColorKeyFrame.Value = (value);
            easingColorKeyFrame.EasingFunction = (ease);
            arg_27_0.Add(easingColorKeyFrame);
        }
        public static void AddSplineDoubleKeyFrame(this DoubleAnimationUsingKeyFrames doubleAnimation, TimeSpan time, double value, KeySpline spline = null)
        {
            ICollection<DoubleKeyFrame> arg_27_0 = doubleAnimation.KeyFrames;
            SplineDoubleKeyFrame splineDoubleKeyFrame = new SplineDoubleKeyFrame();
            splineDoubleKeyFrame.KeyTime = (KeyTime.FromTimeSpan(time));
            splineDoubleKeyFrame.Value = (value);
            splineDoubleKeyFrame.KeySpline = (spline);
            arg_27_0.Add(splineDoubleKeyFrame);
        }
        public static void AddDiscreteObjectKeyFrame(this ObjectAnimationUsingKeyFrames objectAnimation, TimeSpan time, object value)
        {
            ICollection<ObjectKeyFrame> arg_20_0 = objectAnimation.KeyFrames;
            DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame();
            discreteObjectKeyFrame.KeyTime = (KeyTime.FromTimeSpan(time));
            discreteObjectKeyFrame.Value = (value);
            arg_20_0.Add(discreteObjectKeyFrame);
        }
    }

    internal static class KeySplines
    {
        public static KeySpline EntranceTheme
        {
            get
            {
                KeySpline keySpline = new KeySpline();
                keySpline.SetPoints(0.1, 0.9, 0.2, 1.0);
                return keySpline;
            }
        }
        public static KeySpline Popup
        {
            get
            {
                KeySpline keySpline = new KeySpline();
                keySpline.ControlPoint1 = (new Point(0.100000001490116, 0.899999976158142));
                keySpline.ControlPoint1 = (new Point(0.200000002980232, 1.0));
                return keySpline;
            }
        }
    }

    internal static class TargetProperty
    {
        public static string RenderTransformTranslateX = "(UIElement.RenderTransform).(CompositeTransform.TranslateX)";
        public static string RenderTransformTranslateY = "(UIElement.RenderTransform).(CompositeTransform.TranslateY)";
        public static string RenderTransformScaleX = "(UIElement.RenderTransform).(CompositeTransform.ScaleX)";
        public static string RenderTransformScaleY = "(UIElement.RenderTransform).(CompositeTransform.ScaleY)";
        public static string RenderTransformSkewX = "(UIElement.RenderTransform).(CompositeTransform.SkewX)";
        public static string RenderTransformSkewY = "(UIElement.RenderTransform).(CompositeTransform.SkewY)";
        public static string RenderTransformRotation = "(UIElement.RenderTransform).(CompositeTransform.Rotation)";
        public static string Opacity = "(UIElement.Opacity)";
        public static string Visiblity = "(UIElement.Visibility)";
        public static string ProjectionRotationX = "(UIElement.Projection).(PlaneProjection.RotationX)";
        public static string ProjectionRotationY = "(UIElement.Projection).(PlaneProjection.RotationY)";
        public static string ProjectionRotationZ = "(UIElement.Projection).(PlaneProjection.RotationZ)";
        public static string ProjectionGlobalOffsetX = "(UIElement.Projection).(PlaneProjection.GlobalOffsetX)";
        public static string ProjectionGlobalOffsetY = "(UIElement.Projection).(PlaneProjection.GlobalOffsetY)";
        public static string ProjectionGlobalOffsetZ = "(UIElement.Projection).(PlaneProjection.GlobalOffsetZ)";
        public static string ShapeFillSolidColorBrushColor = "(Shape.Fill).(SolidColorBrush.Color)";
    }

    internal static class Base
    {
        public enum TranslateAxis
        {
            X,
            Y
        }
        internal static Random r = new Random();
        internal static List<StoryboardStorage> Storage = new List<StoryboardStorage>();
        public static string GenerateUniqueID()
        {
            return Base.r.NextDouble().ToString() + Base.r.NextDouble().ToString();
        }
        public static void RegisterToWindow(Storyboard storyboard)
        {
        }
        internal static void GetCompositeTransform(FrameworkElement _target)
        {
            _target.EnsureCompositeTransform();
        }
        internal static void GetPlaneProjection(UIElement target)
        {
            if (target.Projection == null)
            {
                target.Projection = new PlaneProjection();
            }
            if (!(target.Projection is PlaneProjection))
            {
                target.Projection = new PlaneProjection();
            }
        }
        public static void SetCompositeTransformCentre(FrameworkElement _target, double X, double Y)
        {
            Base.GetCompositeTransform(_target);
            CompositeTransform compositeTransform = (CompositeTransform)_target.RenderTransform;
            compositeTransform.CenterX = X;
            compositeTransform.CenterY = Y;
            _target.RenderTransformOrigin = (new Point(X, Y));
        }
        internal static void ApplyStoryboardOptions(Storyboard _sb, StoryboardOptions _options)
        {
            _sb.ApplyStoryboardOptions(_options);
        }
        public static void RemoveLocalStoryboards(FrameworkElement _target)
        {
            _target.ClearLocalStoryboards();
        }
        internal static void sb_Completed(object sender, object e)
        {
            Storyboard storyboard = sender as Storyboard;
            storyboard.Completed -= Base.sb_Completed;
            Base.Unregister(storyboard);
        }
        public static void Register(FrameworkElement _target, Storyboard _sb, string _key)
        {
            StoryboardStorage item = new StoryboardStorage
            {
                target = _target,
                sb = _sb,
                Key = _key
            };
            Base.Storage.Add(item);
            _sb.Completed += Base.sb_Completed;
        }
        public static string RegisterToWindow(this FrameworkElement target, Storyboard storyboard, string partialKey)
        {
            string text = storyboard.AddToWindowResources(partialKey);
            StoryboardStorage item = new StoryboardStorage
            {
                target = target,
                sb = storyboard,
                Key = text
            };
            Base.Storage.Add(item);
            storyboard.Completed += Base.sb_Completed;
            return text;
        }
        internal static void Unregister(Storyboard in_story)
        {
            StoryboardStorage s = null;
            foreach (StoryboardStorage current in
                from st in Base.Storage
                where st.sb == in_story
                select st)
            {
                s = current;
            }
            if (s != null)
            {
                ResourceDictionary resources = s.target.Resources;
                object obj = (
                    from r in resources
                    where r.Key.ToString() == s.Key
                    select r.Key).FirstOrDefault<object>();
                if (obj != null)
                {
                    s.target.Resources.Remove(obj);
                }
            }
            in_story = null;
            s.sb = null;
            if (s != null)
            {
                Base.Storage.Remove(s);
            }
            s.target = null;
            s.Key = null;
            s = null;
        }
    }

    internal class StoryboardStorage
    {
        public FrameworkElement target
        {
            get;
            set;
        }
        public string Key
        {
            get;
            set;
        }
        public Storyboard sb
        {
            get;
            set;
        }
    }
    #endregion
}
