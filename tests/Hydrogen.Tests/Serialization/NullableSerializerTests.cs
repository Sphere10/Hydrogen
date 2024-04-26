// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class NullableSerializerTests {

	[Test]
	public void NullableInt() {
		Assert.That(NullableSerializer<int>.Instance.ValueSerializer, Is.InstanceOf<PrimitiveSerializer<int>>());
	}

	[Test]
	public void NullableDecimal() {
		Assert.That(NullableSerializer<decimal>.Instance.ValueSerializer, Is.InstanceOf<PrimitiveSerializer<decimal>>());
	}

	[Test]
	public void NullableDouble() {
		Assert.That(NullableSerializer<double>.Instance.ValueSerializer, Is.InstanceOf<PrimitiveSerializer<double>>());
	}
	
	[Test]
	public void NullableDateTime() {
		Assert.That(NullableSerializer<DateTime>.Instance.ValueSerializer, Is.InstanceOf<DateTimeSerializer>());
	}

	[Test]
	public void NullableTimeSpan() {
		Assert.That(NullableSerializer<TimeSpan>.Instance.ValueSerializer, Is.InstanceOf<TimeSpanSerializer>());
	}

	[Test]
	public void DateTimeOffsetSpan() {
		Assert.That(NullableSerializer<DateTimeOffset>.Instance.ValueSerializer, Is.InstanceOf<DateTimeOffsetSerializer>());
	}

	[Test]
	public void NullableEnum() {
		Assert.That(NullableSerializer<IterateDirection>.Instance.ValueSerializer, Is.InstanceOf<EnumSerializer<IterateDirection>>());
	}

	[Test]
	public void NullableCVarInt() {
		Assert.That(NullableSerializer<CVarInt>.Instance.ValueSerializer, Is.InstanceOf<CVarIntSerializer>());
	}

	[Test]
	public void NullableVarInt() {
		Assert.That(NullableSerializer<VarInt>.Instance.ValueSerializer, Is.InstanceOf<VarIntSerializer>());
	}

	[Test]
	public void NullableUserStructFailsOnInstance() {
		Assert.That(() => NullableSerializer<TestStruct>.Instance.ValueSerializer, Throws.Exception);
	}

	private struct TestStruct {
		long x;
	}
}
