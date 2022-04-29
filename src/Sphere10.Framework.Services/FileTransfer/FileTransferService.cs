//-----------------------------------------------------------------------
// <copyright file="FileTransferService.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Sphere10.Framework.Services {


    public class FileTransferService : IFileTransferService {
        private const int DefaultMaxReadFileChunk = 2048 * 100;
        private readonly IDictionary<string, bool> _compressedRegistry;
        private readonly string _publishedFilesFolder;

        public FileTransferService(int maxReadChunk = DefaultMaxReadFileChunk, string publishedFilesFolder = null) {
            MaxReadFileChunk = maxReadChunk;
            LocalStore = CreateFileStore();
            _compressedRegistry = new Dictionary<string, bool>();
            _publishedFilesFolder = publishedFilesFolder;
        }

        public FileStore LocalStore { get; set; }

        public int MaxReadFileChunk { get; set; }

        public void RegisterFile(string alias, bool compressed) {
            LocalStore.RegisterFile(alias);
            _compressedRegistry[alias] = compressed;

        }

        public void SendFilePart(string alias, byte[] filePart) {
            LocalStore.AppendAllBytes(alias, filePart);
        }

        public void Publish(string alias, string overrideFileName = null, string additionalData = null) {
            var publishedFolder = GetFolderForPublishedFiles(alias, overrideFileName, additionalData);
            if (string.IsNullOrEmpty(publishedFolder)) {
                throw new NotSupportedException("Publishing files is not supported on this file transfer service");
            }

            var filename = alias ?? overrideFileName;

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("alias");

            filename = filename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            var path = Path.Combine(publishedFolder, overrideFileName ?? alias);
            Tools.FileSystem.CopyFile(this.LocalStore.GetFilePath(alias), path, overwrite:true, createDirectories: true);
        }

        public byte[] GetFilePart(string alias, long offset, int fetchSize) {
            if (fetchSize > MaxReadFileChunk) {
                throw new SoftwareException("Unable to read more than '{0}' bytes from a file. Client requested '{1}'.", MaxReadFileChunk, fetchSize);
            }
            var fileInfo = LocalStore.GetInfo(alias);
            if (offset > fileInfo.Length)
                throw new ArgumentOutOfRangeException("offset", "Offset '{0}' larger than file size '{1}'".FormatWith(offset, fileInfo.Length));

            using (var reader = LocalStore.OpenRead(alias)) {
                reader.Seek(offset, SeekOrigin.Begin);
                var bytes = new byte[fetchSize];
                var bytesRead = reader.Read(bytes, 0, fetchSize);
                Array.Resize(ref bytes, bytesRead);
                return bytes;
            }
        }


        public long GetFileSize(string alias) {
            return Tools.FileSystem.GetFileSize(LocalStore.GetFilePath(alias));
        }


        public string SaveObjectForDownload<T>(T @object) {
            var fileAlias = LocalStore.NewFile();
            using (var streamWriter = new StreamWriter(LocalStore.OpenWrite(fileAlias))) {
                Tools.Xml.Write(@object, Encoding.Unicode, streamWriter);
            }
            return fileAlias;
        }

        public T GetUploadedObject<T>(string alias) {
            using (var streamReader = new StreamReader(LocalStore.OpenRead(alias))) {
                return Tools.Xml.Read<T>(streamReader);
            }
        }

        public string[] GetFiles() {
            return LocalStore.FileAliases.ToArray();
        }

        public virtual void Dispose() {
            LocalStore.Dispose();
        }

        protected virtual FileStore CreateFileStore() {
            return new TempFileStore();
        }

        protected virtual string GetFolderForPublishedFiles(string alias, string overrideFileName = null, string additionalData = null) {
            return _publishedFilesFolder;
        }

    }
}
