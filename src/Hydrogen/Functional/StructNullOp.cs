// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen {

	internal sealed class StructNullOp<T> : INullOp<T>, INullOp<T?>
		where T : struct {

		public bool HasValue(T value) {
			return true;
		}

		public bool AddIfNotNull(ref T accumulator, T value) {
			accumulator = Operator<T>.Add(accumulator, value);
			return true;
		}

		public bool HasValue(T? value) {
			return value.HasValue;
		}

		public bool AddIfNotNull(ref T? accumulator, T? value) {
			if (value.HasValue) {
				accumulator = accumulator.HasValue ?
					Operator<T>.Add(
						accumulator.GetValueOrDefault(),
						value.GetValueOrDefault())
					: value;
				return true;
			}
			return false;
		}
	}

}