// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Web.AspNetCore;

public abstract class FormModelBase {

	[Required]
	[HiddenInput(DisplayValue = false)]
	public string ID { get; set; } = Tools.Url.ToHtml4DOMObjectID(Guid.NewGuid().ToStrictAlphaString().ToLowerInvariant(), "_");

	[Required]
	[HiddenInput(DisplayValue = false)]
	public int SubmitCount { get; set; } = 0;
}
