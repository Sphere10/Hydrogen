using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework {

	public class ByteArrayStreamFragmentProvider : IStreamFragmentProvider {
		public const int DefaultNewFragmentSize  = 10;
		private readonly int _newFragmentSize;
		private readonly List<byte[]> _fragments;
		private int _unusedTipFragmentBytes;
		

		public ByteArrayStreamFragmentProvider(int newFragmentSize = DefaultNewFragmentSize) : this(Enumerable.Empty<byte[]>(), newFragmentSize) {
		}

		public ByteArrayStreamFragmentProvider(IEnumerable<byte[]> fragments, int newFragmentSize = DefaultNewFragmentSize) {
			Guard.ArgumentNotNull(fragments, nameof(fragments));
			_fragments = fragments.ToList();
			_newFragmentSize = newFragmentSize;
			_unusedTipFragmentBytes = 0;
		}

		public int FragmentCount => _fragments.Count;

		public long TotalBytes => TotalAllocatedBytes - _unusedTipFragmentBytes;

		private long TotalAllocatedBytes => _fragments.Sum(x => x.Length);

		public Span<byte> GetFragment(int index) {
			return _fragments[index];
		}

		public (int fragmentIndex, int fragmentPosition) MapLogicalPositionToFragment(long position, out Span<byte> fragment) {
			fragment = null;
			var remaining = position;

			if (position > TotalAllocatedBytes - 1)
				remaining = 0;

			for (var i = 0; i < _fragments.Count; i++) {
				var frag = _fragments[i];

				if (frag.Length > remaining) {
					fragment = frag;
					return (i, (int)remaining);
				}

				remaining -= frag.Length;
			}

			return (-1, -1);
		}


		public bool TryRequestSpace(int bytes, out int[] newFragmentIndexes) {

			var newNeededBytes = Math.Max(0, bytes - _unusedTipFragmentBytes);
			int newFragmentsRequired = (int)Math.Ceiling((decimal)newNeededBytes / _newFragmentSize);
			var newIndexes = Enumerable.Range(FragmentCount, newFragmentsRequired).ToList();
			for (int i = 0; i < newFragmentsRequired; i++) {
				_fragments.Add(new byte[_newFragmentSize]);
			}
			newFragmentIndexes = newIndexes.ToArray();
			_unusedTipFragmentBytes = (_unusedTipFragmentBytes - (bytes - newNeededBytes)) + (_newFragmentSize*newFragmentsRequired - newNeededBytes);
			ClearUnusedTip();
			return true;
		}

		public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
			var toRelease = bytes;
			var releasedFragments = new List<int>();

			// First consume the tip fragment
			while (toRelease > 0 && _fragments.Count > 0) {
				var last = _fragments[^1];
				var usedTipFragmentBytes = last.Length - _unusedTipFragmentBytes;
				Debug.Assert(usedTipFragmentBytes >= 0);
				if (toRelease > usedTipFragmentBytes) {
					toRelease -= usedTipFragmentBytes;
					releasedFragments.Add(_fragments.Count - 1);
					_fragments.RemoveAt(^1);
					_unusedTipFragmentBytes = 0;
				} else {
					usedTipFragmentBytes -= toRelease;
					_unusedTipFragmentBytes += toRelease;
					toRelease = 0;
				}
			}
			
			ClearUnusedTip();
			releasedFragmentIndexes = releasedFragments.ToArray();
			return bytes - toRelease;
		}

		public void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan) {
			updateSpan.CopyTo(_fragments[fragmentIndex].AsSpan(fragmentPosition));
		}

		private void ClearUnusedTip() {
			if (_fragments.Count > 0) {
				var last = _fragments[^1];
				for (var i = last.Length - _unusedTipFragmentBytes; i < last.Length; i++)
					last[i] = default;
			}
		}
	}
}
