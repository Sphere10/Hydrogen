// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

internal abstract class StreamPageBase<TItem> : PageBase<TItem> {
	protected StreamPageBase(StreamPagedList<TItem> parent) {
		Parent = parent;
	}

	public long StartPosition { get; protected set; }

	public abstract long ReadItemBytes(long itemIndex, long byteOffset, long? byteLength, out byte[] bytes);

	public abstract void WriteItemBytes(long itemIndex, long byteOffset, ReadOnlySpan<byte> bytes);

	protected StreamPagedList<TItem> Parent { get; }

	protected Stream Stream => Parent.Stream;

	protected EndianBinaryReader Reader => Parent.Reader;

	protected long ItemSize => Parent.Serializer.ConstantSize;

	protected IItemSerializer<TItem> Serializer => Parent.Serializer;

	protected EndianBinaryWriter Writer => Parent.Writer;
}
