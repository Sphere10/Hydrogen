// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.AspNetCore.Components;

namespace Hydrogen.DApp.Presentation.Loader.Tests;

public class TestNavigationManager : NavigationManager {
	public TestNavigationManager() {
		Initialize("http://localhost/", "http://localhost/");
	}

	public TestNavigationManager(string baseUri = null, string uri = null) {
		Initialize(baseUri ?? "http://localhost/", uri ?? baseUri ?? "http://localhost/");
	}

	public new void Initialize(string baseUri, string uri) {
		base.Initialize(baseUri, uri);
	}

	protected override void NavigateToCore(string uri, bool forceLoad) {
		Uri = BaseUri + uri.TrimStart('/');
		NotifyLocationChanged(forceLoad);
	}
}
