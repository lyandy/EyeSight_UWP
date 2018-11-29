using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.VideoSolutionUICtrl
{
    public sealed partial class VideoSolutionUIControl : UserControl
    {
        public VideoSolutionUIControl()
        {
            this.InitializeComponent();
        }

        public Microsoft.PlayerFramework.InteractiveViewModel vm
        {
            set
            {
                this.grid.Background = new SolidColorBrush(AppEnvironment.IsPhone ? Color.FromArgb(55, 0, 0, 0) : (value.IsFullScreen ? Color.FromArgb(55, 0, 0, 0): Colors.White));
                (this.Resources["videoSolutionTbKey"] as SolidColorBrush).Color = AppEnvironment.IsPhone ? Colors.White : (value.IsFullScreen ? Colors.White : Colors.Black);
            }
        }

        public List<Playinfo> PlayInfoList
        {
            set
            {
                this.solutionListView.ItemsSource = value;

                this.solutionListView.SelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, 0);

                this.Loaded += async (ss, ee) =>
                {

                    //await this.grid.MoveToAsync(0.3,new Point(0, 132), new CubicEase { EasingMode = EasingMode.EaseOut });

                    await this.grid.AnimateAsync(new FadeInDownAnimation());

                    if (countTimer != null)
                    {
                        countTimer.Stop();
                        countTimer = null;
                    }

                    if (countTimer == null)
                    {
                        countTimer = new DispatcherTimer();
                        countTimer.Interval = TimeSpan.FromMilliseconds(100);
                        countTimer.Tick += (sss, eee) =>
                        {
                            if (count == 15)
                            {
                                var pop = this.Parent as Popup;
                                if (pop != null)
                                {
                                    //await DisposeControl();

                                    pop.IsOpen = false;
                                    pop.Child = null;
                                    pop = null;
                                }
                            }
                            else
                            {
                                count++;
                            }
                        };
                        countTimer.Start();
                    }
                };

                this.Unloaded += (ss, ee) =>
                {
                    if (countTimer != null)
                    {
                        countTimer.Stop();
                        countTimer = null;
                    }
                };
            }
        }

        DispatcherTimer countTimer = null;
        int count = 0;

        private void solutionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.solutionListView.SelectedIndex != -1)
            {
                DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, this.solutionListView.SelectedIndex); 
            }
        }

        int previousSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, 0);

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int currentSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_VIDEO_SOLUTION_SELECTED_INDEX, 0);

            var pop = this.Parent as Popup;
            if (pop != null)
            {
                pop.IsOpen = false;
                pop.Child = null;
                pop = null;
            }

            if (previousSelectedIndex != currentSelectedIndex)
            {
                var grid = sender as Grid;
                if (grid != null)
                {
                    var model = grid.DataContext as Playinfo;
                    if (model != null)
                    {
                        mp = CommonHelper.Instance.GetCurrentPlayerFramework();

                        if (mp != null)
                        {
                            timespan = mp.Duration - mp.TimeRemaining;

                            mp.MediaQualityChanged += Mp_MediaQualityChanged;

                            mp.Source = new Uri(await CommonHelper.Instance.PlayUrlConverte(model.url), UriKind.RelativeOrAbsolute);
                        }
                    }
                }
            }
        }

        Microsoft.PlayerFramework.MediaPlayer mp;
        TimeSpan timespan = TimeSpan.Zero;

        private void Mp_MediaQualityChanged(object sender, RoutedEventArgs e)
        {
            mp.MediaQualityChanged -= Mp_MediaQualityChanged;
            var szPC = CommonHelper.Instance.GetCurrentSemanticZoom("szPC");
            if (szPC != null && szPC.IsZoomedInViewActive == false && mp != null)
            {
                mp.StartupPosition = timespan;
            }
        }
    }
}
