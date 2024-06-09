// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using System.Reflection;

namespace Hydrogen;

public class ErrorMessageAttribute : Attribute {
	private string _errMsg;

	public ErrorMessageAttribute(string errMsg) {
		ErrorMessage = errMsg;
	}

	public string ErrorMessage {
		get { return _errMsg; }
		set { _errMsg = value; }
	}

	public static bool HasErrorMessage(Enum enumeration) {
		return enumeration.GetType().GetCustomAttributes(typeof(ErrorMessageAttribute), true).Length > 0;
	}

	public static string GetErrorMessage(Enum enumeration) {
		StringBuilder retval = new StringBuilder();
		FieldInfo enumDecl = enumeration.GetType().GetField(enumeration.ToString());
		if (enumDecl != null) {
			object[] attrs = enumDecl.GetCustomAttributes(typeof(ErrorMessageAttribute), true);
			for (int i = 0; i < attrs.Length; i++) {
				if (i > 0) {
					retval.Append(Environment.NewLine);
				}
				retval.Append(((ErrorMessageAttribute)attrs[i]).ErrorMessage);
			}
		}
		return retval.ToString();
	}

}
