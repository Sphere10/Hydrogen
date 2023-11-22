// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Hydrogen.DApp.Presentation.Components.Wizard;
using Hydrogen.DApp.Presentation.WidgetGallery.Extensions;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Components;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.ViewModels;

public class NewWidgetWizardStepViewModel : WizardStepViewModelBase<NewWidgetModel> {
	private IValidator<NewWidgetModel> Validator { get; }

	public NewWidgetWizardStepViewModel(IValidator<NewWidgetModel> validator) {
		Validator = validator ?? throw new ArgumentNullException(nameof(validator));
	}

	/// <inheritdoc />
	public override async Task<Result> OnNextAsync() {
		ValidationResult result = await Validator.ValidateAsync(Model);

		if (result.IsValid) {
			if (Model.AreDimensionsKnown) {
				Wizard.UpdateSteps(StepUpdateType.Inject, new[] { typeof(WidgetDimensionsStep) });
			}
		}

		return result.ToResult();
	}

	/// <inheritdoc />
	public override Task<Result> OnPreviousAsync() {
		return Task.FromResult(Result.Success);
	}
}
