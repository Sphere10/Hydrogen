using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sphere10.Framework.Collections {

	// this is like a StreamMappedList except it maps the object over non-contiguous sectors instead of a contiguous stream.
	// It uses a StreamMappedList of sectors under the hood.
	public class SectorMappedList<T> : RangedListBase<T> {
		private readonly IObjectSerializer<T> _serializer;
		private readonly IExtendedList<Sector> _sectors;
		public SectorMappedList(int sectorSize, IObjectSerializer<T> serializer, Stream stream) {
			_sectors = new StreamMappedList<Sector>(1, new SectorSerialier(sectorSize), stream);
			_serializer = serializer;
		}
		public override int Count => throw new NotImplementedException();

		public override void AddRange(IEnumerable<T> items) {
			throw new NotImplementedException();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			throw new NotImplementedException();
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			throw new NotImplementedException();
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			throw new NotImplementedException();
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			throw new NotImplementedException();
		}

		public override void RemoveRange(int index, int count) {
			throw new NotImplementedException();
		}
	}

}
