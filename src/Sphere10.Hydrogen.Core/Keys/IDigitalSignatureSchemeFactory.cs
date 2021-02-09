using Sphere10.Framework;

namespace Sphere10.Hydrogen.Core.Keys {

	public interface IDigitalSignatureSchemeFactory {
		IDigitalSignatureScheme Create(KeyType keyType);
	}

}