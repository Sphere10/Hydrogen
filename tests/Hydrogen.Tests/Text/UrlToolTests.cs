// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class UrlToolTests {


	[Test]
	public void ParseQueryString_WithJSON() {
		Assert.DoesNotThrow(() =>
			Tools.Url.ParseQueryString(
				"&siteContext=1&actionName=SqlSelect&actionParameters=%7B%22sql%22%3A+%22SELECT+%09%09%09%09%09%09FT.ID+AS+FormTypeID%2C+%09%09%09%09%09%09FC.Name+AS+FormCategoryName%2C+%09%09%09%09%09%09FT.Name+AS+FormTypeName%2C+%09%09%09%09%09%09FT.ImageBlobID+AS+PinBlobID+%09%09%09%09%09FROM+%09%09%09%09%09%09FormType+FT+INNER+JOIN+%09%09%09%09%09%09FormCategory+FC+ON+FT.FormCategoryID+%3D+FC.ID+%09%09%09%09%09ORDER+BY+%09%09%09%09%09%09FC.Name%2C+FT.Name%22%7D"));
	}

	[Test]
	public void TryParseTest() {
		var testUrl =
			"https://s3.us-west-2.amazonaws.com/secure.notion-static.com/d949b99c-eaae-4875-9523-dc697b03cfff/FunctionDerivative.cs?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Content-Sha256=UNSIGNED-PAYLOAD&X-Amz-Credential=AKIAT73L2G45EIPT3X45%2F20220531%2Fus-west-2%2Fs3%2Faws4_request&X-Amz-Date=20220531T232930Z&X-Amz-Expires=3600&X-Amz-Signature=a61e78f152657e28a02d10f8bc815c7a27d637d8be9563aaa555b7f955419314&X-Amz-SignedHeaders=host&x-id=GetObject";
		Assert.That(Tools.Url.TryParse(testUrl, out var protocol, out var port, out var host, out var path, out var queryString), Is.True);
		Assert.That(protocol, Is.EqualTo("https"));
		Assert.That(port, Is.Null);
		Assert.That(host, Is.EqualTo("s3.us-west-2.amazonaws.com"));
		Assert.That(path, Is.EqualTo("/secure.notion-static.com/d949b99c-eaae-4875-9523-dc697b03cfff/FunctionDerivative.cs"));
		Assert.That(queryString,
			Is.EqualTo(
				"?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Content-Sha256=UNSIGNED-PAYLOAD&X-Amz-Credential=AKIAT73L2G45EIPT3X45%2F20220531%2Fus-west-2%2Fs3%2Faws4_request&X-Amz-Date=20220531T232930Z&X-Amz-Expires=3600&X-Amz-Signature=a61e78f152657e28a02d10f8bc815c7a27d637d8be9563aaa555b7f955419314&X-Amz-SignedHeaders=host&x-id=GetObject"));
	}

	[Test]
	public void TryParseTest_WithPort() {
		var testUrl =
			"https://s3.us-west-2.amazonaws.com:443/secure.notion-static.com/d949b99c-eaae-4875-9523-dc697b03cfff/FunctionDerivative.cs?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Content-Sha256=UNSIGNED-PAYLOAD&X-Amz-Credential=AKIAT73L2G45EIPT3X45%2F20220531%2Fus-west-2%2Fs3%2Faws4_request&X-Amz-Date=20220531T232930Z&X-Amz-Expires=3600&X-Amz-Signature=a61e78f152657e28a02d10f8bc815c7a27d637d8be9563aaa555b7f955419314&X-Amz-SignedHeaders=host&x-id=GetObject";
		Assert.That(Tools.Url.TryParse(testUrl, out var protocol, out var port, out var host, out var path, out var queryString), Is.True);
		Assert.That(protocol, Is.EqualTo("https"));
		Assert.That(port, Is.EqualTo(443));
		Assert.That(host, Is.EqualTo("s3.us-west-2.amazonaws.com"));
		Assert.That(path, Is.EqualTo("/secure.notion-static.com/d949b99c-eaae-4875-9523-dc697b03cfff/FunctionDerivative.cs"));
		Assert.That(queryString,
			Is.EqualTo(
				"?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Content-Sha256=UNSIGNED-PAYLOAD&X-Amz-Credential=AKIAT73L2G45EIPT3X45%2F20220531%2Fus-west-2%2Fs3%2Faws4_request&X-Amz-Date=20220531T232930Z&X-Amz-Expires=3600&X-Amz-Signature=a61e78f152657e28a02d10f8bc815c7a27d637d8be9563aaa555b7f955419314&X-Amz-SignedHeaders=host&x-id=GetObject"));
	}

}
