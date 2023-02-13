//-----------------------------------------------------------------------
// <copyright file="ClientRequest.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Application {

	[Obfuscation(Exclude = true)]
    public abstract class ClientRequest {

        public ClientRequest()
            : this(
                UserType.HomeUser,
                string.Empty,
                new ProductInformation {
					CompanyName = string.Empty,
					CompanyUrl = string.Empty,
					CopyrightNotice = string.Empty,
					DefaultProductLicense = null,
					ProductCode = Guid.Empty,
					ProductDescription = string.Empty,
					ProductLongVersion = string.Empty,
					ProductName = string.Empty,
					ProductPurchaseUrl = string.Empty,
					ProductUrl = string.Empty,
					ProductVersion = string.Empty
				},
                string.Empty
              ) {
        }

        public ClientRequest(UserType userType, string sender, ProductInformation senderProductInformation, string featureDescription) {
            UserType = userType;
            Sender = sender;
            SenderProductInformation = senderProductInformation;
            Description = featureDescription;
        }

        public int ServiceVersion { get; set; }

        public UserType UserType { get; set; }

        public string Sender { get; set; }

        public string Description { get; set; }

        public ProductInformation SenderProductInformation { get; set; }

    }
}
