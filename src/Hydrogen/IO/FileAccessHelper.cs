using System.IO;

namespace Hydrogen;

public static class FileAccessHelper {

	public static FileStream Open(PagedFileDescriptor descriptor, FileAccessMode mode, out bool requiresLoad, out bool shouldFlushOnDispose) {
		var fileExists = File.Exists(descriptor.Path);
		FileStream stream;
		if (mode.IsReadOnly()) {
			if (!fileExists)
				throw new FileNotFoundException(descriptor.Path);
			shouldFlushOnDispose = false;
			stream = File.Open(descriptor.Path, FileMode.Open, FileAccess.Read);
		} else {
			shouldFlushOnDispose = true;
			stream = File.Open(descriptor.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		}
		requiresLoad = fileExists && Tools.FileSystem.GetFileSize(descriptor.Path) > 0;
		return stream;
	}


}
