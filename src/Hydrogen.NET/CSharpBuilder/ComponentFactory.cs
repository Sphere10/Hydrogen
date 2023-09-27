// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace Hydrogen.CodeBuilder;

public abstract class CodeBuilderAttribute : Attribute {
	public abstract void InjectCode(ClassBuilder ownerClass, MethodBuilder method);
}


public abstract class MethodImplementationAttribute : CodeBuilderAttribute {
}


public class AutoMetrics : CodeBuilderAttribute {
	public override void InjectCode(ClassBuilder ownerClass, MethodBuilder method) {
		method.BodyCode =
			string.Format(
				"using (new ElapsedTimeLogger({1}, EnablePerformanceLogging)) {{{0}{2}{0}}}",
				Environment.NewLine,
				method.BuildLogEntryHeader(),
				method.BodyCode.Tabbify(1)
			);
	}
}


public class AutoLog : CodeBuilderAttribute {
	public override void InjectCode(ClassBuilder ownerClass, MethodBuilder method) {
		method.BodyCode =
			string.Format(
				"if (EnableInformationLogging){{Log.LogInfo({1});}}{0}{0}{2}",
				Environment.NewLine,
				method.BuildLogEntryHeader(),
				method.BodyCode
			);
	}
}


public class AutoCache : MethodImplementationAttribute {
	public override void InjectCode(ClassBuilder ownerClass, MethodBuilder method) {

		#region Inject cache declaration and initialization into class (if not present)

		if (!ownerClass.ContainsField("_cache")) {
			FieldBuilder field = ownerClass.AppendField(
				ProtectionLevel.Private,
				"Dictionary<string, object>",
				"_cache"
			);
			ownerClass.Constructor.BodyCode = "_cache = new Dictionary<string, object>();";
		}

		#endregion

		#region Inject caching code snippet into method

		method.BodyCode = string.Format(
			@"string cacheKey = {1};{0}if (!_cache.ContainsKey(cacheKey)) {{{0}  _cache[cacheKey] = {2}{0}}}{0}return ({3})_cache[cacheKey];",
			Environment.NewLine,
			method.BuildLogEntryHeader(),
			method.BuildCallToBaseMethod(),
			method.ReturnTypeName
		);

		#endregion

	}
}


internal class ReturnBaseImpl : MethodImplementationAttribute {
	public override void InjectCode(ClassBuilder ownerClass, MethodBuilder method) {
		method.BodyCode = string.Format(
			@"{0} {1}",
			method.ReturnTypeName == "void" ? string.Empty : "return",
			method.BuildCallToBaseMethod()
		);
	}

}


public class ComponentFactory {

	static private readonly ComponentFactory _instance = new ComponentFactory();
	private List<string> _references;
	IDictionary<Type, Type> _proxifiedClasses;

	private ComponentFactory() {
		_proxifiedClasses = new Dictionary<Type, Type>();
		_references = new List<string>();
		// add System.dll by default
		_references.Add("System.dll");
	}

	public static ComponentFactory Instance {
		get {
			Debug.Assert(_instance != null);
			return _instance;
		}
	}

	public List<string> References {
		get { return _references; }
		set { _references = value; }
	}

	public IDictionary<Type, Type> ProxifiedClasses {
		get { return _proxifiedClasses; }
		set { _proxifiedClasses = value; }
	}

	public T New<T>() where T : class {
		if (!ProxifiedClasses.Keys.Contains(typeof(T))) {
			ProxifiedClasses.Add(
				typeof(T),
				ProxifyType(typeof(T))
			);
		}
		Debug.Assert(ProxifiedClasses.Keys.Contains(typeof(T)));

		Type proxifiedType = ProxifiedClasses[typeof(T)];

		// create new instance of this type
		return Activator.CreateInstance(ProxifiedClasses[typeof(T)]) as T;
	}

	private Type ProxifyType(Type superType) {
		Debug.Assert(superType != null);
		Debug.Assert(!superType.IsSealed);

		return CompileClass(BuildProxyClassSource(superType));
	}

	private Type CompileClass(ClassBuilder classBuilder) {
		List<string> errorMessages = new List<string>();

		string source = classBuilder.ToString();

		Microsoft.CSharp.CSharpCodeProvider cp
			= new Microsoft.CSharp.CSharpCodeProvider();
		System.CodeDom.Compiler.ICodeCompiler ic = cp.CreateCompiler();
		System.CodeDom.Compiler.CompilerParameters cpar
			= new System.CodeDom.Compiler.CompilerParameters();
		cpar.GenerateInMemory = true;
		cpar.GenerateExecutable = false;
		foreach (string reference in References) {
			cpar.ReferencedAssemblies.Add(reference);
		}

		System.CodeDom.Compiler.CompilerResults cr
			= ic.CompileAssemblyFromSource(cpar, source);

		foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors) {
			errorMessages.Add(ce.ErrorText);
		}

