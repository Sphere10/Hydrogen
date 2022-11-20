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


using System;
using System.Threading;

namespace Hydrogen {

	public interface ISynchronizedObject {
		ISynchronizedObject ParentSyncObject { get; set; }
		ReaderWriterLockSlim ThreadLock { get; }
		IDisposable EnterReadScope();
		IDisposable EnterWriteScope();

	}

    public interface ISynchronizedObject<TReadScope, TWriteScope> : ISynchronizedObject
        where TReadScope : IDisposable
        where TWriteScope : IDisposable {
		new ISynchronizedObject<TReadScope, TWriteScope> ParentSyncObject { get; set; }
        ReaderWriterLockSlim ThreadLock { get; }
        new TReadScope EnterReadScope();
        new TWriteScope EnterWriteScope();
    }

}
