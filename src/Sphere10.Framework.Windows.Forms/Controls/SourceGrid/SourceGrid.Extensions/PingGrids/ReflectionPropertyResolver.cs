//-----------------------------------------------------------------------
// <copyright file="ReflectionPropertyResolver.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Extensions.PingGrids
{
	public class ReflectionPropertyResolver : IPropertyResolver
	{
		public static ReflectionPropertyResolver SharedInstance = new ReflectionPropertyResolver();
		
		public object ReadValue(object obj, string propertyPath)
		{
			var propertyInfo = obj.GetType().GetProperty(propertyPath);
			if (propertyInfo == null)
				return string.Empty;
			return propertyInfo.GetValue(obj, null);
		}
		
	}
}
