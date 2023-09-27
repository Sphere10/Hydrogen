﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ProtocolSerializerBuilder<TBase> : ProtocolSerializerBuilderBase<TBase, ProtocolSerializerBuilder<TBase>> {

	public ProtocolSerializerBuilder() : this(new FactorySerializer<TBase>()) {
	}

	internal ProtocolSerializerBuilder(FactorySerializer<TBase> serializer)
		: base(serializer) {
	}

	public FactorySerializer<TBase> Build() {
		return Serializer;
	}

}
