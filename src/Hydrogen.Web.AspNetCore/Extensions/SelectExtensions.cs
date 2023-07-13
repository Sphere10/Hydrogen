// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Hydrogen.Web.AspNetCore;

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

		return htmlHelper.DropDownList(inputName, Tools.Web.AspNetCore.ToSelectList(typeof(TProperty), value as Enum));
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
			items.Insert(0,
				new SelectListItem() {
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
