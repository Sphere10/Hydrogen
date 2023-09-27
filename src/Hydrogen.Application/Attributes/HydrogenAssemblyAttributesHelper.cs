// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Tools;

namespace Hydrogen.Application;

internal static class HydrogenAssemblyAttributesHelper {

	public static string GetAssemblyTitle() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyTitleAttribute>(false);
		if (attributes.Any()) {
			var firstTitleAttr = attributes.First();
			if (!string.IsNullOrWhiteSpace(firstTitleAttr.Title)) {
				return firstTitleAttr.Title;
			}
		}
		return Path.GetFileNameWithoutExtension(Tools.Runtime.GetEntryAssembly().CodeBase);
	}

	public static string GetAssemblyVersion() {
		return Tools.Runtime.GetEntryAssembly().GetName().Version?.ToString();
	}

	public static string GetAssemblyDescription() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyDescriptionAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Description);
	}

	public static string GetAssemblyCompanyLink() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyCompanyLinkAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().CompanyLink);
	}

	public static string GetAssemblyProductLink() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductLinkAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().ProductLink);
	}

	public static string GetAssemblyProductPurchaseLink() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductPurchaseLinkAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().ProductPurchaseLink);
	}

	public static string GetAssemblyProduct() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Product);
	}

	public static Guid? GetAssemblyProductCode() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductCodeAttribute>(false);
		return !attributes.Any() ? null : attributes.First().ProductCode;
	}

	public static ProductDistribution? GetAssemblyProductDistribution() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductDistributionAttribute>(false);
		return !attributes.Any() ? null : attributes.First().Distribution;
	}

	public static string GetAssemblyCopyright() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyCopyrightAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Copyright);
		;
	}

	public static string GetAssemblyCompany() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyCompanyAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Company);
	}

	public static string GetAssemblyCompanyNumber() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyCompanyNumberAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().CompanyNumber);
	}

	public static bool HasAssemblyDefaultProductKey() {
		return Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductLicenseAttribute>(false).Any();
	}

	public static ProductLicenseActivationDTO GetAssemblyDefaultProductLicenseActivation() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductLicenseAttribute>(false);
		return !attributes.Any() ? null : attributes.First().License;
	}

	public static string GetProductDrmApi() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductDrmApi>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().ApiUrl);
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

	public static IList<Tuple<HelpType, string>> GetAssemblyProductHelpResources() {
		return (
				from helpAttribute in Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyProductHelpResourceAttribute>(false)
				select Tuple.Create(helpAttribute.HelpType, helpAttribute.Path)
			)
			.ToList()
			.AsReadOnly();
	}

	public static string GetAssemblyAuthorName() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyAuthorAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Name);
	}

	public static string GetAssemblyAuthorEmail() {
		var attributes = Tools.Runtime.GetEntryAssembly().GetCustomAttributesOfType<AssemblyAuthorAttribute>(false);
		return !attributes.Any() ? null : StringFormatter.FormatEx(attributes.First().Email ?? string.Empty);
	}

}
