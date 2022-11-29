using System;

namespace Hydrogen.Web.AspNetCore;

[Flags]
public enum FormScopeOptions {
		
	/// <summary>
	/// Data is retained in the form after submit (e.g. CRUD editor), or cleared after submit (e.g. contact form)
	/// </summary>
	ClearOnSuccess = 1 << 0,

	/// <summary>
	/// Ommit form tag.
	/// </summary>
	OmitFormTag = 1 << 1,

	Default = ClearOnSuccess,

}
