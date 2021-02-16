//-----------------------------------------------------------------------
// <copyright file="StandardLicenseKeyDecoder.cs" company="Sphere 10 Software">
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

#if !__MOBILE__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sphere10.Framework.Application.Components;

using System.Diagnostics;
using System.Reflection;
using Sphere10.Framework;

namespace Sphere10.Framework.Application {

	[Obfuscation(Feature = "encryptmethod", Exclude = false)]
	public class StandardLicenseKeyDecoder : ILicenseKeyDecoder {
		public StandardLicenseKeyDecoder(IProductInformationServices productInformationServices) {
			ProductInformationServices = productInformationServices;
		}

		protected IProductInformationServices ProductInformationServices { get; private set; }

#region ILicenseKeyDecoder Implementation

		public ProductLicense Decode(string key) {
			ProductLicense license;
			if (!TryDecode(key, out license)) {
				throw new SoftwareException("Invalid license key");
			}
			return license;
		} 

        public bool TryDecode(string code, out ProductLicense license) {
            // License format:
            //  No. Bytes       Field                   Byte index
            //  1               X                       0
            //  1               License flags           1
            //  1               VersionNoApplicable     2   
            //  2               ExpirationDate          3
            //  1               Y                       5
            //  1               ExpirationDays          6
            //  1               ExpirationLoads         7
            //  1               MaxSimultaneousUsers    8
            //  1               Z                       9

            // X = random(0,255)
            // Y = (byte[1] + byte[2] + byte[3] + byte[6] + byte[7] + byte[8]) % 255
            // Z = (X^2 + Y^2) % 255 
            license = new ProductLicense();
            if (string.IsNullOrEmpty(code)) {
                return false;
            }

            byte[] bytes = Base32Converter.FromBase32String(code.Replace("-", string.Empty));
            if (bytes.Length != 10) {
                return false;
            }

			bytes = bytes.Xor(ProductInformationServices.ProductInformation.ProductCode.ToByteArray());
            
			// apply internal mask
            StandardLicenseCodecTools.ApplyInternalMask(bytes);

            int X = (int)bytes[0];
            int Y = (int)bytes[5];
            int Z = (int)bytes[9];
            if (Y != (((int)bytes[1]) + ((int)bytes[2]) + ((int)bytes[3]) + ((int)bytes[6]) + ((int)bytes[7]) + ((int)bytes[8])) % 255) {
                return false;
            }
            if ((X * X + Y * Y) % 255 != Z) {
                return false;
            }
            license.Flags = (ProductLicenseFlags)bytes[1];
            license.MajorVersionApplicable = bytes[2];
			license.ExpirationDate = StandardLicenseCodecTools.FromDaysSince2008Jan1(BitConverter.ToUInt16(bytes, 3));
            license.ExpirationDays = bytes[6];
            license.ExpirationLoads = bytes[7];
            license.MaxSimultaneousUsers = bytes[8];

            return true;
        }

#endregion

#region Auxillary methods

		

#endregion

	}
}


#endif
