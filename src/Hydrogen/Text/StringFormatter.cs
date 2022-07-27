using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen {
	public static class StringFormatter {
		private static readonly char TokenStartChar= '{';
		private static readonly char TokenEndChar= '}';
		private static readonly char[] TokenTrimDelimitters = { TokenStartChar, TokenEndChar };

		private static readonly IFuture<ITokenResolver[]> Resolvers = Tools.Values.Future.LazyLoad(() => new []{ new DefaultTokenResolver() }.Concat(TinyIoC.TinyIoCContainer.Current.ResolveAll<ITokenResolver>(true)).ToArray());

        public static string FormatEx(string formatString, params object[] formatArgs) {
            return FormatEx(formatString, ResolveInternalToken, false, formatArgs);
        }

        public static string FormatWithDictionary(string formatString, IDictionary<string, object> userTokenResolver, bool recursive, params object[] formatArgs) {
			return FormatEx(
				formatString,
				(token) => {
					if (userTokenResolver.TryGetValue(token, out var value))
						return value;
					return null;
				},
				recursive,
				formatArgs
			);
		}

		public static string FormatEx(string @string, Func<string, object> userTokenResolver, bool recursive, params object[] formatArgs) {
			var alreadyVisited = new HashSet<string>();

			return FormatExInternal(@string);

			string FormatExInternal(string formatString) {
				Guard.ArgumentNotNull(formatString, nameof(formatString));
				Guard.ArgumentNotNull(userTokenResolver, nameof(userTokenResolver));
				var splits = new Stack<string>(formatString.SplitAndKeep(TokenTrimDelimitters, StringSplitOptions.RemoveEmptyEntries).Reverse());
				var resolver = userTokenResolver ?? ResolveInternalToken;
				var resultBuilder = new StringBuilder();
				var currentFormatItemBuilder = new StringBuilder();
				var inFormatItem = false;
				var depth = 0;
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
							inFormatItem = true;
							depth++;
							currentFormatItemBuilder.Append("{");
							break;
						case "}":
							if (inFormatItem) {
								currentFormatItemBuilder.Append("}");
								if (--depth == 0) {
									// end of format item, process and add to string
									var token = currentFormatItemBuilder.ToString();
									if (!TryResolveFormatItem(token.Chomp(TokenTrimDelimitters), out var value, recursive, resolver, formatArgs))
										value = token;
									else if (recursive && !alreadyVisited.Contains(token)) {
										alreadyVisited.Add(token);
										value = FormatExInternal(value?.ToString() ?? string.Empty);
									}
									resultBuilder.Append(value);
									inFormatItem = false;
									currentFormatItemBuilder.Clear();
								}
							} else if (splits.Count > 0 && splits.Peek() == "}") {
								// Escaped }}
								splits.Pop();
								resultBuilder.Append("}");
							} else {
								currentFormatItemBuilder.Append("}");
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
					resultBuilder.Append(currentFormatItemBuilder.ToString());

				return resultBuilder.ToString();
			}
		}

		private static bool TryResolveFormatItem(string token, out object value, bool recursive, Func<string, object> resolver, params object[] formatArgs) {
			var alreadyVisited = new HashSet<string>();
			return _TryResolveFormatItem(token, out value);

			bool _TryResolveFormatItem(string token, out object tokenValue) {
				token = token.TrimEnd();
				tokenValue = null;
				
				// This is an indexed .NET format argument (e.g. {0:yyyy-MM-dd}), so do not try to resolve this
		        if (IsStandardFormatIndex(token, out var formatIndex, out var formatOptions)) {
			        if (formatIndex >= formatArgs.Length)
				        return false;
			        tokenValue = formatArgs[formatIndex];
					return true;
				}
		        
				// This is a formatted item that needs to be looked up
				var tokenSplits = token.Split(':');
				if (tokenSplits.Length > 1 && token.CountSubstring("://") != 1) {   // Note :// is used for url-looking tokens (i.e. https://www.sphere10.com);
					token = tokenSplits[0].TrimEnd();
					formatOptions = ":" + tokenSplits.Skip(1).Select(s => s.Trim()).ToDelimittedString(":");
				}

				// Lookup the token
				tokenValue = resolver(token);
				
				// Found token, pass it through string.Format with any formatting options if specified
				if (tokenValue != null) {
					tokenValue = string.Format("{0" + (formatOptions ?? string.Empty) + "}", tokenValue);
					return true;
				}

				// Token not found, if recursive mode is on and token name has nested tokens, try to resolve them and try again

				if (recursive && !alreadyVisited.Contains(token) && token.IndexOf(TokenStartChar) < token.IndexOf(TokenEndChar)) {
					// token itself contains tokens, to recursively resolve the token name itself
					alreadyVisited.Add(token);
					token = FormatEx(token, resolver, recursive, formatArgs);
					return _TryResolveFormatItem(token, out tokenValue); 
				}
		
				// Couldn't resolve it, default to internal token resolution (i.e. these are registered in config files and framework)
				return TryResolveInternalToken(token, out tokenValue);
			}
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

        private static object ResolveInternalToken(string token) {
	        if (!TryResolveInternalToken(token, out var value))
				return token;
	        return value;
        }

        private static bool TryResolveInternalToken(string token, out object value) {
	        value = default;
	        foreach (var resolver in Resolvers.Value) 
		        if (resolver.TryResolve(token, out value))
			        return true;
	        return false;
        }

        class DefaultTokenResolver : ITokenResolver {
	        public bool TryResolve(string token, out object value) {
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
