using System;
using System.CodeDom;

namespace Hydrogen;

[Flags]
public enum FileAccessMode {
	Read = 1 << 0,
	Write = 1 << 1,
	Truncate = 1 << 2,
	Append = 1 << 3,
	AutoLoad = 1 << 4,
	
	OpenOrCreate = Read | Write | Append,
	CreateOrOverwrite = Read | Write | Truncate,
	
	Default = OpenOrCreate | AutoLoad

}


public static class FileAccessModeExtensions {


	public static bool IsReadOnly(this FileAccessMode mode) => mode.HasFlag(FileAccessMode.Read) && !mode.HasFlag(FileAccessMode.Write);

	public static FileAccessMode WithoutAutoLoad(this FileAccessMode accessMode) 
		=> accessMode.CopyAndClearFlags(FileAccessMode.AutoLoad);

}