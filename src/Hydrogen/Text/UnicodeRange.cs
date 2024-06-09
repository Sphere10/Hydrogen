// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Utility class providing a number of singleton instances of
/// Range&lt;char&gt; to indicate the various ranges of unicode characters,
/// as documented at http://msdn.microsoft.com/en-us/library/20bw873z.aspx.
/// Note that this does not indicate the Unicode category of a character,
/// merely which range it's in.
/// TODO: Work out how to include names. Can't derive from Range[char].
/// </summary>
public static class UnicodeRange {

	static readonly List<ValueRange<char>> allRanges = new List<ValueRange<char>>();

	private static ValueRange<char> CreateRange(char from, char to) {
		// TODO: Check for overlaps
		ValueRange<char> ret = new ValueRange<char>(from, to);
		allRanges.Add(ret);
		return ret;
	}

	static readonly ValueRange<char> basicLatin = CreateRange('\u0000', '\u007f');
	static readonly ValueRange<char> latin1Supplement = CreateRange('\u0080', '\u00ff');
	static readonly ValueRange<char> latinExtendedA = CreateRange('\u0100', '\u017f');
	static readonly ValueRange<char> latinExtendedB = CreateRange('\u0180', '\u024f');
	static readonly ValueRange<char> ipaExtensions = CreateRange('\u0250', '\u02af');
	static readonly ValueRange<char> spacingModifierLetters = CreateRange('\u02b0', '\u02ff');
	static readonly ValueRange<char> combiningDiacriticalMarks = CreateRange('\u0300', '\u036f');
	static readonly ValueRange<char> greekAndCoptic = CreateRange('\u0370', '\u03ff');
	static readonly ValueRange<char> cyrillic = CreateRange('\u0400', '\u04ff');
	static readonly ValueRange<char> cyrillicSupplement = CreateRange('\u0500', '\u052f');
	static readonly ValueRange<char> armenian = CreateRange('\u0530', '\u058f');
	static readonly ValueRange<char> hebrew = CreateRange('\u0590', '\u05FF');
	static readonly ValueRange<char> arabic = CreateRange('\u0600', '\u06ff');
	static readonly ValueRange<char> syriac = CreateRange('\u0700', '\u074f');
	static readonly ValueRange<char> thaana = CreateRange('\u0780', '\u07bf');
	static readonly ValueRange<char> devangari = CreateRange('\u0900', '\u097f');
	static readonly ValueRange<char> bengali = CreateRange('\u0980', '\u09ff');
	static readonly ValueRange<char> gurmukhi = CreateRange('\u0a00', '\u0a7f');
	static readonly ValueRange<char> gujarati = CreateRange('\u0a80', '\u0aff');
	static readonly ValueRange<char> oriya = CreateRange('\u0b00', '\u0b7f');
	static readonly ValueRange<char> tamil = CreateRange('\u0b80', '\u0bff');
	static readonly ValueRange<char> telugu = CreateRange('\u0c00', '\u0c7f');
	static readonly ValueRange<char> kannada = CreateRange('\u0c80', '\u0cff');
	static readonly ValueRange<char> malayalam = CreateRange('\u0d00', '\u0d7f');
	static readonly ValueRange<char> sinhala = CreateRange('\u0d80', '\u0dff');
	static readonly ValueRange<char> thai = CreateRange('\u0e00', '\u0e7f');
	static readonly ValueRange<char> lao = CreateRange('\u0e80', '\u0eff');
	static readonly ValueRange<char> tibetan = CreateRange('\u0f00', '\u0fff');
	static readonly ValueRange<char> myanmar = CreateRange('\u1000', '\u109f');
	static readonly ValueRange<char> georgian = CreateRange('\u10a0', '\u10ff');
	static readonly ValueRange<char> hangulJamo = CreateRange('\u1100', '\u11ff');
	static readonly ValueRange<char> ethiopic = CreateRange('\u1200', '\u137f');
	static readonly ValueRange<char> cherokee = CreateRange('\u13a0', '\u13ff');
	static readonly ValueRange<char> unifiedCanadianAboriginalSyllabics = CreateRange('\u1400', '\u167f');
	static readonly ValueRange<char> ogham = CreateRange('\u1680', '\u169f');
	static readonly ValueRange<char> runic = CreateRange('\u16a0', '\u16ff');
	static readonly ValueRange<char> tagalog = CreateRange('\u1700', '\u171f');
	static readonly ValueRange<char> hanunoo = CreateRange('\u1720', '\u173f');
	static readonly ValueRange<char> buhid = CreateRange('\u1740', '\u175f');
	static readonly ValueRange<char> tagbanwa = CreateRange('\u1760', '\u177f');
	static readonly ValueRange<char> khmer = CreateRange('\u1780', '\u17ff');
	static readonly ValueRange<char> mongolian = CreateRange('\u1800', '\u18af');
	static readonly ValueRange<char> limbu = CreateRange('\u1900', '\u194f');
	static readonly ValueRange<char> taiLe = CreateRange('\u1950', '\u197f');
	static readonly ValueRange<char> khmerSymbols = CreateRange('\u19e0', '\u19ff');
	static readonly ValueRange<char> phoneticExtensions = CreateRange('\u1d00', '\u1d7f');
	static readonly ValueRange<char> latinExtendedAdditional = CreateRange('\u1e00', '\u1eff');
	static readonly ValueRange<char> greekExtended = CreateRange('\u1f00', '\u1fff');
	static readonly ValueRange<char> generalPunctuation = CreateRange('\u2000', '\u206f');
	static readonly ValueRange<char> superscriptsandSubscripts = CreateRange('\u2070', '\u209f');
	static readonly ValueRange<char> currencySymbols = CreateRange('\u20a0', '\u20cf');
	static readonly ValueRange<char> combiningDiacriticalMarksforSymbols = CreateRange('\u20d0', '\u20ff');
	static readonly ValueRange<char> letterlikeSymbols = CreateRange('\u2100', '\u214f');
	static readonly ValueRange<char> numberForms = CreateRange('\u2150', '\u218f');
	static readonly ValueRange<char> arrows = CreateRange('\u2190', '\u21ff');
	static readonly ValueRange<char> mathematicalOperators = CreateRange('\u2200', '\u22ff');
	static readonly ValueRange<char> miscellaneousTechnical = CreateRange('\u2300', '\u23ff');
	static readonly ValueRange<char> controlPictures = CreateRange('\u2400', '\u243f');
	static readonly ValueRange<char> opticalCharacterRecognition = CreateRange('\u2440', '\u245f');
	static readonly ValueRange<char> enclosedAlphanumerics = CreateRange('\u2460', '\u24ff');
	static readonly ValueRange<char> boxDrawing = CreateRange('\u2500', '\u257f');
	static readonly ValueRange<char> blockElements = CreateRange('\u2580', '\u259f');
	static readonly ValueRange<char> geometricShapes = CreateRange('\u25a0', '\u25ff');
	static readonly ValueRange<char> miscellaneousSymbols = CreateRange('\u2600', '\u26ff');
	static readonly ValueRange<char> dingbats = CreateRange('\u2700', '\u27bf');
	static readonly ValueRange<char> miscellaneousMathematicalSymbolsA = CreateRange('\u27c0', '\u27ef');
	static readonly ValueRange<char> supplementalArrowsA = CreateRange('\u27f0', '\u27ff');
	static readonly ValueRange<char> braillePatterns = CreateRange('\u2800', '\u28ff');
	static readonly ValueRange<char> supplementalArrowsB = CreateRange('\u2900', '\u297f');
	static readonly ValueRange<char> miscellaneousMathematicalSymbolsB = CreateRange('\u2980', '\u29ff');
	static readonly ValueRange<char> supplementalMathematicalOperators = CreateRange('\u2a00', '\u2aff');
	static readonly ValueRange<char> miscellaneousSymbolsandArrows = CreateRange('\u2b00', '\u2bff');
	static readonly ValueRange<char> cjkRadicalsSupplement = CreateRange('\u2e80', '\u2eff');
	static readonly ValueRange<char> kangxiRadicals = CreateRange('\u2f00', '\u2fdf');
	static readonly ValueRange<char> ideographicDescriptionCharacters = CreateRange('\u2ff0', '\u2fff');
	static readonly ValueRange<char> cjkSymbolsandPunctuation = CreateRange('\u3000', '\u303f');
	static readonly ValueRange<char> hiragana = CreateRange('\u3040', '\u309f');
	static readonly ValueRange<char> katakana = CreateRange('\u30a0', '\u30ff');
	static readonly ValueRange<char> bopomofo = CreateRange('\u3100', '\u312f');
	static readonly ValueRange<char> hangulCompatibilityJamo = CreateRange('\u3130', '\u318f');
	static readonly ValueRange<char> kanbun = CreateRange('\u3190', '\u319f');
	static readonly ValueRange<char> bopomofoExtended = CreateRange('\u31a0', '\u31bf');
	static readonly ValueRange<char> katakanaPhoneticExtensions = CreateRange('\u31f0', '\u31ff');
	static readonly ValueRange<char> enclosedCjkLettersandMonths = CreateRange('\u3200', '\u32ff');
	static readonly ValueRange<char> cjkCompatibility = CreateRange('\u3300', '\u33ff');
	static readonly ValueRange<char> cjkUnifiedIdeographsExtensionA = CreateRange('\u3400', '\u4dbf');
	static readonly ValueRange<char> yijingHexagramSymbols = CreateRange('\u4dc0', '\u4dff');
	static readonly ValueRange<char> cjkUnifiedIdeographs = CreateRange('\u4e00', '\u9fff');
	static readonly ValueRange<char> yiSyllables = CreateRange('\ua000', '\ua48f');
	static readonly ValueRange<char> yiRadicals = CreateRange('\ua490', '\ua4cf');
	static readonly ValueRange<char> hangulSyllables = CreateRange('\uac00', '\ud7af');
	static readonly ValueRange<char> highSurrogates = CreateRange('\ud800', '\udb7f');
	static readonly ValueRange<char> highPrivateUseSurrogates = CreateRange('\udb80', '\udbff');
	static readonly ValueRange<char> lowSurrogates = CreateRange('\udc00', '\udfff');
	static readonly ValueRange<char> privateUse = CreateRange('\ue000', '\uf8ff');
	static readonly ValueRange<char> privateUseArea = CreateRange('\uf900', '\ufaff');
	static readonly ValueRange<char> cjkCompatibilityIdeographs = CreateRange('\ufb00', '\ufb4f');
	static readonly ValueRange<char> alphabeticPresentationForms = CreateRange('\ufb50', '\ufdff');
	static readonly ValueRange<char> arabicPresentationFormsA = CreateRange('\ufe00', '\ufe0f');
	static readonly ValueRange<char> variationSelectors = CreateRange('\ufe20', '\ufe2f');
	static readonly ValueRange<char> combiningHalfMarks = CreateRange('\ufe30', '\ufe4f');
	static readonly ValueRange<char> cjkCompatibilityForms = CreateRange('\ufe50', '\ufe6f');
	static readonly ValueRange<char> smallFormVariants = CreateRange('\ufe70', '\ufeff');
	static readonly ValueRange<char> arabicPresentationFormsB = CreateRange('\uff00', '\uffef');
	static readonly ValueRange<char> halfwidthandFullwidthForms = CreateRange('\ufff0', '\uffff');

#pragma warning disable 1591
	public static ValueRange<char> BasicLatin {
		get { return basicLatin; }
	}

