using System.Text;
using System.Text.Encodings.Web;
using Hydrogen;
using Hydrogen.Web.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class IHtmlContentBuilderExtensions {
	public static IHtmlContent ToHtmlContent(this IHtmlContentBuilder contentBuilder, HtmlEncoder encoder) {
		var content = new StringBuilder();
		using var writer = new StringBuilderTextWriter(content);
		contentBuilder.WriteTo(writer, encoder);
		return new HtmlString(content.ToString());
	}
		
	
}
