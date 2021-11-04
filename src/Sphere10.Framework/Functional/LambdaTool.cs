//-----------------------------------------------------------------------
// <copyright file="LambdaTool.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Lambda {

		public static Action Action(Action action) {
            return action;
        }

        /// <summary>
        /// Wraps the action with a try/catch that ignores all exceptions.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Wrapped action</returns>
        public static Action ActionIgnoringExceptions(Action action) {
            return ActionWithExceptionHandler(action, (e) => { });
        }

        public static Action<T> ActionIgnoringExceptions<T>(Action<T> action) {
            return ActionWithExceptionHandler(action, (e) => { });
        }

        /// <summary>
        /// Wraps the action with an exception handler.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="exceptionHandler">The catch block.</param>
        /// <returns></returns>
        public static Action ActionWithExceptionHandler(Action action, Action<Exception> exceptionHandler) {
            return
                () => {
                    try {
                        action();
                    } catch (Exception e) {
                        exceptionHandler(e);
                    }
                };
        }

        /// <summary>
        /// Wraps the action with an exception handler.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="exceptionHandler">The catch block.</param>
        /// <returns></returns>
        public static Action<T> ActionWithExceptionHandler<T>(Action<T> action, Action<Exception> exceptionHandler) {
            return
                (p1) => {
                    try {
                        action(p1);
                    } catch (Exception e) {
                        exceptionHandler(e);
                    }
                };
        }

        /// <summary>
        /// Returns the action to run asyncronously (queued in the ThreadPool).
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="dismissExceptions">Whether or not to propagate exceptions</param>
        /// <returns></returns>
        public static Action ActionAsAsyncronous(Action action) => () => Task.Factory.StartNew(action.Invoke);

        public static Action<T1> ActionAsAsyncronous<T1>(Action<T1> action) => (a1) => Task.Factory.StartNew(() => action.Invoke(a1));

        public static Action<T1, T2> ActionAsAsyncronous<T1, T2>(Action<T1, T2> action) => (a1, a2) => Task.Factory.StartNew(() => action.Invoke(a1, a2));

        public static Action<T1, T2, T3> ActionAsAsyncronous<T1, T2, T3>(Action<T1, T2, T3> action) => (a1, a2, a3) => Task.Factory.StartNew(() => action.Invoke(a1, a2, a3));

        /// <summary>
        /// Wraps an action with retry failover code.
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="retryCount">Number of attempts to retry upon failure</param>
        /// <param name="failAction">Action to execute when a failure occurs (e.g. could log, sleep, etc)</param>
        /// <param name="completedAction">Action to execute when action completes </param>
        /// <returns></returns>
        public static Action ActionWithRetry(Action action, int retryCount, Action<int, Exception> failAction = null, Action<int> completedAction = null) {
            return
                action.WithFailOver(
                    (attempt, error) => {
                        if (failAction != null) {
                            failAction(attempt, error);
                        }
                        return attempt < retryCount + 1;
                    },
                    completedAction
                    );
        }


        /// <summary>
        /// Adds failover redundancy code to the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="decideRetry">Functor to decide whether or not to retry. Parameters are attempt, Exception and returns true/false.</param>
        /// <param name="attempt">The attempt.</param>
        /// <returns>The given action wrapped with failover code.</returns>
        public static Action ActionWithFailOver(Action action, Func<int, Exception, bool> decideRetry, Action<int> completedAction = null, int attempt = 1) {
            return () => {
                try {
                    action();
                } catch (Exception error) {
                    if (decideRetry(attempt, error)) {
                        action.WithFailOver(decideRetry, completedAction, ++attempt).Invoke();
                    }
                }
                if (completedAction != null) {
                    completedAction.Invoke(attempt);
                }
            };
        }


        // http://weblogs.asp.net/marianor/archive/2009/04/10/using-expression-trees-to-get-property-getter-and-setters.aspx
        //http://stackoverflow.com/questions/2823236/creating-a-property-setter-delegate
	    public static Expression<Action<TObject, TProperty>> CreateSetterExpression<TObject, TProperty>(Expression<Func<TObject, TProperty>> getter) {
		    var member = (MemberExpression) getter.Body;
		    var param = Expression.Parameter(typeof(TProperty), "value");
		    var setter =
			    Expression.Lambda<Action<TObject, TProperty>>(
				    Expression.Assign(member, param),
				    getter.Parameters[0],
				    param
			    );
		    return setter;
	    }

	    public static void NoOp() {
        }

		public static void NoOp<T1, T2>(T1 t1, T2 t2) {
		}

		public static object NoOp<T>(T o) => o;

        public static T ExceptionExpression<T>(string errorMessage, params object[] formatArgs) {
            throw new Exception(string.Format(errorMessage, formatArgs));
        }
    }
}

