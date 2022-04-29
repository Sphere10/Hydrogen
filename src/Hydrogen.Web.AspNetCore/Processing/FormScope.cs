using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Omu.AwesomeMvc;

namespace Sphere10.Framework.Web.AspNetCore {

	public sealed class FormScope<T> : IDisposable where T : FormModelBase {
		public const string DefaultControllerName = "Form";

		//private const string ErrorDivHtml = 
		//    @"<div class=""alert alert-danger""><button type=""button"" class=""close"" data-dismiss=""alert"" aria-hidden=""true"">×</button>{0}</div>";

		private const string ResultDivHtml = @"<div id=""{0}""></div>";

		private const string BeforeSubmitJS =
			@"<script type=""text/javascript"">
    function F{0}_BeforeSubmit(o) {{      

		if (!$('#{0} :input[type=""submit""]')
				.prop('disabled'))
		{{
		 $('#{0} :input[type=""submit""]')
						.prop('disabled', true)
						.after('<img class=""formLoadingImage animated fadeIn margin-left-10"" src=""{1}""/>');
		}}     
		}}
</script>";

		private const string JsonSuccessJS =
			@"<script>
    function F{0}_Success(result) {{	
         $('#{0} :input[type=""submit""]').prop('disabled', false);
        $('#{0} .formLoadingImage').remove();
	if (result.result){{
		  if (result.resultType === ""Redirect""){{
			location.assign(result.location);
			}}
          if (result.resultType === ""ShowMessage""){{
			var alertType = ""success"";
	        var alertHeader = ""Okay!"";
	        var alertIcon = ""fa fa-check""; 
	        $(""#{1}"").replaceWith('<div id={1} class=""form-result alert alert-' + alertType+'""><strong><i class=""' + alertIcon + '""></i> ' + alertHeader + '</strong> ' + result.message + '</div>');				
		  }}
          if (result.resultType === ""ReplacePage""){{
var doc = document;			
doc = doc.open(""text/html"");
			doc.write(result.content);
			doc.close();
		  }}

		if (result.resultType === ""ReplaceFormWithPartial""){{
			$(""#{0}"").replaceWith(result.content)
}}		
	else
			{{
				if (""{2}"" === ""True""){{
						$('#{0}')[0].reset();
						$('#{0}').closest('form').find('input[type=text], textarea').val('');
				}}          
			}}
	}}
else
{{
		var alertType = ""danger"";
        var alertHeader = ""Apologies"";
        var alertIcon = ""fa fa-exclamation""; 
        $(""#{1}"").replaceWith('<div id={1} class=""form-result alert alert-dismissible alert-' + alertType+'""><strong><i class=""' + alertIcon + '""></i> ' + alertHeader + '</strong> ' + result.message + '</div>');
}}
}}

</script>";

		private readonly string _formID;
		private readonly bool _omitForm;
		private readonly string _formName;
		private readonly string _formClass;
		private readonly string _formResultID;
		private readonly IHtmlHelper<T> _htmlHelper;

		private readonly bool _resetOnSuccess;

		public FormScope(IHtmlHelper<T> htmlHelper, T formModel, string clientFormClass = null, bool resetOnSuccess = true)
		:this(htmlHelper, formModel.FormName, DefaultControllerName, formModel, clientFormClass, resetOnSuccess) {
		}

		public FormScope(IHtmlHelper<T> htmlHelper, string action, string controller, T formModel, string clientFormClass = null, bool resetOnSuccess = true) {
			_formID = "_" + formModel.ID.ToStrictAlphaString().ToLowerInvariant();
			_htmlHelper = htmlHelper;
			_formName = formModel.FormName;
			_formClass = (_formName + "Form");
			_formResultID = _formID + "_Result";
			_omitForm = _htmlHelper.ViewData.ContainsKey(FormActionAttribute.OmitFormTag);
			_resetOnSuccess = resetOnSuccess;

			if (!_omitForm) {
				htmlHelper.BeginForm(
					action,
					controller,
					FormMethod.Post,
					new {
						id = _formID,
						@class = _formClass + (!string.IsNullOrWhiteSpace(clientFormClass) ? (" " + clientFormClass) : string.Empty)
					});
				//Write(htmlHelper.HiddenFor(o => o.ID, new { @Value = formModel.ID.ToString() }));
			} else {
				//if (!htmlHelper.ViewData.ModelState.IsValid) {
				//    Write(ErrorDivHtml, htmlHelper.ValidationSummary());
				//}
			}
		}

		public void Dispose() {
			if (!_omitForm) {
				_htmlHelper.EndForm();
				Write(ResultDivHtml, _formResultID);
				Write(BeforeSubmitJS.FormatWith(_formID, "/images/bx_loader.gif"));
				Write(JsonSuccessJS.FormatWith(_formID, _formResultID, _resetOnSuccess));

				Write(_htmlHelper
					.Awe()
					.Form()
					.FormClass(_formClass)
					.Confirm(false)
					.Success("F" + _formID + "_Success")
					.FillFormOnContent(true)
					.BeforeSubmit("F" + _formID + "_BeforeSubmit"));
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
	}
}