using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Hydrogen.Web.AspNetCore;

public sealed class BootstrapFormScope<TModel> : IDisposable where TModel : FormModelBase {
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
<div id="{formId}_result_marker" class="invisible pb-5"></div>
<script language="javascript">
    F_Init('{formId}', {formOptions});
</script>
""";

	private readonly string _formID;
	private readonly string _formClass;
	private readonly IHtmlHelper _htmlHelper;
	private readonly FormScopeOptions _options;
	
	public BootstrapFormScope(IHtmlHelper<TModel> htmlHelper, string action, string controller, TModel formModel, string formClass = null, FormScopeOptions options = FormScopeOptions.Default) {
		Guard.ArgumentNotNull(htmlHelper, nameof(htmlHelper));
		Guard.ArgumentNotNull(action, nameof(action));
		Guard.ArgumentNotNull(controller, nameof(controller));
		Guard.ArgumentNotNull(formModel, nameof(formModel));
	
		_formID = formModel.ID;
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
					@class = _formClass + (!string.IsNullOrWhiteSpace(formClass) ? (" " + formClass) : string.Empty)
				});

			Write(htmlHelper.HiddenFor(m => m.ID, _formID));
		}
	}

	public void Dispose() {
		Write(
			Tools.Text.FormatWithDictionary(FormEndHtmlSnippet, 
				new Dictionary<string, object> { 
					["formId"] = _formID,
					["formOptions"] = JsonConvert.SerializeObject(ToSerializableSurrogate(_options), Formatting.None)
				},
				false
			)
		);
		if (!_options.HasFlag(FormScopeOptions.OmitFormTag)) {
			_htmlHelper.EndForm();
		}
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

	private SerializableFormScopeOptions ToSerializableSurrogate(FormScopeOptions options) {
		var result = new SerializableFormScopeOptions();
		result.ClearOnSuccess = options.HasFlag(FormScopeOptions.ClearOnSuccess);
		return result;
	}

	private class SerializableFormScopeOptions {

		[JsonProperty("clearOnSuccess")]
		public bool ClearOnSuccess { get; set; }
	}
}