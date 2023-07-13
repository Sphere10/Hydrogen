// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;

namespace Hydrogen;

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
