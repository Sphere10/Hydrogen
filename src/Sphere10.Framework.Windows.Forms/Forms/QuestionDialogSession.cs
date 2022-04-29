//-----------------------------------------------------------------------
// <copyright file="QuestionDialogSession.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Windows.Forms {

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
}
