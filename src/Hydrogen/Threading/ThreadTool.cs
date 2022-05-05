//-----------------------------------------------------------------------
// <copyright file="ThreadTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {

    public static class Threads {

        public static Task CreateAwaitTask(Func<Task> invokeAsyncMethod) {
            //return new Task( async () => await invokeAsyncMethod() );
            return new Task(() => invokeAsyncMethod().WaitSafe());
        }

        public static IDisposable EnterLockScope(object lockObject) {
            var lockTaken = false;
            Monitor.Enter(lockObject, ref lockTaken);
            return new ActionScope(
                () => {
                    if (lockTaken)
                        System.Threading.Monitor.Exit(lockObject);
                }
            );
        }

        public static void RaiseAsync(EventHandlerEx handler) => Task.Factory.StartNew(() => handler?.Invoke());

        public static void RaiseAsync<T>(EventHandlerEx<T> handler, T arg) => Task.Factory.StartNew(() => handler?.Invoke(arg));

        public static void RaiseAsync<T1, T2>(EventHandlerEx<T1, T2> handler, T1 arg1, T2 arg2) => Task.Factory.StartNew(() => handler?.Invoke(arg1, arg2));

        public static void RaiseAsync<T1, T2, T3>(EventHandlerEx<T1, T2, T3> handler, T1 arg1, T2 arg2, T3 arg3) => Task.Factory.StartNew(() => handler?.Invoke(arg1, arg2, arg3));

        public static void ExecuteAsync(Action action) {
            action.AsAsyncronous().Invoke();
        }

        public static void ExecuteConcurrently(IEnumerable<Action> actions) {
            ExecuteConcurrently(actions.ToArray());
        }

        public static void ExecuteConcurrently(params Action[] actions) {
#if DISABLE_CONCURRENT
				ExecuteSequential(actions);
#else
            ExecuteConcurrentlyInternal(actions);
#endif
        }

        private static void ExecuteConcurrentlyInternal(params Action[] actions) {
            if (actions.Length == 0)
                return;

            var errors = new SynchronizedList<Exception>();
            var numberOfActions = actions.Length;
            WaitHandle[] waitHandlers = new WaitHandle[numberOfActions];

            for (var i = 0; i < numberOfActions; i++) {
                var action = actions[i];
                var waitHandler = new AutoResetEvent(false);
                waitHandlers[i] = waitHandler;

                void Callback(object state) {
                    try {
                        action();
                    } catch (Exception error) {
                        using (errors.EnterWriteScope()) {
                            errors.Add(error);
                        }
                    }
                    waitHandler.Set();
                }

                ThreadPool.QueueUserWorkItem(Callback);
            }

            WaitHandle.WaitAll(waitHandlers);

            if (errors.Count > 0) {
                throw new AggregateException(errors);
            }
        }

        private static void ExecuteSequential(params Action[] actions) {
            foreach (var action in actions)
                action();
        }

        public static void QueueAction<TArg>(Action<TArg> action, TArg arg) {
            ThreadPool.QueueUserWorkItem((o) => action((TArg)o), arg);
        }

        public static void QueueAction(Action action) {
            ThreadPool.QueueUserWorkItem((o) => action());
        }

        public static void QueueActionIgnoringException(Action action) {
            ThreadPool.QueueUserWorkItem((o) => Tools.Exceptions.ExecuteIgnoringException(action));
        }

    }
}

