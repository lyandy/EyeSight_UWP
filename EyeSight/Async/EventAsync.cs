﻿//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Async
//类名称:       EventAsync
//创建时间:     2015/9/21 星期一 15:13:39
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace EyeSight.Async
{
    public static class EventAsync
    {
        /// <summary>
        /// Creates a <see cref="System.Threading.Tasks.Task"/>
        /// that waits for an event to occur.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// await EventAsync.FromEvent(
        ///     eh => storyboard.Completed += eh,
        ///     eh => storyboard.Completed -= eh,
        ///     storyboard.Begin);
        /// ]]>
        /// </example>
        /// <param name="addEventHandler">
        /// The action that subscribes to the event.
        /// </param>
        /// <param name="removeEventHandler">
        /// The action that unsubscribes from the event when it first occurs.
        /// </param>
        /// <param name="beginAction">
        /// The action to call after subscribing to the event.
        /// </param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task"/> that
        /// completes when the event registered in
        /// <paramref name="addEventHandler"/> occurs.
        /// </returns>
        public static Task<object> FromEvent<T>(
            Action<EventHandler<T>> addEventHandler,
            Action<EventHandler<T>> removeEventHandler,
            Action beginAction = null)
        {
            return new EventHandlerTaskSource<T>(
                addEventHandler,
                removeEventHandler,
                beginAction).Task;
        }

        /// <summary>
        /// Creates a <see cref="System.Threading.Tasks.Task"/>
        /// that waits for an event to occur.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// await EventAsync.FromEvent(
        ///     eh => button.Click += eh,
        ///     eh => button.Click -= eh);
        /// ]]>
        /// </example>
        /// <param name="addEventHandler">
        /// The action that subscribes to the event.
        /// </param>
        /// <param name="removeEventHandler">
        /// The action that unsubscribes from the event when it first occurs.
        /// </param>
        /// <param name="beginAction">
        /// The action to call after subscribing to the event.
        /// </param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task"/> that
        /// completes when the event registered in
        /// <paramref name="addEventHandler"/> occurs.
        /// </returns>
        public static Task<RoutedEventArgs> FromRoutedEvent(
            Action<RoutedEventHandler> addEventHandler,
            Action<RoutedEventHandler> removeEventHandler,
            Action beginAction = null)
        {
            return new RoutedEventHandlerTaskSource(
                addEventHandler,
                removeEventHandler,
                beginAction).Task;
        }

        private sealed class EventHandlerTaskSource<TEventArgs>
        {
            private readonly TaskCompletionSource<object> tcs;
            private readonly Action<EventHandler<TEventArgs>> removeEventHandler;

            public EventHandlerTaskSource(
                Action<EventHandler<TEventArgs>> addEventHandler,
                Action<EventHandler<TEventArgs>> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                this.tcs = new TaskCompletionSource<object>();
                this.removeEventHandler = removeEventHandler;
                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            /// <summary>
            /// Returns a task that waits for the event to occur.
            /// </summary>
            public Task<object> Task
            {
                get { return tcs.Task; }
            }

            private void EventCompleted(object sender, TEventArgs args)
            {
                this.removeEventHandler.Invoke(EventCompleted);
                this.tcs.SetResult(args);
            }
        }

        private sealed class RoutedEventHandlerTaskSource
        {
            private readonly TaskCompletionSource<RoutedEventArgs> tcs;
            private readonly Action<RoutedEventHandler> removeEventHandler;

            public RoutedEventHandlerTaskSource(
                Action<RoutedEventHandler> addEventHandler,
                Action<RoutedEventHandler> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                this.tcs = new TaskCompletionSource<RoutedEventArgs>();
                this.removeEventHandler = removeEventHandler;
                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            /// <summary>
            /// Returns a task that waits for the routed to occur.
            /// </summary>
            public Task<RoutedEventArgs> Task
            {
                get { return tcs.Task; }
            }

            private void EventCompleted(object sender, RoutedEventArgs args)
            {
                this.removeEventHandler.Invoke(EventCompleted);
                this.tcs.SetResult(args);
            }
        }
    }
}