		if (cr.Errors.Count == 0 && cr.CompiledAssembly != null) {
			return cr.CompiledAssembly.GetType(classBuilder.FullName);
		}

		throw new ApplicationException("Compiler error"); // assemble error messages here
	}

	private ClassBuilder BuildProxyClassSource(Type superType) {
		// build source code for sub-type which inherits superType

		#region Validate

		if (superType.IsSealed) {
			throw new ApplicationException(
				string.Format(
					"Could not generate proxy type as specified sub-type '{0}' is was declared sealed.",
					superType.Name
				)
			);
		}

		#endregion

		ClassBuilder classBuilder = new ClassBuilder();
		classBuilder.Namespace = superType.Namespace;
		classBuilder.Imports.Add("System.Collections.Generic");
		classBuilder.Imports.Add("DocCube.DataAccessLayer");
		classBuilder.Imports.Add("DocCube.SystemServices");
		classBuilder.ProtectionLevel = ProtectionLevel.Public;
		classBuilder.Name = string.Format("_{0}Proxy", superType.Name);
		classBuilder.BaseTypeName = superType.Name;

		// get all methods
		foreach (MethodInfo method in superType.GetMethods()) {
			if (ContainsProxyAttributes(method)) {

				#region Validate

				if (method.IsStatic) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was declared static.",
							superType.Name,
							method.Name
						)
					);
				}

				if (method.IsAbstract) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was declared abstract.",
							superType.Name,
							method.Name
						)
					);
				}

				if (method.IsPrivate) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was declared private.",
							superType.Name,
							method.Name
						)
					);
				}

				if (!method.IsVirtual) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was not declared virtual.",
							superType.Name,
							method.Name
						)
					);
				}

				if (method.IsGenericMethod) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was generic.",
							superType.Name,
							method.Name
						)
					);
				}

				if (method.IsGenericMethodDefinition) {
					throw new ApplicationException(
						string.Format(
							"Could not generate proxy type for sub-type '{0}' as proxyable method '{1}' was a generic definition.",
							superType.Name,
							method.Name
						)
					);
				}

				#endregion

				MethodBuilder methodBuilder = classBuilder.AppendMethod();
				methodBuilder.PolymorphicLevel = PolymorphicLevel.Override;
				methodBuilder.ProtectionLevel = DetermineProtectionLevel(method);
				methodBuilder.ReturnTypeName = method.ReturnParameter.ParameterType.FullName;
				methodBuilder.Name = method.Name;

				// build method parameters
				foreach (ParameterInfo parameter in method.GetParameters()) {
					ParameterBuilder paramBuilder = methodBuilder.AppendParameter();
					paramBuilder.ParameterName = parameter.Name;
					paramBuilder.TypeName = parameter.ParameterType.FullName;
				}

				// if no actual implementation code of the method (i.e. what returns a value) was
				// injected, then simply return the base class implementation
				if (!ContainsMethodImplementationAttribute(method)) {
					ReturnBaseImpl attr = new ReturnBaseImpl();
					attr.InjectCode(classBuilder, methodBuilder);
				}

				// inject decorator snippets
				foreach (CodeBuilderAttribute attr in GetProxyAttributes(method)) {
					attr.InjectCode(classBuilder, methodBuilder);
				}
			}
		}
		return classBuilder;
	}

	private ProtectionLevel DetermineProtectionLevel(MethodInfo method) {
		ProtectionLevel retval = ProtectionLevel.Public;
		if (method.IsPrivate) {
			retval = ProtectionLevel.Private;
		} else if (method.IsPublic) {
			retval = ProtectionLevel.Public;
		} else if (method.IsFamily) {
			retval = ProtectionLevel.Protected;
		} else if (method.IsAssembly) {
			retval = ProtectionLevel.Internal;
		}
		return retval;
	}

	private bool ContainsProxyAttributes(MethodInfo method) {
		return GetProxyAttributes(method).Length > 0;
	}

	private CodeBuilderAttribute[] GetProxyAttributes(MethodInfo method) {
		List<CodeBuilderAttribute> attributes = new List<CodeBuilderAttribute>();
		foreach (object attr in method.GetCustomAttributes(true)) {
			if (attr is CodeBuilderAttribute) {
				attributes.Add(attr as CodeBuilderAttribute);
			}
		}
		return attributes.ToArray();
	}

	private bool ContainsMethodImplementationAttribute(MethodInfo method) {
		foreach (CodeBuilderAttribute attr in GetProxyAttributes(method)) {
			if (attr is MethodImplementationAttribute) {
				return true;
			}
		}
		return false;
	}


}
