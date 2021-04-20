using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public class ByteArrayStreamFragmentProvider : IStreamFragmentProvider {

		private readonly int _newFragmentSize = 10;
		private readonly List<byte[]> _fragments;

		public ByteArrayStreamFragmentProvider(IEnumerable<byte[]> fragments) {
			Guard.ArgumentNotNull(fragments, nameof(fragments));
			_fragments = fragments.ToList();
		}

		public ByteArrayStreamFragmentProvider() {
			_fragments = new List<byte[]>();
		}

		public int Count => _fragments.Count;

		public long Length => _fragments.Sum(x => x.Length);

		public Span<byte> GetFragment(int index) {
			return _fragments[index];
		}

		public (int fragmentIndex, int fragmentPosition) GetFragment(long position, out Span<byte> fragment) {
			fragment = null;
			long remaining = position;

			if (position > Length - 1)
				remaining = 0;

			for (int i = 0; i < _fragments.Count; i++) {
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
			int fragmentsRequired = (int)Math.Floor((decimal)bytes / _newFragmentSize);
			int partial = bytes % _newFragmentSize;

			var newIndexes = Enumerable.Range(Count, fragmentsRequired).ToList();

			for (int i = 0; i < fragmentsRequired; i++) {
				_fragments.Add(new byte[_newFragmentSize]);
			}

			if (partial > 0) {
				_fragments.Add(new byte[partial]);
				newIndexes.Add(Count - 1);
			}

			newFragmentIndexes = newIndexes.ToArray();

			return true;
		}

		public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
			int remaining = bytes;
			List<int> releasedFragments = new List<int>();

			while (remaining > 0) {
				var last = _fragments[^1];
				int removeFragmentCount = Math.Min(last.Length, remaining);

				if (removeFragmentCount >= last.Length) {
					releasedFragments.Add(_fragments.Count - 1);
					_fragments.RemoveAt(_fragments.Count - 1);
				} else {
					byte[] trimmed = new byte[last.Length - removeFragmentCount];
					Buffer.BlockCopy(last, 0, trimmed, 0, trimmed.Length);
					_fragments[^1] = trimmed;
				}

				remaining -= removeFragmentCount;
			}

			releasedFragmentIndexes = releasedFragments.ToArray();
			return bytes;
		}

		public void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan) {
			updateSpan.CopyTo(_fragments[fragmentIndex].AsSpan(fragmentPosition));
		}
	}
}
