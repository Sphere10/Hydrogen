//-----------------------------------------------------------------------
// <copyright file="IReadWriteSafeObject.cs" company="Sphere 10 Software">
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


using System.Threading;

namespace Sphere10.Framework {

    public interface IThreadSafeObject : IThreadSafeObject<Scope, Scope> {
	}

    public interface IThreadSafeObject<out TReadScope, out TWriteScope> 
        where TReadScope : IScope
        where TWriteScope : IScope {
        ReaderWriterLockSlim ThreadLock { get; }
        TReadScope EnterReadScope();
        TWriteScope EnterWriteScope();
    }
}
