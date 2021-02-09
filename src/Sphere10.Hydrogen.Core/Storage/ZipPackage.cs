using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.DataObjects;


namespace Sphere10.Hydrogen.Core.Storage {

    public class ZipPackage : KeyValueStoreBase<string> {
        protected FileStream _stream;
        protected ZipArchive _archive;

        public ZipPackage(string filename) {
            Filename = filename;
        }
        
        public virtual string Name {
            get => Path.GetFileNameWithoutExtension(Filename);
            set => Tools.FileSystem.RenameFile(Filename, value);
        }

        public string Filename { get; set; }

        public void ExtractTo(string directory, bool overwrite = false) {
            EnsureReadable();
            if (overwrite && Directory.Exists(directory))
                Tools.FileSystem.DeleteAllFiles(directory, true);
            _archive.ExtractToDirectory(directory);
        }

        protected override Stream OpenReadInternal(string key) {
            var entry = _archive.GetEntry(key);
            return entry.Open();
        }

        protected override IQueryable<string> GetKeysInternal() {
            return _archive.Entries.Select(x => x.FullName).AsQueryable();
        }

        protected override Stream OpenWriteInternal(string key) {
            ZipArchiveEntry entry;
            switch (_archive.Mode) {
                case ZipArchiveMode.Create:
                    entry = _archive.CreateEntry(key);
                    break;
                case ZipArchiveMode.Update:
                    entry = _archive.GetEntry(key);
                    if (entry == null)
                        entry = _archive.CreateEntry(key);
                    break;
                default:
                    entry = _archive.GetEntry(key);
                    break;
            }
            return entry.Open();
        }

        protected override void OnSetupRead() {
            base.OnSetupRead();
            if (!File.Exists(Filename))
                throw new FileNotFoundException("Package not found", Filename);
            _stream = File.OpenRead(Filename);
            _archive = new ZipArchive(_stream, ZipArchiveMode.Read);
        }

        protected override void OnSetupWrite() {
            base.OnSetupWrite();
            var exists = File.Exists(Filename);
            _stream = File.Open(Filename, FileMode.OpenOrCreate);
            _archive = new ZipArchive(_stream, exists ? ZipArchiveMode.Update : ZipArchiveMode.Create);
        }

        protected override void OnCleanupRead() {
            base.OnCleanupRead();
            _archive.Dispose();
            _stream.Dispose();
        }

        protected override void OnCleanupWrite() {
            base.OnCleanupRead();
            _stream.Flush(true);
            _archive.Dispose();
            _stream.Dispose();
        }

        public static ZipPackage Create(string filename) {
            var package = new ZipPackage(filename);
            using (package.EnterWriteScope()) {
                // file should be created
            }
            return package;
        }

    }
}