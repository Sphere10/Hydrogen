//-----------------------------------------------------------------------
// <copyright file="TreeExtensions.cs" company="Sphere 10 Software">
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

}
