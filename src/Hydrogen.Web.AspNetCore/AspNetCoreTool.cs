// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Hydrogen;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Rendering;
using IPNetwork = System.Net.IPNetwork;

namespace Tools.Web;

public static partial class AspNetCore {


	public static IPNetwork ParseNetwork(string cidr) {
		var parts = cidr.Split('/');
		var ipAddressString = parts[0];
		var prefixLength = int.Parse(parts[1]);

		var ipAddress = IPAddress.Parse(ipAddressString);
		var ipAddressBytes = ipAddress.GetAddressBytes();

		var bits = ipAddressBytes.Length * 8;
		var networkMask = (1 << prefixLength) - 1;
		var networkBytes = new byte[ipAddressBytes.Length];
		for (var i = 0; i < ipAddressBytes.Length; i++) {
			networkBytes[i] = (byte)(ipAddressBytes[i] & (networkMask >> (bits - 8)));
			bits -= 8;
		}

		var networkAddress = new IPAddress(networkBytes);
		var network = new IPNetwork(networkAddress, prefixLength);

		return network;
	}

	public static SelectList ToSelectList<TEnum>(object selectedItem = default, SortDirection? sort = null) where TEnum : Enum
		=> ToSelectList(typeof(TEnum), selectedItem, sort);

	public static SelectList ToSelectList(Type enumType, object selectedItem = default, SortDirection? sort = null) {
		List<SelectListItem> items = new List<SelectListItem>();
		foreach (Enum item in Enum.GetValues(enumType)) {
			FieldInfo fi = enumType.GetField(item.ToString());
			//var attribute =  fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
			var title = Tools.Enums.GetDescription(item); //  attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
			var listItem = new SelectListItem {
				Value = item.ToString(),
				Text = title,
				Selected = selectedItem switch { null => false, _ => selectedItem.Equals(item) }
			};
			items.Add(listItem);
		}
		if (sort != null) {
			IComparer<SelectListItem> comparer = new ProjectionComparer<SelectListItem, string>(x => x.Text, StringComparer.InvariantCultureIgnoreCase);
			if (sort.Value == SortDirection.Descending)
				comparer = comparer.AsInverted();
			items.Sort(comparer);
		}

		return new SelectList(items, "Value", "Text");
	}
}
