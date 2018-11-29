using EyeSight.Api.ApiRoot;
using EyeSight.Base;
using EyeSight.Common;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Extension.IOEx;
using EyeSight.Helper;
using EyeSight.Model;
using EyeSight.UIControl.UserControls.WelcomeUICtrl;
using EyeSight.View;
using EyeSight.View.Daily;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Download;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UmengSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace EyeSight
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
        }

        private async void App_Resuming(object sender, object e)
        {
            await UmengAnalytics.StartTrackAsync("58575add7666131aec00185d", AppEnvironment.IsPhone ? "Phone" : "PC");
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            // 确保当前窗口处于活动状态
            Window.Current.Activate();

            await UmengAnalytics.StartTrackAsync("58575add7666131aec00185d", AppEnvironment.IsPhone ? "Phone" : "PC");
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }

            UmengSDK.UmengAnalytics.IsDebug = true;
#endif
            await ShowWindow(e);

            await UmengAnalytics.StartTrackAsync("58575add7666131aec00185d", AppEnvironment.IsPhone ? "Phone" : "PC");
        }

        private async Task ShowWindow(LaunchActivatedEventArgs e)
        {
            //获取并记录屏幕宽度，一定要在程序启动的时候记录，这样记录到的数据才是竖屏下的屏幕高度。屏幕旋转Window.Current.Bounds.With宽高会互换
            AppEnvironment.ScreenPortraitWith = Window.Current.Bounds.Width;
            //初始化MvvmLight线程
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();

            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;

            Frame rootFrame = null;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootPage == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootPage = new NavigationRootPage();

                rootFrame = (Frame)rootPage.FindName("rootFrame");

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                //如果是手机，则默认全屏模式
                if (AppEnvironment.IsPhone)
                {
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    //ApplicationView.GetForCurrentView().TitleBar
                }
                else
                {
                    // DisplayProperties.CurrentOrientation 发生变化时触发的事件

                    //设置平板窗口的最小宽高
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(AppEnvironment.DesktopSize);
                }

                //初始化数据库
                await InitDatabase();

                //获取数据库数据
                await GetDatabaseCollection();

                //处理自定义SplashScreenImage
                var isLoadSplashImageSuccess = await InitSplashScreenImage();
                //如果SplashScreenI加载失败，则不显示欢迎屏幕，直接进入主界面
                if (isLoadSplashImageSuccess)
                {
                    //存储第一次启动的表示，是为了防止DailyPage第一页加载数据的时候把ProgressUIControl或者RetryUIControl显示出来，达到正确显示的逻辑处理
                    DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_APP_FIRST_LAUNCH, true);

                    //这句话放在Activate()之前是为了保证在系统的初始屏幕SplashScreen.png消失之后能够显示ShowWelcome的内容。这是正确的处理逻辑
                    WelcomeBox.Instance.ShowWelcome();
                }

                //这句话调用以后，在Package.appxmanifest文件配置的初始屏幕SplashScreen.png就会消失。不写这句话的话，会一直停留显示在初始屏幕SplashScreen.png处。因此，自定义SplashScreenImage的处理逻辑应该放在这句话之前
                //确保当前窗口处于活动状态
                Window.Current.Activate();

                //这个放在Activate()方法下面是为了防止在ShowWelcome没显示之前Window.Current.Content就显示出来的问题。因为只要写了这句话，rootPage就会立刻被激活显示，此时的ShowWelcome可能还没显示出来，造成闪一下的问题。放在ShowWelcome()和Activate()之后才是正确的处理逻辑。
                // 将框架放在当前窗口中
                Window.Current.Content = rootPage;
            }

            if (rootFrame != null && rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                rootFrame.Navigate(typeof(DailyPage), "每日精选");
            }

            Window.Current.Activate();
        }

        #region 数据库初始化处理
        private async Task InitDatabase()
        {
            #region 我的收藏数据库
            SQLiteAsyncConnection dbConnection;
            //使用漫游目录来存储
            var dbFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.EyeSightFavoriteDatabaseCacheRelativePath, CreationCollisionOption.OpenIfExists);

            var file = await dbFolder.CreateFileAsync(AppCommonConst.EYESIGHT_FAVORITE_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            //读取数据库文件的长度
            //var fileLength = (await file.OpenStreamForReadAsync()).Length;
            //如果长度为0的话，说明这个数据库文件里面啥也没有，连表都没有，及应用的初始化状态
            //if (fileLength == 0)
            //{
                //开始创建数据库表
                dbConnection = new SQLiteAsyncConnection(file.Path);
                Type[] favouriteTables = new Type[5];
            favouriteTables[0] = typeof(ModelPropertyBase);
            favouriteTables[1] = typeof(Provider);
            favouriteTables[2] = typeof(Consumption);
            favouriteTables[3] = typeof(Playinfo);
            favouriteTables[4] = typeof(VideoTag);

                //数据库一下创建多个表
                await dbConnection.CreateTablesAsync(favouriteTables);
            //}
            //根据数据库路径返回SQlite数据库实例，以便能够存储方便应用程序全局访问
            dbConnection = new SQLiteAsyncConnection(file.Path);

            if (dbConnection != null)
            {
                DicStore.AddOrUpdateValue<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_FAVORITE_DATABASE, dbConnection);
            }
            else
            {
            }
            #endregion

            #region 我的下载
            SQLiteAsyncConnection dbDownloadConnection;
            //使用漫游目录来存储
            var dbDownloadFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.EyeSightFavoriteDatabaseCacheRelativePath, CreationCollisionOption.OpenIfExists);

            var downloadFile = await dbDownloadFolder.CreateFileAsync(AppCommonConst.EYESIGHT_DOWNLOAD_DATABASE_NAME, CreationCollisionOption.OpenIfExists);
            //读取数据库文件的长度
            //var downloadFileLength = (await downloadFile.OpenStreamForReadAsync()).Length;
            //如果长度为0的话，说明这个数据库文件里面啥也没有，连表都没有，及应用的初始化状态
            //if (downloadFileLength == 0)
            //{
                //开始创建数据库表
                dbDownloadConnection = new SQLiteAsyncConnection(downloadFile.Path);
                Type[] downloadTables = new Type[5];
                downloadTables[0] = typeof(ModelPropertyBase);
                downloadTables[1] = typeof(Provider);
                downloadTables[2] = typeof(Consumption);
                downloadTables[3] = typeof(Playinfo);
            downloadTables[4] = typeof(VideoTag);

            //数据库一下创建多个表
            await dbDownloadConnection.CreateTablesAsync(downloadTables);
            //}
            //根据数据库路径返回SQlite数据库实例，以便能够存储方便应用程序全局访问
            dbDownloadConnection = new SQLiteAsyncConnection(downloadFile.Path);

            if (dbDownloadConnection != null)
            {
                DicStore.AddOrUpdateValue<SQLiteAsyncConnection>(AppCommonConst.EYESIGHT_DOWNLOAD_DATABASE, dbDownloadConnection);
            }
            else
            {
            }
            #endregion
        }
        #endregion

        #region 获取数据库收藏数据
        private async Task GetDatabaseCollection()
        {
            if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
            {
                SimpleIoc.Default.Register<CollectionViewModel>();
            }
            var collectionViewModel = SimpleIoc.Default.GetInstance<CollectionViewModel>();
            
            if (collectionViewModel != null)
            {
                await collectionViewModel.GetFavoriteCollection();
            }

            if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
            {
                SimpleIoc.Default.Register<DownloadViewModel>();
            }
            var downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();

            if (downloadViewModel != null)
            {
                await downloadViewModel.GetDownloadCollection();
            }

            Debug.WriteLine("我是1");

            Task.Run(() =>
            {
                if (collectionViewModel != null && downloadViewModel != null)
                {
                    collectionViewModel.FavoriteCollection.ForEach(o =>
                    {
                        //由于数据库中没有存这个字段，所以读取出来以后要再自身设置一次
                        o.isFavorite = true;

                        var cs = from m in downloadViewModel.DownloadCollection
                                 where m.id == o.id
                                 select m;
                        var model = cs.FirstOrDefault() as ModelPropertyBase;
                        if (model != null)
                        {
                            model.isFavorite = true;
                        }
                    });

                    downloadViewModel.DownloadCollection.ForEach(oo =>
                    {
                        //由于数据库中没有存这个字段，所以读取出来以后要再自身设置一次
                        oo.isAleadyDownload = true;

                        var css = from m in collectionViewModel.FavoriteCollection
                                  where m.id == oo.id
                                  select m;
                        var modell = css.FirstOrDefault() as ModelPropertyBase;
                        if (modell != null)
                        {
                            modell.isAleadyDownload = true;
                        }
                    });

                    Debug.WriteLine("我是2");
                }
            });

            Debug.WriteLine("我是3");
        }
        #endregion

        #region 欢迎屏幕图片处理
        private async Task<bool> InitSplashScreenImage()
        {
            string splashScreenScreenUrl = AppEnvironment.IsPhone ? "ms-appx:///Assets/SplashDefault/Mobile/1.jpg" : "ms-appx:///Assets/SplashDefault/PC/2.jpg";

            if (AppEnvironment.IsPhone)
            {
                var phoneFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.PhoneSplashScreenImageCacheRelativePath, CreationCollisionOption.OpenIfExists);
                var phoneFile = await phoneFolder.TryGetItemAsync("phone.jpg");
                if (phoneFile != null)
                {
                    //因为手机端始终都是一张图片名称永远都是phone.jpg，所以直接指定路径就可以了
                    splashScreenScreenUrl = "ms-appdata:///local/" + CacheConfig.Instance.PhoneSplashScreenImageCacheRelativePath + "/phone.jpg";
                }

                //都要转换成东八区北京时间来计算
                var saveDate = SettingsStore.GetValueOrDefault<long>(AppCommonConst.DATE_HAS_SAVE, DateTime.Now.ToChinaStandardTime().AddDays(-1).ToUnixTime());
                if ((DateTime.Now.ToChinaStandardTime().Date - saveDate.ToDateTime().Date).Days >= 1)
                {
                    await Task.Run(async () =>
                    {
                        var backJson = await WebDataHelper.Instance.GetFromUrlWithAuthReturnString(ApiSplashScreenRoot.Instance.SplashScreenUrl, null, 20);
                        if (backJson != null)
                        {
                            var model = JsonConvertHelper.Instance.DeserializeObject<PhoneSplashScreenModel>(backJson);
                            if (model != null)
                            {
                                //专题数据处理
                                if (model.campaignInFeed != null && AppEnvironment.IsMemoryLimitMoreThan185)
                                {
                                    SettingsStore.AddOrUpdateValue<bool>(AppCommonConst.IS_CAMPAIGN_AVAILABLE, model.campaignInFeed.available);
                                    SettingsStore.AddOrUpdateValue<string>(AppCommonConst.CAMPAIGN_IMAGE_URL, model.campaignInFeed.imageUrl);
                                    SettingsStore.AddOrUpdateValue<string>(AppCommonConst.CAMPAIGN_ACTION_URL, model.campaignInFeed.actionUrl);
                                }

                                WebDownFileHelper.Instance.SaveAsyncWithHttp(model.startPage.imageUrl, "phone.jpg", phoneFolder);
                            }
                        }
                    });
                }
            }
            else
            {
                var pcFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheConfig.Instance.PCSplashScreenImageCacheRelativePath, CreationCollisionOption.OpenIfExists);
                var pcFiles = await pcFolder.GetAllFilesAsync();
                if (pcFiles.Count != 0)
                {
                    var randomIndex = new Random().Next(0, pcFiles.Count);
                    splashScreenScreenUrl = "ms-appdata:///local/" + CacheConfig.Instance.PCSplashScreenImageCacheRelativePath + "/" + pcFiles[randomIndex].Name;
                }
            }

            var bitmapImage = await CommonHelper.Instance.LoadImageSource(splashScreenScreenUrl);

            if(bitmapImage != null)
            {
                DicStore.AddOrUpdateValue<BitmapImage>(AppCommonConst.SPLASH_BITMAPIMAGE, bitmapImage);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        /// 
        async void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            await new MessageDialog("基础框架页面导航初始化失败。", "提示").ShowAsyncQueue();
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            await UmengAnalytics.EndTrackAsync();
            deferral.Complete();
        }
    }
}
