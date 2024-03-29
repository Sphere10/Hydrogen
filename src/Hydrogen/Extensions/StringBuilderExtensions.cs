// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;

namespace Hydrogen;

public static class StringBuilderExtensions {

	static public void AppendLine(this StringBuilder sb, string text, params object[] formatParams) {
		if (formatParams != null && formatParams.Length > 0) {
			sb.AppendLine(text.FormatWith(formatParams));
		} else {
			sb.AppendLine(text);
		}
	}

	static public void Clear(this StringBuilder stringBuilder) {
		stringBuilder.Remove(0, stringBuilder.Length);
	}

}
