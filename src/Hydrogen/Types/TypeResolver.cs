// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Used to resolve types by their names. Can supply short name, full names or fully qualified names. Intelligently
/// loads assemblies as needed. Inner classes are pre-fixed by "ParentClass+" in both short and long names.
/// </summary>
public static class TypeResolver {

	private static readonly ICache<Assembly, AssemblyInfo> AssemblyLoader;

	static TypeResolver() {
		AssemblyLoader = new ActionCache<Assembly, AssemblyInfo>(
			(assembly) => new AssemblyInfo(assembly)
		);
	}

	public static Type Resolve(string typeNameHint, string assemblyHint = null, string pathHint = null) {
		typeNameHint = typeNameHint.Trim();
		// If fully qualified type name, load from assembly inside typename
		if (typeNameHint.Contains(",")) {
			var commaIndex = typeNameHint.IndexOf(",", StringComparison.Ordinal);
			return ResolveTypeInAssembly(
				typeNameHint.Substring(commaIndex + 1).Trim(),
				typeNameHint.Substring(0, commaIndex).Trim(),
				pathHint
			);
		}

		// Specified assembly, so try to load from there
		if (!string.IsNullOrWhiteSpace(assemblyHint))
			return ResolveTypeInAssembly(assemblyHint, typeNameHint, pathHint);

		// Assembly not specified, so load from currently loaded assemblies
		return ResolveTypeInAssemblies(
			AppDomain.CurrentDomain.GetAssemblies(),
			typeNameHint
		);
	}

	public static Type ResolveTypeInAssembly(string assemblyName, string typeName, string path = null) {
		// Load the assembly if its not loaded
		var assemblySearch =
			AppDomain
				.CurrentDomain
				.GetAssemblies()
				.Where(a => string.Equals(a.FullName.Split(',')[0], assemblyName, StringComparison.InvariantCultureIgnoreCase))
				.ToArray();

		// Find the loaded, or load the assembly
		var assembly = assemblySearch.Any() ? assemblySearch.First() : LoadAssembly(assemblyName, path);

		return ResolveTypeInAssembly(assembly, typeName);

	}

	public static Type ResolveTypeInAllAssemblies(string typeName) {
		return ResolveTypeInAssemblies(AppDomain.CurrentDomain.GetAssemblies(), typeName);
	}

	public static Type ResolveTypeInAssembly(Assembly assembly, string typeName) {
		return ResolveTypeInAssemblies(new[] { assembly }, typeName);
	}

	public static Type ResolveTypeInAssemblies(IEnumerable<Assembly> assemblies, string typeName) {
		// Full name search
		var assembliesArr = assemblies as Assembly[] ?? assemblies.ToArray();
		var longNameSearchResult =
			assembliesArr
				.Where(a => AssemblyLoader[a].TypesByLongName.ContainsKey(typeName))
				.Select(a => Tuple.Create(a, AssemblyLoader[a].TypesByLongName[typeName]))
				.ToArray();


		// Found results using full name search
		if (longNameSearchResult.Any()) {
			if (longNameSearchResult.Length > 1) {
				throw new SoftwareException(
					"Found multiple types matching full name '{0}'. The matches were:{2}{1}",
					typeName,
					longNameSearchResult
						.Select(r => r.Item2.AssemblyQualifiedName)
						.ToDelimittedString("," + Environment.NewLine),
					Environment.NewLine
				);
			}
			return longNameSearchResult.Single().Item2;
		}

		// Search by name only
		var shortNameSearchResult =
			assembliesArr
				.Where(a => AssemblyLoader[a].TypesByShortName.Count > 0)
				.SelectMany(a => AssemblyLoader[a].TypesByShortName[typeName].Select(t => Tuple.Create(a, t)))
				.ToArray();

		// Found result using name search
		if (shortNameSearchResult.Any()) {
			if (shortNameSearchResult.Length > 1) {
				throw new SoftwareException(
					"Found multiple types matching short name '{0}'. The matches where:{2}{1}",
					typeName,
					shortNameSearchResult
						.Select(r => r.Item2.AssemblyQualifiedName)
						.ToDelimittedString("," + Environment.NewLine),
					Environment.NewLine
				);
			}
			return shortNameSearchResult.Single().Item2;
		}

		// No results found
		throw new SoftwareException(
			"Unable to find type '{0}' in assembly(s):{2}{1}",
			typeName,
			assembliesArr.Select(a => "'" + a.FullName + "'").ToDelimittedString("," + Environment.NewLine),
			Environment.NewLine
		);
	}

	public static Assembly LoadAssembly(string assemblyName, string path = null) {
		try {
			assemblyName = assemblyName.Trim();
			if (string.IsNullOrWhiteSpace(path)) {
				if (assemblyName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
					assemblyName = assemblyName.TrimEnd(".dll", false);
				return Assembly.Load(assemblyName);
			}

			if (!Path.IsPathRooted(path)) {
				path = Path.Combine(AppDllDirectory, path);
			}

			if (!assemblyName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				assemblyName += ".dll";

			assemblyName = Path.Combine(path, assemblyName);
			return Assembly.LoadFrom(assemblyName);

		} catch (Exception error) {
			throw new SoftwareException(error, "Unable to load assembly '{0}'", assemblyName);
		}
	}

	private static string AppDllDirectory => AppDomain.CurrentDomain.BaseDirectory;


	public class AssemblyInfo {

		public AssemblyInfo(Assembly assembly) {
			Assembly = assembly;
			TypesByShortName = new ExtendedLookup<string, Type>();
			TypesByLongName = new Dictionary<string, Type>();
			Load();
		}

		public ExtendedLookup<string, Type> TypesByShortName { get; }
		public IDictionary<string, Type> TypesByLongName { get; }

		public Assembly Assembly { get; }

		private void Load() {
			try {
				var types = Assembly.GetTypes();
				foreach (var type in types) {
					if (type.IsAnonymousType())
						continue;

					var shortName = type.GetShortName();
					TypesByShortName.Add(shortName, type);
					TypesByLongName.Add(type.FullName, type);
				}
			} catch (ReflectionTypeLoadException rte) {
				throw new SoftwareException(
					rte,
					"Unable to assembly '{1}' due to the following errors:{0}{2}",
					Environment.NewLine,
					Assembly.FullName,
					rte.LoaderExceptions.Select(x => x.ToDiagnosticString()).ToDelimittedString(Environment.NewLine, "(null)"));
			}
		}
	}
}
