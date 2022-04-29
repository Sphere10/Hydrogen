using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {
	public class BufferAdapter : ExtendedListAdapter<byte>, IBuffer {

        public BufferAdapter()
            : this(new List<byte>()) {
        }

        public BufferAdapter(IList<byte> endpoint) 
            : base(endpoint) {
        }

        public ReadOnlySpan<byte> ReadSpan(int index, int count) {
            return base.ReadRange(index, count).ToArray();
        }

        public void AddRange(ReadOnlySpan<byte> span) {
            base.AddRange((IEnumerable<byte>)span.ToArray());
        }

        public void UpdateRange(int index, ReadOnlySpan<byte> items) {
            base.UpdateRange(index, (IEnumerable<byte>)items.ToArray());
        }

        public void InsertRange(int index, ReadOnlySpan<byte> items) {
            base.InsertRange(index, (IEnumerable<byte>)items.ToArray());
        }

        public Span<byte> AsSpan(int index, int count) {
            throw new NotSupportedException();
        }

        public void ExpandTo(int totalBytes) {
			var newBytes = totalBytes - base.Count;
			if (newBytes > 0)
				ExpandBy(newBytes);
		}

        public void ExpandBy(int newBytes) {
	        for (var i = 0; i < newBytes; i++)
		        base.Add(default);
        }
	}

}