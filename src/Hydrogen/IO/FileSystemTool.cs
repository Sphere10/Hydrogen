//-----------------------------------------------------------------------
// <copyright file="FileSystemTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

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
namespace Tools {
    public static class FileSystem {
        public readonly static string DirectorySeparatorString;


        static FileSystem() {
            DirectorySeparatorString = new string(new[] { Path.DirectorySeparatorChar });
        }

        public static void TruncateFile(string filename, long size) {
            using (var stream = File.OpenWrite(filename)) {
                if (stream.Length != size)
                    stream.SetLength(size);
            }
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
            var invalidFIleNameChars = new string(Path.GetInvalidFileNameChars());
            invalidFIleNameChars += @":/?*" + "\"";
            var containsABadCharacter = new Regex("[" + Regex.Escape(invalidFIleNameChars) + "]");

            if (containsABadCharacter.IsMatch(fileName))
                return false;
            else
                return true;
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
            return StringFormatter.FormatEx(pathTemplate, (token) => tokenResolver(token) ?? DefaultPathTokenResolver(token));
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
                Guid.NewGuid().ToStrictAlphaString(),
                ext != null ? (ext.StartsWith(".") ? ext : "." + ext) : String.Empty);
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

        public static string DetermineAvailableFileName(string directoryPath, string desiredFileName) {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(directoryPath);

            // Try desired filename
            var desiredPath = Path.Combine(directoryPath, desiredFileName);
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
            throw new SoftwareException("Too many files starting with '{0}' found in folder '{1}'", fileName,
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

        public static void CopyFile(string sourcePath, string destPath, bool overwrite = false,
            bool createDirectories = false) {
            var dir = Path.GetDirectoryName(destPath);
            if (createDirectories)
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

            File.Copy(sourcePath, destPath, overwrite);
        }

        public static void RenameFile(string sourcePath, string newName) {
            var fileInfo = new FileInfo(sourcePath);
            fileInfo.MoveTo(Path.Combine( fileInfo.Directory.FullName, newName));
        }

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
                : Directory.Delete;
            DeleteAllFiles(directory, true, true);
            if (Directory.GetFiles(directory).Length == 0) {
                deleteDirAction(directory);
            }
        }

        public static void DeleteDirectories(params string[] directories) {
            DeleteDirectories(false, directories);
        }


        public static void DeleteDirectories(bool ignoreIfLocked, params string[] directories) {
            foreach (var dir in directories)
                DeleteDirectory(dir, ignoreIfLocked);
        }

        public static Task DeleteDirectoryAsync(string directory, bool ignoreIfLocked = false) {
            return Task.Run(() => DeleteDirectory(directory, ignoreIfLocked));
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

        public static string[] GetSubDirectories(string directory) {
            return Directory.EnumerateDirectories(directory).Select(Path.GetFileName).ToArray();
        }

        public static string[] GetFiles(string directory) {
            return Directory.EnumerateFiles(directory).Select(Path.GetFileName).ToArray();
        }

        public static string[] GetFiles(string directory, string pattern) {
            return Directory.EnumerateFiles(directory, pattern).Select(Path.GetFileName).ToArray();
        }

        public static bool DirectoryContainsFiles(string directory, params string[] filenames) {
            var files = new HashSet<string>(GetFiles(directory));
            return filenames.All(files.Contains);
        }
    }
}