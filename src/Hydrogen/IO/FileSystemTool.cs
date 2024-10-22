// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.FastReflection;

// ReSharper disable CheckNamespace
namespace Tools;

public static class FileSystem {
	public static readonly string DirectorySeparatorString;
	public static readonly char[] CrossPlatformInvalidFileNameChars;
	public static readonly string[] CrossPlatformForbiddenFileNames;
	public const int MaxWindowsFileNameLength = 260;
	public const int MaxLinuxFileNameLength = 255;
	static FileSystem() {
		DirectorySeparatorString = new string(new[] { Path.DirectorySeparatorChar });
		var asciiControlChars = Enumerable.Range(0, 31).Select(x => (char)x).ToArray();
		CrossPlatformInvalidFileNameChars =
			new[] { '<', '>', ':', '\"', '/', '\\', '|', '?', '*' }.Union(asciiControlChars).Union(Path.GetInvalidFileNameChars())
				.ToArray(); // https://stackoverflow.com/questions/1976007/what-characters-are-forbidden-in-windows-and-linux-directory-names
		CrossPlatformForbiddenFileNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", ".", ".." };
	}


	public static bool DoPathsReferToSameFileName(string path1, string path2) {
		
		// Get the filename from the first path by splitting on '/' and '\'
		string fileName1 = GetFileNameFromPath(path1);

		// Get the filename from the second path by splitting on '/' and '\'
		string fileName2 = GetFileNameFromPath(path2);

		// Compare the filenames, ignoring case
		return fileName1.Equals(fileName2, StringComparison.OrdinalIgnoreCase);

		string GetFileNameFromPath(string path) {
			// Split by both '/' and '\' to handle URLs and local file paths
			char[] separators = new char[] { '/', '\\' };
			string[] tokens = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			// Return the last token, which should be the filename
			return tokens[^1];
		}
	}

	public static IDisposable MonitorFile(string filePath, Action<WatcherChangeTypes, string> handler) {
		var watcher = new FileSystemWatcher();
		watcher.Path = Path.GetDirectoryName(filePath);
		watcher.Filter = Path.GetFileName(filePath);
		watcher.EnableRaisingEvents = true;
		watcher.Changed += (_, args) => { handler.Invoke(args.ChangeType, filePath); };
		watcher.Deleted += (_, args) => { handler.Invoke(WatcherChangeTypes.Deleted, filePath); };
		watcher.Created += (_, args) => { handler.Invoke(WatcherChangeTypes.Created, filePath); };
		watcher.Renamed += (_, args) => { handler.Invoke(WatcherChangeTypes.Renamed, filePath); };

		return new Disposables(watcher);
	}

	public static byte[] CalculateContentHash(string filename, CHF chf) {
		using var fileStream = File.OpenRead(filename);
		using var hashingStream = new HashingStream(chf);
		Tools.Streams.RouteStream(fileStream, hashingStream);
		return hashingStream.GetDigest();
	}

	public static void TruncateFile(string filename, long size) {
		using (var stream = File.OpenWrite(filename)) {
			if (stream.Length != size)
				stream.SetLength(size);
		}
	}

	public static string ToValidFolderOrFilename(string filename) {
		Guard.ArgumentNotNullOrWhitespace(filename, nameof(filename));
		var result = new string(filename.Where(c => !CrossPlatformInvalidFileNameChars.Contains(c)).ToArray());
		if (result.ToUpperInvariant().IsIn(CrossPlatformForbiddenFileNames))
			result += "_";
		return result;
	}

	public static string ToAbsolutePathIfRelative(string path)
		=> ToAbsolutePathIfRelative(path, Environment.CurrentDirectory);

	public static string ToAbsolutePathIfRelative(string path, string baseFolder) {
		Guard.ArgumentNotNull(path, nameof(path));
		Guard.ArgumentNotNullOrWhitespace(baseFolder, nameof(baseFolder));
		if (string.IsNullOrWhiteSpace(path))
			return baseFolder;

		if (!Path.IsPathFullyQualified(path))
			return Path.Combine(baseFolder, path);
		return path;
	}

	public static bool IsWellFormedDirectoryPath(string path) {
		if (String.IsNullOrWhiteSpace(path))
			return false;

		var driveCheck = new Regex(@"^[a-zA-Z]:\\$");
		if (!driveCheck.IsMatch(path.Substring(0, 3)))
			return false;
		var strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
		strTheseAreInvalidFileNameChars += @":/?*" + "\"";
		var containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
		if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
			return false;

		return true;
	}

