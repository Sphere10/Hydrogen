// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Services;
using Hydrogen.DApp.Presentation.ViewModels;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels;

public class TablesViewModel : ExtendedComponentViewModel {
	public INodeService NodeService { get; }

	public TablesViewModel(INodeService nodeService, IEndpointManager endpointManager) : base(endpointManager) {
		NodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
	}

	protected override async Task InitCoreAsync() {
		await Task.Delay(3000);
	}
}
