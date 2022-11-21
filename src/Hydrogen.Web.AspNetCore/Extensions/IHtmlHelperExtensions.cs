using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Hydrogen;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hydrogen.Web.AspNetCore;
public static class HtmlHelperExtensions {


	public static IDisposable BeginBootstrapForm(this IHtmlHelper htmlHelper, string action, string controller, string formId, string formClass = null, FormScopeOptions options = FormScopeOptions.Default){
		return new BootstrapFormScope(htmlHelper, action, controller, formId, formClass, options);
	}

	public static IHtmlContent BootstrapFormButton(this IHtmlHelper htmlHelper, string formId, string text) {
		return htmlHelper.Raw( Tools.Text.FormatWithDictionary( BootstrapFormScope.ButtonHtmlSnippet, new Dictionary<string, object> { ["formId"] = formId, ["text"] = text  }, false));
	}
	public static IHtmlContent ValidationMessageForEx<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool floatEnd = false) {
		return helper.ValidationMessageFor(expression, null, new { @class = $"label label-warning{" float-end".AsAmendmentIf(floatEnd)}" });
	}

	public static SelectList ToSelectList<TEnum>(this TEnum @enum)
		where TEnum : struct, IComparable, IFormattable, IConvertible {
		var values = Enum.GetValues(typeof(TEnum))
			.Cast<TEnum>()
			.Select(e => new {
				Id = e,
				Name = e.ToString(CultureInfo.InvariantCulture)
			});
		return new SelectList(values, "Id", "Name", @enum);
	}

}