	public static bool IsWellFormedFileName(string fileName) {
		if (fileName.Length > MaxLinuxFileNameLength)
			return false;

		var invalidFileNameChars = new string(CrossPlatformInvalidFileNameChars);
		var containsABadCharacter = new Regex("[" + Regex.Escape(invalidFileNameChars) + "]");
		if (containsABadCharacter.IsMatch(fileName))
			return false;
		return true;
	}

	public static string ToWellFormedPath(string path) {
		foreach (var c in CrossPlatformInvalidFileNameChars)
			path = path.Replace(c.ToString(), String.Empty);
		return path.IsIn(CrossPlatformForbiddenFileNames) ? "_" : path;
	}

	public static string ToWellFormedFileName(string path) {
		path = ToWellFormedPath(path);
		if (path.Length > MaxLinuxFileNameLength)
			path.Substring(0, MaxLinuxFileNameLength);
		return path;
	}

	public static string GetParentDirectoryPath(string path, int parentLevel = 1) {
		for (var i = 0; i < parentLevel; i++)
			path = Path.GetDirectoryName(path);
		return path;
	}

	public static string GetParentDirectoryName(string path) {
		var splits = path.Split(Path.DirectorySeparatorChar);
		return splits.Length < 2 ? null : splits[^2];
	}

	public static string GetRelativePath(string basePath, string absolutePath) {
		char separator = Path.DirectorySeparatorChar;
		if (String.IsNullOrEmpty(basePath))
			basePath = Directory.GetCurrentDirectory();
		var returnPath = "";
		var commonPart = "";
		var basePathFolders = basePath.Split(separator);
		var absolutePathFolders = absolutePath.Split(separator);
		var i = 0;
		while (i < basePathFolders.Length & i < absolutePathFolders.Length) {
			if (basePathFolders[i].ToLower() == absolutePathFolders[i].ToLower()) {
				commonPart += basePathFolders[i] + separator;
			} else {
				break;
			}

			i += 1;
		}

		if (commonPart.Length > 0) {
			var parents = basePath.Substring(commonPart.Length - 1).Split(separator);
			foreach (var parentDir in parents) {
				if (!String.IsNullOrEmpty(parentDir))
					returnPath += ".." + separator;
			}
		}

		returnPath += absolutePath.Substring(commonPart.Length);
		return returnPath;
	}

	/// <summary>
	/// Resolves tokens within a path string 
	/// </summary>
	/// <param name="pathTemplate"></param>
	/// <returns></returns>
	public static string ResolvePathTemplate(string pathTemplate) {
		return ResolvePathTemplate(pathTemplate, DefaultPathTokenResolver);
	}

	public static string ResolvePathTemplate(string pathTemplate, Func<string, string> tokenResolver) {
		return StringFormatter.FormatEx(pathTemplate, (token) => tokenResolver(token) ?? DefaultPathTokenResolver(token), true);
	}

	public static string DefaultPathTokenResolver(string token) {
		switch (token) {
			case "\\":
			case "/":
				return DirectorySeparatorString;
			case string specialFolder
				when typeof(Environment.SpecialFolder).FastGetEnumNames().Contains(specialFolder):
				return Environment.GetFolderPath(Enum.Parse<Environment.SpecialFolder>(specialFolder));
		}

		return token;
	}

	public static string GenerateTempFilename(string ext = null) {
		return Path.Combine(
			Path.GetTempPath(),
			Guid.NewGuid().ToStrictAlphaString() + (ext != null ? $".{ext.TrimStart('.')}" : String.Empty)
		);
	}

	public static string GetCaseCorrectFilePath(string filepath) {
		if (String.IsNullOrWhiteSpace(filepath))
			return String.Empty;
		return new FileInfo(filepath).FullName;
	}

	public static string GetCaseCorrectDirectoryPath(string dirpath) {
		if (String.IsNullOrWhiteSpace(dirpath))
			return String.Empty;
		return new DirectoryInfo(dirpath).FullName;
	}

	public static long GetFileSize(string filePath) {
		return new FileInfo(filePath).Length;
	}

	public static bool IsFileEmpty(string file) {
		return new FileInfo(file).Length == 0;
	}

	public static string DetermineAvailableFileName(string filePath)
		=> DetermineAvailableFileName(Tools.FileSystem.GetParentDirectoryPath(filePath), Path.GetFileName(filePath));

