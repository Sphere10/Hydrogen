//-----------------------------------------------------------------------
// <copyright file="FileTransferClient.cs" company="Sphere 10 Software">
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
using System.Text;

namespace Sphere10.Framework.Services {

    public class FileTransferClient : IFileTransferClient {
		private const int DefaultTransferChunkSize = 2048 * 100;

		public FileTransferClient(IFileTransferService service, int transferChunkSize = DefaultTransferChunkSize) {
			Service = service;
			TransferChunkSize = transferChunkSize;
            LocalStore = new TempFileStore();
	
		}

        public FileStore LocalStore { get; set; }

		public IFileTransferService Service { get; set;}

        public int TransferChunkSize { get; set; }

		public void UploadFile(string alias, string localFilePath, bool compress = false, Action<long> progressCallback = null) {
		    if (progressCallback == null)
		        progressCallback = (x) => Tools.Lambda.NoOp();
		    int transferChunkSize = TransferChunkSize;
			using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read)) {
				var bytes = new byte[TransferChunkSize];
				var chunk = new byte[TransferChunkSize];
				var bytesRead = 0;
			    long totalRead = 0;
				Service.RegisterFile(alias, compress);
                while ((bytesRead = stream.Read(bytes, 0, transferChunkSize)) > 0) {
				    totalRead += bytesRead;
					Array.Resize(ref chunk, bytesRead);
					Array.Copy(bytes, chunk, bytesRead);
					Service.SendFilePart(alias, chunk);
                    progressCallback(totalRead);
				}
			}
		}

		public void DownloadFile(string alias, Action<long> progressCallback = null) {
            if (progressCallback == null)
                progressCallback = (x) => Tools.Lambda.NoOp();
		    var transferChunkSize = TransferChunkSize;
            if (LocalStore.ContainsAlias(alias))
                LocalStore.Delete(alias);
            LocalStore.RegisterFile(alias);
			long currentRead = 0;
			var bytesRead = 0;
			do {
                var data = Service.GetFilePart(alias, currentRead, transferChunkSize);
				bytesRead = data.Length;
				currentRead += bytesRead;
                LocalStore.AppendAllBytes(alias, data);
			    progressCallback(currentRead);
			} while (bytesRead >= TransferChunkSize);
		}

        public void Publish(string alias, string overrideFileName = null, string additionalData = null) {
            Service.Publish(alias, overrideFileName, additionalData);
        }

        public long GetServerFileSize(string alias) {
            return Service.GetFileSize(alias);
        }

		public virtual void Dispose() {
            LocalStore.Dispose();
		}

	}
}
