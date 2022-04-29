//-----------------------------------------------------------------------
// <copyright file="IFileTransferClientExtensions.cs" company="Sphere 10 Software">
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
    public static class IFileTransferClientExtensions {
        public static T DownloadObject<T>(this IFileTransferClient client, string alias) {
            client.DownloadFile(alias);
            using (var streamReader = new StreamReader(client.LocalStore.OpenRead(alias))) {       
                return Tools.Xml.Read<T>(streamReader);
            }           
        }

        public static string UploadObject<T>(this IFileTransferClient client, T @object, bool compress = false) {
            var fileAlias = client.LocalStore.NewFile();
            using (var streamWriter = new StreamWriter(client.LocalStore.OpenWrite(fileAlias))) {
                Tools.Xml.Write(@object, Encoding.Unicode, streamWriter);
            }
            client.UploadFile(fileAlias, client.LocalStore.GetFilePath(fileAlias));
            return fileAlias;
        }
    }
}
