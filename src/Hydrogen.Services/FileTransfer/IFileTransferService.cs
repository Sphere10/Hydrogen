//-----------------------------------------------------------------------
// <copyright file="IFileTransferService.cs" company="Sphere 10 Software">
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
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;

namespace Sphere10.Framework.Services {

	[ServiceContract]
	public interface IFileTransferService {

		[OperationContract]
		void RegisterFile(string alias, bool compressed);

		[OperationContract]
		void SendFilePart(string alias, byte[] filePart);

        [OperationContract]
        void Publish(string alias, string overrideFileName = null, string additionalData = null);

		[OperationContract]
		byte[] GetFilePart(string alias, long offset, int fetchSize);

		[OperationContract]
		string[] GetFiles();


	    [OperationContract]
	    long GetFileSize(string alias);

	}
}
