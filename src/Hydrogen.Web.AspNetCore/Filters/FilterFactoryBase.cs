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
