//-----------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;

namespace Sphere10.Framework {

    public static class TaskExtensions {
        public static void WaitSafe(this Task task) {
            Task.Run(() => task.Wait());
        }

        public static void WaitSafe(this Task task, CancellationToken cancellationToken) {
            Task.Run(() => task.Wait(cancellationToken));
        }

        public static void RunSyncronouslySafe(this Task task) {
            Task.Run(() => task.RunSynchronously());
        }

        public static T ResultSafe<T>(this Task<T> task) {
            return Task.Run(() => task.Result).Result;
        }
        public static T ResultSafe<T>(this Task<T> task, CancellationToken cancellationToken) {
            return Task.Run(() => task.Result, cancellationToken).Result;
        }

        public static async Task WithAggregateException(this Task task, bool unwrapSingleAggregateException = true) {
            try {
                await task;
            } catch (Exception) {
                if (task.Exception == null)
                    throw;
                if (unwrapSingleAggregateException && task.Exception.InnerExceptions.Count == 1) {
                    throw task.Exception.InnerExceptions[0];
                }                
                throw task.Exception;
            }            
        }
    }
}
