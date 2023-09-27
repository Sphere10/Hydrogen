// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
