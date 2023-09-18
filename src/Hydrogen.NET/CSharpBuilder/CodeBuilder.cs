// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.CodeBuilder;

public enum ProtectionLevel {
	Private,
	Public,
	Internal,
	Protected
}


public enum PolymorphicLevel {
	None,
	Static,
	Virtual,
	Override,
	Abstract
}


public class ClassBuilder {
	private ProtectionLevel _protectionLevel;
	private string _name;
	private string _baseTypeName;
	private List<MethodBuilder> _methods;
	private List<FieldBuilder> _fields;
	private string _namespace;
	private List<string> _imports;
	private MethodBuilder _constructor;

	public ClassBuilder() {
		ProtectionLevel = ProtectionLevel.Public;
		Name = string.Empty;
		BaseTypeName = string.Empty;
		Fields = new List<FieldBuilder>();
		Methods = new List<MethodBuilder>();
		Namespace = string.Empty;
		Imports = new List<string>();
		Imports.Add("System");
		_constructor = new MethodBuilder();
		Constructor.ProtectionLevel = ProtectionLevel.Public;
		Constructor.ReturnTypeName = string.Empty;
	}

	public List<string> Imports {
		get { return _imports; }
		set { _imports = value; }
	}

	public string Namespace {
		get { return _namespace; }
		set { _namespace = value; }
	}

	public ProtectionLevel ProtectionLevel {
		get { return _protectionLevel; }
		set { _protectionLevel = value; }
	}

	public string Name {
		get { return _name; }
		set {
			_name = value;
			if (Constructor != null) {
				Constructor.Name = _name;
			}
		}
	}

	public string FullName {
		get {
			return string.Format(
				"{0}.{1}",
				string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace,
				string.IsNullOrEmpty(Name) ? string.Empty : Name
			);
		}
	}

	public string BaseTypeName {
		get { return _baseTypeName; }
		set { _baseTypeName = value; }
	}

	public MethodBuilder Constructor {
		get { return _constructor; }
	}

	public List<FieldBuilder> Fields {
		get { return _fields; }
		set { _fields = value; }
	}

	public List<MethodBuilder> Methods {
		get { return _methods; }
		set { _methods = value; }
	}

	public bool ContainsField(string name) {
		foreach (FieldBuilder field in Fields) {
			if (field.Name == name) {
				return true;
			}
		}
		return false;
	}

	public FieldBuilder AppendField(ProtectionLevel protection, string typeName, string name) {
		FieldBuilder field = AppendField();
		field.ProtectionLevel = protection;
		field.TypeName = typeName;
		field.Name = name;
		return field;
	}

	public FieldBuilder AppendField() {
		FieldBuilder field = new FieldBuilder();
		AppendField(field);
		return field;
	}

	public void AppendField(FieldBuilder field) {
		Fields.Add(field);
	}

	public MethodBuilder AppendMethod() {
		MethodBuilder method = new MethodBuilder();
		AppendMethod(method);
		return method;
	}

	public void AppendMethod(MethodBuilder method) {
		Methods.Add(method);
	}

	public override string ToString() {
		return string.Format(
			"{1}{0}namespace {2} {{{0}{3}{0}}}",
			Environment.NewLine,
			BuildImportDeclarations(),
			Namespace,
			BuildClassDeclaration().Tabbify(1)
		);
	}

	private string BuildImportDeclarations() {
		StringBuilder code = new StringBuilder();
		foreach (string usingClause in Imports) {
			code.AppendFormat("using {0};{1}", usingClause, Environment.NewLine);
		}
		return code.ToString();
	}

	private string BuildClassDeclaration() {
		return string.Format(
			"{1} class {2} : {3} {{{0}{4}{5}{6}}}",
			Environment.NewLine,
			GetProtectionLevelName(),
			Name,
			BaseTypeName,
			BuildFieldDeclarations().Tabbify(1),
			BuildConstructDeclarations().Tabbify(1),
			BuildMethodDeclarations().Tabbify(1)
		);
	}

	private string BuildFieldDeclarations() {
		StringBuilder fields = new StringBuilder();
		foreach (FieldBuilder field in Fields) {
			fields.AppendFormat(
				"{0}{1}{0}",
				Environment.NewLine,
				field.ToString()
			);
		}
		return fields.ToString();
	}

	private string BuildConstructDeclarations() {
		return string.Format(
			"{0}{1}{0}",
			Environment.NewLine,
			Constructor.ToString()
		);
	}

	private string BuildMethodDeclarations() {
		StringBuilder code = new StringBuilder();
		foreach (MethodBuilder method in Methods) {
			code.AppendFormat(
				"{0}{1}{0}",
				Environment.NewLine,
				method.ToString()
			);
		}
		return code.ToString();
	}

	private string GetProtectionLevelName() {
		return ProtectionLevel.ToString().ToLower();
	}

}


public class FieldBuilder {
	private ProtectionLevel _protectionLevel;
	private string _typeName;
	private string _name;


	public FieldBuilder() {
		ProtectionLevel = ProtectionLevel.Private;
		TypeName = string.Empty;
		Name = string.Empty;
	}

	public ProtectionLevel ProtectionLevel {
		get { return _protectionLevel; }
		set { _protectionLevel = value; }
	}

