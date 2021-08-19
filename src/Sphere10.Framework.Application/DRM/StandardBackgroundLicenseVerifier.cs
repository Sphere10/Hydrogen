//-----------------------------------------------------------------------
// <copyright file="StandardBackgroundLicenseVerifier.cs" company="Sphere 10 Software">
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
using System.Net;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using Sphere10.Framework;

namespace Sphere10.Framework.Application {
	public class StandardBackgroundLicenseVerifier : IBackgroundLicenseVerifier
	{
		private readonly Encoding _textEncoding = Encoding.UTF8;
		
		public StandardBackgroundLicenseVerifier(IProductInformationServices productInformationServices, IWebsiteLauncher websiteLauncher, ILicenseServices licenseServices, ILicenseEnforcer licenseEnforcer) {
			ProductInformationServices = productInformationServices;
			WebsiteLauncher = websiteLauncher;
			LicenseServices = licenseServices;
			LicenseEnforcer = licenseEnforcer;
		}

		public ILicenseEnforcer LicenseEnforcer { get; private set; }
		public IUserInterfaceServices UserInterfaceServices { get; private set; }

		public IProductInformationServices ProductInformationServices { get; private set; }

		public IWebsiteLauncher WebsiteLauncher { get; private set; }

		public ILicenseServices LicenseServices { get; private set; }


		public virtual void VerifyLicense() {
			Tools.Lambda
				.Action(() => {
					using (var wc = new WebClient()) {
						ApplyLicenseCommand(
							Tools.Xml.ReadFromFile<ProductLicenseCommand>(
								wc.DownloadString(GenerateVerificationUrl()),
								Encoding.ASCII
							)
						);
					}
				})
				.WithRetry(
					3,
					(attempt, error) => {
						// Log error here
						Thread.Sleep(TimeSpan.FromMinutes(attempt));
					}
				)
				.AsAsyncronous()
				.IgnoringExceptions()
				.Invoke();
		}


		protected virtual string GenerateVerificationUrl() {
			var builder = new UriBuilder(
				Uri.UriSchemeHttp,
				"www.sphere10.com"
			);

			string productKey = string.Empty;
			if (LicenseServices.LicenseInformation.HasRegisteredLicenseKey) {
				productKey = LicenseServices.LicenseInformation.RegisteredLicenseKey;
			} else if (LicenseServices.LicenseInformation.HasDefaultLicenseKey) {
				productKey = LicenseServices.LicenseInformation.DefaultLicenseKey;
			}
			builder.Path = "OnlineServices/LicenseVerify.aspx";
            var qBuilder = new QueryStringBuilder();
            qBuilder.Add("ProductCode", ProductInformationServices.ProductInformation.ProductCode.ToString());
            qBuilder.Add("ProductKey", productKey);
            qBuilder.Add("MachineID", GenerateMachineID());
            qBuilder.Add("UserID", GenerateUserID());
		    builder.Query = qBuilder.ToString();            
			return builder.Uri.ToString();
		}

		protected virtual string GenerateMachineID() {
			string mac = "UnableToGenerateMachineID";
			// get network interfaces physical addresses
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			if (interfaces.Length > 0) {
				mac = interfaces[0].GetPhysicalAddress().ToString().Replace(':', '-').Replace('.', '-');
			}
			
			return Convert.ToBase64String(Hashers.Hash(CHF.MD5, _textEncoding.GetBytes(mac)));
		}

		protected virtual string GenerateUserID() {
			return Convert.ToBase64String(Framework.Hashers.Hash(CHF.MD5, _textEncoding.GetBytes(Environment.UserName)));
		}


		protected void ApplyLicenseCommand(ProductLicenseCommand command) {
			ProductLicenseCommand oldCommand = null;
			if (LicenseServices.LicenseInformation.HasLicenseOverrideCommand) {
				oldCommand = LicenseServices.LicenseInformation.LicenseOverrideCommand;
			}
			LicenseServices.RegisterLicenseOverrideCommand(command);
			LicenseEnforcer.ApplyLicense(command.NotifyUser && LicenseEnforcer.CountNagged == 0);		// nag user if he hasn't already been nagged
		}		

	}
}