	public static string DetermineAvailableFileName(string directoryPath, string desiredFileName) {
		var desiredPath = Path.Combine(directoryPath, desiredFileName);

		// If parent folder exists, then desired filepath is available
		if (!Directory.Exists(directoryPath))
			return desiredPath;

		// If file does not exist in parent folder, filepath is available
		if (!File.Exists(desiredPath))
			return desiredPath;

		// Try variants of desired filename until a free one is found
		var fileName = Path.GetFileNameWithoutExtension(desiredFileName);
		var ext = Path.GetExtension(desiredFileName);
		for (uint i = 2; i <= UInt32.MaxValue; i++) {
			var candidatePath = Path.Combine(directoryPath, String.Format("{0} {1}{2}", fileName, i, ext));
			if (!File.Exists(candidatePath))
				return candidatePath;
		}

		// Weird folder! User should not be using computers.
		throw new SoftwareException("Too many files starting with '{0}' found in folder '{1}'",
			fileName,
			directoryPath);
	}

	public static void CreateBlankFile(string filename, bool createDirectories = false) {
		string dir = Path.GetDirectoryName(filename);
		Debug.Assert(dir != null, "dir != null");
		if (createDirectories)
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		File.WriteAllBytes(filename, new byte[0]);
	}

	public static Task CreateBlankFileAsync(string filename, bool createDirectories = false) {
		return Task.Run(() => CreateBlankFile(filename, createDirectories));
	}

	public static void CopyFile(string sourcePath, string destPath, bool overwrite = false, bool createDirectories = false) {
		var dir = Path.GetDirectoryName(destPath);
		if (createDirectories)
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

		File.Copy(sourcePath, destPath, overwrite);
	}

	public static void DeleteFile(string path)
		=> File.Delete(path);

	public static void DeleteFileThenEmptyDir(string path, bool recurseEmptyDirs = false) {
		DeleteFile(path);
		var parentPath =  GetParentDirectoryPath(path);
		DeleteEmptyDirectory(parentPath, recurseEmptyDirs);
	}


	public static async Task DeleteFileAsync(string path) {
		await Task.Run(() => DeleteFile(path));
	}

	public static async Task CopyFileAsync(string sourcePath, string destPath, bool overwrite = false, bool createDirectories = false) {
		await Task.Run(() => CopyFile(sourcePath, destPath, overwrite, createDirectories));
	}

	public static void RenameFile(string sourcePath, string newName) {
		var fileInfo = new FileInfo(sourcePath);
		fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
	}

	public static void CreateDirectory(string directory)
		=> Directory.CreateDirectory(directory);

	public static Task CreateDirectoryAsync(string directory)
		=> Task.Run(() => CreateDirectory(directory));

