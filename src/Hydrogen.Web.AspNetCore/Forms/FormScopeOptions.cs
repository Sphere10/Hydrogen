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

	
	/// <summary>
	/// Provides protection from generalized bot form submissions (not targeted attacks)
	/// </summary>
	BotProtect = 1 << 3,


	/// <summary>
	/// Sends form via HTTP POST with fields inside the request
	/// </summary>
	UsePost = 1 << 2,

	/// <summary>
	/// Sends form via HTTP GET field fields encoded in url query string
	/// </summary>
	UseGet = 1 << 3,

	Default = ClearOnSuccess | UsePost,

	DefaultButKeepValues = UsePost

}
