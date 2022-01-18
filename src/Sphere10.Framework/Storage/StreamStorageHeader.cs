using System.IO;

namespace Sphere10.Framework {
	public interface IStreamStorageHeader {
		void AttachTo(Stream rootStream, Endianness endianness);
	}
}
