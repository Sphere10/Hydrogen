//-----------------------------------------------------------------------
// <copyright file="MetadataAttribute.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Hydrogen {
	public class MetadataAttribute : Attribute {

		public const int NoSequence = -1;

		public MetadataAttribute()
			: this(string.Empty) {
		}

		public MetadataAttribute(string text)
			: this(text, NoSequence) {
		}

		public MetadataAttribute(string description, int sequence) {
			Description = description;
			Sequence = sequence;
		}

		public string Description { get; set; }

		public int Sequence { get; set; }

		public static bool HasMetadata(Enum enumeration) {
			return enumeration.GetType().GetCustomAttributes(typeof(MetadataAttribute), true).Length > 0;
		}

		public static string GetMetadata(Enum enumeration) {
			return GetMetadataAndSequence(enumeration).Item1;
		}

		public static Tuple<string, int> GetMetadataAndSequence(object enumValue) {
			var sequence = 0;
			string description = string.Empty;
			FieldInfo enumDecl = enumValue.GetType().GetField(enumValue.ToString());

			if (enumDecl != null) {
				object[] attrs = enumDecl.GetCustomAttributes(typeof(MetadataAttribute), true);
				for (int i = 0; i < attrs.Length && i < 1; i++) {
					var attr = (MetadataAttribute)attrs[i];
					if (attr.Sequence != MetadataAttribute.NoSequence) {
						sequence = attr.Sequence;
					} else {
						sequence = (int)enumValue;
					}

					description = attr.Description;
				}
			}
			return Tuple.Create(description.ToString(), sequence);
		}

		public static IEnumerable<Tuple<object, string>> GetValuesWithMetadata(Type enumType, bool sortBySequence) {
			var res = new List<Tuple<object, string, int>>();
			foreach (var v in Enum.GetValues(enumType)) {
				var descSeq = GetMetadataAndSequence((Enum)v);
				res.Add(Tuple.Create(v, descSeq.Item1, descSeq.Item2));
			}

			return
					from r in res
					orderby r.Item3
					select Tuple.Create(r.Item1, r.Item2);


		}

	}
}
