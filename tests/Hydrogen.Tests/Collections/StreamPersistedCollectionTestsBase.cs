// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hydrogen.Tests;

public class StreamPersistedCollectionTestsBase {

	protected IDisposable CreateStream(StorageType storageType, int estimatedMaxByteSize, out Stream stream) {
		var disposables = new Disposables();

		switch (storageType) {
			case StorageType.MemoryStream:
				stream = new MemoryStream();
				break;
			case StorageType.MemoryBuffer:
				stream = new ExtendedMemoryStream(new MemoryBuffer());
				break;
			case StorageType.BinaryFile_1Page_1InMem:
				var tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize))));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_2Page_1InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2))));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.BinaryFile_10Page_5InMem:
				tmpFile = Tools.FileSystem.GetTempFileName(false);
				stream = new ExtendedMemoryStream(new FileMappedBuffer(PagedFileDescriptor.From(tmpFile, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10))));
				disposables.Add(new ActionScope(() => File.Delete(tmpFile)));
				break;
			case StorageType.TransactionalBinaryFile_1Page_1InMem:
				var baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				var fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From( fileName, baseDir, Math.Max(1, estimatedMaxByteSize), 1 * Math.Max(1, estimatedMaxByteSize))));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			case StorageType.TransactionalBinaryFile_2Page_1InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 2), 2 * Math.Max(1, estimatedMaxByteSize / 2))));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;

			case StorageType.TransactionalBinaryFile_10Page_5InMem:
				baseDir = Tools.FileSystem.GetTempEmptyDirectory(true);
				fileName = Path.Combine(baseDir, "File.dat");
				stream = new ExtendedMemoryStream(new TransactionalFileMappedBuffer(TransactionalFileDescriptor.From(fileName, baseDir, Math.Max(1, estimatedMaxByteSize / 10), 5 * Math.Max(1, estimatedMaxByteSize / 10))));
				disposables.Add(new ActionScope(() => Tools.FileSystem.DeleteDirectory(baseDir)));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(storageType), storageType, null);
		}
		if (stream is ILoadable loadable)
			loadable.Load();

		return disposables;
	}


	public enum StorageType {
		MemoryStream,
		MemoryBuffer,
		BinaryFile_1Page_1InMem,
		BinaryFile_2Page_1InMem,
		BinaryFile_10Page_5InMem,
		TransactionalBinaryFile_1Page_1InMem,
		TransactionalBinaryFile_2Page_1InMem,
		TransactionalBinaryFile_10Page_5InMem
	}
}
