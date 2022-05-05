namespace Hydrogen.DApp.Core.Keys {

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

}