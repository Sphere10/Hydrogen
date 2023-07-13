// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Hydrogen.Windows;

public class UnicodeNamedFile {
	public static void Create(string fileName) {
		SafeFileHandle fileHandle = WinAPI.KERNEL32.CreateFileW(
			@"\\?\" + UnicodePath.GetFullPath(fileName),
			FileAccess.Write,
			FileShare.None,
			IntPtr.Zero,
			FileMode.CreateNew,
			FileAttributes.Normal,
			IntPtr.Zero);

		if (fileHandle.IsInvalid) {
			throw new System.ComponentModel.Win32Exception();
		}

		fileHandle.Close();
	}

	public static void Delete(string fileName) {
		if (!WinAPI.KERNEL32.DeleteFileW(@"\\?\" + UnicodePath.GetFullPath(fileName))) {
			throw new System.ComponentModel.Win32Exception();
		}
	}

	public static void Copy(string sourceFileName, string destinationFileName) {
		if (!WinAPI.KERNEL32.CopyFileW(@"\\?\" + UnicodePath.GetFullPath(sourceFileName), @"\\?\" + UnicodePath.GetFullPath(destinationFileName), true)) {
			throw new System.ComponentModel.Win32Exception();
		}
	}

	public static bool Exists(string filename) {
		return UnicodePath.Exists(filename);
	}


}


public class UnicodeNamedDirectory {
	public const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

	public static string[] GetFiles(string path) {
		WinAPI.KERNEL32.WIN32_FIND_DATA findData;
		SafeFindHandle findHandle;
		List<string> files = new List<string>();
		int win32Error;
		string basePath = UnicodePath.GetFullPath(path);
		findHandle = WinAPI.KERNEL32.FindFirstFileW(@"\\?\" + UnicodePath.GetFullPath(path + @"\*"), out findData);

		if (findHandle.IsInvalid) {
			win32Error = Marshal.GetLastWin32Error();
			if (win32Error != WinAPI.KERNEL32.ERROR_NO_MORE_FILES && win32Error != WinAPI.KERNEL32.ERROR_FILE_NOT_FOUND && win32Error != WinAPI.KERNEL32.ERROR_PATH_NOT_FOUND) {
				throw new System.ComponentModel.Win32Exception();
			}
			return files.ToArray();
		}

		try {
			do {
				if ((findData.FileAttributes & FILE_ATTRIBUTE_DIRECTORY) != FILE_ATTRIBUTE_DIRECTORY) {
					files.Add(string.Format("{0}{1}{2}", basePath, Path.DirectorySeparatorChar, findData.FileName));
				}
			} while (WinAPI.KERNEL32.FindNextFileW(findHandle, out findData));

			win32Error = Marshal.GetLastWin32Error();

			if (win32Error != WinAPI.KERNEL32.ERROR_NO_MORE_FILES) {
				throw new System.ComponentModel.Win32Exception(win32Error);
			}
		} finally {
			findHandle.Close();
		}

		return files.ToArray();
	}

	public static string[] GetDirectories(string path) {
		WinAPI.KERNEL32.WIN32_FIND_DATA findData;
		SafeFindHandle findHandle;
		List<string> directories = new List<string>();
		int win32Error;
		string basePath = UnicodePath.GetFullPath(path);

		findHandle = WinAPI.KERNEL32.FindFirstFileW(@"\\?\" + UnicodePath.GetFullPath(path + @"\*"), out findData);

		if (findHandle.IsInvalid) {
			win32Error = Marshal.GetLastWin32Error();
			if (win32Error != WinAPI.KERNEL32.ERROR_NO_MORE_FILES && win32Error != WinAPI.KERNEL32.ERROR_FILE_NOT_FOUND && win32Error != WinAPI.KERNEL32.ERROR_PATH_NOT_FOUND) {
				throw new System.ComponentModel.Win32Exception();
			}
			return directories.ToArray();
		}

		try {
			do {
				if ((findData.FileAttributes & FILE_ATTRIBUTE_DIRECTORY) == FILE_ATTRIBUTE_DIRECTORY) {
					if (findData.FileName != "." && findData.FileName != "..") {
						directories.Add(string.Format("{0}{1}{2}", basePath, Path.DirectorySeparatorChar, findData.FileName));
					}
				}
			} while (WinAPI.KERNEL32.FindNextFileW(findHandle, out findData));

			win32Error = Marshal.GetLastWin32Error();

			if (win32Error != WinAPI.KERNEL32.ERROR_NO_MORE_FILES) {
				throw new System.ComponentModel.Win32Exception(win32Error);
			}
		} finally {
			findHandle.Close();
		}

		return directories.ToArray();
	}

	public static void Create(string path) {
		if (!WinAPI.KERNEL32.CreateDirectoryW(@"\\?\" + UnicodePath.GetFullPath(path), IntPtr.Zero)) {
			throw new System.ComponentModel.Win32Exception();
		}
	}

	public static void Delete(string path) {
		if (!WinAPI.KERNEL32.RemoveDirectoryW(@"\\?\" + UnicodePath.GetFullPath(path))) {
			throw new System.ComponentModel.Win32Exception();
		}
	}

	public static bool Exists(string directory) {
		return UnicodePath.Exists(directory);
	}


}


public class UnicodePath {
	public static string GetFullPath(string path) {
		StringBuilder longPath;
		uint result;

		StringBuilder filePart = null;
		result = WinAPI.KERNEL32.GetFullPathNameW(path, 0, null, out filePart);

		if (0 == result) {
			throw new System.ComponentModel.Win32Exception();
		}

		var longPathSize = result;
		longPath = new StringBuilder((int)longPathSize);

		result = WinAPI.KERNEL32.GetFullPathNameW(path, longPathSize, longPath, out filePart);

		if (0 == result) {
			throw new System.ComponentModel.Win32Exception();
		}

		return longPath.ToString();
	}

	public static bool Exists(string path) {
		WinAPI.KERNEL32.WIN32_FIND_DATA findData;
		SafeFindHandle findHandle;
		List<string> files = new List<string>();
		bool exists = false;
		findHandle = WinAPI.KERNEL32.FindFirstFileW(@"\\?\" + UnicodePath.GetFullPath(path), out findData);
		try {
			if (findHandle.IsInvalid) {
				int lastErrorCode = Marshal.GetLastWin32Error();
				if (lastErrorCode != WinAPI.KERNEL32.ERROR_NO_MORE_FILES && lastErrorCode != WinAPI.KERNEL32.ERROR_FILE_NOT_FOUND && lastErrorCode != WinAPI.KERNEL32.ERROR_PATH_NOT_FOUND) {
					throw new System.ComponentModel.Win32Exception();
				}
			} else {
				exists = true;
			}
		} finally {
			findHandle.Close();
		}
		return exists;
	}
}


public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid {
	private SafeFileHandle()
		: base(true) {
	}

	public SafeFileHandle(IntPtr preexistingHandle, bool ownsHandle)
		: base(ownsHandle) {
		SetHandle(preexistingHandle);
	}


	override protected bool ReleaseHandle() {
		return WinAPI.KERNEL32.CloseHandle(handle);
	}
}


public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid {
	public SafeFindHandle() : base(true) {
	}

	override protected bool ReleaseHandle() {
		return WinAPI.KERNEL32.FindClose(handle);
	}
}
