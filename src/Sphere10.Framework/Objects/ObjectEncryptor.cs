//-----------------------------------------------------------------------
// <copyright file="ObjectEncryptor.cs" company="Sphere 10 Software">
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
using System.Reflection;



namespace Sphere10.Framework {


	public static class ObjectEncryptor {

        public static void DecryptMembers(object obj) {
            ApplyInternal(obj, (attr, val) => attr.Decrypt(val), "decrypt");
        }

        public static void EncryptMembers(object obj) {
            ApplyInternal(obj, (attr, val) => attr.Encrypt(val), "encrypt");
        }

        private static void ApplyInternal(object obj, Func<EncryptedAttribute, string, string> func, string action) {
            var bindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField;
	        // HS: Removed 2019-02-19, .NET Standard 2.0 assume has unrestricted
	        //if (Tools.CodeAccessSecurity.HasUnrestrictedFeatureSet)
	        //    bindingFlags |= BindingFlags.NonPublic;
	        bindingFlags |= BindingFlags.NonPublic;

			foreach (var field in obj.GetType().GetFields(bindingFlags)) {
                var encryptionAttributes = field.GetCustomAttributesOfType<EncryptedAttribute>().ToArray();
                if (!encryptionAttributes.Any())
                    continue;
                if (encryptionAttributes.Count() != 1)
                    throw new SoftwareException("Unable to {0} field '{1}.{2}' as it specified multiple encryption attributes. Class members can only have 0 or 1 EncryptionAttributes.", action, field.DeclaringType.FullName, field.Name);

                if (!field.FieldType.IsAssignableFrom(typeof(string)))
                    throw new SoftwareException("Unable to {0} field '{1}.{2}' as its type is not assignable from String", action, field.DeclaringType.FullName, field.Name);

                var encryptionAttribute = encryptionAttributes.Single();

#if USE_FAST_REFLECTION
					field.SetValue(obj, func(encryptionAttribute, field.FastGetValue(obj).ToString()));  // using FastReflection
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
                var encryptionAttributes = property.GetCustomAttributesOfType<EncryptedAttribute>().ToArray();
                if (!encryptionAttributes.Any())
                    continue;
                if (encryptionAttributes.Count() != 1)
                    throw new SoftwareException("Unable to {0} property '{1}.{2}' as it specified multiple encryption attributes. Class members can only have 0 or 1 EncryptionAttribute.", action, property.DeclaringType.FullName, property.Name);

                if (!property.PropertyType.IsAssignableFrom(typeof(string)))
                    throw new SoftwareException("Unable to {0} property '{1}.{2}' as its type is not assignable from String", action, property.DeclaringType.FullName, property.Name);

                var encryptionAttribute = encryptionAttributes.Single();

#if USE_FAST_REFLECTION
					property.FastSetValue(obj, func(encryptionAttribute, property.FastGetValue(obj).ToString()));  // using FastReflection
#else
                property.SetValue(obj, func(encryptionAttribute, property.GetValue(obj).ToString())); // using standard reflectio
#endif

            }
        }

    }
}


