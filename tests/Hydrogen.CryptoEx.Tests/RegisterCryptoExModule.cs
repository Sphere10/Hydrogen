using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

[SetUpFixture]
public class RegisterCryptoExModule {

	[OneTimeSetUp]
	public void RegisterModule() {
		Hydrogen.CryptoEx.ModuleConfiguration.Initialize();
	}

	[OneTimeTearDown]
	public void DeregisterModule() {
		Hydrogen.CryptoEx.ModuleConfiguration.Finalize();
	}
}
