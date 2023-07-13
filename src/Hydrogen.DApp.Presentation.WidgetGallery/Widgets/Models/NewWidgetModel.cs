// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel.DataAnnotations;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Widgets.Models;

public class NewWidgetModel {
	[Required(AllowEmptyStrings = false)] public string Name { get; set; }

	[Required(AllowEmptyStrings = false)] public string Description { get; set; }

	public bool AreDimensionsKnown { get; set; }

	[Required] [Range(0, double.MaxValue)] public decimal? Price { get; set; }

	[Range(1, 100)] public int? Height { get; set; }

	[Range(1, 100)] public int? Length { get; set; }
}