	//https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories#example
	public static void CopyDirectory(string sourceDir, string destDirectory, bool copySubDirectories = false, bool createIfDoesNotExist = true, bool clearIfNotEmpty = false) {

		// Get the subdirectories for the specified directory.
		var sourceDirInfo = new DirectoryInfo(sourceDir);
		if (!sourceDirInfo.Exists) {
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDir);
		}

		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirectory)) {
			if (createIfDoesNotExist)
				Directory.CreateDirectory(destDirectory);
			else
				throw new DirectoryNotFoundException(
					"Destination directory does not exist or could not be found: " +
					destDirectory);
		}

		var sourceSubDirs = sourceDirInfo.GetDirectories();

		// Get the files in the directory and copy them to the new location.
		var files = sourceDirInfo.GetFiles();
		foreach (var file in files) {
			var tempPath = Path.Combine(destDirectory, file.Name);
			file.CopyTo(tempPath, false);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirectories) {
			foreach (var subDir in sourceSubDirs) {
				var tempPath = Path.Combine(destDirectory, subDir.Name);
				CopyDirectory(subDir.FullName, tempPath, true);
			}
		}
	}

	public static async Task CopyDirectoryAsync(string sourceDir, string destDirectory,
	                                            bool copySubDirectories = false,
	                                            bool createIfDoesNotExist = true, bool clearIfNotEmpty = false) {
		await Task.Run(() =>
			CopyDirectory(sourceDir, destDirectory, copySubDirectories, createIfDoesNotExist, clearIfNotEmpty));
	}

	public static void DeleteDirectory(string directory, bool ignoreIfLocked = false) {
		Action<string> deleteDirAction = ignoreIfLocked
			? Lambda.ActionIgnoringExceptions<string>(Directory.Delete)
			: x => Directory.Delete(x, true);
		DeleteAllFiles(directory, true, true);
		if (Directory.GetFiles(directory).Length == 0) {
			deleteDirAction(directory);
		}
	}

	public static void DeleteDirectories(params string[] directories) {
		DeleteDirectories(false, directories);
	}

	public static Task DeleteDirectoriesAsync(params string[] directories)
		=> DeleteDirectoriesAsync(false, directories);

	public static void DeleteDirectories(bool ignoreIfLocked, params string[] directories) {
		foreach (var dir in directories)
			DeleteDirectory(dir, ignoreIfLocked);
	}

	public static async Task DeleteDirectoriesAsync(bool ignoreIfLocked, params string[] directories) {
		foreach (var dir in directories)
			await DeleteDirectoryAsync(dir, ignoreIfLocked);
	}

	public static Task DeleteDirectoryAsync(string directory, bool ignoreIfLocked = false) {
		return Task.Run(() => DeleteDirectory(directory, ignoreIfLocked));
	}

	public static void DeleteEmptyDirectory(string directory, bool recurseSubDirs = false) {
		if (!Directory.Exists(directory))
			return;

		if (recurseSubDirs) {
			foreach(var subDir in GetSubDirectories(directory, FileSystemPathType.Absolute)) {
				DeleteEmptyDirectory(subDir, recurseSubDirs);
			}
		}

		if (GetDirectoryContents(directory, out _, out _) == 0)
			DeleteDirectory(directory);
	}


	public static void DeleteAllFiles(string directory, bool deleteSubDirectories = true,
	                                  bool ignoreIfLocked = false) {
		Action<string> deleteFileAction =
			ignoreIfLocked ? Lambda.ActionIgnoringExceptions<string>(File.Delete) : File.Delete;
		Action<string> deleteDirAction = ignoreIfLocked
			? Lambda.ActionIgnoringExceptions<string>(Directory.Delete)
			: Directory.Delete;
		foreach (var file in Directory.GetFiles(directory))
			deleteFileAction(file);

		if (deleteSubDirectories) {
			foreach (var subDirectory in Directory.GetDirectories(directory)) {
				DeleteAllFiles(subDirectory, true, ignoreIfLocked);
				deleteDirAction(subDirectory);
			}
		}
	}
	public static Task DeleteAllFilesAsync(string directory, bool deleteSubDirectories = true,
	                                       bool ignoreIfLocked = false) {
		return Task.Run(() => DeleteAllFiles(directory, deleteSubDirectories, ignoreIfLocked));
	}

	public static void AppendAllBytes(string path, byte[] bytes) {
		Guard.ArgumentNotNull(path, nameof(path));
		Guard.ArgumentNotNull(bytes, nameof(bytes));
		using (var stream = new FileStream(path, FileMode.Append)) {
			stream.Write(bytes, 0, bytes.Length);
		}
	}

	public static void AppendAllBytes(string path, Stream bytes) {
		Guard.ArgumentNotNull(path, nameof(path));
		Guard.ArgumentNotNull(bytes, nameof(bytes));
		using (var stream = new FileStream(path, FileMode.Append)) {
			bytes.RouteTo(stream);
		}
	}

	public static void WriteAllBytes(string path, byte[] bytes) {
		Guard.ArgumentNotNull(path, nameof(path));
		Guard.ArgumentNotNull(bytes, nameof(bytes));
		using (var stream = new FileStream(path, FileMode.Truncate)) {
			stream.Write(bytes, 0, bytes.Length);
		}
	}

	public static void WriteAllBytes(string path, Stream bytes) {
		Guard.ArgumentNotNull(path, nameof(path));
		Guard.ArgumentNotNull(bytes, nameof(bytes));
		using (var stream = new FileStream(path, FileMode.Truncate)) {
			bytes.RouteTo(stream);
		}
	}

	public static byte[] GetFilePart(string filePath, long offset, int fetchSize) {
		if (!File.Exists(filePath))
			throw new FileNotFoundException("File not found", filePath);

		var fileInfo = new FileInfo(filePath);
		if (offset > fileInfo.Length)
			throw new ArgumentOutOfRangeException("offset",
				"Offset '{0}' larger than file size '{1}'".FormatWith(offset, fileInfo.Length));

		using (var reader = File.OpenRead(filePath)) {
			reader.Seek(offset, SeekOrigin.Begin);
			var bytes = new byte[fetchSize];
			var bytesRead = reader.Read(bytes, 0, fetchSize);
			System.Array.Resize(ref bytes, bytesRead);
			return bytes;
		}
	}

	public static void CompressFile(string sourcePath, string destPath, string password = null) {
		CompressFile<AesManaged>(sourcePath, destPath, password);
	}

	public static void DecompressFile(string sourcePath, string destPath, string password = null) {
		DecompressFile<AesManaged>(sourcePath, destPath, password);
	}

	public static void CompressFile<TSymmetricAlgorithm>(string sourcePath, string destPath,
	                                                     string password = null,
	                                                     PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		var hasPassword = !String.IsNullOrEmpty(password);
		Action<Stream, Stream> compressor = Streams.GZipCompress;
		Action<Stream, Stream> encryptor = (source, dest) =>
			Streams.Encrypt<TSymmetricAlgorithm>(source, dest, password, null, paddingMode, cipherMode);
		Action<Stream, Stream> noop = (source, dest) => Streams.RouteStream(source, dest);
		using (var sourceStream = File.OpenRead(sourcePath))
		using (var destStream = File.OpenWrite(destPath))
		using (var streamPipeline = new StreamPipeline(compressor, hasPassword ? encryptor : noop)) {
			streamPipeline.Run(sourceStream, destStream);
		}
	}

	public static void DecompressFile<TSymmetricAlgorithm>(string sourcePath, string destPath,
	                                                       string password = null, PaddingMode paddingMode = PaddingMode.PKCS7,
	                                                       CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		var hasPassword = !String.IsNullOrEmpty(password);
		Action<Stream, Stream> decryptor = (source, dest) =>
			Streams.Decrypt<TSymmetricAlgorithm>(source, dest, password, null, paddingMode, cipherMode);
		Action<Stream, Stream> decompressor = Streams.GZipDecompress;
		Action<Stream, Stream> noop = (source, dest) => Streams.RouteStream(source, dest);
		using (var sourceStream = File.OpenRead(sourcePath))
		using (var destStream = File.OpenWrite(destPath))
		using (var streamPipeline = new StreamPipeline(hasPassword ? decryptor : noop, decompressor))
			streamPipeline.Run(sourceStream, destStream);
	}

	public static string GetTempEmptyDirectory(bool create = true) {
		var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToStrictAlphaString());
		if (create)
			Directory.CreateDirectory(path);
		return path;
	}

	public static string GetTempFileName(bool create = true) {
		return GetTempFileName(Path.GetTempPath(), create);
	}

	public static string GetTempFileName(string basePath, bool create = true) {
		var path = Path.Combine(basePath, Guid.NewGuid().ToStrictAlphaString() + ".tmp");
		if (create)
			CreateBlankFile(path, false);
		return path;
	}

	public static string[] GetSubDirectories(string directory, FileSystemPathType pathType = FileSystemPathType.Relative) {
		var subDirs = Directory.EnumerateDirectories(directory);

		if (pathType == FileSystemPathType.Relative)
			subDirs = subDirs.Select(Path.GetFileName);

		return subDirs.ToArray();
	}

	public static IEnumerable<string> GetFiles(string directory, string pattern = "*", bool recursive = false, FileSystemPathType pathType = FileSystemPathType.Relative) {
		var files = Directory.EnumerateFiles(directory, pattern);

		if (pathType == FileSystemPathType.Relative)
			files = files.Select(Path.GetFileName);

		if (recursive) {
			files = files.Concat(
				Directory
					.EnumerateDirectories(directory)
					.SelectMany(
						subDir =>
							GetFiles(subDir, pattern, recursive, pathType)
								.Select(
									subDirItem => pathType switch {
										FileSystemPathType.Relative => Path.Join(Path.GetRelativePath(directory, subDir), subDirItem),
										FileSystemPathType.Absolute => subDirItem,
										_ => throw new NotSupportedException(pathType.ToString())
									}
								)
					)
			);
		}

		return files;
	}

	public static int GetDirectoryContents(string directory, out string[] files, out string[] folders) {
		files = Directory.GetFiles(directory);
		folders = Directory.GetDirectories(directory);
		return files.Length + folders.Length;
	}

	public static bool DirectoryContainsFiles(string directory, params string[] filenames) {
		var files = new HashSet<string>(GetFiles(directory));
		return filenames.All(files.Contains);
	}

	public static bool IsDirectoryEmpty(string getParentDirectoryPath)
		=> CountDirectoryContents(getParentDirectoryPath) == 0;

	public static int CountDirectoryContents(string directory)
		=> GetDirectoryContents(directory, out _, out _);

	public static void SplitFilePath(string filepath, out string folder, out string fileName) {
		folder = Path.GetDirectoryName(filepath);
		fileName = Path.GetFileName(filepath);
	}
}
