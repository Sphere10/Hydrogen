using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hydrogen.Web.AspNetCore;

public sealed class BootstrapFormScope : IDisposable {
	public const string DefaultFormClasses = "";
	public const string ButtonHtmlSnippet =
		"""
<button class="btn btn-primary" type="submit" form="{formId}">
    <span id="{formId}_left_spinner" class="spinner-border spinner-border-sm me-2 invisible" role="status" aria-hidden="true"></span>
    {text}
    <span id="{formId}_right_spinner" class="spinner-border spinner-border-sm ms-2 invisible" role="status" aria-hidden="true"></span>
</button>
""";

	public const string FormEndHtmlSnippet = """
<div id="{formId}_result_marker" class="invisible"></div>
<script language="javascript">
    F_Init('{formId}');
</script>
""";

	private readonly string _formID;
	private readonly string _formClass;
	private readonly IHtmlHelper _htmlHelper;
	private readonly FormScopeOptions _options;


	public BootstrapFormScope(IHtmlHelper htmlHelper, string action, string controller, string formId, string clientFormClasses = null, FormScopeOptions options = FormScopeOptions.Default) {
		_formID = Tools.Url.ToHtml4DOMObjectID(formId, "_");
		_htmlHelper = htmlHelper;
		_formClass = DefaultFormClasses;
		_options = options;

		if (!options.HasFlag(FormScopeOptions.OmitFormTag)) {
			htmlHelper.BeginForm(
				action,
				controller,
				FormMethod.Post,
				new {
					id = _formID,
					@class = _formClass + (!string.IsNullOrWhiteSpace(clientFormClasses) ? (" " + clientFormClasses) : string.Empty)
				});

			Write(htmlHelper.Hidden("id", _formID));
		}

	}


	public void Dispose() {
		if (!_options.HasFlag(FormScopeOptions.OmitFormTag)) {
			_htmlHelper.EndForm();
		}
		Write(Tools.Text.FormatWithDictionary(FormEndHtmlSnippet, new Dictionary<string, object> { ["formId"] = _formID }, false));
	}


	private void Write(HtmlString text) {
		Write(text.Value);
	}

	private void Write(IHtmlContent htmlContent) {
		using (var writer = new System.IO.StringWriter()) {
			htmlContent.WriteTo(writer, HtmlEncoder.Default);
			Write(writer.ToString());
		}
	}

	private void Write(string text, params object[] formatArgs) {
		for (var i = 0; i < formatArgs.Length; i++) {
			if (formatArgs[i] is HtmlString)
				formatArgs[i] = ((HtmlString)formatArgs[i]).Value;
		}
		if (formatArgs.Length == 0)
			_htmlHelper.ViewContext.Writer.Write(text);
		else
			_htmlHelper.ViewContext.Writer.Write(text, formatArgs);
	}
}
