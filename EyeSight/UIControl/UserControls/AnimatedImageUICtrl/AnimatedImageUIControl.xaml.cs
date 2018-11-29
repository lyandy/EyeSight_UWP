using EyeSight.Async;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.AnimatedImageUICtrl
{
    //INotifyPropertyChanged
    public sealed partial class AnimatedImageUIControl : UserControl
    {
        public AnimatedImageUIControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.BackgroundImage.Source = DicStore.GetValueOrDefault<BitmapImage>(AppCommonConst.SPLASH_BITMAPIMAGE, null);
        }

        #region ImageUrl
        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register("ImageUrl", typeof(string), typeof(AnimatedImageUIControl), new PropertyMetadata(string.Empty));
        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }
        #endregion
    }
}
