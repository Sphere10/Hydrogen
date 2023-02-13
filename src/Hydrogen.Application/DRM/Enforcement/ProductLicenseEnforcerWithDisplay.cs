using System;

namespace Hydrogen.Application;

public class ProductLicenseEnforcerWithDisplay : IProductLicenseEnforcer {
	private readonly IProductLicenseEnforcer _internalEnforcer;

	public ProductLicenseEnforcerWithDisplay(IUserInterfaceServices userInterfaceServices, IProductLicenseEnforcer internalEnforcer) {
		UserInterfaceServices = userInterfaceServices;
		_internalEnforcer = internalEnforcer;
	}

	private IUserInterfaceServices UserInterfaceServices { get; }
		

	public bool ValidateLicense(ProductLicenseActivationDTO licenseActivation, ProductLicenseAuthorityDTO licenseAuthority) 
		=> _internalEnforcer.ValidateLicense(licenseActivation, licenseAuthority);

	public bool ValidateLicenseCommand(ProductLicenseActivationDTO licenseActivation, SignedItem<ProductLicenseCommandDTO> command) 
		=> _internalEnforcer.ValidateLicenseCommand(licenseActivation, command);


	public void HandleEnforcementError(Exception error, bool isDrmServerError) {
		UserInterfaceServices.ReportError(error);
		_internalEnforcer.HandleEnforcementError(error, isDrmServerError);
	}

	public void ClearDrmServerErrors() => _internalEnforcer.ClearDrmServerErrors();

	public void EnforceLicense(bool suppressNag) 
		=> _internalEnforcer.EnforceLicense(suppressNag);

	public ProductRights CalculateRights(out string nagMessage)
		=> _internalEnforcer.CalculateRights(out nagMessage);

}
