//-----------------------------------------------------------------------
// <copyright file="NotPersistedDictionary.cs" company="Sphere 10 Software">
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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Reflection;

namespace Sphere10.Framework {

	public class NotPersistedDictionary<T1, T2> : DictionaryDecorator<T1, T2>, IPersistedDictionary<T1, T2> {

		public void Load() {
		}

		public void Save() {
		}

		public void Delete() {
		}
	}
}



	
