//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Extension.DependencyObjectEx
//类名称:       MessageDialogEx
//创建时间:     2015/9/21 星期一 15:38:04
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace EyeSight.Extension.DependencyObjectEx
{
    public static class MessageDialogEx
    {
        public static IAsyncOperation<IUICommand> ShowTwoOptionsDialog(string text, string leftButtonText, string rightButtonText, Action leftButtonAction, Action rightButtonAction)
        {
            var dialog = new MessageDialog(text);

            dialog.AddButton(leftButtonText, leftButtonAction);
            dialog.AddButton(rightButtonText, rightButtonAction);

            dialog.DefaultCommandIndex = 1;

            return dialog.ShowAsync();
        }

        public static void AddButton(this MessageDialog dialog, string caption, Action action)
        {
            var cmd = new UICommand(
                caption,
                c =>
                {
                    if (action != null)
                        action.Invoke();
                });
            dialog.Commands.Add(cmd);
        }

        private static TaskCompletionSource<MessageDialog> _currentDialogShowRequest;

        /// <summary>
        /// 此处扩展可避免手机端多次弹出MessageDialog导致应用崩溃的问题，平板应用无此问题
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public static async Task<IUICommand> ShowAsyncQueue(this MessageDialog dialog)
        {
            if (!Window.Current.Dispatcher.HasThreadAccess)
            {
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
            }

            while (_currentDialogShowRequest != null)
            {
                await _currentDialogShowRequest.Task;
            }

            var request = _currentDialogShowRequest = new TaskCompletionSource<MessageDialog>();
            var result = await dialog.ShowAsync();
            _currentDialogShowRequest = null;
            request.SetResult(dialog);

            return result;
        }

        public static async Task<IUICommand> ShowAsyncIfPossible(this MessageDialog dialog)
        {
            if (!Window.Current.Dispatcher.HasThreadAccess)
            {
                throw new InvalidOperationException("This method can only be invoked from UI thread.");
            }

            while (_currentDialogShowRequest != null)
            {
                return null;
            }

            var request = _currentDialogShowRequest = new TaskCompletionSource<MessageDialog>();
            var result = await dialog.ShowAsync();
            _currentDialogShowRequest = null;
            request.SetResult(dialog);

            return result;
        }
    }
}
