using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public class ExceptionDialog : DialogEx {

	public ExceptionDialog() : this(string.Empty, new Exception()) {

	}

	private ExceptionDialog(string title, Exception error)
		: base(SystemIconType.Error, title, error.ToDisplayString(), false, "&Close", "&Detail") {
		Exception = error;
	}

	public Exception Exception { get; init; }

	protected override void OnActivated(EventArgs e) {
		base.OnActivated(e);
		TopMost = false;
	}

	protected override void OnProcessButton(DialogExResult button) {
		if (button == DialogExResult.Button2) {
			var detailForm = new TextEditorForm(Exception.ToDiagnosticString());
			detailForm.ShowDialog(this);
		} else base.OnProcessButton(button);
	}

	public static void Show(Exception error) {
		Show("Error", error);
	}


	public static void Show(string title, Exception error) {
		Show(null, title, error);
	}

	public static void Show(IWin32Window owner, Exception error) {
		Show(owner, "Error", error);
	}

	public static void Show(IWin32Window owner, string title, Exception error) {
		if (System.Windows.Forms.Application.OpenForms.Count > 0) {
			System.Windows.Forms.Application.OpenForms[0].InvokeEx(() => {
				var form = new ExceptionDialog(title, error);
				form.ShowDialog(owner);
			});
		} else {
			var form = new ExceptionDialog(title, error);
			form.ShowDialog(owner);
		}
	}


}
