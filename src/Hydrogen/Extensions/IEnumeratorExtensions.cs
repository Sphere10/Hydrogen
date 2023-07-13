// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public static class IEnumeratorExtensions {

	public static IEnumerator<T> OnMoveNext<T>(this IEnumerator<T> enumerator, Action action)
		=> new OnMoveNextEnumerator<T>(enumerator, action);

	public static IEnumerator<T> OnMoveNext<T>(this IEnumerator<T> enumerator, Action preMoveNextAction, Action<bool> postMoveNextAction)
		=> new OnMoveNextEnumerator<T>(enumerator, preMoveNextAction, postMoveNextAction);

	public static IEnumerator<T> OnMoveNext<T>(this IEnumerator<T> enumerator, Func<bool?> preMoveNextFunc, Func<bool, bool?> postMoveNextFunc)
		=> new OnMoveNextEnumerator<T>(enumerator, preMoveNextFunc, postMoveNextFunc);

	public static IEnumerator<T> OnDispose<T>(this IEnumerator<T> enumerator, Action action)
		=> new OnDisposeEnumerator<T>(enumerator, action);

	public static IEnumerator<T> WithMemory<T>(this IEnumerator<T> enumerator)
		=> new MemorizingEnumerator<T>(enumerator);

	public static IEnumerator<T> WithBoundary<T>(this IEnumerator<T> enumerator, int maxCount)
		=> new BoundedEnumerator<T>(enumerator, maxCount);

	public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator) {
		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}

}
