//-----------------------------------------------------------------------
// <copyright file="BoolExtensions.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public static class BoolExtensions
	{
		public static string YesNo(this bool value)
		{
			if (value)
				return "Yes";
			else
				return "No";
		}

		public static string OpenClosed(this bool value)
		{
			if (value)
				return "Closed";
			else
				return "Open";
		}
	}
}
