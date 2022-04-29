//-----------------------------------------------------------------------
// <copyright file="iOSFileTransferService.cs" company="Sphere 10 Software">
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

#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Sphere10.Framework.Services {
    public sealed class iOSFileTransferService : iOSClientBase<IFileTransferService> {

        private class iOSFileTransferServiceChannel : iOSChannelBase, IFileTransferService {
       
            public iOSFileTransferServiceChannel(iOSFileTransferService client) :
                base(client) {
            }

            public void RegisterFile(string alias, bool compressed) {
                Call("RegisterFile", alias, compressed);
            }

            public void SendFilePart(string alias, byte[] filePart) {
                Call("SendFilePart", alias, filePart);
            }

            public void Publish(string alias, string overrideFileName = null, string additionalData = null) {
                Call("Publish", alias, overrideFileName, additionalData);
            }

            public byte[] GetFilePart(string alias, long offset, int fetchSize) {
                return Call<byte[]>("GetFilePart", alias, offset, fetchSize);
            }

            public string[] GetFiles() {
                return Call<string[]>("GetFiles");
            }

            public long GetFileSize(string alias) {
                return Call<long>("GetFileSize", alias);
            }

        }


        public iOSFileTransferService(Binding binding, EndpointAddress address)
            : base(binding, address) {
        }

        protected override IFileTransferService CreateChannel() {
            return new iOSFileTransferServiceChannel(this);
        }



    }
}
#endif
