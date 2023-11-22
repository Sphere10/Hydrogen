// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels;

public class WidgetDimensionsStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
	public override Task<Result> OnNextAsync() {
		return Task.FromResult(Result.Success);
	}

	public override Task<Result> OnPreviousAsync() {
		return Task.FromResult(Result.Success);
	}
}
