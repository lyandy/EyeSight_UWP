//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       TranslateTransformEx
//创建时间:     2015/10/16 20:49:59
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace EyeSight.Extension.DependencyObjectEx
{
    public static class TranslateTransformEx
    {
        public static void AnimateX(this DependencyObject target, double from, double to, int duration, int startTime, EasingFunctionBase easing = null, Action completed = null)
        {
            if (easing == null)
                easing = new SineEase();

            var db = new DoubleAnimation();
            db.To = to;
            db.From = from;
            db.EasingFunction = easing;
            db.Duration = TimeSpan.FromMilliseconds(duration);

            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, "X");

            var sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(startTime);

            if (completed != null)
            {
                sb.Completed += (s, e) => completed();
            }

            sb.Children.Add(db);
            sb.Begin();
        }

        public static void AnimateY(this DependencyObject target, double from, double to, int duration, int startTime, EasingFunctionBase easing = null, Action completed = null)
        {
            if (easing == null)
                easing = new SineEase();

            var db = new DoubleAnimation();
            db.To = to;
            db.From = from;
            db.EasingFunction = easing;
            db.Duration = TimeSpan.FromMilliseconds(duration);

            Storyboard.SetTarget(db, target);
            Storyboard.SetTargetProperty(db, "Y");

            var sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(startTime);

            if (completed != null)
            {
                sb.Completed += (s, e) => completed();
            }

            sb.Children.Add(db);
            sb.Begin();
        }
    }
}
