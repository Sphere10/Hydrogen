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

namespace Hydrogen;

public static class TreeExtensions {

	public static IEnumerable<R> TraverseDepthFirst<T, R>(
		this T t,
		Func<T, R> valueselect,
		Func<T, IEnumerable<T>> childselect) {
		return t.TraverseDepthFirstWithParent(valueselect, childselect).Select(x => x.Key);
	}

	public static IEnumerable<KeyValuePair<R, T>> TraverseDepthFirstWithParent<T, R>(
		this T t,
		Func<T, R> valueselect,
		Func<T, IEnumerable<T>> childselect) {
		return t.TraverseDepthFirstWithParent(default(T), valueselect, childselect);
	}

	static IEnumerable<KeyValuePair<R, T>> TraverseDepthFirstWithParent<T, R>(
		this T t,
		T parent,
		Func<T, R> valueselect,
		Func<T, IEnumerable<T>> childselect) {
		yield return new KeyValuePair<R, T>(valueselect(t), parent);

		foreach (var i in childselect(t)) {
			foreach (var item in i.TraverseDepthFirstWithParent(t, valueselect, childselect)) {
				yield return item;
			}
		}
	}

	public static IEnumerable<R> TraverseBreadthFirst<T, R>(
		this T t,
		Func<T, R> valueselect,
		Func<T, IEnumerable<T>> childselect) {
		return t.TraverseBreadthFirstWithParent(valueselect, childselect).Select(x => x.Key);
	}

	public static IEnumerable<KeyValuePair<R, T>> TraverseBreadthFirstWithParent<T, R>(
		this T t,
		Func<T, R> valueselect,
		Func<T, IEnumerable<T>> childselect) {
		yield return new KeyValuePair<R, T>(valueselect(t), default(T));

		List<T> children = new List<T>();

		foreach (var e in childselect(t)) {
			children.Add(e);
			yield return new KeyValuePair<R, T>(valueselect(e), t);
		}

		while (children.Count > 0) {
			foreach (var e in new List<T>(children)) {
				children.Remove(e);
				foreach (var c in childselect(e)) {
					children.Add(c);
					yield return new KeyValuePair<R, T>(valueselect(c), e);
				}
			}
		}
	}
}
