//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Async
//类名称:       AsyncProperty
//创建时间:     2015/9/21 星期一 15:12:53
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EyeSight.Async
{
    public class AsyncProperty<T> : INotifyPropertyChanged
    {
        #region Fields

        readonly object locker = new object();

        T defValue;
        Lazy<Task<T>> lazy;
        private string _Error = null;
        private bool _HasError = false;
        private bool _HasValue = false;
        #endregion Fields

        #region Constructors

        public AsyncProperty(Func<T> valueFactory, T defVal = default(T))
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            defValue = defVal;
            lazy = new Lazy<Task<T>>(() => AppendTask(Task.Run(valueFactory)), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public AsyncProperty(Func<Task<T>> taskFactory, T defVal = default(T))
        {
            if (taskFactory == null)
            {
                throw new ArgumentNullException("taskFactory");
            }

            defValue = defVal;
            lazy = new Lazy<Task<T>>(() => AppendTask(Task.Run(taskFactory)), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        #endregion Constructors

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public T AsyncValue
        {
            get
            {
                if (HasError)
                    return defValue;
                if (lazy.Value.IsCompleted)
                {
                    return lazy.Value.Result;
                }
                else
                {
                    lock (locker)
                    {
                        return defValue;
                    }
                }
            }
            set
            {
                if (value == null) HasError = true;
                SetProperty(ref defValue, value);
            }
        }

        public string Error
        {
            get { return _Error; }
            private set { SetProperty(ref _Error, value); }
        }

        public bool HasError
        {
            get { return _HasError; }
            private set { SetProperty(ref _HasError, value); }
        }

        public bool HasValue
        {
            get { return _HasValue; }
            private set { SetProperty(ref _HasValue, value); }
        }

        #endregion Properties

        #region Methods

        public TaskAwaiter<T> GetAwaiter()
        {
            return lazy.Value.GetAwaiter();
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<K>(ref K storage, K value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        Task<T> AppendTask(Task<T> task)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    if (t.Exception is AggregateException)
                        Error = ((AggregateException)t.Exception).InnerExceptions[0].Message;
                    else
                        Error = t.Exception.Message;
                    HasValue = HasError = true;
                }
                else
                {
                    lock (locker)
                    {
                        AsyncValue = t.Result;
                    }
                    HasValue = true;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }

        #endregion Methods
    }
}
