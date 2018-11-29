using EyeSight.Extension.DependencyObjectEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls
{
    public sealed partial class WPToastUIControl : UserControl
    {
        public WPToastUIControl()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(WPToastUIControl), new PropertyMetadata(null));

        public async Task HidMsg()
        {
            //sb.Begin();
            Storyboard sb = new Storyboard();

            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 1;
            anim.To = 0;
            anim.Duration = TimeSpan.FromMilliseconds(300);

            CircleEase ease = new CircleEase();
            ease.EasingMode = EasingMode.EaseOut;
            anim.EasingFunction = ease;

            Storyboard.SetTarget(anim, st);
            Storyboard.SetTargetProperty(anim, "ScaleY");
            sb.Children.Add(anim);
            await sb.BeginAsync();
        }
    }
}
