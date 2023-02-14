namespace Hydrogen.Data;

public class EncryptedJsonSerializer : SerializerDecorator {

	public EncryptedJsonSerializer(IJsonSerializer unencryptedSerializer, string password) 
		: base(unencryptedSerializer) {
		Password = password;	
	}

	public string Password { get; set; }

	public override string Serialize<T>(T value) 
		=> Tools.Crypto.EncryptStringAES(base.Serialize(value), Password, string.Empty);
	

	public override T Deserialize<T>(string value) 
		=> base.Deserialize<T>(Tools.Crypto.DecryptStringAES(value, Password, string.Empty));
		


}
