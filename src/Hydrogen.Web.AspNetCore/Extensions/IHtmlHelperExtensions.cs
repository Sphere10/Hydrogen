using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Hydrogen;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hydrogen.Web.AspNetCore;
public static class HtmlHelperExtensions {


	public static IDisposable BeginBootstrapForm<TModel>(this IHtmlHelper<TModel> htmlHelper, string action, string controller, TModel formModel, string formClass = null, FormScopeOptions options = FormScopeOptions.Default) where TModel : FormModelBase {
		return new BootstrapFormScope<TModel>(htmlHelper, action, controller, formModel, formClass, options);
	}

	public static IHtmlContent BootstrapFormButton<TModel>(this IHtmlHelper htmlHelper, TModel formModel,  string text) where TModel : FormModelBase  {
		return htmlHelper.Raw( Tools.Text.FormatWithDictionary( BootstrapFormScope<TModel>.ButtonHtmlSnippet, new Dictionary<string, object> { ["formId"] = formModel.ID, ["text"] = text  }, false));
	}

	public static IHtmlContent ValidationMessageForEx<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool floatEnd = false) {
		return helper.ValidationMessageFor(expression, null, new { @class = $"label label-warning{" float-end".AsAmendmentIf(floatEnd)}" });
	}

	public static IHtmlContent BootstrapLabelFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression){
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		return htmlHelper.LabelFor(expression, display);
	}

	public static IHtmlContent BootstrapTextBoxFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression){
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value ) && value.ValidationState == ModelValidationState.Invalid;
		var attributes = new Dictionary<string, object>() {
			["placeholder"] = display,
			["class"] = "form-control"
		};

		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		return htmlHelper.TextBoxFor(expression, display, attributes );
	}

	public static IHtmlContent BootstrapTextAreaFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, int rows){
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value ) && value.ValidationState == ModelValidationState.Invalid;
		var attributes = new Dictionary<string, object>() {
			["placeholder"] = display,
			["class"] = "form-control",
			["rows"] = rows
		};

		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		return htmlHelper.TextAreaFor(expression, attributes );
	}



	public static IHtmlContent BootstrapDropDownListFor<T, TProperty>(this IHtmlHelper<T> htmlHelper, Expression<Func<T, TProperty>> expression, IEnumerable<SelectListItem> selectList, bool useChoicesJS = true){
		var member = expression.ResolveMember();
		var display = member.TryGetCustomAttributeOfType<DisplayNameAttribute>(true, out var displayName) ? displayName.DisplayName : string.Empty;
		if (member.HasAttribute<RequiredAttribute>(true)) {
			display += " *";
		}
		var hasError = htmlHelper.ViewData.ModelState.TryGetValue(member.Name, out var value ) && value.ValidationState == ModelValidationState.Invalid;
		var attributes = new Dictionary<string, object>() {
			["placeholder"] = display,
			["class"] = "form-select"
		};

		if (hasError) {
			attributes["class"] += $" is-invalid";
			attributes["data-bs-toggle"] = "tooltip";
			attributes["data-bs-placement"] = "top";
			attributes["data-bs-original-title"] = value.Errors.Select(x => x.ErrorMessage).ToDelimittedString(" ");
		}

		if (useChoicesJS)
			attributes["sp10-choices"] = null;

		return htmlHelper.DropDownListFor(expression, selectList, display, attributes );

		// @Html.DropDownListFor(o => o.Budget, budgets, "Estimated Budget", new { @class = "form-select input-validated", @data_choices=""  })
	}


}

