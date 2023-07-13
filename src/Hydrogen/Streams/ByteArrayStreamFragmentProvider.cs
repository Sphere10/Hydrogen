// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
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

	public ReadOnlySpan<byte> GetFragment(long index) {
		var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		return _fragments[fragmentIndexI];
	}

	public void UpdateFragment(long fragmentIndex, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
		var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(fragmentIndex);
		var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);
		updateSpan.CopyTo(_fragments[fragmentIndexI].AsSpan(fragmentPositionI));
	}

	public bool TryMapStreamPosition(long position, out long fragmentIndex, out long fragmentPosition) {
		var fragmentPositionL = position;
		for (fragmentIndex = 0; fragmentIndex < _fragments.Count; fragmentIndex++) {
			var fragmentIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(fragmentIndex);
			var fragmentLength = _fragments[fragmentIndexI].Length;
			if (fragmentPositionL < fragmentLength) {
				fragmentPosition = (int)fragmentPositionL;
				return true;
			}
			fragmentPositionL -= fragmentLength;
		}
		fragmentPosition = (int)fragmentPositionL;
		return false;
	}

	public bool TrySetTotalBytes(long length, out long[] newFragments, out long[] deletedFragments) {
		newFragments = Array.Empty<long>();
		deletedFragments = Array.Empty<long>();

		if (length == TotalBytes)
			return true;

		if (length > TotalBytes)
			return TryGrowSpace(length - TotalBytes, out newFragments);

		return TryReleaseSpace(TotalBytes - length, out deletedFragments);

		bool TryGrowSpace(long bytes, out long[] newFragments) {
			var newFragmentIX = new List<long>();
			while (bytes > 0) {
				var newFragment = _fragmentGenerator();
				_fragments.Add(newFragment);
				TotalBytes += newFragment.Length;
				newFragmentIX.Add(_fragments.Count - 1);
				bytes -= newFragment.Length;
			}
			newFragments = newFragmentIX.ToArray();
			return true;
		}

		bool TryReleaseSpace(long bytes, out long[] releasedFragmentIndexes) {
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
			return true;
		}

	}


}
