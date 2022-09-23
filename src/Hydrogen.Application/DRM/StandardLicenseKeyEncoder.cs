//-----------------------------------------------------------------------
// <copyright file="StandardLicenseKeyEncoder.cs" company="Sphere 10 Software">
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
using System.Reflection;
using System.Diagnostics;


namespace Hydrogen.Application {

	[Obfuscation(Feature = "encryptmethod", Exclude = false)]
	public class StandardLicenseKeyEncoder : ILicenseKeyEncoder {

		public StandardLicenseKeyEncoder(IProductInformationServices productInformationServices) {
			ProductInformationServices = productInformationServices;
		}

		protected IProductInformationServices ProductInformationServices { get; private set; }

		public string GenerateKey(ProductLicense license)
		{
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


			byte[] byteArray = new byte[10];
			Tools.Maths.RNG.NextBytes(byteArray);


			byteArray[1] = (byte)license.Flags;
			if (license.VersionAware)
			{
				byteArray[2] = license.MajorVersionApplicable;
			}
			if (license.ExpiresAfterDate)
			{
				Array.Copy(
					BitConverter.GetBytes(StandardLicenseCodecTools.ToDaysSince2008Jan1(license.ExpirationDate)),
					0,
					byteArray,
					3,
					2
				);
			}

			if (license.ExpiresAfterDays)
			{
				byteArray[6] = license.ExpirationDays;
			}
			if (license.ExpiresAfterLoads)
			{
				byteArray[7] = license.ExpirationLoads;
			}
			if (license.LimitSimultaneousUsers)
			{
				byteArray[8] = license.MaxSimultaneousUsers;
			}


			// add in license validity bytes

			int X = Tools.Maths.RNG.Next(0, 255);
			int Y = (((int)byteArray[1]) + ((int)byteArray[2]) + ((int)byteArray[3]) + ((int)byteArray[6]) + ((int)byteArray[7]) + ((int)byteArray[8])) % 255;
			int Z = (X * X + Y * Y) % 255;

			byteArray[0] = (byte)X;
			byteArray[5] = (byte)Y;
			byteArray[9] = (byte)Z;

			// apply internal mask
			StandardLicenseCodecTools.ApplyInternalMask(byteArray);

			// apply product code mask
			byteArray = byteArray.Xor(ProductInformationServices.ProductInformation.ProductCode.ToByteArray());

			return Beutify(Base32Converter.ToBase32String(byteArray));
		}

		private string Beutify(string code)
		{
			Debug.Assert(code.Length == 16);
			if (code.Length != 16)
			{
				throw new SoftwareException("Cannot beautify code '{0}' as it is not 16 characters long", code);
			}
			return code.Insert(4, "-").Insert(9, "-").Insert(14, "-");
		}
	}
}
