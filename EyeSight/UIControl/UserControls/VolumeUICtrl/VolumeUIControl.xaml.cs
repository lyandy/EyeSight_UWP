using Brain.Animate;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.Locator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace EyeSight.UIControl.UserControls.VolumeUICtrl
{
    public sealed partial class VolumeUIControl : UserControl
    {
        public VolumeUIControl()
        {
            this.InitializeComponent();

            this.Loaded += async (ss, ee) =>
            {
                if (vm != null)
                {
                    var me = vm.MediaPlayer;
                    if (me != null)
                    {
                        volumeAppBarBtn.Icon = me.Volume == 0 ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);

                        this.volumeSlider.Value = me.Volume * 100;

                        if (AppEnvironment.IsPhone)
                        {
                            VisualStateManager.GoToState(this, "VolumeVertical", true);
                        }

                        if (AppEnvironment.IsPhone)
                        {
                            await this.grid.AnimateAsync(new FadeInAnimation());
                        }
                        else
                        {
                            await this.grid.AnimateAsync(new BounceInAnimation());
                        }

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
                    }
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

        public async Task DisposeControl()
        {
            if (AppEnvironment.IsPhone)
            {
                await this.grid.AnimateAsync(new FadeOutAnimation());
            }
            else
            {
                await this.grid.AnimateAsync(new BounceOutAnimation());
            }
        }

        public Microsoft.PlayerFramework.InteractiveViewModel _vm;
        public Microsoft.PlayerFramework.InteractiveViewModel vm
        {
            get
            {
                return _vm;
            }
            set
            {
                _vm = value;

                if (!AppEnvironment.IsPhone)
                {
                    this.grid.Background = new SolidColorBrush(value.IsFullScreen ? Color.FromArgb(150, 255, 255, 255) : Color.FromArgb(200, 255, 255, 255));

                }
            }

        }

        DispatcherTimer countTimer = null;
        int count = 0;

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            count = 0;

            if (vm != null)
            {
                var me = vm.MediaPlayer;
                if (me != null)
                {
                    me.Volume = this.volumeSlider.Value / 100;

                    SettingsStore.AddOrUpdateValue<double>(AppCommonConst.CURRETN_VIDEO_VOLUME_VALUE, me.Volume);

                    ViewModelLocator.Instance.VideoVolumeToMute(me.Volume);

                    volumeAppBarBtn.Icon = me.Volume == 0 ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);
                }
            }
        }

        private void volumeAppBarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vm != null)
            {
                var me = vm.MediaPlayer;
                if (me != null)
                {
                    if (me.Volume == 0)
                    {
                        this.volumeSlider.Value = 50;
                    }
                    else
                    {
                        this.volumeSlider.Value = 0;
                    }

                    volumeAppBarBtn.Icon = me.Volume == 0 ? new SymbolIcon(Symbol.Mute) : new SymbolIcon(Symbol.Volume);
                }
            }
        }
    }
}
