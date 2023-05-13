// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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



	