	public static ValueRange<char> Latin1Supplement {
		get { return latin1Supplement; }
	}

	public static ValueRange<char> LatinExtendedA {
		get { return latinExtendedA; }
	}

	public static ValueRange<char> LatinExtendedB {
		get { return latinExtendedB; }
	}

	public static ValueRange<char> IpaExtensions {
		get { return ipaExtensions; }
	}

	public static ValueRange<char> SpacingModifierLetters {
		get { return spacingModifierLetters; }
	}

	public static ValueRange<char> CombiningDiacriticalMarks {
		get { return combiningDiacriticalMarks; }
	}

	public static ValueRange<char> GreekAndCoptic {
		get { return greekAndCoptic; }
	}

	public static ValueRange<char> Cyrillic {
		get { return cyrillic; }
	}

	public static ValueRange<char> CyrillicSupplement {
		get { return cyrillicSupplement; }
	}

	public static ValueRange<char> Armenian {
		get { return armenian; }
	}

	public static ValueRange<char> Hebrew {
		get { return hebrew; }
	}

	public static ValueRange<char> Arabic {
		get { return arabic; }
	}

	public static ValueRange<char> Syriac {
		get { return syriac; }
	}

	public static ValueRange<char> Thaana {
		get { return thaana; }
	}

	public static ValueRange<char> Devangari {
		get { return devangari; }
	}

