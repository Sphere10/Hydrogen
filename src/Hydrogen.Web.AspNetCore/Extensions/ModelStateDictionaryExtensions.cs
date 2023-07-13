// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hydrogen.Web.AspNetCore;

public static class ModelStateDictionaryExtensions {

	public static void AddModelError(this ModelStateDictionary modelState, Result result) {
		result.ErrorMessages.ForEach(modelState.AddModelError);
	}

	public static void AddModelError(this ModelStateDictionary modelState, string key, Result result) {
		result.ErrorMessages.ForEach(r => modelState.AddModelError(key, r));
	}

	public static void AddModelError(this ModelStateDictionary modelState, string error) {
		modelState.AddModelError(string.Empty, error);
	}

	public static void AddModelError(this ModelStateDictionary modelState, string error, params object[] formatArgs) {
		modelState.AddModelError(string.Empty, error.FormatWith(formatArgs));
	}

	/// <summary>
	/// Excludes the list of model properties from model validation.
	/// </summary>
	/// <param name="modelState">The model state dictionary which holds the state of model data being interpreted.</param>
	/// <param name="modelProperties">A string array of delimited string property names of the model to be excluded from the model state validation.</param>
	public static void Remove(this ModelStateDictionary modelState, params string[] modelProperties) {
		foreach (var prop in modelProperties)
			modelState.Remove(prop);
	}

}
