// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public sealed class ValueTupleSerializer<T1>(IItemSerializer<T1> s1) : ItemSerializerBase<ValueTuple<T1>> {
	public override bool IsConstantSize => s1.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize;
	public override long CalculateSize(SerializationContext context, ValueTuple<T1> item)
		=> s1.CalculateSize(context, item.Item1);
	public override void Serialize(ValueTuple<T1> item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
	}
	public override ValueTuple<T1> Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> new (
			s1.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2>(IItemSerializer<T1> s1, IItemSerializer<T2> s2) : ItemSerializerBase<(T1, T2)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2);
	public override void Serialize((T1, T2) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
	}
	public override (T1, T2) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3) : ItemSerializerBase<(T1, T2, T3)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2, T3) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3);
	public override void Serialize((T1, T2, T3) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
	}
	public override (T1, T2, T3) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3, T4>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3, IItemSerializer<T4> s4) : ItemSerializerBase<(T1, T2, T3, T4)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize && s4.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize + s4.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2, T3, T4) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3) + s4.CalculateSize(context, item.Item4);
	public override void Serialize((T1, T2, T3, T4) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
		s4.Serialize(item.Item4, writer, context);
	}
	public override (T1, T2, T3, T4) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context),
			s4.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3, T4, T5>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3, IItemSerializer<T4> s4, IItemSerializer<T5> s5) : ItemSerializerBase<(T1, T2, T3, T4, T5)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize && s4.IsConstantSize && s5.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize + s4.ConstantSize + s5.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2, T3, T4, T5) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3) + s4.CalculateSize(context, item.Item4) + s5.CalculateSize(context, item.Item5);
	public override void Serialize((T1, T2, T3, T4, T5) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
		s4.Serialize(item.Item4, writer, context);
		s5.Serialize(item.Item5, writer, context);
	}
	public override (T1, T2, T3, T4, T5) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context),
			s4.Deserialize(reader, context),
			s5.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3, T4, T5, T6>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3, IItemSerializer<T4> s4, IItemSerializer<T5> s5, IItemSerializer<T6> s6) : ItemSerializerBase<(T1, T2, T3, T4, T5, T6)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize && s4.IsConstantSize && s5.IsConstantSize && s6.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize + s4.ConstantSize + s5.ConstantSize + s6.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2, T3, T4, T5, T6) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3) + s4.CalculateSize(context, item.Item4) + s5.CalculateSize(context, item.Item5) + s6.CalculateSize(context, item.Item6);
	public override void Serialize((T1, T2, T3, T4, T5, T6) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
		s4.Serialize(item.Item4, writer, context);
		s5.Serialize(item.Item5, writer, context);
		s6.Serialize(item.Item6, writer, context);
	}
	public override (T1, T2, T3, T4, T5, T6) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context),
			s4.Deserialize(reader, context),
			s5.Deserialize(reader, context),
			s6.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3, IItemSerializer<T4> s4, IItemSerializer<T5> s5, IItemSerializer<T6> s6, IItemSerializer<T7> s7) : ItemSerializerBase<(T1, T2, T3, T4, T5, T6, T7)> {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize && s4.IsConstantSize && s5.IsConstantSize && s6.IsConstantSize && s7.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize + s4.ConstantSize + s5.ConstantSize + s6.ConstantSize + s7.ConstantSize;
	public override long CalculateSize(SerializationContext context, (T1, T2, T3, T4, T5, T6, T7) item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3) + s4.CalculateSize(context, item.Item4) + s5.CalculateSize(context, item.Item5) + s6.CalculateSize(context, item.Item6) + s7.CalculateSize(context, item.Item7);
	public override void Serialize((T1, T2, T3, T4, T5, T6, T7) item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
		s4.Serialize(item.Item4, writer, context);
		s5.Serialize(item.Item5, writer, context);
		s6.Serialize(item.Item6, writer, context);
		s7.Serialize(item.Item7, writer, context);
	}
	public override (T1, T2, T3, T4, T5, T6, T7) Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context),
			s4.Deserialize(reader, context),
			s5.Deserialize(reader, context),
			s6.Deserialize(reader, context),
			s7.Deserialize(reader, context)
		);
}

public sealed class ValueTupleSerializer<T1, T2, T3, T4, T5, T6, T7, TRest>(IItemSerializer<T1> s1, IItemSerializer<T2> s2, IItemSerializer<T3> s3, IItemSerializer<T4> s4, IItemSerializer<T5> s5, IItemSerializer<T6> s6, IItemSerializer<T7> s7, IItemSerializer<TRest> sRest) : ItemSerializerBase<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>> 
	where TRest : struct {
	public override bool IsConstantSize => s1.IsConstantSize && s2.IsConstantSize && s3.IsConstantSize && s4.IsConstantSize && s5.IsConstantSize && s6.IsConstantSize && s7.IsConstantSize && sRest.IsConstantSize;
	public override long ConstantSize => s1.ConstantSize + s2.ConstantSize + s3.ConstantSize + s4.ConstantSize + s5.ConstantSize + s6.ConstantSize + s7.ConstantSize + sRest.ConstantSize;
	public override long CalculateSize(SerializationContext context, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> item)
		=> s1.CalculateSize(context, item.Item1) + s2.CalculateSize(context, item.Item2) + s3.CalculateSize(context, item.Item3) + s4.CalculateSize(context, item.Item4) + s5.CalculateSize(context, item.Item5) + s6.CalculateSize(context, item.Item6) + s7.CalculateSize(context, item.Item7) + sRest.CalculateSize(context, item.Rest);
	public override void Serialize(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> item, EndianBinaryWriter writer, SerializationContext context) {
		s1.Serialize(item.Item1, writer, context);
		s2.Serialize(item.Item2, writer, context);
		s3.Serialize(item.Item3, writer, context);
		s4.Serialize(item.Item4, writer, context);
		s5.Serialize(item.Item5, writer, context);
		s6.Serialize(item.Item6, writer, context);
		s7.Serialize(item.Item7, writer, context);
		sRest.Serialize(item.Rest, writer, context);
	}

	public override ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> new (
			s1.Deserialize(reader, context), 
			s2.Deserialize(reader, context),
			s3.Deserialize(reader, context),
			s4.Deserialize(reader, context),
			s5.Deserialize(reader, context),
			s6.Deserialize(reader, context),
			s7.Deserialize(reader, context),
			sRest.Deserialize(reader, context)
		);
}