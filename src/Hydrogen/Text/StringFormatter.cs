using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen {
	public static class StringFormatter {
		private static readonly char[] TokenTrimDelimitters = new[] { '{', '}' };

		private static readonly IFuture<ITokenResolver[]> Resolvers = Tools.Values.LazyLoad(() => new []{ new DefaultTokenResolver() }.Concat(TinyIoC.TinyIoCContainer.Current.ResolveAll<ITokenResolver>(true)).ToArray());

        public static string FormatEx(string formatString, params object[] formatArgs) {
            return FormatEx(formatString, ResolveToken, formatArgs);
        }

        public static string FormatWithDictionary(string formatString, IDictionary<string, object> userTokenResolver, params object[] formatArgs)
	        => FormatEx(
		        formatString,
		        (token) => {
			        if (userTokenResolver.TryGetValue(token, out var value))
				        return value;
			        return null;
		        },
		        formatArgs
			);
					

        public static string FormatEx(string formatString, Func<string, object> userTokenResolver, params object[] formatArgs) {
            Guard.ArgumentNotNull(formatString, nameof(formatString));
            Guard.ArgumentNotNull(userTokenResolver, nameof(userTokenResolver));

            var splits = new Stack<string>(formatString.SplitAndKeep(TokenTrimDelimitters, StringSplitOptions.RemoveEmptyEntries).Reverse());
            var resolver = userTokenResolver ?? ResolveToken;
            var resultBuilder = new StringBuilder();
            var currentFormatItemBuilder = new StringBuilder();
            var inFormatItem = false;
            while (splits.Count > 0) {
                var split = splits.Pop();
                switch (split) {
                    case "{":
                    if (splits.Count > 0 && splits.Peek() == "{") {
                        // Escaped {{
                        splits.Pop();
                        if (inFormatItem)
                            currentFormatItemBuilder.Append("{");
                        else
                            resultBuilder.Append("{");
                        continue;
                    }

                    if (inFormatItem) {
                        // illegal
                        throw new FormatException("Invalid format string");
                    }
                    inFormatItem = true;
                    break;
                    case "}":
                    if (inFormatItem) {
                        // end of format item, process and add to string
                        var token = currentFormatItemBuilder.ToString();
						if (!TryResolveFormatItem(token, out var value, resolver, formatArgs))
							value = "{" + token + "}";
	                    resultBuilder.Append(value);
                        inFormatItem = false;
                        currentFormatItemBuilder.Clear();
                    } else if (splits.Count > 0 && splits.Peek() == "}") {
                        // Escaped }}
                        splits.Pop();
                        resultBuilder.Append("}");
                    } else {
                        // illegal format string
                        throw new FormatException("Incorrect format string");
                    }
                    break;
                    default:
                    if (inFormatItem)
                        currentFormatItemBuilder.Append(split);
                    else
                        resultBuilder.Append(split);
                    break;
                }
            }
            if (inFormatItem)
                throw new FormatException("Incorrect format string");

            return resultBuilder.ToString();
        }

		private static bool TryResolveFormatItem(string token, out string value, Func<string, object> resolver, params object[] formatArgs) {
			token = token.TrimEnd();
			value = null;
			object valueObject;
	        if (IsStandardFormatIndex(token, out var formatIndex, out var formatOptions)) {
		        if (formatIndex >= formatArgs.Length)
			        return false;
		        valueObject = formatArgs[formatIndex];
	        } else {
				var tokenSplits = token.Split(':');
				if (tokenSplits.Length > 1) {
					token = tokenSplits[0].TrimEnd();
					formatOptions = ":" + tokenSplits.Skip(1).Select(s => s.Trim()).ToDelimittedString(":");
				}
				valueObject = resolver(token);
			}
			if (valueObject != null)
				value = string.Format("{0" + (formatOptions ?? string.Empty) + "}", valueObject);
	        return value != null || TryResolveToken(token, out value);
		}

        private static bool IsStandardFormatIndex(string token, out int number, out string formatOptions) {
	        var numberString = new string(token.TakeWhile(Char.IsDigit).ToArray());
	        if (numberString.Length > 0) {
		        number = Int32.Parse(numberString);
		        formatOptions = token.Substring(numberString.Length);
		        return true;
	        }
	        number = 0;
	        formatOptions = null;
	        return false;
        }

        private static string ResolveToken(string token) {
	        if (!TryResolveToken(token, out var value))
				return token;
	        return value;
        }

        private static bool TryResolveToken(string token, out string value) {
	        value = default;
	        foreach (var resolver in Resolvers.Value) 
		        if (resolver.TryResolve(token, out value))
			        return true;
	        return false;
        }

        class DefaultTokenResolver : ITokenResolver {
	        public bool TryResolve(string token, out string value) {
		        value = token.ToUpperInvariant() switch {
			        "CURRENTDATE" => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
			        "CURRENTYEAR" => DateTime.Now.Year.ToString(),
			        "STARTPATH" => System.IO.Path.GetDirectoryName(Tools.Runtime.GetEntryAssembly().Location),
			        _ => null
		        };
				return value != null;
	        }
        }
    }
}
