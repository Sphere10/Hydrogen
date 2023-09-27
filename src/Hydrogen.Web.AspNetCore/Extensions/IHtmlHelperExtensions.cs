// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;

namespace Hydrogen.Web.AspNetCore;

public static class HtmlHelperExtensions {

	public static BootstrapFormScope<TModel> BeginBootstrapForm<TModel>(this IHtmlHelper<TModel> htmlHelper, string url, TModel formModel, AttributeDictionary attributes = null, FormScopeOptions options = FormScopeOptions.Default)
		where TModel : FormModelBase {
		return new BootstrapFormScope<TModel>(htmlHelper, url, formModel, attributes, options);
	}

	public static IDisposable BeginBootstrapForm<TModel>(this IHtmlHelper<TModel> htmlHelper, string action, string controller, TModel formModel, AttributeDictionary attributes = null, FormScopeOptions options = FormScopeOptions.Default)
		where TModel : FormModelBase {
		return new BootstrapFormScope<TModel>(htmlHelper, action, controller, formModel, attributes, options);
	}

	public static IHtmlContent BootstrapFormButton<TModel>(this IHtmlHelper htmlHelper, TModel formModel, string text, AttributeDictionary attributes = null) where TModel : FormModelBase {
		return htmlHelper.Raw(Tools.Text.FormatWithDictionary(BootstrapFormScope<TModel>.ButtonHtmlSnippet,
			new Dictionary<string, object> { ["formId"] = formModel.ID, ["text"] = text, ["class"] = attributes?.ContainsKey("class") == true ? attributes["class"] : string.Empty },
			false));
	}

	public static IHtmlContent ValidationMessageForEx<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, AttributeDictionary attributes = null, bool floatEnd = false) {
		attributes ??= new AttributeDictionary();
		if (!attributes.ContainsKey("class"))
			attributes["class"] = string.Empty;
		attributes["class"] += $" label label-warning{"float-end".AsAmendmentIf(floatEnd)}";
		return helper.ValidationMessageFor(expression, null, attributes?.ToDictionary(x => x.Key, x => x.Value as object));
	}

	public static IHtmlContent BootstrapLabelFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, AttributeDictionary attributes = null) {
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		return htmlHelper.LabelFor(expression, display, attributes?.ToDictionary(x => x.Key, x => x.Value as object));
	}

	public static IHtmlContent BootstrapTextBoxFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, AttributeDictionary attributes = null, bool @readonly = false) {
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value) && value.ValidationState == ModelValidationState.Invalid;

		attributes ??= new AttributeDictionary();
		if (!attributes.ContainsKey("class"))
			attributes["class"] = string.Empty;

		attributes["placeholder"] = display;
		attributes["class"] += $" form-control";


		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		if (@readonly) {
			attributes["readonly"] = string.Empty;
		}

		return htmlHelper.TextBoxFor(expression, attributes?.ToDictionary(x => x.Key, x => x.Value as object));
	}

	public static IHtmlContent BootstrapPasswordFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, AttributeDictionary attributes = null) {
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value) && value.ValidationState == ModelValidationState.Invalid;

		attributes ??= new AttributeDictionary();
		if (!attributes.ContainsKey("class"))
			attributes["class"] = string.Empty;

		attributes["placeholder"] = display;
		attributes["class"] += $" form-control";


		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		return htmlHelper.PasswordFor(expression, attributes?.ToDictionary(x => x.Key, x => x.Value as object));
	}

	public static IHtmlContent BootstrapTextAreaFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, int approxRows = 5, AttributeDictionary attributes = null) {
		Guard.ArgumentInRange(approxRows, 1, 9999, nameof(approxRows));
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value) && value.ValidationState == ModelValidationState.Invalid;

		attributes ??= new AttributeDictionary();
		attributes["placeholder"] = display;

		if (!attributes.ContainsKey("class"))
			attributes["class"] = string.Empty;
		attributes["class"] += $" form-control";

		if (!attributes.ContainsKey("style"))
			attributes["style"] = string.Empty;
		attributes["style"] = $"height: {Math.Round(1.85 * approxRows, 2).ClipTo(4, double.MaxValue)}em";

		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		return htmlHelper.TextAreaFor(expression, attributes?.ToDictionary(x => x.Key, x => x.Value as object));
	}

	public static IHtmlContent BootstrapDropDownListFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, IEnumerable<SelectListItem> selectList, AttributeDictionary attributes = null,
	                                                                  bool useChoicesJS = true) {
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value) && value.ValidationState == ModelValidationState.Invalid;
		attributes ??= new AttributeDictionary();
		if (!attributes.ContainsKey("class"))
			attributes["class"] = string.Empty;

		attributes["placeholder"] = display;
		attributes["class"] += $" form-select";

		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		if (useChoicesJS)
			attributes["sp10-choices"] = null;

		return htmlHelper.DropDownListFor(expression, selectList, display, attributes?.ToDictionary(x => x.Key, x => x.Value as object));

		// @Html.DropDownListFor(o => o.Budget, budgets, "Estimated Budget", new { @class = "form-select input-validated", @data_choices=""  })
	}

	public static IHtmlContent BootstrapCheckBoxAndLabelFor<T>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, bool>> expression, AttributeDictionary inputAttributes = null, AttributeDictionary labelAttributes = null) {
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value) && value.ValidationState == ModelValidationState.Invalid;

		var htmlContentBuilder = new HtmlContentBuilder();

		htmlContentBuilder.AppendHtml(
			htmlHelper.Raw(
				"""
				<div class="form-check">
				"""
			)
		);

		htmlContentBuilder.AppendHtml(WriteCheckbox());
		htmlContentBuilder.AppendHtml(WriteLabel());
		htmlContentBuilder.AppendHtml(
			htmlHelper.Raw(
				"""
				</div>
				"""
			)
		);

		return htmlContentBuilder.ToHtmlContent(HtmlEncoder.Default);

		#region Nested Functions

		IHtmlContent WriteCheckbox() {

			inputAttributes ??= new AttributeDictionary();
			if (!inputAttributes.ContainsKey("class"))
				inputAttributes["class"] = string.Empty;

			inputAttributes["placeholder"] = display;
			inputAttributes["class"] += $" form-check-input";


			if (hasError) {
				inputAttributes["class"] += $" is-invalid";
				inputAttributes["data-bs-toggle"] = "tooltip";
				inputAttributes["data-bs-placement"] = "top";
				inputAttributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
			}

			return htmlHelper.CheckBoxFor(expression, inputAttributes?.ToDictionary(x => x.Key, x => x.Value as object));
		}

		IHtmlContent WriteLabel() {
			labelAttributes ??= new AttributeDictionary();
			if (!labelAttributes.ContainsKey("class"))
				labelAttributes["class"] = string.Empty;

			labelAttributes["placeholder"] = display;
			labelAttributes["class"] += $" form-check-label";


			// TODO: remove error stuff from label?
			if (hasError) {
				labelAttributes["class"] += $" is-invalid";
				labelAttributes["data-bs-toggle"] = "tooltip";
				labelAttributes["data-bs-placement"] = "top";
				labelAttributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
			}

			return htmlHelper.LabelFor(expression, labelAttributes?.ToDictionary(x => x.Key, x => x.Value as object));
		}

		#endregion

	}

}
