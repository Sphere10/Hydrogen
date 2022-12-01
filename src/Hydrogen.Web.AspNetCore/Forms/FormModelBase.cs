using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Web.AspNetCore {

    public abstract class FormModelBase {

	    [Required]
	    [HiddenInput(DisplayValue = false)]
	    public string ID { get; set; } =  Tools.Url.ToHtml4DOMObjectID(Guid.NewGuid().ToStrictAlphaString().ToLowerInvariant(), "_");

		[Required]
		[HiddenInput(DisplayValue = false)]
		public bool IsResponse  { get; set; }
    }
}