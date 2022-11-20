//-----------------------------------------------------------------------
// <copyright file="IPropertyResolver.cs" company="Sphere 10 Software">
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
	/// <summary>
	/// An extension point, where users could define their own 
	/// property resolver
	/// </summary>
	public interface IPropertyResolver 
	{
		object ReadValue(object obj, string propertyPath);
	}
	

}
