//-----------------------------------------------------------------------
// <copyright file="ProductInformation.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;


namespace Sphere10.Framework.Application {

	[Obfuscation(Exclude = true)]
    public class ProductInformation {
		public string CompanyName { get; set; }
        public string CompanyNumber { get; set; }
		public string ProductName { get; set; }
		public string ProductDescription { get; set; }
        public Guid ProductCode { get; set; }
		public string ProductVersion { get; set; }
		public string ProductLongVersion { get; set; }
		public string CopyrightNotice { get; set; }
		public string CompanyUrl { get; set; }
		public string ProductUrl { get; set; }
		public string ProductPurchaseUrl { get; set; }
		public string DefaultProductKey { get; set; }
		public string Path { get; set; }

        public ICollection<Tuple<HelpType, string>> HelpResources;

		public string ProcessTokensInString(string source) {
			source = source.Replace("{CompanyName}", CompanyName);
            source = source.Replace("{CompanyNumber}", CompanyNumber);
			source = source.Replace("{ProductName}", ProductName);
			source = source.Replace("{ProductDescription}", ProductDescription);
			source = source.Replace("{ProductCode}", ProductCode.ToString().Cast<char>().SkipWhile(c=> c=='{').TakeWhile(c => c != '}').ToString());
			source = source.Replace("{ProductVersion}", ProductVersion);
			source = source.Replace("{ProductLongVersion}", ProductLongVersion);
			source = source.Replace("{CopyrightNotice}", CopyrightNotice);
			source = source.Replace("{CompanyUrl}", CompanyUrl);
			source = source.Replace("{ProductUrl}", ProductUrl);
			source = source.Replace("{ProductPurchaseUrl}", ProductPurchaseUrl);
			source = source.Replace("{DefaultProductKey}", DefaultProductKey);
			source = source.Replace("{StartPath}", System.IO.Path.GetDirectoryName(Tools.Runtime.GetEntryAssembly().Location));
			return source;
		}

    }

}
