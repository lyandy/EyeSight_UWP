using Brain.Animate;
using EyeSight.Config;
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

namespace EyeSight.UIControl.UserControls.WinToastUICtrl
{
    public sealed partial class WinToastUIControl : UserControl
    {
        public WinToastUIControl()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        #region Message 依赖属性
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(WinToastUIControl), new PropertyMetadata(null));
        #endregion

        #region IsFavorite 依赖属性
        public bool IsFavorite
        {
            get { return (bool)GetValue(IsFavoriteProperty); }
            set { SetValue(IsFavoriteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFavoriteProperty =
            DependencyProperty.Register("IsFavorite", typeof(bool), typeof(WinToastUIControl), new PropertyMetadata(false));
        #endregion

        public async Task HidMsg()
        {
            await rootGrid.MoveToAsync(0.3, new Point(0, 0), new CubicEase { EasingMode = EasingMode.EaseIn });
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            await grid.MoveToAsync(0.3, new Point(AppEnvironment.IsPhone ? - 150 : -180, 0), new CubicEase { EasingMode = EasingMode.EaseOut });
        }
    }
}
