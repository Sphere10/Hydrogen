using System;
using System.IO;

namespace AbstractProtocol.AnonymousPipeComplex {
    [Serializable]
	public class DirectoryDescriptor : Descriptor {
        public DirectoryDescriptor(DirectoryInfo file) {
            base.Name = file.Name;
            base.CreatedOn = file.CreationTime;
        }

        internal static DirectoryDescriptor GenRandom() {
            var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var folders = Tools.FileSystem.GetSubDirectories(rootFolder);
            return new DirectoryDescriptor(new DirectoryInfo(folders.Length > 0 ? folders[0] : rootFolder));
        }
    }
}
