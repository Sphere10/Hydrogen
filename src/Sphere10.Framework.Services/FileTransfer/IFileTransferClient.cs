//-----------------------------------------------------------------------
// <copyright file="IFileTransferClient.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Services {
    public interface IFileTransferClient : IDisposable {

        FileStore LocalStore { get; }

		IFileTransferService Service { get; }

		int TransferChunkSize { get; }

        void UploadFile(string alias, string localFilePath, bool compress = false, Action<long> progressCallback = null);

        void DownloadFile(string alias, Action<long> progressCallback = null);

        void Publish(string alias, string overrideFileName = null, string additionalData = null);

        long GetServerFileSize(string alias);

    }
}
