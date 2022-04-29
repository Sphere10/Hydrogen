using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sphere10.Framework.Web.AspNetCore {
	public static class HtmlHelperExtensions {

		public static IDisposable BeginFormEx<T>(this IHtmlHelper<T> htmlHelper, T formModel, string formClass = null, bool resetOnSuccess = true) where T : FormModelBase, new() {
			return new FormScope<T>(htmlHelper, formModel ?? new T(), formClass, resetOnSuccess);
		}

		public static IDisposable BeginFormEx<T>(this IHtmlHelper<T> htmlHelper, string action, string controller, T formModel, string formClass = null, bool resetOnSuccess = true) where T : FormModelBase, new() {
			return new FormScope<T>(htmlHelper, action, controller, formModel ?? new T(), formClass, resetOnSuccess);
		}
		
		public static IHtmlContent ValidationMessageForEx<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression) {
			return helper.ValidationMessageFor(expression, null, new { @class = "label label-warning" });
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
}