	public static ValueRange<char> Bengali {
		get { return bengali; }
	}

	public static ValueRange<char> Gurmukhi {
		get { return gurmukhi; }
	}

	public static ValueRange<char> Gujarati {
		get { return gujarati; }
	}

	public static ValueRange<char> Oriya {
		get { return oriya; }
	}

	public static ValueRange<char> Tamil {
		get { return tamil; }
	}

	public static ValueRange<char> Telugu {
		get { return telugu; }
	}

	public static ValueRange<char> Kannada {
		get { return kannada; }
	}

	public static ValueRange<char> Malayalam {
		get { return malayalam; }
	}

	public static ValueRange<char> Sinhala {
		get { return sinhala; }
	}

	public static ValueRange<char> Thai {
		get { return thai; }
	}

	public static ValueRange<char> Lao {
		get { return lao; }
	}

	public static ValueRange<char> Tibetan {
		get { return tibetan; }
	}

	public static ValueRange<char> Myanmar {
		get { return myanmar; }
	}

	public static ValueRange<char> Georgian {
		get { return georgian; }
	}

	public static ValueRange<char> HangulJamo {
		get { return hangulJamo; }
	}

	public static ValueRange<char> Ethiopic {
		get { return ethiopic; }
	}

	public static ValueRange<char> Cherokee {
		get { return cherokee; }
	}

