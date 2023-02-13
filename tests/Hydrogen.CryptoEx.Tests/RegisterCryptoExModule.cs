using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

[SetUpFixture]
public class RegisterCryptoExModule {

	[OneTimeSetUp]
	public void RegisterModule() {
		ExplicitModuleConfiguration.Initialize();
	}

	[OneTimeTearDown]
	public void DeregisterModule() {
		ExplicitModuleConfiguration.Finalize();
	}
}
