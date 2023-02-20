using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Application;

public abstract class BackgroundLicenseVerifierBase : IBackgroundLicenseVerifier {

	protected BackgroundLicenseVerifierBase(IUserInterfaceServices userInterfaceServices,  IProductLicenseStorage productLicenseStorage, IProductInformationProvider productInformationProvider, IProductLicenseProvider productLicenseProvider, IProductLicenseActivator productLicenseActivator, IProductLicenseEnforcer productLicenseEnforcer) {
		UserInterfaceServices = userInterfaceServices;
		LicenseStorage = productLicenseStorage;
		InformationProvider = productInformationProvider;
		LicenseProvider = productLicenseProvider;
		LicenseActivator = productLicenseActivator;
		LicenseEnforcer = productLicenseEnforcer;
	}

	protected IUserInterfaceServices UserInterfaceServices { get; }
	protected IProductLicenseStorage LicenseStorage { get; }
	protected IProductInformationProvider InformationProvider { get; }

	protected IProductLicenseProvider LicenseProvider { get; }

	protected IProductLicenseActivator LicenseActivator { get; }
	protected IProductLicenseEnforcer LicenseEnforcer { get; }

	public virtual async Task VerifyLicense(CancellationToken cancellationToken) {
		try {

			// Gets the activated or default license
			if (!LicenseProvider.TryGetLicense(out var currentActivation))
				return;

			// Notify server of usage
			var commandResult = await NotifyServerOfLicenseUsage(
				InformationProvider.ProductInformation.ProductCode,
				currentActivation.License.Item.ProductKey,
				Environment.MachineName,
				Tools.Network.GetMacAddresses().ToArray()
			);
			if (commandResult.Failure)
				throw new InvalidOperationException(commandResult.ErrorMessages.ToParagraphCase());
			LicenseEnforcer.ClearDrmServerErrors();

			if (!LicenseEnforcer.ValidateLicenseCommand(currentActivation, commandResult.Value))
				throw new InvalidOperationException("DRM authority responded with invalid command");

			// Now save the override command from DRM server if and only if the users license was an activated license (not default) and it didn't 
			// change since before the notification (note: it can change if long-running server response and user changed in UI in that time)
			if (LicenseStorage.TryGetActivatedLicense(out var activatedLicense)) {
				if (activatedLicense.License.Item.ProductKey == currentActivation.License.Item.ProductKey)
					LicenseStorage.SaveOverrideCommand(commandResult.Value); 

				// SPECIAL CASE: if the license is about to expire by a hard date, try to silently re-activate in case the license was renewed via a subscription
				if (activatedLicense.License.Item.ExpirationDate.HasValue && activatedLicense.License.Item.ExpirationDate >= DateTime.UtcNow - TimeSpan.FromDays(14))  {
					try {
						// This works because licenses expires generally 14 days after a subscription ends thus a subscription will generally be renewed 14 days before license will expire
						// By doing this, the license will seamlessly re-activate for subscription based products
						// If not reactivated, the the above override command will generally disable it.
						// At worst, a user can get 14 extra days of free before expiration
						await LicenseActivator.ActivateLicense(currentActivation.License.Item.ProductKey);
					} catch (Exception ex) {
						// Logger.Exception(ex)
						// We don't want to deal with exceptions here since this is a silent process
					}
				}
			}

			
			// NOTE: no license enforcement is performed in the background. If the DRM Server responded with a disable command,
			// it will apply in the next application run by UI. This background verifier only gets command.

			
		} catch (Exception error) {
			// Logger.Exception(error)
			// An error occurred. If errors keep occuring, the license enforcer will eventually terminate app
			LicenseEnforcer.HandleEnforcementError(error, true);
		}
	}

	protected abstract Task<Result<SignedItem<ProductLicenseCommandDTO>>> NotifyServerOfLicenseUsage(Guid productCode, string productKey, string machineName, string[] macAddresses);





}
