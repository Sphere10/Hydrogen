//-----------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hydrogen {

	public static class AssemblyExtensions {
		static readonly string[] FrameworkPrefixes = new[]{
			"mscorlib",
			"System,",
			"System.",
			"Accessibility,",
			"netstandard,",
			"PresentationCore,",
			"WindowsBase,",
			"App_global.asax.",
			"Microsoft.",
			"SMDiagnostics,",
		};


		public static string GetEmbeddedResourceString(this Assembly assembly, string resourceName) {
			using var stream = assembly.GetManifestResourceStream(resourceName);
			using var reader = new StreamReader(stream);
			return Convert.ToString(reader.ReadToEnd());
		}

		public static void ExtractEmbeddedResource(this Assembly assembly, string resourceName, string filePath, bool createDirectories = false) {
			using var stream = assembly.GetManifestResourceStream(resourceName);
			if (!File.Exists(filePath))
				Tools.FileSystem.CreateBlankFile(filePath, createDirectories);
			Tools.FileSystem.WriteAllBytes(filePath, stream);
		}


		public static void ExtractEmbeddedResources(this Assembly assembly, string folder, string removeNamespace = null, bool keepExisting = true, Action<string, string> logAction = null) {
			Guard.ArgumentNotNull(folder, nameof(folder));
			removeNamespace ??= string.Empty;
			logAction ??= (_, _) => { };
			foreach (var embeddedResourceName in assembly.GetManifestResourceNames()) {
				var pathItems = embeddedResourceName.TrimStart(removeNamespace.TrimEnd(".") + ".").Split('.').ToArray();
				if (pathItems.Any(string.IsNullOrEmpty))
					throw new NotSupportedException($"Embedded resources with dotted filenames are not supported: '{embeddedResourceName}'");
				var fileName = pathItems.TakeLast(2).ToDelimittedString(".");
				Array.Resize(ref pathItems, pathItems.Length - 2);
				var folderPath = pathItems.Aggregate(folder, Path.Combine);    // empty folders were 
				var templatePath = Path.Join(folderPath, fileName);
				if (keepExisting && File.Exists(templatePath))
					continue;
				logAction(fileName, templatePath);
				assembly.ExtractEmbeddedResource(embeddedResourceName, templatePath, createDirectories: true);
			}

		}

		public static IEnumerable<Assembly> GetNonFrameworkAssemblies(this AppDomain domain) {
			return domain.GetAssemblies().Where(x => !FrameworkPrefixes.Any(p => x.FullName.StartsWith(p)));
		}
		public static IEnumerable<Type> GetDerivedTypes<T>(this Assembly assembly) {
			return assembly.GetDerivedTypes(typeof(T));
		}

		public static IEnumerable<Type> GetDerivedTypes(this Assembly assembly, Type baseType) {
			return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
		}
	}
}
