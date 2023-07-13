// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public class ModuleConfiguration : ModuleConfigurationBase {
	public override void RegisterComponents(IServiceCollection serviceCollection) {
		if (HydrogenFramework.Instance.Options.HasFlag(HydrogenFrameworkOptions.EnableDrm))
			EnableDRM(serviceCollection);
		else
			DisableDRM(serviceCollection);


		// EnableHooks(serviceCollection);

		if (!serviceCollection.HasImplementationFor<IProductReportBugDialog>())
			serviceCollection.AddTransient<IProductReportBugDialog, ProductProductReportBugDialog>();

		if (!serviceCollection.HasImplementationFor<IProductRequestFeatureDialog>())
			serviceCollection.AddTransient<IProductRequestFeatureDialog, ProductProductRequestFeatureDialog>();

		if (!serviceCollection.HasImplementationFor<IProductSendCommentsDialog>())
			serviceCollection.AddTransient<IProductSendCommentsDialog, ProductProductSendCommentsDialog>();


		// This is the primary form of the application, so register it as a provider of the below services

		if (!serviceCollection.HasControlStateEventProvider<FormEx>())
			serviceCollection.AddControlStateEventProvider<FormEx, FormEx.StateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<UserControlEx>())
			serviceCollection.AddControlStateEventProvider<UserControlEx, UserControlEx.StateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<SplitContainer>())
			serviceCollection.AddControlStateEventProvider<SplitContainer, SplitContainerControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<TabControl>())
			serviceCollection.AddControlStateEventProvider<TabControl, TabControlStateEventProvider>();


		if (!serviceCollection.HasControlStateEventProvider<Panel>())
			serviceCollection.AddControlStateEventProvider<Panel, ContainerControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<GroupBox>())
			serviceCollection.AddControlStateEventProvider<GroupBox, ContainerControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<PathSelectorControl>())
			serviceCollection.AddControlStateEventProvider<PathSelectorControl, PathSelectorControl.StateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<NumericUpDown>())
			serviceCollection.AddControlStateEventProvider<NumericUpDown, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<TextBox>())
			serviceCollection.AddControlStateEventProvider<TextBox, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<ComboBox>())
			serviceCollection.AddControlStateEventProvider<ComboBox, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<RadioButton>())
			serviceCollection.AddControlStateEventProvider<RadioButton, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<CheckBox>())
			serviceCollection.AddControlStateEventProvider<CheckBox, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<CheckedListBox>())
			serviceCollection.AddControlStateEventProvider<CheckedListBox, CommonControlStateEventProvider>();

		if (!serviceCollection.HasControlStateEventProvider<DateTimePicker>())
			serviceCollection.AddControlStateEventProvider<DateTimePicker, CommonControlStateEventProvider>();

		// Initialize Tasks
		if (!serviceCollection.HasImplementation<SessionEndingHandlerInitializer>())
			serviceCollection.AddInitializer<SessionEndingHandlerInitializer>();

		// Start Tasks

		// End Tasks

	}

	private void EnableHooks(IServiceCollection serviceCollection) {

		if (!serviceCollection.HasImplementationFor<IActiveApplicationMonitor>())
			serviceCollection.AddTransient<IActiveApplicationMonitor, PollDrivenActiveApplicationMonitor>();

		if (!serviceCollection.HasImplementationFor<IMouseHook>())
			serviceCollection.AddSingleton<IMouseHook, WindowsMouseHook>();

		if (!serviceCollection.HasImplementationFor<IKeyboardHook>())
			serviceCollection.AddSingleton<IKeyboardHook, WindowsKeyboardHook>();
	}

	private void EnableDRM(IServiceCollection serviceCollection) {

		if (!serviceCollection.HasImplementationFor<IAboutBox>())
			serviceCollection.AddTransient<IAboutBox, DRMAboutBox>();

		if (!serviceCollection.HasImplementationFor<INagDialog>())
			serviceCollection.AddTransient<INagDialog, DRMNagDialog>();
	}

	private void DisableDRM(IServiceCollection serviceCollection) {
		if (!serviceCollection.HasImplementationFor<IAboutBox>())
			serviceCollection.AddTransient<IAboutBox, ProductAboutBox>();
	}
}
