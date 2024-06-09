// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IPagedList<TItem> : IExtendedList<TItem>, ILoadable {

	event EventHandlerEx<object> Accessing;
	event EventHandlerEx<object> Accessed;
	event EventHandlerEx<object, IPage<TItem>> PageAccessing;
	event EventHandlerEx<object, IPage<TItem>> PageAccessed;
	event EventHandlerEx<object, long> PageCreating;
	event EventHandlerEx<object, IPage<TItem>> PageCreated;
	event EventHandlerEx<object, IPage<TItem>> PageReading;
	event EventHandlerEx<object, IPage<TItem>> PageRead;
	event EventHandlerEx<object, IPage<TItem>> PageWriting;
	event EventHandlerEx<object, IPage<TItem>> PageWrite;
	event EventHandlerEx<object, IPage<TItem>> PageDeleting;
	event EventHandlerEx<object, IPage<TItem>> PageDeleted;

	IDisposable EnterOpenPageScope(IPage<TItem> page);

	IReadOnlyList<IPage<TItem>> Pages { get; }

}
