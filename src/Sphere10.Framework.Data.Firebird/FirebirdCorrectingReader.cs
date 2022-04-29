//-----------------------------------------------------------------------
// <copyright file="FirebirdCorrectingReader.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Data.Firebird {
	public class FirebirdCorrectingReader : DataReaderDecorator {

		public FirebirdCorrectingReader(IDataReader internalReader) : base(internalReader) {
		}
	
		public override object GetValue(int i) {
			var result = InternalReader.GetValue(i);
			if (result is Guid) {
				result = CorrectGuid((Guid)result);
			}
			return result;
		}

		public override Guid GetGuid(int i) {
			return CorrectGuid(InternalReader.GetGuid(i));
		}

		public static Guid CorrectGuid(Guid badlyParsedGuid) {
			var rfc4122bytes = badlyParsedGuid.ToByteArray();
			if (BitConverter.IsLittleEndian) {
				Array.Reverse(rfc4122bytes, 0, 4);
				Array.Reverse(rfc4122bytes, 4, 2);
				Array.Reverse(rfc4122bytes, 6, 2);
			}
			return new Guid(rfc4122bytes);
		}
	}
}
