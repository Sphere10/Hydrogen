// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

public static class CustomAttributesProviderExtensions {

	public static IEnumerable<T> GetCustomAttributesOfType<T>(this ICustomAttributeProvider attributesProvider, bool inherited = false) where T : Attribute {
		return
			from attribute in attributesProvider.GetCustomAttributes(inherited)
			where attribute is T
			select (T)attribute;
	}

	public static T GetCustomAttributeOfType<T>(this ICustomAttributeProvider attributesProvider, bool inherited = false, bool throwOnMissing = true) where T : Attribute {
		var attributes = attributesProvider.GetCustomAttributesOfType<T>(inherited).ToArray();
		if (!attributes.Any()) {
			if (throwOnMissing)
				throw new SoftwareException("{0} did not contain (or contained more than one) attribute {1}", attributesProvider.ToString(), typeof(T).Name);
			return default;
		}
		return attributes.Single();
	}

	public static bool TryGetCustomAttributeOfType<T>(this ICustomAttributeProvider attributesProvider, bool inherited, out T attribute) where T : Attribute {
		var attributes = attributesProvider.GetCustomAttributesOfType<T>(inherited).ToArray();
		attribute = default;
		if (attributes.Length != 1)
			return false;
		attribute = attributes[0];
		return true;
	}

	public static bool HasAttribute<T>(this ICustomAttributeProvider attributesProvider, bool inherited) where T : Attribute
		=> attributesProvider.TryGetCustomAttributeOfType<T>(inherited, out _);
}
