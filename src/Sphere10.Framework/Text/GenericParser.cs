using System;

namespace Sphere10.Framework {
    public static class GenericParser {

        public static bool TryParse<T>(string input, out T value) {
            // Special case: when input is empty and T is nullable, then parsed correctly as null
            if (input == string.Empty && typeof(T).IsGenericType) {
                value = default;
                return true;
            }

            // Note: that NULL case is handled by nullable converter            

            // Use component model type convertors
            var converter = TypeDescriptorEx.GetConverter(typeof(T));
            if (converter != null && converter.IsValid(input)) {
                value = (T)converter.ConvertFromString(input);
                return true;
            }
            value = default(T);
            return false;
        }

        public static T Parse<T>(string input) {
            if (!TryParse<T>(input, out var value))
                throw new FormatException((input == null ? "Null string" : $"String '{input}'") + $" could not be parsed into an {typeof(T).Name}");
            return value;
        }

        public static T SafeParse<T>(this string input) {
            TryParse<T>(input, out var value);
            return value;
        }

        public static bool TryParse(Type type, string input, out object value) {
	        // Special case: when input is empty and T is nullable, then parsed correctly as null
	        if (input == string.Empty && type.IsGenericType) {
		        value = default;
		        return true;
	        }

	        // Note: that NULL case is handled by nullable converter            

	        // Use component model type convertors
	        var converter = TypeDescriptorEx.GetConverter(typeof(T));
	        if (converter != null && converter.IsValid(input)) {
		        value = converter.ConvertFromString(input);
		        return true;
	        }
	        value = default;
	        return false;
        }

        public static object Parse(Type type, string input) {
	        if (!TryParse(type,input, out var value))
		        throw new FormatException((input == null ? "Null string" : $"String '{input}'") + $" could not be parsed into an {typeof(T).Name}");
	        return value;
        }

        public static object SafeParse(Type type, string input) {
	        TryParse(type, input, out var value);
	        return value;
        }

    }
}
