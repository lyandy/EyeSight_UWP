using Brain.Animate;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.Model.Daily;
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
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace EyeSight.View.Setting
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private NavigationHelper navigationHelper;
        public SettingPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);

            if (AppEnvironment.IsPhone)
            {
                this.sureGrid.Visibility = Visibility.Visible;
                this.contentGrid.Width = Window.Current.Bounds.Width;
            }
            else
            {
                this.contentGrid.Width = 450;
            }

            this.autoPlayToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTOPLAY_TOGGLLESWITCH_ON, true);
            this.autoBackToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTOBACK_TOGGLLESWITCH_ON, true);
            this.hightQualityToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_HIGHQUALITY_TOGGLLESWITCH_ON, true);
            this.downloadToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_HIGHT_QUALITY_VIDEO, true);
            this.downloadWhenFavoriteToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_WHEN_FAVORITE_VIDEO, false);
            this.tileToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_TILE_ACTIVE, true);
            this.sureToggle.IsOn = SettingsStore.GetValueOrDefault<bool>(AppCommonConst.IS_SURE_TOGGLLESWITCH_ON, true);

            this.Loaded += (s, e) =>
            {
                this.autoPlayToggle.Toggled += ToggleSwitch_Toggled;
                this.autoBackToggle.Toggled += ToggleSwitch_Toggled;
                this.hightQualityToggle.Toggled += ToggleSwitch_Toggled;
                this.downloadToggle.Toggled += ToggleSwitch_Toggled;
                this.downloadWhenFavoriteToggle.Toggled += ToggleSwitch_Toggled;
                this.tileToggle.Toggled += ToggleSwitch_Toggled;
                this.sureToggle.Toggled += ToggleSwitch_Toggled;
            };

            this.Unloaded += (s, e) =>
            {
                this.autoPlayToggle.Toggled -= ToggleSwitch_Toggled;
                this.autoBackToggle.Toggled -= ToggleSwitch_Toggled;
                this.hightQualityToggle.Toggled -= ToggleSwitch_Toggled;
                this.downloadToggle.Toggled -= ToggleSwitch_Toggled;
                this.downloadWhenFavoriteToggle.Toggled -= ToggleSwitch_Toggled;
                this.tileToggle.Toggled -= ToggleSwitch_Toggled;
                this.sureToggle.Toggled -= ToggleSwitch_Toggled;
            };
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            this.autoPlayGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
            await Task.Delay(30);
            this.autoBackGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
            await Task.Delay(30);
            this.hightQualityGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
            await Task.Delay(30);
            this.downloadGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
            await Task.Delay(30);
            this.downloadWhenFavoriteGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
            await Task.Delay(30);
            this.tileGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });

            if (AppEnvironment.IsPhone)
            {
                await Task.Delay(30);
                this.sureGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });
                await Task.Delay(30);
                this.clearCacheGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });

                await Task.Delay(50);
                this.autoBackLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.hightQualityLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.downloadLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.downloadWhenFavoriteLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.tileLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.sureLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
            }
            else
            {
                await Task.Delay(30);
                this.clearCacheGrid.AnimateAsync(new FadeInDownAnimation() { Duration = 0.2 });

                await Task.Delay(50);
                this.autoBackLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.hightQualityLineGrid.AnimateAsync(new FadeInLeftAnimation() { Distance = 600, Duration = 0.2 });
                await Task.Delay(50);
                this.downloadLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.downloadWhenFavoriteLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
                await Task.Delay(50);
                this.tileLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
            }

            await Task.Delay(30);
            this.clearCacheLineGrid.AnimateAsync(new FadeInLeftAnimation() { Duration = 0.2, Distance = 600 });
        }

        //protected 
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            if (toggle != null)
            {
                switch (toggle.Tag.ToString())
                {
                    case "autoPlayToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_AUTOPLAY_TOGGLLESWITCH_ON, toggle.IsOn);
                        break;
                    case "autoBackToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_AUTOBACK_TOGGLLESWITCH_ON, toggle.IsOn);
                        break;
                    case "hightQualityToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_HIGHQUALITY_TOGGLLESWITCH_ON, toggle.IsOn);
                        break;
                    case "downloadToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_HIGHT_QUALITY_VIDEO, toggle.IsOn);
                        break;
                    case "downloadWhenFavoriteToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_AUTO_DOWNLOAD_WHEN_FAVORITE_VIDEO, toggle.IsOn);
                        break;
                    case "tileToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_TILE_ACTIVE, toggle.IsOn);
                        if (toggle.IsOn)
                        {
                            var collection = DicStore.GetValueOrDefault<object>(AppCommonConst.CURRENT_TILE_COLLECTION, null) as List<Videolist>;
                            if (collection != null)
                            {
                                TileHelper.Instance.UpdateTiles(collection);
                            }
                        }
                        else
                        {
                            TileHelper.Instance.CloseTiles();
                        }
                        break;
                    case "sureToggle":
                        SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_SURE_TOGGLLESWITCH_ON, toggle.IsOn);
                        break;
                }
            }
        }
    }
}
