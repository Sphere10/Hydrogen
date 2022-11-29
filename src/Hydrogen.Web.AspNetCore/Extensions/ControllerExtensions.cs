//-----------------------------------------------------------------------
// <copyright file="ControllerExtensions.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Web.AspNetCore {
    public static class ControllerExtensions {
        public static string MapUrl(this Controller controller, string path, bool absolute = false, string schemeOverride = null) {
			// HS 2019-02-24: need to use IHostingEnvironment
	        //string appPath = controller.Server.MapPath("/").ToLower();
	        var appPath = AppDomain.CurrentDomain.BaseDirectory.ToLower();

	        var relUrl = string.Format("/{0}", path.ToLower().Replace(appPath, "").Replace(@"\", "/"));
            if (absolute) {
                var scheme = (schemeOverride ?? controller.Request.GetUri().Scheme).ToLower();
                var port = controller.Request.GetUri().Port;
                var includePort = string.IsNullOrWhiteSpace(schemeOverride) && (scheme == "http" && port != 80 || scheme == "https" && port != 443);
                relUrl = scheme + "://" + controller.Request.GetUri().DnsSafeHost + (includePort ? ":" + port : string.Empty) + relUrl;
            }
            return relUrl;
        }

		public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false) {
			if (string.IsNullOrEmpty(viewName)) {
				viewName = controller.ControllerContext.ActionDescriptor.ActionName;
			}

			controller.ViewData.Model = model;

			using (var writer = new StringWriter()) {
				IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
				ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

				if (viewResult.Success == false) {
					return $"A view with the name {viewName} could not be found";
				}

				ViewContext viewContext = new ViewContext(
					controller.ControllerContext,
					viewResult.View,
					controller.ViewData,
					controller.TempData,
					writer,
					new HtmlHelperOptions()
				);

				try {
				await viewResult.View.RenderAsync(viewContext);
				} catch (Exception ex) {
					var xxx = ex;
				}

				return writer.GetStringBuilder().ToString();
			}
		}
	}


}
