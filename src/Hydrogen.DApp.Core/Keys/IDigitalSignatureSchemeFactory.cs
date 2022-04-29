using Hydrogen;

namespace Hydrogen.DApp.Core.Keys {

	public interface IDigitalSignatureSchemeFactory {
		IDigitalSignatureScheme Create(KeyType keyType);
	}

}