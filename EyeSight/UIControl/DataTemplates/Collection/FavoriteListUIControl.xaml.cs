using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.CommonObjectEx;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using EyeSight.Locator;
using EyeSight.ViewModel.Collection;
using EyeSight.ViewModel.Daily;
using EyeSight.ViewModel.Download;
using EyeSight.ViewModel.Past;
using EyeSight.ViewModel.Rank;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class FavoriteListUIControl : UIControlBase
    {
        public FavoriteListUIControl()
        {
            this.InitializeComponent();

            var phoneWith = Window.Current.Bounds.Width;
            var PCWith = Window.Current.Bounds.Width - 48;

            if (AppEnvironment.IsPhone)
            {
                this.grid.Width = phoneWith;
            }
            else
            {
                if (PCWith >= 700)
                {
                    this.grid.Width = (PCWith - 4 * 3.2) / 3;
                }
                else
                {
                    this.grid.Width = (PCWith - 3 * 3.15) / 2;
                }
            }

            this.grid.Height = this.grid.Width * 2 / 3;

            this.Loaded += (ss, ee) =>
            {
                if (AppEnvironment.IsPhone)
                {
                    this.guestureGrid.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;

                    this.guestureGrid.ManipulationDelta -= Grid_ManipulationDelta;
                    this.guestureGrid.ManipulationCompleted -= Grid_ManipulationCompleted;
                    this.guestureGrid.ManipulationStarted -= GuestureGrid_ManipulationStarted;
                    this.guestureGrid.ManipulationDelta += Grid_ManipulationDelta;
                    this.guestureGrid.ManipulationCompleted += Grid_ManipulationCompleted;
                    this.guestureGrid.ManipulationStarted += GuestureGrid_ManipulationStarted;

                    this.gridEdit.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.gridEdit.Tapped -= gridEdit_Tapped;
                    this.gridEdit.Tapped += gridEdit_Tapped;
                }
            };

            this.Unloaded += (ss, ee) =>
            {
                if (AppEnvironment.IsPhone)
                {
                    this.guestureGrid.ManipulationMode = ManipulationModes.None;

                    this.guestureGrid.ManipulationDelta -= Grid_ManipulationDelta;
                    this.guestureGrid.ManipulationCompleted -= Grid_ManipulationCompleted;
                    this.guestureGrid.ManipulationStarted -= GuestureGrid_ManipulationStarted;

                    ViewModelLocator.Instance.FavoriteOrDownloadListViewScrollHandler -= CancelEditding;
                }
                else
                {
                    this.gridEdit.Tapped -= gridEdit_Tapped;
                }
            };
        }

        #region 侧滑手势处理

        private void coverGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.guestureGrid.GetHorizontalOffset().Value != 0)
            {
                CloseEdit();
            }
        }

        private void CancelEditding()
        {
            if (this.guestureGrid.GetHorizontalOffset().Value != 0)
            {
                CloseEdit();
            }
        }

        private void GuestureGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (this.guestureGrid.GetHorizontalOffset().Value == 0)
            {
                ViewModelLocator.Instance.FavoriteOrDownloadListViewScroll();
            }
        }

        bool isAlreadyTryManipulation = false;
        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (currentOffest < dragDistanceToEdit && isEditOpen)
            {
                OpenEdit();
            }

            if (currentOffest > dragDistanceToEdit && !isEditOpen)
            {
                CloseEdit();
            }

            isAlreadyTryManipulation = true;
        }

        private double dragDistanceToEdit = -60;
        private bool isEditOpen = false;
        private double currentOffest = 0;
        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            this.editGrid.Opacity = 1.0;

            if (e.Delta.Translation.X < 0 && !isEditOpen)
            {
                double offset = this.guestureGrid.GetHorizontalOffset().Value + e.Delta.Translation.X;

                if (offset >= dragDistanceToEdit)
                {
                    this.guestureGrid.SetHorizontalOffset(offset);
                    currentOffest = offset;
                }
                else
                {
                    OpenEdit();
                }
            }

            if (e.Delta.Translation.X > 0 && isEditOpen)
            {
                double offset = this.guestureGrid.GetHorizontalOffset().Value + e.Delta.Translation.X;

                if (offset < dragDistanceToEdit)
                {
                    this.guestureGrid.SetHorizontalOffset(offset);
                    currentOffest = offset;
                }
                else
                {
                    CloseEdit();
                }
            }
        }

        private void OpenEdit()
        {
            var trans = this.guestureGrid.GetHorizontalOffset().Transform;
            trans.AnimateX(trans.X, -95, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            isEditOpen = true;

            ViewModelLocator.Instance.FavoriteOrDownloadListViewScrollHandler += CancelEditding;

            DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_HAS_COLLECTION_EDITING, true);
        }

        private void CloseEdit()
        {
            ViewModelLocator.Instance.FavoriteOrDownloadListViewScrollHandler -= CancelEditding;
            DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_HAS_COLLECTION_EDITING, false);

            var trans = this.guestureGrid.GetHorizontalOffset().Transform;
            trans.AnimateX(trans.X, 0, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            },async () =>
            {
                await DispatcherHelper.RunAsync( () =>
                {
                    this.editGrid.Opacity = 0.0;
                });
            });
            isAlreadyTryManipulation = false;
            isEditOpen = false;
        }
        #endregion

        protected override void OnUIControlBaseNeedCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            //if (isEditOpen)
            //{
            //    CloseEdit();
            //}
        }

        protected override async void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedReleaseHolding(sender, e);

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            Debug.WriteLine("我执行了释放");

            var ctrl = sender as FrameworkElement;
            if (ctrl != null)
            {
                var model = ctrl.DataContext as ModelPropertyBase;
                if (model != null)
                {
                    if (!model.isEditing)
                    {
                        await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
                    }
                }
            }
        }

        protected override async void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedHolding(sender, e);

            var ctrl = sender as FrameworkElement;
            if (ctrl != null)
            {
                var model = ctrl.DataContext as ModelPropertyBase;
                if (model != null)
                {
                    if (!model.isEditing)
                    {
                        await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });
                    }
                }
            }

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
            ViewModelLocator.Instance.ListViewScrollHandler += resetCoverGridState;
        }

        protected override async void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedNavigate(sender, e);

            if (this.guestureGrid.GetHorizontalOffset().Value == 0 && !isAlreadyTryManipulation)
            {
                var isAlreadyCollectionEditing = DicStore.GetValueOrDefault<bool>(AppCommonConst.IS_HAS_COLLECTION_EDITING, false);
                if (isAlreadyCollectionEditing == true)
                {
                    ViewModelLocator.Instance.FavoriteOrDownloadListViewScroll();
                    return;
                }

                var ctrl = sender as FrameworkElement;
                if (ctrl != null)
                {
                    var model = ctrl.DataContext as ModelPropertyBase;
                    if (model != null)
                    {
                        if (!model.isEditing)
                        {
                            var szListPC = CommonHelper.Instance.GetCurrentSemanticZoom("szListPC");
                            if (szListPC != null)
                            {
                                if (szListPC.IsZoomedInViewActive == true)
                                {
                                    DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);

                                    szListPC.IsZoomedInViewActive = false;

                                    ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

                                    await this.coverGrid.AnimateAsync(new ResetAnimation());
                                }
                            }
                        }
                    }
                }
            }

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            isAlreadyTryManipulation = false;
        }

        private void resetCoverGridState()
        {
            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            this.coverGrid.Opacity = 1.0;

            Debug.WriteLine("我执行了resetCoverGridState");
        }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.OnRootFrameSizeChanged(sender, e);

            var with = e.NewSize.Width;

            if (AppEnvironment.IsPhone)
            {
                this.grid.Width = with;
            }
            else
            {
                if (with >= 700)
                {
                    this.grid.Width = (with - 4 * 3.2) / 3;
                }
                else
                {
                    this.grid.Width = (with - 3 * 3.15) / 2;
                }
            }

            this.grid.Height = this.grid.Width * 2 / 3;
        }

        bool isLoaded = false;
        private async void grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //这里和虚拟化有关的，非常重要不可以删除
            if (AppEnvironment.IsPhone)
            {
                this.guestureGrid.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;

                this.guestureGrid.ManipulationDelta -= Grid_ManipulationDelta;
                this.guestureGrid.ManipulationCompleted -= Grid_ManipulationCompleted;
                this.guestureGrid.ManipulationStarted -= GuestureGrid_ManipulationStarted;
                this.guestureGrid.ManipulationDelta += Grid_ManipulationDelta;
                this.guestureGrid.ManipulationCompleted += Grid_ManipulationCompleted;
                this.guestureGrid.ManipulationStarted += GuestureGrid_ManipulationStarted;

                this.gridEdit.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.gridEdit.Tapped -= gridEdit_Tapped;
                this.gridEdit.Tapped += gridEdit_Tapped;
            }

            //虚拟化会重新执行此方法绑定数据
            //await grid.AnimateAsync(new FadeInLeftAnimation());
            //await grid.AnimateAsync(new BounceInDownAnimation());

            var g = sender as Grid;

            if (g != null && !isLoaded)
            {
                await Task.Delay(500);

                var animationName = new FadeInDownAnimation();
                animationName.Distance = 150;
                g.AnimateAsync(animationName);

                isLoaded = true;
            }
        }

        #region 删除收藏的处理
        private async void gridEdit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var curElement = sender as FrameworkElement;
            if (curElement != null)
            {
                var fmodel = curElement.DataContext as ModelPropertyBase;
                if (fmodel != null)
                {
                    //确保在删除条目的时候，动画能够在其他条目之上执行
                    if (!SimpleIoc.Default.IsRegistered<CollectionViewModel>())
                    {
                        SimpleIoc.Default.Register<CollectionViewModel>(false);
                    }
                    var collectionViewModel = SimpleIoc.Default.GetInstance<CollectionViewModel>();
                    if (collectionViewModel != null)
                    {
                        var listView = CommonHelper.Instance.GetCurrentFavoriteListView();
                        if (listView != null)
                        {
                            collectionViewModel.FavoriteCollection.ForEach(A =>
                            {
                                try
                                {
                                    if (fmodel.id == A.id)
                                    {
                                        var containter = listView.ContainerFromItem(A);
                                        Canvas.SetZIndex(containter as UIElement, 1000);
                                    }
                                    else
                                    {
                                        var containterOther = listView.ContainerFromItem(A);
                                        Canvas.SetZIndex(containterOther as UIElement, 999);
                                    }
                                }
                                catch { }
                            });
                        }

                        if (AppEnvironment.IsPhone)
                        {
                            await grid.AnimateAsync(new FlipOutXAnimation() { Centre = 0.5 });
                        }
                        else
                        {
                            //掉落动画
                            await grid.AnimateAsync(new HingeAnimation());
                        }

                        #region 每日精选
                        if (!SimpleIoc.Default.IsRegistered<DailyViewModel>())
                        {
                            SimpleIoc.Default.Register<DailyViewModel>();
                        }
                        var dailyViewModel = SimpleIoc.Default.GetInstance<DailyViewModel>();
                        if (dailyViewModel != null)
                        {
                            var dailyModels = from m in dailyViewModel.DailyFlipViewCollection
                                              where m.id == fmodel.id
                                              select m;
                            var dailyModel = dailyModels.FirstOrDefault() as ModelPropertyBase;
                            if (dailyModel != null)
                            {
                                dailyModel.isFavorite = false;
                            }
                        }
                        #endregion

                        #region 往期分类详细集合处理
                        if (!SimpleIoc.Default.IsRegistered<PastDetailViewModel>())
                        {
                            SimpleIoc.Default.Register<PastDetailViewModel>();
                        }
                        var pastDetailViewModel = SimpleIoc.Default.GetInstance<PastDetailViewModel>();
                        if (pastDetailViewModel != null)
                        {
                            var pastDetailModels = from m in pastDetailViewModel.CategoryDetailCollection
                                                   where m.id == fmodel.id
                                                   select m;
                            var pastDetailModel = pastDetailModels.FirstOrDefault() as ModelPropertyBase;
                            if (pastDetailModel != null)
                            {
                                pastDetailModel.isFavorite = false;
                            }
                        }
                        #endregion

                        #region 排行榜
                        if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                        {
                            SimpleIoc.Default.Register<RankViewModel>();
                        }
                        var rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();
                        if (rankViewModel != null)
                        {
                            #region Performance
                            var rankPerformanceModels = from m in rankViewModel.RankPerformanceCollection
                                                        where m.id == fmodel.id
                                                        select m;
                            var rankPerformanceModel = rankPerformanceModels.FirstOrDefault() as ModelPropertyBase;
                            if (rankPerformanceModel != null)
                            {
                                rankPerformanceModel.isFavorite = false;
                            }
                            #endregion

                            #region Week
                            var rankWeekModels = from m in rankViewModel.WeekCollection
                                                 where m.id == fmodel.id
                                                 select m;
                            var rankWeekModel = rankWeekModels.FirstOrDefault() as ModelPropertyBase;
                            if (rankWeekModel != null)
                            {
                                rankWeekModel.isFavorite = false;
                            }
                            #endregion

                            #region Month
                            var rankMonthModels = from m in rankViewModel.MonthCollection
                                                  where m.id == fmodel.id
                                                  select m;
                            var rankMonthModel = rankMonthModels.FirstOrDefault() as ModelPropertyBase;
                            if (rankMonthModel != null)
                            {
                                rankMonthModel.isFavorite = false;
                            }
                            #endregion

                            #region All
                            var rankAllModels = from m in rankViewModel.RankAllCollection
                                                where m.id == fmodel.id
                                                select m;
                            var rankAllModel = rankAllModels.FirstOrDefault() as ModelPropertyBase;
                            if (rankAllModel != null)
                            {
                                rankAllModel.isFavorite = false;
                            }
                            #endregion
                        }
                        #endregion

                        #region 我的下载
                        if (!SimpleIoc.Default.IsRegistered<DownloadViewModel>())
                        {
                            SimpleIoc.Default.Register<DownloadViewModel>();
                        }
                        var downloadViewModel = SimpleIoc.Default.GetInstance<DownloadViewModel>();
                        if (downloadViewModel != null)
                        {
                            var downloadModels = from m in downloadViewModel.DownloadCollection
                                              where m.id == fmodel.id
                                              select m;
                            var downloadModel = downloadModels.FirstOrDefault() as ModelPropertyBase;
                            if (downloadModel != null)
                            {
                                downloadModel.isFavorite = false;
                            }
                        }
                        #endregion

                        //将指定条目从收藏数据集合中删除
                        collectionViewModel.FavoriteCollection.Remove(fmodel);

                        await DatabaseHelper.Instance.DeleteBySingle(fmodel);

                        if (collectionViewModel.FavoriteCollection.Count == 0)
                        {
                            collectionViewModel.isEmptyShow = true;
                        }
                        else
                        {
                            collectionViewModel.isEmptyShow = false;
                        }

                        //注意恢复动画，因为如果不恢复的话，虚拟化得到的控件是之前动画后的效果，即空白。重置动画后为后续虚拟化控件重用做准备，以便能够正常显示数据
                        await grid.AnimateAsync(new ResetAnimation());

                        fmodel.isEditing = false;
                    }
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModelLocator.Instance.FavoriteOrDownloadListViewScrollHandler -= CancelEditding;
            DicStore.AddOrUpdateValue<bool>(AppCommonConst.IS_HAS_COLLECTION_EDITING, false);

            var trans = this.guestureGrid.GetHorizontalOffset().Transform;
            trans.AnimateX(trans.X, 0, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            }, async () =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    this.editGrid.Opacity = 0.0;
                    gridEdit_Tapped(sender, null);
                });
            });
            isAlreadyTryManipulation = false;
            isEditOpen = false;
        }

        #endregion

    }
}
