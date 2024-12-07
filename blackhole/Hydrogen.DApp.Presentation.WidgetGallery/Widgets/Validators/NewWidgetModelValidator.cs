// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentValidation;
using Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Validators;

public class NewWidgetModelValidator : AbstractValidator<NewWidgetModel> {
	public NewWidgetModelValidator() {
		RuleFor(x => x.Price).NotNull().GreaterThan(0);
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.Description).NotEmpty();
	}
}
