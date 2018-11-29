using Brain.Animate;
using EyeSight.Base;
using EyeSight.Config;
using EyeSight.Const;
using EyeSight.Extension.DependencyObjectEx;
using EyeSight.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EyeSight.UIControl.UserControls.RetryUICtrl
{
    public sealed partial class RetryUIControl : UIControlBase
    {
        public RetryUIControl()
        {
            this.InitializeComponent();

            this.Loaded -= RetryUIControl_Loaded;
            this.Loaded += RetryUIControl_Loaded;

            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;

        }

        private async void RetryUIControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbFailure.Text = msg;

            await this.grid.AnimateAsync(new FadeInAnimation());
        }

        public string msg { get; set; }

        public Type fromType { get; set; }

        public string method { get; set; }

        public object[] parameters { get; set; }

        protected override void OnRootFrameSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppEnvironment.IsInternetAccess)
                {
                    //使用反射好像有问题，会破坏数据绑定的上下文
                    object o = Activator.CreateInstance(fromType);

                    object obj2 = fromType.GetMethod(method).Invoke(o, parameters);

                    Popup pop = this.Parent as Popup;
                    if (pop != null)
                    {
                        pop.IsOpen = false;
                        pop.Child = null;
                        pop = null;
                    }
                }
                else
                {
                    await new MessageDialog(AppNetworkMessageConst.NETWORK_IS_OFFLINEL, "提示").ShowAsyncQueue();
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }
    }
}
