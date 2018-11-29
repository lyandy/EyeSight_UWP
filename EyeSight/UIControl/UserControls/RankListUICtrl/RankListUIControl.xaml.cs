using EyeSight.Const;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Brain.Animate;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using EyeSight.ViewModel.Rank;
using EyeSight.Api.ApiRoot;
using EyeSight.Model.Rank;
using EyeSight.UIControl.UserControls.RetryUICtrl;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.RankListUICtrl
{
    public sealed partial class RankListUIControl : UserControl
    {
        private ObservableCollection<Rank> RankList = new ObservableCollection<Rank>();

        public RankListUIControl()
        {
            this.InitializeComponent();

            this.DataContext = this;

            RankList.Add(new Rank() { RankName = "周排行" });
            RankList.Add(new Rank() { RankName = "月排行" });
            RankList.Add(new Rank() { RankName = "总排行" });

            this.rankListView.ItemsSource = RankList;
            
            this.Loaded += async (ss, ee) =>
            {
                
                this.rankListView.SelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);

                //await this.grid.MoveToAsync(0.3,new Point(0, 132), new CubicEase { EasingMode = EasingMode.EaseOut });

                await this.grid.AnimateAsync(new FlipInYAnimation() { Centre = 0.5 });
            };

            this.Unloaded += (ss, ee) =>
            {
                RankList.Clear();
                RankList = null;
            };
        }

        int previousSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);

        private void rankListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.rankListView.SelectedIndex != -1)
            {
                DicStore.AddOrUpdateValue<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, this.rankListView.SelectedIndex);

                Messenger.Default.Send<int>(this.rankListView.SelectedIndex, AppMessengerTokenConst.IS_RANK_LIST_SELECTED);

                Debug.WriteLine("我被触发2");
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("我被触发1");
            RetryBox.Instance.HideRetry();

            int currentSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);

            var pop = this.Parent as Popup;
            if (pop != null)
            {
                pop.IsOpen = false;
                pop.Child = null;
                pop = null;
            }

            if (previousSelectedIndex != currentSelectedIndex)
            {
                if (!SimpleIoc.Default.IsRegistered<RankViewModel>())
                {
                    SimpleIoc.Default.Register<RankViewModel>();
                }

                var rankViewModel = SimpleIoc.Default.GetInstance<RankViewModel>();

                if (rankListView != null)
                {
                    var rankSelectedIndex = DicStore.GetValueOrDefault<int>(AppCommonConst.CURRENT_RANK_LIST_SELECTED_INDEX, 0);
                    switch (rankSelectedIndex)
                    {
                        case 0:
                            if (rankViewModel.WeekCollection.Count == 0)
                            {
                                rankViewModel.GetRank(rankViewModel.RankPerformanceCollection, rankViewModel.WeekCollection, ApiRankRoot.Instance.RankUrl, AppCacheNewsFileNameConst.CACHE_RANK_WEEK_FILENAME);
                            }
                            else
                            {
                                rankViewModel.RankPerformanceCollection = new ObservableCollection<Videolist>(rankViewModel.WeekCollection);
                            }
                            break;
                        case 1:
                            if (rankViewModel.MonthCollection.Count == 0)
                            {
                                rankViewModel.GetRank(rankViewModel.RankPerformanceCollection, rankViewModel.MonthCollection, ApiRankRoot.Instance.RankUrl, AppCacheNewsFileNameConst.CACHE_RANK_MONTH_FILENAME);
                            }
                            else
                            {
                                rankViewModel.RankPerformanceCollection = new ObservableCollection<Videolist>(rankViewModel.MonthCollection);
                            }
                            break;
                        case 2:
                            if (rankViewModel.RankAllCollection.Count == 0)
                            {
                                rankViewModel.GetRank(rankViewModel.RankPerformanceCollection, rankViewModel.RankAllCollection, ApiRankRoot.Instance.RankUrl, AppCacheNewsFileNameConst.CACHE_RANK_ALL_FILENAME);
                            }
                            else
                            {
                                rankViewModel.RankPerformanceCollection = new ObservableCollection<Videolist>(rankViewModel.RankAllCollection);
                            }
                            break;
                    }
                }
            }
        }
    }

    public class Rank
    {
        public string RankName { get; set; }
    }
}
