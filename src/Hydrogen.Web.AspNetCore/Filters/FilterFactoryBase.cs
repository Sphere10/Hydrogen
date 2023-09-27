// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Web.AspNetCore;

/// <summary>
/// Used as a wrapper for a IoC service activated filters.
/// </summary>
/// <typeparam name="TFilter"></typeparam>
public abstract class FilterFactoryBase<TFilter> : Attribute, IFilterFactory where TFilter : IFilterMetadata {
	public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
		=> serviceProvider.GetService<TFilter>();
	public bool IsReusable => false;
}
