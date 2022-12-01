using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Hydrogen.Web.AspNetCore;

public sealed class BootstrapFormScope<TModel> : IDisposable where TModel : FormModelBase {
	public const string DefaultFormClasses = "";
	public const string ButtonHtmlSnippet =
		"""
<button class="btn btn-primary {class}" type="submit" form="{formId}">
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
	

	public BootstrapFormScope(IHtmlHelper<TModel> htmlHelper, string action, string controller, TModel formModel, AttributeDictionary attributes = null, FormScopeOptions options = FormScopeOptions.Default) 
		: this(htmlHelper, action, controller, null, formModel, attributes, options) {
	}

	public BootstrapFormScope(IHtmlHelper<TModel> htmlHelper, string url, TModel formModel, AttributeDictionary attributes = null, FormScopeOptions options = FormScopeOptions.Default) 
		: this(htmlHelper, null, null, url, formModel, attributes, options) {
	}

	private BootstrapFormScope(IHtmlHelper<TModel> htmlHelper,  string action, string controller, string url, TModel formModel, AttributeDictionary attributes = null, FormScopeOptions options = FormScopeOptions.Default) {
		Guard.ArgumentNotNull(htmlHelper, nameof(htmlHelper));
		Guard.ArgumentNotNull(formModel, nameof(formModel));
		if (options.HasFlag(FormScopeOptions.BotProtect))
			Guard.ArgumentNotNullOrWhitespace(url, nameof(url), "Must be provided when using BotProtect option");
		Guard.Against((!string.IsNullOrEmpty(action) || !string.IsNullOrEmpty(action)) && options.HasFlag(FormScopeOptions.BotProtect), "Cannot use controller/action when BotProtect option is enabled, instead use overload method and pass explicit form action url");
		
		_formID = formModel.ID;
		_htmlHelper = htmlHelper;
		_formClass = DefaultFormClasses;
		_options = options;

		if (options.HasFlag(FormScopeOptions.BotProtect))
			url = url.ToBase64();

		attributes ??= new AttributeDictionary();
		attributes["id"] = _formID;
		if (url != null)
			attributes["action"] = url;

		if (!options.HasFlag(FormScopeOptions.OmitFormTag)) {
			htmlHelper.BeginForm(
				action,
				controller,
				FormMethod.Post,
				attributes?.ToDictionary( x => x.Key, x => x.Value as object)
			);

			Write(htmlHelper.HiddenFor(m => m.ID, _formID));
			Write(htmlHelper.HiddenFor(m => m.IsResponse));
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
		result.BotProtect = options.HasFlag(FormScopeOptions.BotProtect);
		return result;
	}

	private class SerializableFormScopeOptions {

		[JsonProperty("clearOnSuccess")]
		public bool ClearOnSuccess { get; set; }

		[JsonProperty("botProtect")]
		public bool BotProtect { get; set; }
	}
}