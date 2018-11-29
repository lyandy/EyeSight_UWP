using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Helper;
using EyeSight.Locator;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.DataTemplates
{
    public sealed partial class CategoryDetailListUIControl : UIControlBase
    {
        public CategoryDetailListUIControl()
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
        }

        protected override async void OnUIControlBaseNeedReleaseHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedReleaseHolding(sender, e);

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

            await this.coverGrid.AnimateAsync(new FadeInAnimation() { Duration = 0.1 });
        }

        protected override async void OnUIControlBaseNeedHolding(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedHolding(sender, e);

            await this.coverGrid.AnimateAsync(new FadeOutAnimation() { Duration = 0.1 });

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
            ViewModelLocator.Instance.ListViewScrollHandler += resetCoverGridState;
        }

        protected override async void OnUIControlBaseNeedNavigate(object sender, PointerRoutedEventArgs e)
        {
            base.OnUIControlBaseNeedNavigate(sender, e);

            var szListPC = CommonHelper.Instance.GetCurrentSemanticZoom("szListPC");
            if (szListPC != null)
            {
                if (szListPC.IsZoomedInViewActive == true)
                {
                    var control = sender as FrameworkElement;
                    if (control != null)
                    {
                        var model = control.DataContext as ModelPropertyBase;
                        if (model != null)
                        {
                            DicStore.AddOrUpdateValue<ModelPropertyBase>(AppCommonConst.CURRENT_SELECTED_ITEM, model);

                            szListPC.IsZoomedInViewActive = false;

                            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;

                            await this.coverGrid.AnimateAsync(new ResetAnimation());
                        }
                    }
                }
            }

            ViewModelLocator.Instance.ListViewScrollHandler -= resetCoverGridState;
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
    }
}
