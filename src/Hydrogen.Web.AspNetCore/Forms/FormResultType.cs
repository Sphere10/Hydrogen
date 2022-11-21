using System.Runtime.Serialization;

namespace Hydrogen.Web.AspNetCore {
	public enum FormResultType {
		
		[EnumMember(Value = "message")]
		ShowMessage,

		[EnumMember(Value = "redirect")]
		Redirect,

		[EnumMember(Value = "replace_page")]
		ReplacePage,

		[EnumMember(Value = "replace_form")]
		ReplaceForm
	}
}
