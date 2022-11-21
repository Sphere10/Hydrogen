//-----------------------------------------------------------------------
// <copyright file="PageParameterProcessor.cs" company="Sphere 10 Software">
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
using System.Linq;
using Hydrogen;
using Microsoft.AspNetCore.Http;
using Hydrogen;

namespace Hydrogen.Web.AspNetCore {

    /// <summary>
    /// Summary description for PageParameters
    /// </summary>
	public class PageParameterProcessor {


	    public PageParameterProcessor(HttpRequest request) {
			Request = request;
		}

		public bool ContainsParameter(Enum parameter) {
			return !string.IsNullOrEmpty(
				DeterminePageParameterName(parameter)
			);
		}

		public T GetParameter<T>(Enum parameter)  {
			if (!ContainsParameter(parameter)) {
				throw new SoftwareException("Page does not have parameter {0}", DeterminePageParameterName(parameter));
			}
			return Tools.Parser.Parse<T>(Uri.UnescapeDataString(Request.Query[DeterminePageParameterName(parameter)].First()));
		}

		public HttpRequest Request { get; set; }

		static private string GenerateParameter(Enum param, object value) {
			return
				string.Format("{0}={1}", Uri.EscapeDataString(DeterminePageParameterName(param)), Uri.EscapeDataString(value.ToString()));
				
		}

		static private string GenerateInvalidParameterError(string paramName, string expected) {
			return string.Format("Invalid page parameter '{0}' - expected {1}", paramName, expected);
		}


		#region Auxilliary methods

		private static string DeterminePageParameterName(Enum parameter) {
			// Use Parameter attributes name, or if none, just use the Enum name
			var retval = parameter.ToString();
			var attribute =  parameter.GetAttributes<ParameterAttribute>().SingleOrDefault();
			if (attribute != null) {
				retval = attribute.Name;
			}
			return retval;
		}



		#endregion



	}
}
