//-----------------------------------------------------------------------
// <copyright file="DisposalScope.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

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
			: this(true, disposals) {

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
            base.Add(new ActionScope(disposeAction));
		}

		public static Disposables None { get; }

	}
}
