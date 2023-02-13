using System;
using System.Runtime.InteropServices;

namespace Hydrogen.Application;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyProductDrmApi : Attribute {

	public AssemblyProductDrmApi(string url) {
		ApiUrl = url;
	}

	public string ApiUrl { get; set; }

}
