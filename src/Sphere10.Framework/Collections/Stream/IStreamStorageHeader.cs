using System.IO;

namespace Sphere10.Framework {
	public interface IStreamStorageHeader {
		void AttachTo(Stream rootStream, Endianness endianness);
		byte Version { get; set; } 
		int RecordsCount { get; set; }
	}
}
