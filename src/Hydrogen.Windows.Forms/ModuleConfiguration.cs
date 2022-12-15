//-----------------------------------------------------------------------
// <copyright file="ModuleConfiguration.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace Hydrogen.Windows.Forms {
    public class ModuleConfiguration : ModuleConfigurationBase {
        public override void RegisterComponents(IServiceCollection serviceCollection) {

			if (!serviceCollection.HasImplementationFor<IWinFormsApplicationServices>()) 
				serviceCollection.AddSingleton<IWinFormsApplicationServices, WinFormsWinFormsApplicationServices>();

			if (!serviceCollection.HasImplementationFor<IApplicationServices>()) 
				serviceCollection.AddSingleton<IApplicationServices, IWinFormsApplicationServices>(provider => provider.GetService<IWinFormsApplicationServices>());

	        if (!serviceCollection.HasImplementationFor<IAboutBox>())
	            serviceCollection.AddTransient<IAboutBox, StandardAboutBox>();

            if (!serviceCollection.HasImplementationFor<INagDialog>())
	            serviceCollection.AddTransient<INagDialog, StandardNagDialog>();

            if (!serviceCollection.HasImplementationFor<IReportBugDialog>())
	            serviceCollection.AddTransient<IReportBugDialog, StandardReportBugDialog>();

            if (!serviceCollection.HasImplementationFor<IRequestFeatureDialog>())
	            serviceCollection.AddTransient<IRequestFeatureDialog, StandardRequestFeatureDialog>();

            if (!serviceCollection.HasImplementationFor<ISendCommentsDialog>())
	            serviceCollection.AddTransient<ISendCommentsDialog, StandardSendCommentsDialog>();

            if (!serviceCollection.HasImplementationFor<IActiveApplicationMonitor>())
	            serviceCollection.AddTransient<IActiveApplicationMonitor, PollDrivenActiveApplicationMonitor>();

            if (!serviceCollection.HasImplementationFor<IMouseHook>())
	            serviceCollection.AddSingleton<IMouseHook, WindowsMouseHook>();

            if (!serviceCollection.HasImplementationFor<IKeyboardHook>())
	            serviceCollection.AddSingleton<IKeyboardHook, WindowsKeyboardHook>();

			//if (registry.Registrations.Count(r =>
			//	r.InterfaceType == typeof(IBackgroundLicenseVerifier) &&
			//		r.ImplementationType == typeof(NoOpBackgroundLicenseVerifier)) == 1) {
			//	registry.RegisterComponent<IBackgroundLicenseVerifier, BITSBackgroundLicenseVerifier>();
			//} else {
   //             throw new SoftwareException("Illegal tampering with IBackgroundLicenseVerifier");
   //         }

            if (!serviceCollection.HasImplementationFor<IAutoRunServices>())
	            serviceCollection.AddTransient<IAutoRunServices, StartupFolderAutoRunServicesProvider>();

            // This is the primary form of the application, so register it as a provider of the below services
           
            if (!serviceCollection.HasControlStateEventProvider<FormEx>())
                serviceCollection.AddControlStateEventProvider<FormEx, FormEx.StateEventProvider>();

            if (!serviceCollection.HasControlStateEventProvider<UserControlEx>())
				serviceCollection.AddControlStateEventProvider<UserControlEx, UserControlEx.StateEventProvider>();

            if (!serviceCollection.HasControlStateEventProvider<SplitContainer>())
	            serviceCollection.AddControlStateEventProvider<SplitContainer, SplitContainerControlStateEventProvider>();

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

    }
}

// To override use below as guide:
//<ComponentRegistry>
//    <!-- Assemblies to pre-load -->
//    <Assembly dll = "Hydrogen.Application.dll" />
//    < Assembly dll="Hydrogen.Windows.Forms.dll" />
//    <Assembly dll = "Hydrogen.Windows.Forms.dll" />
//    < Assembly dll="Hydrogen.Windows.Forms.Sqlite.dll" />
//    <Assembly dll = "BlockchainSQL.DataAccess.NHibernate" />

