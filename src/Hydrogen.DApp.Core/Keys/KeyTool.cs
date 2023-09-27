// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Keys;

public static class KeyTool {
	public static bool TryParseKeyType(byte[] accountKey, out KeyType keyType) {
		if (Tools.Enums.IsInRange<KeyType>(accountKey[0])) {
			keyType = (KeyType)accountKey[0];
			return true;
		}
		keyType = 0;
		return false;
	}

}
