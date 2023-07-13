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

/// <summary>
/// A future whose value is fetched by a background thread and whose <see cref="Value"/> blocks until that thread complex.
/// </summary>
/// <typeparam name="T"></typeparam>
public class BackgroundFetchedValue<T> : IFuture<T> {
	private readonly ManualResetEventSlim _resetEvent;
	private Exception _fetchError;
	private T _value;

	public BackgroundFetchedValue(Func<T> valueLoader) {
		Guard.ArgumentNotNull(valueLoader, nameof(valueLoader));
		_resetEvent = new ManualResetEventSlim(false);
		_value = default;
		_fetchError = null;
		ThreadPool.QueueUserWorkItem(_ => {
			try {
				_value = valueLoader();
			} catch (Exception error) {
				_fetchError = error;
			} finally {
				_resetEvent.Set();
			}
		});
	}

	public T Value {
		get {
			if (!_resetEvent.IsSet)
				_resetEvent.Wait();

			if (_fetchError != null)
				throw new AggregateException(_fetchError);

			return _value;
		}
	}

	public static BackgroundFetchedValue<T> From(Func<T> valueLoader) {
		return new BackgroundFetchedValue<T>(valueLoader);
	}

	public override string ToString() {
		return _resetEvent.IsSet ? Convert.ToString(_value) : "Future value has not currently been determined";
	}
}