	public static ValueRange<char> UnifiedCanadianAboriginalSyllabics {
		get { return unifiedCanadianAboriginalSyllabics; }
	}

	public static ValueRange<char> Ogham {
		get { return ogham; }
	}

	public static ValueRange<char> Runic {
		get { return runic; }
	}

	public static ValueRange<char> Tagalog {
		get { return tagalog; }
	}

	public static ValueRange<char> Hanunoo {
		get { return hanunoo; }
	}

	public static ValueRange<char> Buhid {
		get { return buhid; }
	}

	public static ValueRange<char> Tagbanwa {
		get { return tagbanwa; }
	}

	public static ValueRange<char> Khmer {
		get { return khmer; }
	}

	public static ValueRange<char> Mongolian {
		get { return mongolian; }
	}

	public static ValueRange<char> Limbu {
		get { return limbu; }
	}

	public static ValueRange<char> TaiLe {
		get { return taiLe; }
	}

	public static ValueRange<char> KhmerSymbols {
		get { return khmerSymbols; }
	}

	public static ValueRange<char> PhoneticExtensions {
		get { return phoneticExtensions; }
	}

	public static ValueRange<char> LatinExtendedAdditional {
		get { return latinExtendedAdditional; }
	}

	public static ValueRange<char> GreekExtended {
		get { return greekExtended; }
	}

	public static ValueRange<char> GeneralPunctuation {
		get { return generalPunctuation; }
	}

	public static ValueRange<char> SuperscriptsandSubscripts {
		get { return superscriptsandSubscripts; }
	}

	public static ValueRange<char> CurrencySymbols {
		get { return currencySymbols; }
	}

	public static ValueRange<char> CombiningDiacriticalMarksforSymbols {
		get { return combiningDiacriticalMarksforSymbols; }
	}

	public static ValueRange<char> LetterlikeSymbols {
		get { return letterlikeSymbols; }
	}

	public static ValueRange<char> NumberForms {
		get { return numberForms; }
	}

	public static ValueRange<char> Arrows {
		get { return arrows; }
	}

	public static ValueRange<char> MathematicalOperators {
		get { return mathematicalOperators; }
	}

	public static ValueRange<char> MiscellaneousTechnical {
		get { return miscellaneousTechnical; }
	}

	public static ValueRange<char> ControlPictures {
		get { return controlPictures; }
	}

	public static ValueRange<char> OpticalCharacterRecognition {
		get { return opticalCharacterRecognition; }
	}

	public static ValueRange<char> EnclosedAlphanumerics {
		get { return enclosedAlphanumerics; }
	}

	public static ValueRange<char> BoxDrawing {
		get { return boxDrawing; }
	}

	public static ValueRange<char> BlockElements {
		get { return blockElements; }
	}

	public static ValueRange<char> GeometricShapes {
		get { return geometricShapes; }
	}

	public static ValueRange<char> MiscellaneousSymbols {
		get { return miscellaneousSymbols; }
	}

	public static ValueRange<char> Dingbats {
		get { return dingbats; }
	}

	public static ValueRange<char> MiscellaneousMathematicalSymbolsA {
		get { return miscellaneousMathematicalSymbolsA; }
	}

	public static ValueRange<char> SupplementalArrowsA {
		get { return supplementalArrowsA; }
	}

	public static ValueRange<char> BraillePatterns {
		get { return braillePatterns; }
	}

	public static ValueRange<char> SupplementalArrowsB {
		get { return supplementalArrowsB; }
	}

