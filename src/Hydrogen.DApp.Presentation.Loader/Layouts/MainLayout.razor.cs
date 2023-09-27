// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation.Components.Modal;
using Hydrogen.DApp.Presentation.Services;

namespace Hydrogen.DApp.Presentation.Loader.Layouts;

/// <summary>
/// Main layout
/// </summary>
public partial class MainLayout {
	/// <summary>
	/// Modal component reference
	/// </summary>
	private ModalHost? _modal;

	// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
	[Inject] private IModalService ModalService { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnAfterRender(bool firstRender) {
		if (_modal is not null) {
			ModalService.Initialize(_modal);
		}

		base.OnAfterRender(firstRender);
	}
}
