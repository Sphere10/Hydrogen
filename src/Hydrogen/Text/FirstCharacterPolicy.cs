using System;

namespace Hydrogen;

[Flags]
public enum FirstCharacterPolicy {
	Anything = 0,
	AllowAsciiLetters = 1 << 0,
	AllowUnderscore = 1 << 1,
	AllowDigits = 1 << 2,
	Default = AllowAsciiLetters | AllowUnderscore,

	HtmlDomObj = AllowAsciiLetters,
	CSharpVar = AllowAsciiLetters | AllowUnderscore,
}
