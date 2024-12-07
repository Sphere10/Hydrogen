//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using Hydrogen.DApp.Presentation.Loader.Components;
//using Hydrogen.DApp.Presentation.Loader.Plugins;
//using Hydrogen.DApp.Presentation.Loader.ViewModels;

//namespace Hydrogen.DApp.Presentation.Loader.Tests.ComponentTests;

//public class Tests : Bunit.TestContext {
//	[SetUp]
//	public void Setup() {
//		Services.AddTransient<BlockMenuViewModel>();
//		Services.AddTransient<IAppManager, DefaultAppManager>();
//		Services.AddTransient<IPluginManager, DefaultPluginManager>();
//		Services.AddTransient<IPluginLocator, StaticPluginLocator>();

//		Services.AddLogging();
//	}

//	[Test]
//	public void BlockMenuRenders() {
//		var component = RenderComponent<BlockMenu>();

//		ClassicAssert.AreEqual(1, component.RenderCount);
//		ClassicAssert.NotNull(component.Instance);
//	}
//}
