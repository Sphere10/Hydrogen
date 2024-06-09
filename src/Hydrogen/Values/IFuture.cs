// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Class representing a value which will be available some time in the future.
/// </summary>
public interface IFuture<out T> {
	/// <summary>
	/// Retrieves the value, if available, and throws InvalidOperationException
	/// otherwise.
	/// </summary>
	T Value { get; }
}


public static class IFutureExtensions {
	public static IFuture<TProjection> AsProjection<T, TProjection>(this IFuture<T> future, Func<T, TProjection> projection)
		=> Tools.Values.Future.Projection(future, projection);
}
