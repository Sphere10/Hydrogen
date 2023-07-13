// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Windows.Forms;

public class QuestionDialogSession {
	private readonly IDictionary<Guid, DialogExResult> _autoAnswers;

	public QuestionDialogSession() {
		_autoAnswers = new Dictionary<Guid, DialogExResult>();
	}

	public DialogExResult AskQuestion(Guid questionID, SystemIconType iconType, string title, string text, params string[] buttonNames) {
		if (_autoAnswers.ContainsKey(questionID))
			return _autoAnswers[questionID];

		var dialog = new QuestionDialog(iconType, title, text, buttonNames);
		dialog.ShowDialog();

		if (dialog.AlwaysFlag)
			_autoAnswers[questionID] = dialog.DialogResult;

		return dialog.DialogResult;
	}

}
