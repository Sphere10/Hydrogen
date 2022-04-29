using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen {

	public class ByteArrayStreamFragmentProvider : IStreamFragmentProvider {
		public const int DefaultNewFragmentSize = 10;
		private readonly List<byte[]> _fragments;
		private readonly Func<byte[]> _fragmentGenerator;

		public ByteArrayStreamFragmentProvider() : this(DefaultNewFragmentSize) {
		}

		public ByteArrayStreamFragmentProvider(int newFragmentSize)
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

		public int FragmentCount => _fragments.Count;

		public long TotalBytes { get; private set; }

		public ReadOnlySpan<byte> GetFragment(int index) {
			return _fragments[index];
		}

		public void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan)
			=> updateSpan.CopyTo(_fragments[fragmentIndex].AsSpan(fragmentPosition));

		public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
			var fragmentPositionL = position;
			for (fragmentIndex = 0; fragmentIndex < _fragments.Count; fragmentIndex++) {
				var fragmentLength = _fragments[fragmentIndex].Length;
				if (fragmentPositionL < fragmentLength) {
					fragmentPosition = (int)fragmentPositionL;
					return true;
				}
				fragmentPositionL -= fragmentLength;
			}
			fragmentPosition = (int)fragmentPositionL;
			return false;
		}

		public bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments) {
			newFragments = Array.Empty<int>();
			deletedFragments = Array.Empty<int>();

			if (length == TotalBytes)
				return true;

			if (length > TotalBytes)
				return TryGrowSpace(length - TotalBytes, out newFragments);

			return TryReleaseSpace(TotalBytes - length, out deletedFragments);

			bool TryGrowSpace(long bytes, out int[] newFragments) {
				var newFragmentIX = new List<int>();
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

			bool TryReleaseSpace(long bytes, out int[] releasedFragmentIndexes) {
				var releasedFragments = new List<int>();
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
}
