// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

	public class ActionItemSerializer<T> : ActionItemSizer<T>, IItemSerializer<T> {
		private readonly Func<T, EndianBinaryWriter, int> _serializer;
		private readonly Func<int, EndianBinaryReader, T> _deserializer;

		public ActionItemSerializer(Func<T, int> sizer, Func<T, EndianBinaryWriter, int> serializer, Func<int, EndianBinaryReader, T> deserializer)
			: base(sizer) {
			Guard.ArgumentNotNull(serializer, nameof(serializer));
			Guard.ArgumentNotNull(deserializer, nameof(deserializer));
			_serializer = serializer;
			_deserializer = deserializer;
		}
		
		public bool TrySerialize(T item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				bytesWritten = _serializer(item, writer);
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out T item) {
			try {
				item = _deserializer(byteSize, reader);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}

}