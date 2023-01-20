//-----------------------------------------------------------------------
// <copyright file="WebTool.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Hydrogen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tools.Web {

    public static partial class AspNetCore {


		public static SelectList ToFriendlyCountryList() {
			var sortedCountries = ToSelectList<ISO3166Country>(sort:SortDirection.Ascending);
			var betterList = new List<SelectListItem>();
			// United States
			var usa = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.UnitedStatesOfAmerica.ToString());

			// Canada
			var canada = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.Canada.ToString());

			// United Kingdom
			var uk = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.UnitedKingdom.ToString());

			// India
			var india = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.India.ToString());

			// China
			var china = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.China.ToString());

			// Australia
			var australia = sortedCountries.Items.Cast<SelectListItem>().Single(x => x.Value == ISO3166Country.Australia.ToString());

			var privilegedCountries = new[] { usa, canada, uk, india, china, australia };

			betterList = privilegedCountries.Concat(sortedCountries.Except(privilegedCountries)).ToList();
			return new SelectList(betterList, "Value", "Text");
			
		}



		public static SelectList ToSelectList<TEnum>(object selectedItem = default, SortDirection? sort = null) where TEnum : Enum
			=> ToSelectList(typeof(TEnum), selectedItem, sort);

		public static SelectList ToSelectList(Type enumType, object selectedItem = default, SortDirection? sort = null) {
			List<SelectListItem> items = new List<SelectListItem>();
			foreach (Enum item in Enum.GetValues(enumType)) {
				FieldInfo fi = enumType.GetField(item.ToString());
				//var attribute =  fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
				var title = Tools.Enums.GetDescription(item); //  attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
				var listItem = new SelectListItem {
					Value = item.ToString(),
					Text = title,
					Selected = selectedItem switch { null => false, _ => selectedItem.Equals(item) }
				};
				items.Add(listItem);
			}
			if (sort != null) {
				IComparer<SelectListItem> comparer = new ProjectionComparer<SelectListItem, string>(x => x.Text, StringComparer.InvariantCultureIgnoreCase);
				if (sort.Value == SortDirection.Descending)
					comparer = new ReverseComparer<SelectListItem>(comparer);
				items.Sort(comparer);
			}

			return new SelectList(items, "Value", "Text");
		}
	}
}