	public string TypeName {
		get { return _typeName; }
		set { _typeName = value; }
	}


	public string Name {
		get { return _name; }
		set { _name = value; }
	}

	public override string ToString() {
		return string.Format(
			"{0} {1} {2};",
			GetProtectionLevelName(),
			TypeName,
			Name
		);
	}

	private string GetProtectionLevelName() {
		return ProtectionLevel.ToString().ToLower();
	}
}


public class PropertyBuilder {
}


public class MethodBuilder {
	private ProtectionLevel _protectionLevel;
	private PolymorphicLevel _polymorphicLevel;
	private string _returnTypeName;
	private string _name;
	private List<ParameterBuilder> _parameters;
	private string _bodyCode;

	public MethodBuilder() {
		ProtectionLevel = ProtectionLevel.Public;
		PolymorphicLevel = PolymorphicLevel.None;
		ReturnTypeName = string.Empty;
		Name = string.Empty;
		_parameters = new List<ParameterBuilder>();
		BodyCode = string.Empty;
	}

	public ProtectionLevel ProtectionLevel {
		get { return _protectionLevel; }
		set { _protectionLevel = value; }
	}

	public PolymorphicLevel PolymorphicLevel {
		get { return _polymorphicLevel; }
		set { _polymorphicLevel = value; }
	}

	public string Name {
		get { return _name; }
		set { _name = value; }
	}

	public string ReturnTypeName {
		get { return _returnTypeName; }
		set {
			if (value == typeof(void).FullName) {
				_returnTypeName = "void";
			} else {
				_returnTypeName = value;
			}
		}
	}

	public List<ParameterBuilder> Parameters {
		get { return _parameters; }
		set { _parameters = value; }
	}

	public string BodyCode {
		get { return _bodyCode; }
		set { _bodyCode = value; }
	}

	public ParameterBuilder AppendParameter() {
		return AppendParameter(
			string.Format("param{0}", Parameters.Count)
		);
	}

	public ParameterBuilder AppendParameter(string name) {
		return AppendParameter(
			typeof(object).Name,
			name
		);
	}

	public ParameterBuilder AppendParameter(string typeName, string name) {
		ParameterBuilder param = new ParameterBuilder();
		param.TypeName = typeName;
		param.ParameterName = name;
		AppendParameter(param);
		return param;
	}

	public void AppendParameter(ParameterBuilder param) {
		Parameters.Add(param);
	}

	public override string ToString() {
		StringBuilder code = new StringBuilder();
		code.Append(
			string.Format(
				"{1} {2} {3} {4}( {5} ) {{{0} {6} {0}}}",
				Environment.NewLine,
				GetProtectionLevelName(),
				GetPolymorphicLevelName(),
				ReturnTypeName,
				Name,
				BuildParameterDeclaration(),
				BodyCode.Tabbify(1)
			)
		);
		return code.ToString();
	}

	public string GetPolymorphicLevelName() {
		string retval = PolymorphicLevel.ToString().ToLower();
		if (retval == "none") {
			retval = string.Empty;
		}
		return retval;
	}

	public string GetProtectionLevelName() {
		return ProtectionLevel.ToString().ToLower();
	}

	public string BuildParameterDeclaration() {
		StringBuilder code = new StringBuilder();
		for (int i = 0; i < Parameters.Count; i++) {
			if (i > 0) {
				code.Append(", ");
			}
			code.Append(Parameters[i].ToString());
		}
		return code.ToString();
	}

	public string BuildMethodArgumentList() {
		StringBuilder arguments = new StringBuilder();
		for (int i = 0; i < Parameters.Count; i++) {
			if (i > 0) {
				arguments.Append(',');
			}
			arguments.Append(Parameters[i].ParameterName);
		}
		return arguments.ToString();
	}

	public string BuildCallToBaseMethod() {
		return string.Format("base.{0}({1});", this.Name, BuildMethodArgumentList());
	}

	/// <summary>
	/// Returns something like MethodName(paramValue1, paramValue2)
	/// </summary>
	/// <returns></returns>
	public string BuildLogEntryHeader() {
		StringBuilder codeSnippet = new StringBuilder();
		codeSnippet.AppendFormat(
			"string.Format(\"{0}(",
			Name
		);
		for (int i = 0; i < Parameters.Count; i++) {
			if (i > 0) {
				codeSnippet.Append(", ");
			}
			codeSnippet.AppendFormat("{{{0}}}", i);
		}
		codeSnippet.Append(")\"");
		if (Parameters.Count > 0) {
			codeSnippet.Append(',');
			codeSnippet.Append(BuildMethodArgumentList());
		}
		codeSnippet.Append(")");
		return codeSnippet.ToString();
	}
}


public class ParameterBuilder {
	private string _typeName;
	private string _parameterName;

	public ParameterBuilder(string typeName, string parameterName) {
		TypeName = typeName;
		ParameterName = parameterName;
	}

	public ParameterBuilder()
		: this(string.Empty, string.Empty) {
	}

	public string TypeName {
		get { return _typeName; }
		set { _typeName = value; }
	}

	public string ParameterName {
		get { return _parameterName; }
		set { _parameterName = value; }
	}

	public override string ToString() {
		return string.Format("{0} {1}", TypeName, ParameterName);
	}
}
