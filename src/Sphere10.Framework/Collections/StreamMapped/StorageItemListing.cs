using System.Runtime.InteropServices;

namespace Sphere10.Framework.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StorageItemListing
    {
        public int StartIndex { get; set; }

        public int Size { get; set; }
    }

}