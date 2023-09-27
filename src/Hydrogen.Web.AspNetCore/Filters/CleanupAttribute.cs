// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hydrogen.Web.AspNetCore;

/// <summary>
///     [Cleanup]
///     ActionResult MyAction(int id, string otherParam) {
///         var fileName = id.ToString() + otherParam + ".txt";
///         File.Create(fileName)
///         ViewBag.Cleanup.Add( () => File.Delete(fileName));
///         return base.FileResult(fileName);
/// }
/// </summary>
public class CleanupAttribute : ActionFilterAttribute {
	public override void OnActionExecuting(ActionExecutingContext filterContext) {

		((Controller)filterContext.Controller).ViewBag.Cleanup = new List<Action>();
		base.OnActionExecuting(filterContext);
	}

	public override void OnResultExecuted(ResultExecutedContext filterContext) {
		List<Action> cleanupActions = ((Controller)filterContext.Controller).ViewBag.Cleanup;
		foreach (var cleanupAction in cleanupActions) {
			try {
				cleanupAction();
			} catch (Exception error) {
				SystemLog.Exception(error);
			}
		}
	}
}
