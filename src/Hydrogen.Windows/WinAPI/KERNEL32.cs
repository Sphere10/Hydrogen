// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Hydrogen.Windows;

public static partial class WinAPI {

	public static class KERNEL32 {

		#region Constants

		public const int MAX_PATH = 260;
		public const int ERROR_NO_MORE_FILES = 18;
		public const int ERROR_FILE_NOT_FOUND = 2;
		public const int ERROR_PATH_NOT_FOUND = 3;

		public const uint ES_AWAYMODE_REQUIRED = 0x00000040;
		public const uint ES_CONTINUOUS = 0x80000000;
		public const uint ES_DISPLAY_REQUIRED = 0x00000002;
		public const uint ES_SYSTEM_REQUIRED = 0x00000001;
		public const uint ES_USER_PRESENT = 0x00000004;

		#endregion


		#region Structs

		[StructLayout(LayoutKind.Sequential)]
		public struct FILETIME {
			public uint DateTimeLow;
			public uint DateTimeHigh;
		}


		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct WIN32_FIND_DATA {
			internal UInt32 FileAttributes;
			internal FILETIME CreationTime;
			internal FILETIME LastAccessTime;
			internal FILETIME LastWriteTime;
			internal UInt32 FileSizeHigh;
			internal UInt32 FileSizeLow;
			internal UInt32 Reserved0;
			internal UInt32 Reserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			internal String FileName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			internal String AlternateFileName;
		}

		#endregion

		#region Enums & Flags

		[Flags]
		public enum LoadLibraryFlags : uint {
			DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
			LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
			LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
			LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
			LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
			LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008
		}


		[Flags]
		public enum ErrorModes : uint {
			SYSTEM_DEFAULT = 0x0,
			SEM_FAILCRITICALERRORS = 0x0001,
			SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
			SEM_NOGPFAULTERRORBOX = 0x0002,
			SEM_NOOPENFILEERRORBOX = 0x8000
		}

		#endregion

		#region KERNEL32.DLL Functions

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FreeConsole();

		[DllImport("kernel32", SetLastError = true)]
		public static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, UInt32 nSize);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetModuleFileName(IntPtr hModule, string fileName, UInt32 bufferSize);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern SafeFileHandle CreateFile(string lpFileName, [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes,
		                                               [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeFileHandle CreateFileW(String lpFileName, [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes,
		                                                [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);


		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteFile(string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteFileW(String lpFileName);


		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool CreateDirectoryW(string lpPathName, IntPtr lpSecurityAttributes);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveDirectory(string lpPathName);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RemoveDirectoryW(string lpPathName);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern SafeFindHandle FindFirstFile(string lpFileName, out WIN32_FIND_DATA findData);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeFindHandle FindFirstFileW(string lpFileName, out WIN32_FIND_DATA findData);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool FindNextFile(SafeFindHandle hndFindFile, out WIN32_FIND_DATA findData);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool FindNextFileW(SafeFindHandle hndFindFile, out WIN32_FIND_DATA findData);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FindClose(IntPtr handle);


		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CopyFileW(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);


		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern uint GetFullPathName(string lpFileName, uint nBufferLength, [Out] StringBuilder lpBuffer, out StringBuilder lpFilePart);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern uint GetFullPathNameW(string lpFileName, uint nBufferLength, [Out] StringBuilder lpBuffer, out StringBuilder lpFilePart);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentProcess();


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr LocalFree(IntPtr hMem);


		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, LoadLibraryFlags dwFlags);

		public static IntPtr LoadLibraryEx(string lpfFileName, LoadLibraryFlags flags) {
			return LoadLibraryEx(lpfFileName, IntPtr.Zero, flags);
		}

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);


		[DllImport("kernel32.dll")]
		public static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpType, IntPtr lpName, ushort wLanguage);

		[DllImport("kernel32.dll")]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern IntPtr FindResource(IntPtr hModule, string lpName, int lpType);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

		[DllImport("kernel32.dll")]
		public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);


		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);


		[DllImport("kernel32.dll")]
		public static extern int FreeResource(IntPtr hglbResource);


		[DllImport("kernel32.dll")]
		public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);


		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll")]
		public static extern ErrorModes SetErrorMode(ErrorModes uMode);

		[DllImport("kernel32.dll")]
		public static extern int GetLastError();


		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);


		[DllImport("kernel32.dll")]
		public static extern uint SetThreadExecutionState(uint esFlags);

		#endregion

	}

}
