// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A scope that carries with it a collection of disposable items. If not salvaged, the items are
/// disposed at the end of the DisposalScope.
/// </summary>
public class Disposables : ListDecorator<IDisposable>, IDisposable {
	private readonly bool _ignoreExceptions;

	static Disposables() {
		None = new Disposables();
	}

	public Disposables(params IDisposable[] disposals)
		: this(false, disposals) {

	}

	public Disposables(bool ignoreExceptions, params IDisposable[] disposals)
		: base(new List<IDisposable>()) {
		_ignoreExceptions = ignoreExceptions;
		if (disposals.Any())
			this.AddRangeSequentially(disposals);
	}


	public void Dispose() {
		if (_ignoreExceptions)
			this.ForEach(disposable => Tools.Exceptions.ExecuteIgnoringException(disposable.Dispose));
		else
			this.ForEach(disposable => disposable.Dispose());
	}

	public void Add(Action disposeAction) {
		base.Add(new ActionDisposable(disposeAction));
	}

	public void Add(Func<Task> disposeTask) {
		base.Add(new TaskDisposable(disposeTask));
	}

	public static Disposables None { get; }

}
