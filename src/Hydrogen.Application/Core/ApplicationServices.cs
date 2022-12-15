//-----------------------------------------------------------------------
// <copyright file="ApplicationServices.cs" company="Sphere 10 Software">
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
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Application;


/// <summary>
/// Facade class for the application framework. Basically wraps all the services into one class.
/// </summary>
public class ApplicationServices : ProductServices, IApplicationServices {

	public ApplicationServices(IProductInformationServices productInformationServices, IProductUsageServices productUsageServices, IProductInstancesCounter productInstancesCounter, IConfigurationServices configurationServices, ILicenseServices licenseServices, ILicenseEnforcer licenseEnforcer, IHelpServices helpServices, IUserInterfaceServices userInterfaceServices, IUserNotificationServices userNotificationServices, IWebsiteLauncher websiteLauncher, IAutoRunServices autoRunServices) :
		base(productInformationServices, productUsageServices, productInstancesCounter) {
		ConfigurationServices = configurationServices;
		LicenseServices = licenseServices;
		LicenseEnforcer = licenseEnforcer;
		HelpServices = helpServices;
		UserInterfaceServices = userInterfaceServices;
		UserNotificationServices = userNotificationServices;
		WebsiteLauncher = websiteLauncher;
		AutoRunServices = autoRunServices;
	}

	private IConfigurationServices ConfigurationServices { get; }

	private ILicenseServices LicenseServices { get; }
	
	private ILicenseEnforcer LicenseEnforcer { get; }
	
	private IHelpServices HelpServices { get; }
	
	private IUserInterfaceServices UserInterfaceServices { get; }
	
	private IUserNotificationServices UserNotificationServices { get; }
	
	private IWebsiteLauncher WebsiteLauncher { get; }

	private IAutoRunServices AutoRunServices { get; }
	
	public event EventHandler ConfigurationChanged {
		add => ConfigurationServices.ConfigurationChanged += value;
		remove => ConfigurationServices.ConfigurationChanged -= value;
	}

	public void NotifyConfigurationChangedEvent() => ConfigurationServices.NotifyConfigurationChangedEvent();
	
	public ISettingsProvider UserSettings => ConfigurationServices.UserSettings;

	public ISettingsProvider SystemSettings => ConfigurationServices.SystemSettings;

	public void RegisterLicenseKey(string key) => LicenseServices.RegisterLicenseKey(key);

	public void RegisterLicenseOverrideCommand(ProductLicenseCommand command) => LicenseServices.RegisterLicenseOverrideCommand(command);

	public void RemoveLicenseOverrideCommand() => LicenseServices.RemoveLicenseOverrideCommand();

	public LicenseInformation LicenseInformation => LicenseServices.LicenseInformation;

	public int CountAppliedLicense => LicenseEnforcer.CountAppliedLicense;

	public int CountNagged => LicenseEnforcer.CountNagged;

	public void ApplyLicense(bool nagUser = true) => LicenseEnforcer.ApplyLicense(nagUser);

	public bool DetermineLicenseCompliance(out bool systemShouldNagUser, out string nagMessage) => LicenseEnforcer.DetermineLicenseCompliance(out systemShouldNagUser, out nagMessage);

	public ProductRights Rights => LicenseEnforcer.Rights;

	public void ShowContextHelp(IHelpableObject helpableObject) => HelpServices.ShowContextHelp(helpableObject);
	
	public void ShowHelp() => HelpServices.ShowHelp();

	public void Exit(bool force = false) => UserInterfaceServices.Exit(force);

	public bool ApplicationExiting {
		get => UserInterfaceServices.ApplicationExiting;
		set => UserInterfaceServices.ApplicationExiting = value;
	}

	public string Status {
		get => UserInterfaceServices.Status;
		set => UserInterfaceServices.Status = value;
	}

	public void ExecuteInUIFriendlyContext(Action function, bool executeAsync = false) => UserInterfaceServices.ExecuteInUIFriendlyContext(function, executeAsync);

	public void ShowNagScreen(bool modal = false, string nagMessage = null) => UserInterfaceServices.ShowNagScreen(modal, nagMessage);

	public object PrimaryUIController => UserInterfaceServices.PrimaryUIController;

	public void ShowSendCommentDialog() => UserNotificationServices.ShowSendCommentDialog();

	public void ShowSubmitBugReportDialog() => UserNotificationServices.ShowSubmitBugReportDialog();

	public void ShowRequestFeatureDialog() => UserNotificationServices.ShowRequestFeatureDialog();

	public void ShowAboutBox() => UserNotificationServices.ShowAboutBox();

	public void ReportError(Exception e) => UserNotificationServices.ReportError(e);

	public void ReportError(string msg) => UserNotificationServices.ReportError(msg);

	public void ReportError(string title, string msg) => UserNotificationServices.ReportError(title, msg);

	public void ReportFatalError(string title, string msg) => UserNotificationServices.ReportFatalError(title, msg);

	public void ReportInfo(string title, string msg) => UserNotificationServices.ReportInfo(title, msg);

	public bool AskYN(string question) => UserNotificationServices.AskYN(question);

	public void LaunchWebsite(string url) => WebsiteLauncher.LaunchWebsite(url);

	public void LaunchCompanyWebsite() => WebsiteLauncher.LaunchCompanyWebsite();

	public void LaunchProductWebsite() => WebsiteLauncher.LaunchProductWebsite();

	public void LaunchProductPurchaseWebsite() => WebsiteLauncher.LaunchProductPurchaseWebsite();

	public bool DoesAutoRun(AutoRunType type, string applicationName, string executable) => AutoRunServices.DoesAutoRun(type, applicationName, executable);

	public void SetAutoRun(AutoRunType type, string applicationName, string executable) => AutoRunServices.SetAutoRun(type, applicationName, executable);

	public void RemoveAutoRun(AutoRunType type, string applicationName, string executable) => AutoRunServices.RemoveAutoRun(type, applicationName, executable);

}
