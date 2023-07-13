// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Hydrogen.Web.AspNetCore;

/// <summary>
/// Set up a custom route for a controller. This attribute can be used
/// multiple times on the same class. It is NOT inherited.
/// 
/// <example>
///     [CustomRoute("admin/advertorialcontent/mixtape/{action}")]
///     [CustomRoute("my/mogsubscribe", DefaultAction = "Index")]
///     [CustomRoute("my/ChangeSubscription/{planType}", DefaultAction = "ChangeSubscription")]
///     [CustomRoute("my/Register", DefaultAction = "Register")]
///     [CustomRoute("RAA/SAML2SignOn", DefaultAction = "SAML2SignOn", RaaRelayState = RaaAction.SubscriptionModify)]
///     [CustomRoute("RAA/SAML2SignOn", DefaultAction = "SAML2SignOn", RaaRelayState = RaaAction.Subscription)]
///     [CustomRoute("RAA/gotoOnFail", DefaultAction = "SAML2FailSubscriptionModify", RaaRelayState = RaaAction.SubscriptionModify)]
///     [CustomRoute("RAA/gotoOnFail", DefaultAction = "SAML2FailSubscription", RaaRelayState = RaaAction.Subscription)]
///     [CustomRoute("my/mogsubscribe/freetrial", DefaultAction = "SubscribeToPlanType", Defaults = "planType=Trial")]
///     [CustomRoute("my/mogsubscribe/basic", DefaultAction = "SubscribeToPlanType", Defaults = "planType=Basic")]
///     [CustomRoute("my/mogsubscribe/premium", DefaultAction = "SubscribeToPlanType", Defaults = "planType=Premium")]
///     [CustomRoute("my/mogsubscribe/promotion", DefaultAction = "SubscribeToPromotion")]
///     [CustomRoute("my/mogsubscribe/success", DefaultAction = "SubscribeConfirm")]
///     [CustomRoute("my/mogsubscribe/CheckScreenName", DefaultAction = "CheckScreenName")]
///     [CustomRoute("my/mogsubscribe/ChangePaymentMethod", DefaultAction = "ChangePaymentMethod")]
/// </example>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CustomRouteAttribute : Attribute {
	private string _defaults;

	/// <summary>
	/// The default action for this route
	/// </summary>
	public string DefaultAction { get; set; }


	/// <summary>
	/// A key1=value1&amp;key2=value2 key/value pair string of defaults for this route
	/// </summary>
	public string Defaults {
		get {
			// concatenate _defaults and DefaultAction into a key=value string
			var toReturn = _defaults ?? string.Empty;
			if (!string.IsNullOrEmpty(DefaultAction)) {
				toReturn += (toReturn.Length > 0 ? "&" : string.Empty) + "action=" + DefaultAction;
			}
			return toReturn;
		}
		set { _defaults = value; }
	}


	public string Url { get; private set; }

	/// <summary>
	/// Determines the priority of this route.
	/// Routes with a higher (larger value) priority are selected before those with a lower priority.
	/// The default priority is zero.
	/// </summary>
	public int Priority { get; set; }

	/// <summary>
	/// Sets up a custom route for a <seealso cref="Controller"/> derived class
	/// </summary>
	/// <param name="url">The route url</param>
	public CustomRouteAttribute(string url) {
		if ((!String.IsNullOrEmpty(url)) && (url.ToLower().Contains("{controller}")))
			throw new Exception("A custom route contains '{controller}'. Please revise the route.");
		Url = url;
	}


	public static void RegisterCustomRoutesFromAssembly(Assembly assembly) {
		assembly.GetDerivedTypes<Controller>().ForEach(ProcessType);
	}

	public static void ProcessType(Type type) {
		(from customRouteAttribute in type.GetCustomAttributesOfType<CustomRouteAttribute>()
		 orderby customRouteAttribute.Priority descending
		 select customRouteAttribute)
			.ForEach(customRouteAttribute => ProcessCustomRouteAttribute(type, customRouteAttribute));

	}

	public static void ProcessCustomRouteAttribute(Type sourceType, CustomRouteAttribute attribute) {
		var defaults = new RouteValueDictionary();
		var defaultDict = StringExtensions.ParseQueryString(attribute.Defaults);
		foreach (string key in defaultDict.Keys) {
			defaults.Add(key, defaultDict[key]);
		}
		defaults.Add("controller", sourceType.Name.Substring(0, sourceType.Name.LastIndexOf("Controller")));
		RouteValueDictionary constraints = new RouteValueDictionary();
		//constraints.Add("RaaRelayState", new RaaRelayStateConstraint(attribute.RaaRelayState));
		throw new NotImplementedException();
		//RouteTable.Routes.Add(new Route(attribute.Url, defaults, constraints, new MvcRouteHandler()));
	}

}
