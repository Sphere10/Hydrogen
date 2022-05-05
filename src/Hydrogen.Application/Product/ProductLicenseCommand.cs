//-----------------------------------------------------------------------
// <copyright file="ProductLicenseCommand.cs" company="Sphere 10 Software">
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

/// WARNING: Make sure that these two files are synchronized
///     SchoenfeldSoftware.Application/Licensing/ProductLicenseCommand.cs
///     www.sphere10.com/OnlineServices/ProductLicenseCommand.cs
using System.Xml.Serialization;
using System.Reflection;

namespace Hydrogen.Application {

	[Obfuscation(Exclude = true)]
    [XmlRoot("ProductLicenseCommand")]
    public class ProductLicenseCommand {

        public ProductLicenseCommand()
            : this(ProductLicenseAction.Enable, false, string.Empty, string.Empty) {
        }

        public ProductLicenseCommand(
            ProductLicenseAction action,
            bool notifyUser,
            string notificationMessage,
            string link) {
            Action = action;
            NotifyUser = notifyUser;
            NoticationMessage = notificationMessage;
            BuyNowLink = link;
        }

        [XmlAttribute("Action")]
        public ProductLicenseAction Action { get; set; }

		[XmlAttribute("NotifyUser")]
		public bool NotifyUser { get; set; }

		[XmlElement("NotificationMessage")]
		public string NoticationMessage { get; set; }

		[XmlElement("BuyNowLink")]
		public string BuyNowLink { get; set; }

    }
}
