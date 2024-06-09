// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define USE_FAST_REFLECTION

using Hydrogen.FastReflection;
using System;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

public static class ObjectEncryptor {

	public static void DecryptMembers(object obj) {
		ApplyInternal(obj, (attr, val) => attr.Decrypt(val));
	}

	public static void EncryptMembers(object obj) {
		ApplyInternal(obj, (attr, val) => attr.Encrypt(val));
	}

	private static void ApplyInternal(object obj, Func<EncryptedStringAttribute, string, string> func) {
		var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField;
		// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
		//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
		//    bindingFlags |= BindingFlags.NonPublic;
		bindingFlags |= BindingFlags.NonPublic;

		foreach (var field in obj.GetType().GetFields(bindingFlags)) {
			var encryptionAttributes = field.GetCustomAttributesOfType<EncryptedStringAttribute>().ToArray();
			if (!encryptionAttributes.Any())
				continue;
			if (encryptionAttributes.Count() != 1)
				throw new SoftwareException("Unable to encrypt/decrypt field '{0}.{1}' as it specified multiple encryption attributes. Class members can only have 0 or 1 EncryptionAttributes.", field.DeclaringType.FullName, field.Name);

			if (!field.FieldType.IsAssignableFrom(typeof(string)))
				throw new SoftwareException("Unable to encrypt/decrypt field '{0}.{1}' as its type is not assignable from String", field.DeclaringType.FullName, field.Name);

			var encryptionAttribute = encryptionAttributes.Single();

#if USE_FAST_REFLECTION
			field.SetValue(obj, func(encryptionAttribute, field.FastGetValue(obj).ToString())); // using FastReflection
#else
                field.SetValue(obj, func(encryptionAttribute, field.GetValue(obj).ToString())); // using standard reflection
#endif
		}

		bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetProperty;
		// HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
		//if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
		//    bindingFlags |= BindingFlags.NonPublic;
		bindingFlags |= BindingFlags.NonPublic;

		foreach (var property in obj.GetType().GetProperties(bindingFlags)) {
			var encryptionAttributes = property.GetCustomAttributesOfType<EncryptedStringAttribute>().ToArray();
			if (!encryptionAttributes.Any())
				continue;
			if (encryptionAttributes.Count() != 1)
				throw new SoftwareException("Unable to encrypt/decrypt property '{0}.{1}' as it specified multiple encryption attributes. Class members can only have 0 or 1 EncryptionAttribute.", property.DeclaringType.FullName, property.Name);

			if (!property.PropertyType.IsAssignableFrom(typeof(string)))
				throw new SoftwareException("Unable to encrypt/decrypt property '{0}.{1}' as its type is not assignable from String", property.DeclaringType.FullName, property.Name);

			var encryptionAttribute = encryptionAttributes.Single();

#if USE_FAST_REFLECTION
			property.FastSetValue(obj, func(encryptionAttribute, property.FastGetValue(obj)?.ToString())); // using FastReflection
#else
                property.SetValue(obj, func(encryptionAttribute, property.GetValue(obj).ToString())); // using standard reflectio
#endif

		}
	}

}
