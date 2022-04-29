//-----------------------------------------------------------------------
// <copyright file="SelectExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sphere10.Framework.Web.AspNetCore {
    public static class SelectExtensions {

		public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression) {
			if (expression.Body.NodeType == ExpressionType.Call) {
				MethodCallExpression methodCallExpression = (MethodCallExpression)expression.Body;
				string name = GetInputName(methodCallExpression);
				return name.Substring(expression.Parameters[0].Name.Length + 1);

			}
			return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
		}

		private static string GetInputName(MethodCallExpression expression) {
			// p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...
			MethodCallExpression methodCallExpression = expression.Object as MethodCallExpression;
			if (methodCallExpression != null) {
				return GetInputName(methodCallExpression);
			}
			return expression.Object.ToString();
		}

		public static IHtmlContent EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class {
			string inputName = GetInputName(expression);
			var value = htmlHelper.ViewData.Model == null
				? default(TProperty)
				: expression.Compile()(htmlHelper.ViewData.Model);

			return htmlHelper.DropDownList(inputName, ToSelectList(typeof(TProperty), value.ToString()));
		}

		public static SelectList ToSelectList(Type enumType, string selectedItem) {
			List<SelectListItem> items = new List<SelectListItem>();
			foreach (var item in Enum.GetValues(enumType)) {
				FieldInfo fi = enumType.GetField(item.ToString());
				var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
				var listItem = new SelectListItem {
					Value = ((int)item).ToString(),
					Text = title,
					Selected = selectedItem == ((int)item).ToString()
				};
				items.Add(listItem);
			}

			return new SelectList(items, "Value", "Text");
		}

		public static List<SelectListItem> ToSelectList<T>(
				this IEnumerable<T> enumerable,
				Func<T, string> text,
				Func<T, string> value,
				string defaultOption = null,
				Func<T, bool> selected = null
			) {
			var items = enumerable.Select(f => new SelectListItem() {
				Text = text(f),
				Value = value(f),
				Selected = selected != null ? selected(f) : false
			}).ToList();
			if (defaultOption != null) {
				items.Insert(0, new SelectListItem() {
					Text = defaultOption,
					Value = "-1"
				});
			}
			return items;
		}


		public static SelectListItem GetSelectedItem(this IEnumerable<SelectListItem> selectListItems) {
			return selectListItems.Single(i => i.Selected);
		}
	}
}
