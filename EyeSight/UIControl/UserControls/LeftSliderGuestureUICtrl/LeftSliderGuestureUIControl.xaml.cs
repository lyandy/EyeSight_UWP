using EyeSight.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.LeftSliderGuestureUICtrl
{
    public sealed partial class LeftSliderGuestureUIControl : UserControl
    {
        public LeftSliderGuestureUIControl()
        {
            this.InitializeComponent();

            this.Width = 30;
            this.Height = Window.Current.Bounds.Height - 48;

            this.Loaded += (ss, ee) =>
            {
                this.grid.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateInertia | ManipulationModes.System;
                this.grid.ManipulationStarted += grid_ManipulationStarted;
                this.grid.ManipulationDelta += grid_ManipulationDelta;
            };

            this.Unloaded += (ss, ee) =>
            {
                this.grid.ManipulationMode = ManipulationModes.None;
                this.grid.ManipulationStarted -= grid_ManipulationStarted;
                this.grid.ManipulationDelta -= grid_ManipulationDelta;

            };
        }

        Point start;
        private void grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                int threshold = -40;

                Debug.WriteLine(start.X);

                if (start.X - e.Position.X < threshold) //swipe left
                {
                    e.Complete();

                    Debug.WriteLine("我被触发了");

                    NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;
                    if (rootPage != null)
                    {
                        rootPage.splitViewToggleButton_Click(null, null);
                    }
                }
            }
        }

        private void grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            start = e.Position;
        }
    }
}
