// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

    public class FactorySerializerBuilder<TBase> : FactorySerializerBuilderBase<TBase, FactorySerializerBuilder<TBase>> {

		public FactorySerializerBuilder() : this(new FactorySerializer<TBase>()) {
        }

		internal FactorySerializerBuilder(IFactorySerializer<TBase> serializer) 
			: base(serializer) {
        }

		public IFactorySerializer<TBase> Build() {
			return Serializer;
		}

	}
}
