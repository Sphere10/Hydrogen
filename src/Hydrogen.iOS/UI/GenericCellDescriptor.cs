//-----------------------------------------------------------------------
// <copyright file="GenericCellDescriptor.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using UIKit;

namespace Hydrogen.iOS {
	public sealed class GenericCellDescriptor {
		public GenericCellDescriptor() {
			ImageContentMode = UIViewContentMode.ScaleAspectFill;
			CanSelect = true;
			Accessory = UITableViewCellAccessory.None;
		}

		public UIViewContentMode ImageContentMode;
		public Func<UIImage> ImageGetter;
		public string Title;
		public string Description;
		public bool CanSelect;
		public object Tag;
		public Action Action;
		public UITableViewCellAccessory Accessory;
		public string BadgeText;
		public UIColor BadgeBackgroundColor;
		public UIColor BadgeTextColor;
		public Action<UITableViewCell, GenericCellDescriptor> CellConfig;
	}
}

