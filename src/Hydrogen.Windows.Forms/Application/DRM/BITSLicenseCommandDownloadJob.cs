//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.


//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Net.NetworkInformation;
//using System.IO;
//using System.Reflection;
//using Hydrogen.Application;
//using Hydrogen;
//using Hydrogen.Windows.BITS;
//using Hydrogen.Application;


//namespace Hydrogen.Windows.Forms {

//    public class BITSLicenseCommandDownloadJob : JobEx {

//		public event EventHandler<BITSLicenseCommandDownloadEvent> CommandDownloaded; 

//		public BITSLicenseCommandDownloadJob(ILicenseServices licenseServices, IProductInformationProvider productInformationProvider, string verifyUrl) {
//			VerifyUrl = verifyUrl;
//			LicenseServices = licenseServices;
//			ProductInformationProvider = productInformationProvider;
//			LocalFileName = Path.GetTempFileName();
//		}

//		public override string Name {
//			get { return string.Format("{0} {1}", ProductInformationProvider.ProductInformation.ProductName, "License Verify"); }
//		}

//		public string VerifyUrl { get; private set; }

//		public string LocalFileName { get; private set; }

//        public ILicenseServices LicenseServices { get; private set; }

//		public IProductInformationProvider ProductInformationProvider { get; private set; }

//		public override JobType JobType {
//            get { return JobType.Download; }
//        }

//        public override bool DeleteOnFailure {
//            get {
//				return true;
//            }
//        }

//        public override JobOwner Owner {
//            get { return JobOwner.CurrentUser; }
//        }

//        public override void OnCreate(BitsJob jobToConfigure) {
//            jobToConfigure.Description =
//				string.Format("License verification for {0}", Name);

//            jobToConfigure.AddFile(
//			   VerifyUrl,
//               LocalFileName
//            );
//        }

//        public override void OnError(string message) {
//            try {
//                base.OnError(message);
//                // do nothing
//                //ServicesProvider.ReportError(message);
//            } catch (Exception error) {
//            }
//        }

//        public override void OnCompleted() {
//            try {
//                // apply license verification
//                UnderlyingJob.Complete();
//                if (File.Exists(LocalFileName)) {
//                    var command = Tools.Xml.ReadFromFile<ProductLicenseCommandDTO>(
//                        LocalFileName,
//                        Encoding.ASCII
//                    );
//                    LicenseServices.RegisterLicenseOverrideCommand(command);
//					if (CommandDownloaded != null) {
//						CommandDownloaded(this, new BITSLicenseCommandDownloadEvent { Command = command });
//					}
//                }
//            } catch {
//               // ServicesProvider.ReportError(error);
//            } finally {
//                if (File.Exists(LocalFileName)) {
//                    try {
//                        File.Delete(LocalFileName);
//                    } catch {
//                    }
//                }
//            }
//        }
//    }
//}


