//-----------------------------------------------------------------------
// <copyright file="ExceptionTool.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using Sphere10.Framework;


// ReSharper disable CheckNamespace
namespace Tools {


    public static class Exceptions {

        public static void ExecuteIgnoringException(Action action, Action<Exception> handler = null) {
            Exception discard;
            ExecuteIgnoringException(action, out discard);
	        if (handler != null && discard != null) {
		        ExecuteIgnoringException(()=> handler(discard));
	        }
        }

        public static void ExecuteIgnoringException(Action action, ICollection<Exception> exceptionList ) {
            Exception error;
            ExecuteIgnoringException(action, out error);
            if (error != null)
                exceptionList.Add(error);
        }

        public static void ExecuteIgnoringException(Action action, out Exception caughtError) {
            try {
                action();
                caughtError = null;
            } catch (Exception error) {
                caughtError = error;
            }
        }


        public static T TryOrDefault<T>(Func<T> func, T @default) {
            try {
                return func();
            } catch {
                return @default;
            }
        }

        public static void ValidateTasks(params Task[] tasks) {
            var exceptions = (
                from task in tasks 
                where task != null && task.Exception != null 
                select task.Exception)
                .Cast<Exception>()
                .ToList();
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}
