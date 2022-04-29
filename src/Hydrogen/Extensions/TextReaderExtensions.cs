using System;
using System.IO;
using System.Linq;


namespace Hydrogen {
	public static class TextReaderExtensions {
		public static char? PeekChar(this TextReader reader) {
			var nextValue = reader.Peek();
			if (nextValue == -1)
				return null;
			return (char)nextValue;
		}

		public static char? ReadChar(this TextReader reader) {
			var nextValue = reader.Peek();
			if (nextValue == -1) {
				return null;
			}
			reader.Read();
			return (char)nextValue;
		}

		public static bool MatchChar(this TextReader reader, params char?[] characters) {
			var nextValue = reader.Peek();
			if (nextValue == -1) {
				if (characters.Any(c => c == null)) {
					return true;
				}
				return false;
			}
			if (!characters.Contains((char)nextValue)) {
				return false;
			}
			reader.Read();
			return true;
		}
	}
}