//    <!--Hydrogen Framework Components -->
//    <Component interface="IBackgroundLicenseVerifier"   implementation="StandardBackgroundLicenseVerifier" />
//    <Component interface="ISettingsServices"            implementation="StandardUserSettingsProvider"     activation="Singleton" name="UserSettings" />
//    <Component interface="ISettingsServices"            implementation="StandardSystemSettingsProvider"   activation="Singleton" name="SystemSettings" />
//    <Component interface="IConfigurationServices"       implementation="StandardConfigurationServices"    activation="Singleton" />
//    <Component interface="IDuplicateProcessDetector"    implementation="StandardDuplicateProcessDetector" />
//    <Component interface="IHelpServices"                implementation="StandardHelpServices" />
//    <Component interface="ILicenseEnforcer"             implementation="StandardLicenseEnforcer"          activation="Singleton" />
//    <Component interface="ILicenseKeyDecoder"           implementation="StandardLicenseKeyDecoder" />
//    <Component interface="ILicenseKeyValidator"         implementation="StandardLicenseKeyValidatorWithVersionCheck" />
//    <Component interface="ILicenseKeyEncoder"           implementation="StandardLicenseKeyEncoder" />
//    <Component interface="ILicenseKeyServices"          implementation="StandardLicenseKeyProvider" />
//    <Component interface="ILicenseServices"             implementation="StandardLicenseServices"          activation="Singleton" />
//    <Component interface="IProductInformationServices"  implementation="StandardProductInformationServices" activation="Singleton" />
//    <Component interface="IProductInstancesCounter"     implementation="StandardProductInstancesCounter" />
//    <Component interface="IProductUsageServices"        implementation="StandardProductUsageProvider"     activation="Singleton" />
//    <Component interface="IWebsiteLauncher"             implementation="StandardWebsiteLauncher" />

//    <!-- Sphere 10 WinForms Framework Components -->
//    <Component interface="IAboutBox"                    implementation="StandardAboutBox" />
//    <Component interface="INagDialog"                   implementation="StandardNagDialog" />
//    <Component interface="IReportBugDialog"             implementation="StandardReportBugDialog" />
//    <Component interface="IRequestFeatureDialog"        implementation="StandardRequestFeatureDialog" />
//    <Component interface="ISendCommentsDialog"          implementation="StandardSendCommentsDialog" />
//    <Component interface="IActiveApplicationMonitor"    implementation="PollDrivenActiveApplicationMonitor" />
//    <Component interface="IMouseHook"                   implementation="WindowsMouseHook" Activation="Singleton" />
//    <Component interface="IKeyboardHook"                implementation="WindowsKeyboardHook" Activation="Singleton" />
//    <Component interface="IBackgroundLicenseVerifier"   implementation="BITSBackgroundLicenseVerifier" />
//    <Component interface="IAutoRunServices"             implementation="StartupFolderAutoRunServicesProvider" />
//    <ComponentSet interface="IControlStateChangeManager">
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="Hydrogen.Windows.Forms.ApplicationControl" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.NumericUpDown" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.TextBox" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.ComboBox" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.RadioButton" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.CheckBox" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.CheckedListBox" />
//      <Component implementation = "CommonControlsControlStateChangeManager"   name="System.Windows.Forms.DateTimePicker" />
//    </ComponentSet>
    
//    <!-- Application Initialize Tasks -->
//    <ComponentSet interface="IApplicationInitializer">
//      <Component implementation = "StandardProductUsageProvider+Initializer" />
//      < Component implementation="IncrementUsageByOneTask" />
//      <Component implementation = "RegisterSettingsViaIocTask" />
//      < Component implementation="SessionEndingHandlerTask" />
//    </ComponentSet>

//    <!-- Application Start Tasks -->
//    <ComponentSet interface="IApplicationStartTask">
//    </ComponentSet>

//    <!-- Application End Tasks-->
//    <ComponentSet interface="IApplicationEndTask">
//      <Component implementation = "SaveSettingsEndTask" />
//    </ComponentSet >


//    <!--Application Main Form -->
//    <Component interface="IMainForm"             implementation="BlockchainSQL.Server.MainForm"  Activation="Singleton" />
//    <Proxy interface="IApplicationIconProvider"  proxy="IMainForm" />
//    <Proxy interface="IUserInterfaceServices"    proxy="IMainForm" />
//    <Proxy interface="IUserInterfaceServices"    proxy="IMainForm" />
//    <Proxy interface="IUserNotificationServices" proxy="IMainForm" />
//    <Proxy interface="IApplicationIconProvider"  proxy="IMainForm" />
//    <!--Proxy interface="IBlockManager"  proxy="IMainForm" /-->
    
    
//  </ComponentRegistry>
