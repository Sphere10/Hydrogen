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

public class ByteArrayStreamFragmentProvider : IStreamFragmentProvider {
	public const int DefaultNewFragmentSize = 10;
	private readonly List<byte[]> _fragments;
	private readonly Func<byte[]> _fragmentGenerator;

	public ByteArrayStreamFragmentProvider() : this(DefaultNewFragmentSize) {
	}

	public ByteArrayStreamFragmentProvider(long newFragmentSize)
		: this(() => new byte[newFragmentSize]) {
	}

	public ByteArrayStreamFragmentProvider(Func<byte[]> fragmentGenerator)
		: this(Enumerable.Empty<byte[]>(), fragmentGenerator) {
	}

	public ByteArrayStreamFragmentProvider(IEnumerable<byte[]> fragments, Func<byte[]> fragmentGenerator) {
		Guard.ArgumentNotNull(fragments, nameof(fragments));
		_fragments = fragments.ToList();
		TotalBytes = fragments.Sum(x => x.Length);
		_fragmentGenerator = fragmentGenerator;
	}

	public long FragmentCount => _fragments.Count;

	public long TotalBytes { get; private set; }

	public ReadOnlySpan<byte> GetFragment(long fragmentID) {
		var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(fragmentID);
		return _fragments[fragmentIndexI];
	}

	public void UpdateFragment(long fragmentID, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
		var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(fragmentID);
		var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);
		updateSpan.CopyTo(_fragments[fragmentIndexI].AsSpan(fragmentPositionI));
	}

	public void MapStreamPosition(long position, out long fragmentID, out long fragmentPosition) {
		fragmentPosition = position;
		for (fragmentID = 0; fragmentID < _fragments.Count; fragmentID++) {
			var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(fragmentID);
			var fragmentLength = _fragments[fragmentIndexI].Length;
			if (position < fragmentLength) {
				fragmentPosition = position;
				break;
			}
			position -= fragmentLength;
		}
	}

	public void SetTotalBytes(long length) {

		if (length == TotalBytes)
			return;

		if (length > TotalBytes) {
			GrowSpace(length - TotalBytes, out _);
			return;
		}

		ReleaseSpace(TotalBytes - length, out _);

		void GrowSpace(long bytes, out long[] newFragments) {
			var newFragmentIX = new List<long>();
			while (bytes > 0) {
				var newFragment = _fragmentGenerator();
				_fragments.Add(newFragment);
				TotalBytes += newFragment.Length;
				newFragmentIX.Add(_fragments.Count - 1);
				bytes -= newFragment.Length;
			}
			newFragments = newFragmentIX.ToArray();
		}

		void ReleaseSpace(long bytes, out long[] releasedFragmentIndexes) {
			var releasedFragments = new List<long>();
			for (var i = _fragments.Count - 1; i >= 0; i--) {
				var fragmentLength = _fragments[i].Length;
				if (bytes >= fragmentLength) {
					releasedFragments.Add(i);
					_fragments.RemoveAt(i);
					TotalBytes -= fragmentLength;
					bytes -= fragmentLength;
				} else {
					// Not releasing a fragment, but need to clear the right tip of it
					Tools.Array.Gen((int)bytes, (byte)0).AsSpan().CopyTo(_fragments[i].AsSpan(^(int)bytes));
					bytes -= bytes;
				}

			}
			releasedFragmentIndexes = releasedFragments.ToArray();
		}

	}
}
