//-----------------------------------------------------------------------
// <copyright file="ProductUsageInformation.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Sphere10.Framework.Application {

	public class ProductUsageInformation {

		public DateTime FirstUsedDateBySystemUTC { get; set; }
		public int DaysUsedBySystem { get; set; }
		public int NumberOfUsesBySystem { get; set; }
		public DateTime FirstUsedDateByUserUTC { get; set; }
		public int DaysUsedByUser { get; set; }
		public int NumberOfUsesByUser { get; set; }



		public string ProcessTokensInString(string source) {
			source = source.Replace("{FirstUsedDateBySystemUTC}", string.Format("{0:yyyy-MM-dd}",FirstUsedDateBySystemUTC));
			source = source.Replace("{DaysUsedBySystem}", DaysUsedBySystem.ToString());
			source = source.Replace("{NumberOfUsesBySystem}", NumberOfUsesBySystem.ToString());
			source = source.Replace("{FirstUsedDateByUserUTC}", string.Format("{0:yyyy-MM-dd}", FirstUsedDateByUserUTC));
			source = source.Replace("{DaysUsedByUser}", DaysUsedByUser.ToString());
			source = source.Replace("{NumberOfUsesByUser}", NumberOfUsesByUser.ToString());

			// System specific stuff
			source = source.Replace("{CurrentYear}", DateTime.Now.Year.ToString());
			source = source.Replace("{StartPath}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			return source;
		}


	}
}
