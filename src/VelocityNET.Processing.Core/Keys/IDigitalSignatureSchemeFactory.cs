using Sphere10.Framework;

namespace VelocityNET.Core.Keys {

	public interface IDigitalSignatureSchemeFactory {
		IDigitalSignatureScheme Create(KeyType keyType);
	}

}