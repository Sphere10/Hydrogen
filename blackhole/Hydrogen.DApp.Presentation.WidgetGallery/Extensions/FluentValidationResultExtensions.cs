// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using FluentValidation.Results;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Extensions;

public static class FluentValidationResultExtensions {

	public static Result ToResult(this ValidationResult validationResult) {
		if (validationResult.IsValid) {
			return Result.Success;
		} else {
			var result = new Result();
			foreach (var error in validationResult.Errors) {
				result.AddError(error.ErrorMessage);
			}
			return result;
		}
	}

}
