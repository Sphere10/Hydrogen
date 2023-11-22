// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class WizardDialog<T> : FormEx {

	public WizardDialog() {
		this.StartPosition = FormStartPosition.CenterParent;
		InitializeComponent();
		Closing = false;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IWizard<T> WizardManager { get; set; }

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	internal new bool Closing { get; private set; }

	public Size DialogSizeOverhead => new Size(Width - _contentPanel.Width, Height - _contentPanel.Height);

	public async Task SetContent(WizardScreen<T> screen) {
		if (_contentPanel.Controls.Count > 0) {
			_contentPanel.RemoveAllControls();
		}
		screen.Dock = DockStyle.Fill;
		_contentPanel.Controls.Add(screen);
	}

	public new void Close() {
		throw new MethodAccessException("Call CloseDialog");
	}

	public void CloseDialog() {
		Closing = true;
		base.Close();
	}

	protected override void OnFormClosing(FormClosingEventArgs e) {
		base.OnFormClosing(e);
		if (!Closing) {
			var closeValidation = WizardManager.CancelRequested();
			e.Cancel = closeValidation.IsFailure;
			if (e.Cancel) {
				DialogEx.Show(this, SystemIconType.Error, closeValidation.ErrorMessages.ToParagraphCase(true), "Error");
			}
		}
	}

	protected override void OnFormClosed(FormClosedEventArgs e) {
		base.OnFormClosed(e);
	}

	private async void _previousButton_Click(object sender, EventArgs e) {
		try {
			using (loadingCircle1.BeginAnimationScope(this)) {
				await WizardManager.Previous();
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}

	}

	private async void _nextButton_Click(object sender, EventArgs e) {
		try {
			using (loadingCircle1.BeginAnimationScope(this)) {
				await WizardManager.Next();
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}

	}
}
