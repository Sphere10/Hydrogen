using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Web.AspNetCore;

public class XmlResult : ContentResult {

	public XmlResult() : this(200) {
	}

	public XmlResult(int statusCode) : this(string.Empty, 200){
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

