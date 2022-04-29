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

using System.Collections.Generic;

namespace Hydrogen {

	public class NotPersistedDictionary<T1, T2> : DictionaryDecorator<T1, T2>, IPersistedDictionary<T1, T2> {

        public NotPersistedDictionary() : base(new Dictionary<T1, T2>()){

        }
		public void Load() {
		}

		public void Save() {
		}

		public void Delete() {
		}
	}
}



	
