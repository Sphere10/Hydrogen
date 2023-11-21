// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class ProductLicenseEnforcer : IProductLicenseEnforcer {

	public const string FullVersionText = "You are using the full version of this software.";
	public const string TrialVersionNagText = "You are using an evaluation version of this software.";
	public const string DisabledVersionNagText = "Your evaluation period has expired.";
	public const string DowngradeMessageText = "Your license has expired and your product rights are defaulted to default free license shipped with product.";
	public const string DateExpiresText = "Your license is valid up to and until {0:yyyy-MM-dd}.";
	public const string DateExpiredText = "Your license expired on {0:yyyy-MM-dd}.";
	public const string DaysExpiredText = "You have exceeded the maximum number of days permitted in your evalation period.";
	public const string NumDaysUsedText = "This is day {0} out of the {1} day evaluation period.";
	public const string InstancesExpiredText = "You are running too many instances of this application.";
	public const string NumInstancesUsed = "You have {0} seats left out of your licensed {1}.";
	public const string UsesExpiredText = "You have exceeded the maximum number of application starts permitted in your evaluation period.";
	public const string NumUsesText = "You have started this application {0} times out of the permitted {1}";
	public const string WrongVersion = "The registered license applies to version {0} of this product (this is version {1}).";
	public const string PleasePurchaseText = "Please purchase the product to continue using it.";
	public const string PleaseUpgradeToUnlockText = "Please upgrade your license to unlock all the features.";
	public const string PleaseUpgradeBeforeExpiration = "";
	public const int MaxDaysToAllowDrmError = 28;

	private Settings _settings;
	private readonly object _lock = new object();

	public ProductLicenseEnforcer(IProductLicenseStorage productLicenseStorage, IProductInformationProvider productInformationProvider, IProductUsageServices productUsageServices, IUserInterfaceServices userInterfaceServices,
	                              IDuplicateProcessDetector duplicateProcessDetector, ISettingsServices settingsServices) {
		ProductLicenseStorage = productLicenseStorage;
		ProductInformationProvider = productInformationProvider;
		ProductUsageServices = productUsageServices;
		UserInterfaceServices = userInterfaceServices;
		DuplicateProcessDetector = duplicateProcessDetector;
		SettingsServices = settingsServices;
		LoadSettings();
	}

	protected IProductLicenseStorage ProductLicenseStorage { get; }

	protected IProductInformationProvider ProductInformationProvider { get; }

	protected IProductUsageServices ProductUsageServices { get; }

	protected IUserInterfaceServices UserInterfaceServices { get; }

	protected IDuplicateProcessDetector DuplicateProcessDetector { get; }

	protected ISettingsServices SettingsServices { get; }

	public virtual bool ValidateLicense(ProductLicenseActivationDTO licenseActivation, ProductLicenseAuthorityDTO licenseAuthority) {
		Guard.ArgumentNotNull(licenseActivation, nameof(licenseActivation));
		Guard.ArgumentNotNull(licenseAuthority, nameof(licenseAuthority));

		// 1. Check that license authority is the same as the authority shipped with product 
		if (!licenseActivation.Authority.Equals(licenseAuthority))
			return false;

		// 2. Check that license is validly signed
		if (!licenseActivation.License.Verify(new ProductLicenseDTOSerializer(), CHF.SHA2_256, licenseActivation.Authority.LicenseDSS, licenseActivation.Authority.LicensePublicKey))
			return false;

		// 3. Check that license command is validly signed
		if (!licenseActivation.Command.Verify(new ProductLicenseCommandDTOSerializer(), CHF.SHA2_256, licenseActivation.Authority.LicenseDSS, licenseActivation.Authority.LicensePublicKey))
			return false;

		// 4. Check that license command & license are matched
		if (licenseActivation.License.Item.ProductKey != licenseActivation.Command.Item.ProductKey)
			return false;

		return true;
	}

	public bool ValidateLicenseCommand(ProductLicenseActivationDTO licenseActivation, SignedItem<ProductLicenseCommandDTO> command) {

		// 1. Check that command is validly signed
		if (!command.Verify(new ProductLicenseCommandDTOSerializer(), CHF.SHA2_256, licenseActivation.Authority.LicenseDSS, licenseActivation.Authority.LicensePublicKey))
			return false;

		// 2. Check that license command & license are matched
		if (licenseActivation.License.Item.ProductKey != command.Item.ProductKey)
			return false;

		return true;
	}

	public virtual void HandleEnforcementError(Exception error, bool isDrmServerError) {
		try {
			try {
				if (isDrmServerError && _settings.FirstDrmErrorDateTime == null) {
					_settings.FirstDrmErrorDateTime = DateTime.UtcNow;
					SaveSettings();
				}
			} finally {
				if (error is ProductLicenseTamperedException || _settings.FirstDrmErrorDateTime.HasValue && DateTime.UtcNow.Subtract(_settings.FirstDrmErrorDateTime.Value).TotalDays >= MaxDaysToAllowDrmError)
					UserInterfaceServices.Exit(true);
			}
		} catch {
			UserInterfaceServices.Exit(true);
		}
	}

	public void ClearDrmServerErrors() {
		_settings.FirstDrmErrorDateTime = null;
		SaveSettings();
	}

	protected virtual void LoadSettings() {
		if (_settings != null)
			_settings.Load();
		else
			_settings = SettingsServices.SystemSettings.Get<Settings>();
	}

	protected virtual void SaveSettings() {
		_settings?.Save();
	}

	public virtual void EnforceLicense(bool suppressNag) {
		lock (_lock) {
			try {
				var rights = CalculateRights(out var message);
				var nagged = false;
				if (rights.FeatureRights.IsIn(ProductLicenseFeatureLevelDTO.Free, ProductLicenseFeatureLevelDTO.None) && !string.IsNullOrWhiteSpace(message) && !suppressNag) {
					// Show nag screen, allow user opportunity to add new license
					UserInterfaceServices.ShowNagScreen(message);
					nagged = true;
					rights = CalculateRights(out message); // user may have updated license in nag screen
				}

				if (rights.FeatureRights == ProductLicenseFeatureLevelDTO.None) {
					if (!nagged && !suppressNag)
						UserInterfaceServices.ReportError("License Notification", message);
					UserInterfaceServices.Exit(true);
					return;
				}

				SaveSettings();
			} catch (Exception error) {
				HandleEnforcementError(error, false);
			}
		}
	}

	public virtual ProductRights CalculateRights(out string nagMessage) {
		ProductLicenseStorage.TryGetActivatedLicense(out var activatedLicense);
		ProductLicenseStorage.TryGetDefaultLicense(out var trialLicense);

		if (activatedLicense == null && trialLicense == null) {
			nagMessage = "No license found, full rights granted";
			return ProductRights.Full;
		}

		if (activatedLicense != null)
			return CalculateRightsForActivatedLicense(out nagMessage);
		return CalculateRightsForDefaultLicense(out nagMessage);

		ProductRights CalculateRightsForActivatedLicense(out string nagMessage) {
			var rights = CalculateLicenseRights(activatedLicense, out var isExpired, out nagMessage);
			// If command says downgrade, then downgrade
			switch (activatedLicense.Command.Item.Action) {
				case ProductLicenseActionDTO.Enable:
					rights.Enable(activatedLicense.License.Item.FeatureLevel);
					// Don't return, let below isExpired check apply
					break;
				case ProductLicenseActionDTO.Downgrade:
					if (trialLicense != null)
						return CalculateRightsForDefaultLicense(out _);
					break;
				case ProductLicenseActionDTO.Disable:
					rights.Disable();
					return rights;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (isExpired) {
				switch (activatedLicense.Command.Item.Action) {
					case ProductLicenseActionDTO.Enable:
						rights.Enable(activatedLicense.License.Item.FeatureLevel);
						return rights;
					case ProductLicenseActionDTO.Downgrade:
						return CalculateRightsForDefaultLicense(out _);
						break;
					case ProductLicenseActionDTO.Disable:
						rights.Disable();
						return rights;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return rights;
		}

		ProductRights CalculateRightsForDefaultLicense(out string nagMessage) {
			var rights = CalculateLicenseRights(trialLicense, out var isExpired, out nagMessage);
			if (isExpired) {
				// apply expiration policy
				switch (trialLicense.License.Item.ExpirationPolicy) {
					case ProductLicenseExpirationPolicyDTO.Downgrade:
					case ProductLicenseExpirationPolicyDTO.Disable:
						rights.Disable();
						break;
					case ProductLicenseExpirationPolicyDTO.None:
					default:
						break;

				}
			}
			return rights;
		}
	}

	protected virtual ProductRights CalculateLicenseRights(ProductLicenseActivationDTO licenseActivation, out bool isExpired, out string nagMessage) {
		var messageBuilder = new ParagraphBuilder();
		isExpired = false;
		if (licenseActivation.Command.Item.Action.IsIn(ProductLicenseActionDTO.Disable, ProductLicenseActionDTO.Downgrade)) {
			isExpired = true;
			nagMessage = licenseActivation.Command.Item.NotificationMessage;
			if (string.IsNullOrWhiteSpace(nagMessage)) {
				if (licenseActivation.Command.Item.Action == ProductLicenseActionDTO.Disable)
					nagMessage = DisabledVersionNagText;
				else
					nagMessage = DowngradeMessageText;
			}

			return ProductRights.None;
		}
		var rights = ProductRights.None;
		var license = licenseActivation.License.Item;

		// Copy limit features first
		rights.LimitFeatureA = license.LimitFeatureA;
		rights.LimitFeatureB = license.LimitFeatureB;
		rights.LimitFeatureC = license.LimitFeatureC;
		rights.LimitFeatureD = license.LimitFeatureD;

		if (license.MajorVersionApplicable.HasValue) {
			rights.AppliesToVersion = true;
			rights.Version = license.MajorVersionApplicable.Value;
		}

		#region Get license data

		if (license.ExpirationPolicy == ProductLicenseExpirationPolicyDTO.None) {
			rights.ExpiresAfterDate = false;
			rights.HasFiniteDays = false;
			rights.HasFiniteInstances = false;
			rights.HasFiniteUses = false;
			nagMessage = null;
		} else {
			// Version check
			if (license.MajorVersionApplicable.HasValue) {
				rights.AppliesToVersion = true;
				rights.Version = license.MajorVersionApplicable.Value;
				var runningVersion = ProductInformationProvider.ProductInformation.ProductVersion.Major;
				if (runningVersion != rights.Version) {
					isExpired = true;
					messageBuilder.AppendSentence(WrongVersion.FormatWith(rights.Version, runningVersion));
				}
			}

			// Date
			if (license.ExpirationDate.HasValue) {
				rights.ExpiresAfterDate = true;
				rights.ExpirationDateUTC = license.ExpirationDate.Value;
				if (DateTime.UtcNow > rights.ExpirationDateUTC) {
					isExpired = true;
					messageBuilder.AppendSentence(string.Format(DateExpiredText, rights.ExpirationDateUTC));
				} else {
					messageBuilder.AppendSentence(string.Format(DateExpiresText, rights.ExpirationDateUTC));
				}
			} else {
				rights.ExpiresAfterDate = false;
				rights.ExpirationDateUTC = DateTime.MaxValue;
			}
			// Days 
			if (license.ExpirationDays.HasValue) {
				rights.HasFiniteDays = true;
				rights.TotalDaysAllowed = license.ExpirationDays.Value;
				rights.DaysRemaining = (license.ExpirationDays.Value - ProductUsageServices.ProductUsageInformation.DaysUsedBySystem + 1).ClipTo(0, int.MaxValue);

				if (rights.DaysRemaining == 0) {
					isExpired = true;
					messageBuilder.AppendSentence(DaysExpiredText);
				} else {
					messageBuilder.AppendSentence(string.Format(NumDaysUsedText, ProductUsageServices.ProductUsageInformation.DaysUsedBySystem, rights.TotalDaysAllowed));
				}

			} else {
				rights.HasFiniteDays = false;
				rights.TotalDaysAllowed = int.MaxValue;
				rights.DaysRemaining = int.MaxValue;
			}

			// Uses
			if (license.ExpirationLoads.HasValue) {
				rights.HasFiniteUses = true;
				rights.TotalUsesAllowed = license.ExpirationLoads.Value;
				rights.UsesRemaining = (license.ExpirationLoads.Value - ProductUsageServices.ProductUsageInformation.NumberOfUsesBySystem + 1).ClipTo(0, int.MaxValue);

				if (rights.UsesRemaining == 0) {
					isExpired = true;
					messageBuilder.AppendSentence(UsesExpiredText);
				} else {
					messageBuilder.AppendSentence(string.Format(NumUsesText, ProductUsageServices.ProductUsageInformation.NumberOfUsesBySystem, rights.TotalUsesAllowed));
				}

			} else {
				rights.HasFiniteUses = false;
				rights.TotalUsesAllowed = int.MaxValue;
				rights.UsesRemaining = int.MaxValue;
			}

			// Instances
			if (license.MaxConcurrentInstances.HasValue) {
				rights.HasFiniteInstances = true;
				rights.TotalInstancesAllowed = license.MaxConcurrentInstances.Value;
				rights.InstancesRemaining = (license.MaxConcurrentInstances.Value - DuplicateProcessDetector.CountRunningInstancesOfThisApplication() + 1).ClipTo(0, int.MaxValue);

				if (rights.InstancesRemaining == 0) {
					messageBuilder.AppendSentence(InstancesExpiredText);
					isExpired = true;
				} else {
					messageBuilder.AppendSentence(string.Format(NumInstancesUsed, rights.InstancesRemaining - 1, rights.TotalInstancesAllowed));
				}

			} else {
				rights.HasFiniteInstances = false;
				rights.TotalInstancesAllowed = int.MaxValue;
				rights.InstancesRemaining = int.MaxValue;
			}
		}

		// Get feature rights
		rights.FeatureRights = license.FeatureLevel;

		#endregion

		if (rights.FeatureRights == ProductLicenseFeatureLevelDTO.None) {
			messageBuilder.AppendSentence(DisabledVersionNagText);
		} else if (rights.FeatureRights == ProductLicenseFeatureLevelDTO.Free) {
			messageBuilder.AppendSentence(TrialVersionNagText);
		} else {
			messageBuilder.AppendSentence(FullVersionText);
		}

		nagMessage = messageBuilder.ToString();
		return rights;
	}


	public class Settings : SettingsObject {
		public DateTime? FirstDrmErrorDateTime { get; set; }
	}

}
