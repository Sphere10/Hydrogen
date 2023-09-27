// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Hydrogen.DApp.Presentation.Loader.Components;

public partial class MainMenu {
	// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
	[Inject] private IJSRuntime JsRuntime { get; set; } = null!;

	protected override Task OnAfterRenderAsync(bool firstRender) {
		if (firstRender) {
			JsRuntime.InvokeVoidAsync("addDropdownHover");
		}

		return base.OnAfterRenderAsync(firstRender);
	}
}
