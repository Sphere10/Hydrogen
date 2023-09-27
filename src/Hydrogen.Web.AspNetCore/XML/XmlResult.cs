// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Web.AspNetCore;

public class XmlResult : ContentResult {

	public XmlResult() : this(200) {
	}

	public XmlResult(int statusCode) : this(string.Empty, 200) {
	}

	public XmlResult(string xml) : this(xml, 200) {
	}

	public XmlResult(object serializableResult) : this(serializableResult, 200) {
	}

	public XmlResult(object serializableResult, int statusCode) : this(Tools.Xml.WriteToString(serializableResult, Encoding.UTF8), statusCode) {
	}

	public XmlResult(string xml, int statusCode) {
		this.ContentType = "application/xml";
		this.Content = xml;
		this.StatusCode = statusCode;
	}

}
