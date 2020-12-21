//-----------------------------------------------------------------------
// <copyright file="SemaphoreSlimExtensions.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections.Concurrent;



namespace Sphere10.Framework {

    public static class SemaphoreSlimExtensions {
        public static IDisposable EnterWaitScope(this SemaphoreSlim semaphore) {
            semaphore.Wait();
            return new ActionScope(() => semaphore.Release());
        }

    }
}
