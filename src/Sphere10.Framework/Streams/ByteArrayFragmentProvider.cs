using System;
using System.Collections.Generic;
using System.Linq;
using Array = Tools.Array;

namespace Sphere10.Framework {
	public class ByteArrayFragmentProvider : IFragmentProvider {

		private readonly int _newFragmentSize = 10;
		private List<byte[]> _fragments;

		public ByteArrayFragmentProvider(IEnumerable<byte[]> fragments) {
			Guard.ArgumentNotNull(fragments, nameof(fragments));

			_fragments = fragments.ToList();
		}

		public int Count => _fragments.Count;

		public Span<byte> GetFragment(int index) => _fragments[index];

		public bool TryRequestSpace(int bytes, out int[] newFragmentIndexes) {

			int fragmentsRequired = (int)Math.Ceiling((decimal)bytes / _newFragmentSize);
			newFragmentIndexes = Enumerable.Range(Count, fragmentsRequired).ToArray();

			for (int i = 0; i < fragmentsRequired; i++) {
				_fragments.Add(new byte[_newFragmentSize]);
			}

			return true;
		}

		public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
			int toRemove = (int)Math.Floor((decimal)bytes / _newFragmentSize);
			releasedFragmentIndexes = Enumerable.Range(_fragments.Count - toRemove - 1, toRemove).ToArray();
			int remainder = bytes % _newFragmentSize;
			
			_fragments.RemoveRange(Count - toRemove, toRemove);

			if (remainder > 0) {
				byte[] trimmed = new byte[_newFragmentSize - remainder];
				Buffer.BlockCopy(_fragments[^0], 0, trimmed, 0, trimmed.Length);
				_fragments[^0] = trimmed;
			}

			return bytes;
		}
	}
}