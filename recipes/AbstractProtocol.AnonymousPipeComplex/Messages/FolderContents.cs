using System;
using System.IO;
using System.Linq;
using System.Text;
using Hydrogen;

namespace AbstractProtocol.AnonymousPipeComplex {

    [Serializable]
    public class FolderContents {
		public Descriptor[] Items { get; init; }

        internal static FolderContents GenRandom() {
            var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var folders = Tools.FileSystem.GetSubDirectories(rootFolder).Select(x => new DirectoryInfo(x)).ToArray();
            var files = Tools.FileSystem.GetFiles(rootFolder).Select(x => new FileInfo(x)).ToArray();
            return new FolderContents {
                Items =
                    folders
                        .Take(10)
                        .Select(x => new DirectoryDescriptor(x))
                        .Cast<Descriptor>()
                        .Union(
                            files
                                .Take(10)
                                .Select(x => new FileDescriptor(x))
                        )
                        .Randomize()
                        .ToArray()
            };
        }
    }
}
