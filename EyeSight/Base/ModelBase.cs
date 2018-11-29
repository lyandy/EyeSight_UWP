//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Base
//类名称:       ModelBase
//创建时间:     2015/9/21 星期一 15:14:57
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Base
{
    /// <summary>
    /// 用于简化模型的 <see cref="INotifyPropertyChanged"/> 实现。
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class ModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 针对属性更改通知的多播事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 检查属性是否已与所需值匹配。仅当需要时才设置
        /// 该属性并通知侦听器。
        /// </summary>
        /// <typeparam name="T">属性的类型。</typeparam>
        /// <param name="storage">对具有 getter 和 setter 的属性的引用。</param>
        /// <param name="value">属性的所需值。</param>
        /// <param name="propertyName">用于通知侦听器的属性的名称。此
        /// 值是可选的，可以在从支持 CallerMemberName 的编译器调用时
        /// 自动提供。</param>
        /// <returns>如果更改了值，则为 true，如果现有值与所需值匹配，
        /// 则为 false。</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 向侦听器通知已更改了某个属性值。
        /// </summary>
        /// <param name="propertyName">用于通知侦听器的属性的名称。此
        /// 值是可选的，可以在从支持
        /// <see cref="CallerMemberNameAttribute"/> 的编译器调用时自动提供。</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var eventHandler = this.PropertyChanged;
                if (eventHandler != null)
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                }
            });
        }
    }
}
