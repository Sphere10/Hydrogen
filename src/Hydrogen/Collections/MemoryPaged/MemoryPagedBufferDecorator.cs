// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen {
	public abstract class MemoryPagedBufferDecorator<TMemoryPagedBuffer> : MemoryPagedListDecorator<byte, TMemoryPagedBuffer>, IMemoryPagedBuffer
		where TMemoryPagedBuffer : IMemoryPagedBuffer {

		protected MemoryPagedBufferDecorator(TMemoryPagedBuffer internalBuffer)
            : base(internalBuffer) {
        }

        IReadOnlyList<IBufferPage> IMemoryPagedBuffer.Pages => InternalCollection.Pages;

        public virtual void AddRange(ReadOnlySpan<byte> span) => InternalCollection.AddRange(span);

        public virtual Span<byte> AsSpan(int index, int count) => InternalCollection.AsSpan(index, count);

        public virtual void InsertRange(int index, ReadOnlySpan<byte> items) => InternalCollection.InsertRange(index, items);

        public virtual ReadOnlySpan<byte> ReadSpan(int index, int count) => InternalCollection.ReadSpan(index, count);

        public virtual void UpdateRange(int index, ReadOnlySpan<byte> items) => InternalCollection.UpdateRange(index, items);

        public virtual void ExpandTo(int totalBytes) => InternalCollection.ExpandTo(totalBytes);

        public virtual void ExpandBy(int newBytes) => InternalCollection.ExpandBy(newBytes);

	}

	public abstract class MemoryPagedBufferDecorator : MemoryPagedBufferDecorator<IMemoryPagedBuffer> {
		protected MemoryPagedBufferDecorator(IMemoryPagedBuffer internalBuffer)
			: base(internalBuffer) {
		}
	}

}