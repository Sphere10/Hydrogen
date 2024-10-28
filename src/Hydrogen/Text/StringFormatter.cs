// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen;

public static class StringFormatter {
	private static readonly char TokenStartChar = '{';
	private static readonly char TokenEndChar = '}';
	private static readonly char[] TokenTrimDelimitters = { TokenStartChar, TokenEndChar };

	// TODO: add ApplicationInitializers in appropriate modules to add other resolvers
	private static readonly HashSet<ITokenResolver> _resolvers = new(new[] { new DefaultTokenResolver() });

	public static void RegisterResolvers(IEnumerable<ITokenResolver> resolvers)
		=> _resolvers.AddRange(resolvers);

	public static void RegisterResolver(ITokenResolver resolver)
		=> _resolvers.AddRange(new[] { resolver });

	public static string FormatEx(string formatString, params object[] formatArgs) {
		return FormatEx(formatString, ResolveInternalToken, true, formatArgs);
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
		Guard.ArgumentNotNull(@string, nameof(@string));
		var alreadyResolved = new Dictionary<string, object>();

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
								// Here we encountered the last }
								var potentialToken = currentFormatItemBuilder.ToString();

								// Get the contents (i.e. all text within the { })
								var potentialTokenContents = potentialToken.Chomp(TokenTrimDelimitters);

								// Attempt to resolve the potential token 
								if (!TryResolveFormatItem(alreadyResolved, potentialTokenContents, out var value, recursive, resolver, formatArgs)) {
									// The potential token has not resolved to anything but could 3 cases remain: 
									// (1) we're recursive mode and potential token is json-like object with actual sub-tokens inside
									// (2) we're recursive mode and potential token is json-like object without any sub-tokens
									// (3) we're not recursive and token is json-like object that may or may not have sub-tokens inside

									if (recursive) {
										var tokenWithSubtokenResolutions = FormatExInternal(potentialTokenContents);
										if (!tokenWithSubtokenResolutions.Equals(potentialTokenContents)) {
											// (1)
											value = "{" + tokenWithSubtokenResolutions + "}";
										} else {
											// (2)
											value = potentialToken;
										}
									} else {
										// (3)
										value = potentialToken;
									}
									alreadyResolved[potentialToken] = value;
								}

								// We have resolved the potential token to a value, but the resolution 
								// itself may need to be recursively resolved
								if (recursive) {
									if (!alreadyResolved.ContainsKey(potentialToken)) {
										alreadyResolved.Add(potentialToken, potentialToken); // on infinite recursive loop short circuit to token name itself 
										value = FormatExInternal(value?.ToString() ?? string.Empty);
										alreadyResolved[potentialToken] = value;
									} else {
										value = alreadyResolved[potentialToken];
									}
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
							resultBuilder.Append("}");
						}
						break;
					default:
						if (inFormatItem) {
							currentFormatItemBuilder.Append(split);
						} else
							resultBuilder.Append(split);
						break;
				}
			}
			if (inFormatItem)
				resultBuilder.Append(currentFormatItemBuilder.ToString());

			return resultBuilder.ToString();
		}
	}

	private static bool TryResolveFormatItem(Dictionary<string, object> alreadyVisited, string token, out object value, bool recursive, Func<string, object> resolver, params object[] formatArgs) {
		if (alreadyVisited.TryGetValue(token, out value))
			return true;

		var origToken = token;
		token = token.TrimWithCapture(out var trimmedStart, out var trimmedEnd);

		// This is an indexed .NET format argument (e.g. {0:yyyy-MM-dd}), so do not try to resolve this
		if (IsStandardFormatIndex(token, out var formatIndex, out var formatOptions)) {
			if (formatIndex >= formatArgs.Length)
				return false;
			value = string.Format("{0" + (formatOptions ?? string.Empty) + "}", formatArgs[formatIndex]);
			return true;
		}

		// This is a formatted item that needs to be looked up
		var tokenSplits = token.Split(':');
		var tokenIsUriLink = token.CountSubstring("://") == 1;
		if (tokenSplits.Length > 1 && !tokenIsUriLink) {
			// Note :// is used for url-looking tokens (i.e. https://www.sphere10.com);
			token = tokenSplits[0].TrimEnd();
			formatOptions = ":" + tokenSplits.Skip(1).Select(s => s.Trim()).ToDelimittedString(":");
		}

		// Lookup the token
		value = resolver(token);

		// Found token, pass it through string.Format with any formatting options if specified
		if (value != null) {
			value = string.Format("{0" + (formatOptions ?? string.Empty) + "}", value);
			return true;
		}

		// Token not found, if recursive mode is on and token name has nested tokens, try to resolve them and try again
		var indexTokenStartChar = token.IndexOf(TokenStartChar);
		var indexTokenEndChar = token.IndexOf(TokenEndChar);
		var isEmptyBracePair = indexTokenEndChar - indexTokenStartChar == 1;
		if (recursive && indexTokenStartChar >= 0 && indexTokenStartChar < indexTokenEndChar && !isEmptyBracePair) {
			// token itself contains tokens, to recursively resolve the token name itself
			//alreadyVisited.Add(token, token); // infinite recursion short circuit
			alreadyVisited[token] = token;
			var modifiedToken = FormatEx(token, resolver, recursive, formatArgs);
			if (!TryResolveFormatItem(alreadyVisited, modifiedToken, out value, recursive, resolver, formatArgs) || modifiedToken.Equals(value)) {
				// resolved token name didn't resolve to anything (or resolved to itself), but since token contained token names, return the resolved token name
				var snippedOutFormatArgs = !tokenIsUriLink && tokenSplits.Length > 1 ? ":" + tokenSplits.Skip(1).ToDelimittedString(":") : string.Empty;
				value = "{" + trimmedStart + modifiedToken + snippedOutFormatArgs + trimmedEnd + "}" ;
			}
			alreadyVisited[token] = value;
			return true;
		}

		// Couldn't resolve it, default to internal token resolution (i.e. these are registered in config files and framework)
		return TryResolveInternalToken(token, out value);

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
		foreach (var resolver in _resolvers)
			if (resolver.TryResolve(token, out value))
				return true;
		return false;
	}


	class DefaultTokenResolver : ITokenResolver {
		public bool TryResolve(string token, out object value) {
			value = token.ToUpperInvariant() switch {
				"CURRENTDATE" => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
				"CURRENTYEAR" => DateTime.Now.Year.ToString(),
				"STARTPATH" => System.IO.Path.GetDirectoryName(Tools.FileSystem.GetParentDirectoryPath(Tools.Runtime.GetExecutablePath())),
				_ => null
			};
			return value != null;
		}
	}
}
