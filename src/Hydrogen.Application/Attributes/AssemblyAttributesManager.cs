//-----------------------------------------------------------------------
// <copyright file="AssemblyAttributesManager.cs" company="Sphere 10 Software">
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
using System.Reflection;

using System.IO;
using Sphere10.Framework;
using Tools;

namespace Sphere10.Framework.Application {

	public class AssemblyAttributesManager {
		public const string DefaultLink = "www.sphere10.com";

        public AssemblyAttributesManager() {
        }

		public string GetAssemblyTitle() {
			var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyTitleAttribute>(false);
			if (attributes.Any()) {
				if (attributes.ElementAt(0).Title != "") {
					return attributes.ElementAt(0).Title;
				}
			}
			return Path.GetFileNameWithoutExtension(Tools.Runtime.GetEntryAssembly().CodeBase);
		}

		public string GetAssemblyVersion() {
			return Tools.Runtime.GetEntryAssembly().GetName().Version?.ToString();
		}

		public string GetAssemblyDescription() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyDescriptionAttribute>(false).Description;
		}

		public string GetAssemblyCompanyLink() {
			var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyCompanyLinkAttribute>(false);
			if (!attributes.Any()) {
				return DefaultLink;
			}
			return attributes.First().CompanyLink;
		}

		public string GetAssemblyProductLink() {
			var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductLinkAttribute>(false);
			if (!attributes.Any()) {
				return DefaultLink;
			}
			return attributes.First().ProductLink;
		}

		public string GetAssemblyProductPurchaseLink() {
			var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductPurchaseLinkAttribute>(false);
			if (!attributes.Any()) {
				return DefaultLink;
			}
			return attributes.First().ProductPurchaseLink;
		}

		public string GetAssemblyProduct() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyProductAttribute>(false).Product;
		}

		public Guid GetAssemblyProductCode() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyProductCodeAttribute>(false).ProductCode;
		}
        

        public string GetAssemblyCopyright() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyCopyrightAttribute>(false).Copyright.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
        }

        public string GetAssemblyCompany() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyCompanyAttribute>(false)?.Company;
        }

        public string GetAssemblyCompanyNumber() {
            return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyCompanyNumberAttribute>(false).CompanyNumber;
        }

		public bool HasAssemblyDefaultProductKey() {
			return Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyDefaultProductKeyAttribute>(false).Any();
		}

        public string GetAssemblyDefaultProductKey() {
			return Tools.Runtime.GetEntryAssembly().GetCustomAttributeOfType<AssemblyDefaultProductKeyAttribute>(false).ProductKey;
        }

		//public static int GetHelpTopic(ApplicationScreen screen) {
		//    int retval = 0;
		//    object[] attributes = screen.GetType().GetCustomAttributes(typeof(HelpIDAttribute), true);
		//    // return first one
		//    if (attributes.Length > 0) {
		//        retval = ((HelpIDAttribute)attributes[0]).HelpID;
		//    }
		//    return retval;
		//}
               
        public IList<Tuple<HelpType, string>> GetAssemblyProductHelpResources()
        {
            return (
                from helpAttribute in Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductHelpResourceAttribute>(false)
                select Tuple.Create(helpAttribute.HelpType, helpAttribute.Path)
            )
            .ToList()
            .AsReadOnly();
        }


    
    }
}