	public static ValueRange<char> MiscellaneousMathematicalSymbolsB {
		get { return miscellaneousMathematicalSymbolsB; }
	}

	public static ValueRange<char> SupplementalMathematicalOperators {
		get { return supplementalMathematicalOperators; }
	}

	public static ValueRange<char> MiscellaneousSymbolsandArrows {
		get { return miscellaneousSymbolsandArrows; }
	}

	public static ValueRange<char> CjkRadicalsSupplement {
		get { return cjkRadicalsSupplement; }
	}

	public static ValueRange<char> KangxiRadicals {
		get { return kangxiRadicals; }
	}

	public static ValueRange<char> IdeographicDescriptionCharacters {
		get { return ideographicDescriptionCharacters; }
	}

	public static ValueRange<char> CjkSymbolsandPunctuation {
		get { return cjkSymbolsandPunctuation; }
	}

	public static ValueRange<char> Hiragana {
		get { return hiragana; }
	}

	public static ValueRange<char> Katakana {
		get { return katakana; }
	}

	public static ValueRange<char> Bopomofo {
		get { return bopomofo; }
	}

	public static ValueRange<char> HangulCompatibilityJamo {
		get { return hangulCompatibilityJamo; }
	}

	public static ValueRange<char> Kanbun {
		get { return kanbun; }
	}

	public static ValueRange<char> BopomofoExtended {
		get { return bopomofoExtended; }
	}

	public static ValueRange<char> KatakanaPhoneticExtensions {
		get { return katakanaPhoneticExtensions; }
	}

	public static ValueRange<char> EnclosedCjkLettersandMonths {
		get { return enclosedCjkLettersandMonths; }
	}

	public static ValueRange<char> CjkCompatibility {
		get { return cjkCompatibility; }
	}

	public static ValueRange<char> CjkUnifiedIdeographsExtensionA {
		get { return cjkUnifiedIdeographsExtensionA; }
	}

	public static ValueRange<char> YijingHexagramSymbols {
		get { return yijingHexagramSymbols; }
	}

	public static ValueRange<char> CjkUnifiedIdeographs {
		get { return cjkUnifiedIdeographs; }
	}

	public static ValueRange<char> YiSyllables {
		get { return yiSyllables; }
	}

	public static ValueRange<char> YiRadicals {
		get { return yiRadicals; }
	}

	public static ValueRange<char> HangulSyllables {
		get { return hangulSyllables; }
	}

	public static ValueRange<char> HighSurrogates {
		get { return highSurrogates; }
	}

	public static ValueRange<char> HighPrivateUseSurrogates {
		get { return highPrivateUseSurrogates; }
	}

	public static ValueRange<char> LowSurrogates {
		get { return lowSurrogates; }
	}

	public static ValueRange<char> PrivateUse {
		get { return privateUse; }
	}

	public static ValueRange<char> PrivateUseArea {
		get { return privateUseArea; }
	}

	public static ValueRange<char> CjkCompatibilityIdeographs {
		get { return cjkCompatibilityIdeographs; }
	}

	public static ValueRange<char> AlphabeticPresentationForms {
		get { return alphabeticPresentationForms; }
	}

	public static ValueRange<char> ArabicPresentationFormsA {
		get { return arabicPresentationFormsA; }
	}

	public static ValueRange<char> VariationSelectors {
		get { return variationSelectors; }
	}

	public static ValueRange<char> CombiningHalfMarks {
		get { return combiningHalfMarks; }
	}

	public static ValueRange<char> CjkCompatibilityForms {
		get { return cjkCompatibilityForms; }
	}

	public static ValueRange<char> SmallFormVariants {
		get { return smallFormVariants; }
	}

	public static ValueRange<char> ArabicPresentationFormsB {
		get { return arabicPresentationFormsB; }
	}

	public static ValueRange<char> HalfwidthandFullwidthForms {
		get { return halfwidthandFullwidthForms; }
	}
#pragma warning restore 1591

	/// <summary>
	/// Returns the unicode range containing the specified character.
	/// </summary>
	/// <param name="c">Character to look for</param>
	/// <returns>The unicode range containing the specified character, or null if the character
	/// is not in a unicode range.</returns>
	public static ValueRange<char> GetRange(char c) {
		// TODO: Make this efficient. SortedList should do it with a binary search, but it
		// doesn't give us quite what we want
		foreach (ValueRange<char> range in allRanges) {
			if (range.Contains(c)) {
				return range;
			}
		}
		return null;
	}
}
