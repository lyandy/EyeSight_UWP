using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ifeng.Ctrls
{
    public partial class ExitTips : UserControl
    {
        public ExitTips()
        {
            InitializeComponent();
            grid.Width = App.Current.Host.Content.ActualWidth;
            grid.Height = App.Current.Host.Content.ActualHeight;
            this.Loaded += ExitTips_Loaded;
        }

        async void ExitTips_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Loaded -= ExitTips_Loaded;
                rect.Fill = ((SolidColorBrush)Application.Current.Resources["PhoneForegroundBrush"]);
                gs1.Color = (Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color;
                double from = 0.1;
                while (from < 1)
                {
                    gs1.Offset = gs2.Offset = from;
                    from += 0.02;
                    await Task.Delay(10);
                }

                sb.Begin();
                await Task.Delay(100);
                var popup = this.Parent as Popup;
                if (popup != null)
                {
                    popup.IsOpen = false;
                }
            }
            catch { }
        }
    }
}
