﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ProtocolSerializerBuilderBase<TBase, TFactorySerializerBuilder> where TFactorySerializerBuilder : ProtocolSerializerBuilderBase<TBase, TFactorySerializerBuilder> {

	public ProtocolSerializerBuilderBase(FactorySerializer<TBase> serializer) {
		Serializer = serializer;
	}

	public FactorySerializer<TBase> Serializer { get; protected set; }

	protected ushort TypeCode { get; private set; }

	public SerializerBuilder<TConcrete> For<TConcrete>() where TConcrete : TBase
		//=> For<TConcrete>(Serializer.GenerateTypeCode());
		=> throw new NotSupportedException();

	public SerializerBuilder<TConcrete> For<TConcrete>(Enum value) where TConcrete : TBase
		=> For<TConcrete>(Convert.ToUInt16(value));


	public SerializerBuilder<TConcrete> For<TConcrete>(ushort typeCode) where TConcrete : TBase {
		TypeCode = typeCode;
		return new(this as TFactorySerializerBuilder);
	}


	public class SerializerBuilder<TConcrete>
		where TConcrete : TBase {
		private readonly TFactorySerializerBuilder _parentBuilder;

		public SerializerBuilder(TFactorySerializerBuilder parentBuilder) {
			_parentBuilder = parentBuilder;
		}

		public TFactorySerializerBuilder SerializeWith(IItemSerializer<TConcrete> serializer) {
			throw new NotSupportedException();
			//_parentBuilder.Serializer.RegisterSerializer(_parentBuilder.TypeCode, serializer);
			return _parentBuilder;
		}
	}
}
