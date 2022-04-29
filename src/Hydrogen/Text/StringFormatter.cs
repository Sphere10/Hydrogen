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

        public static string FormatEx(string formatString, Func<string, string> userTokenResolver, params object[] formatArgs) {
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
                        resultBuilder.Append(ResolveFormatItem(currentFormatItemBuilder.ToString(), resolver, formatArgs));
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

        private static string ResolveFormatItem(string token, Func<string, string> resolver, params object[] formatArgs) {
	        int formatIndex;
	        string formatOptions;
	        if (IsStandardFormatIndex(token, out formatIndex, out formatOptions)) {
		        if (formatIndex >= formatArgs.Length)
			        throw new ArgumentOutOfRangeException("formatArgs", String.Format("Insufficient format arguments"));

		        return String.Format("{0" + (formatOptions ?? String.Empty) + "}", formatArgs[formatIndex]);
	        }
	        return resolver(token) ?? ResolveToken(token);
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
	        foreach (var resolver in Resolvers.Value) {
		        var value = resolver.TryResolve(token);
                if (value != null)
			        return value;
	        }
	        return token;
        }


        class DefaultTokenResolver : ITokenResolver {
	        public string TryResolve(string token) {
		        return token.ToUpperInvariant() switch {
			        "CURRENTDATE" => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
			        "CURRENTYEAR" => DateTime.Now.Year.ToString(),
			        "STARTPATH" => System.IO.Path.GetDirectoryName(Tools.Runtime.GetEntryAssembly().Location),
			        _ => null
		        };
	        }
        }
    }
}
