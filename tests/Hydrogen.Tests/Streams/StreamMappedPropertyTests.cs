// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Hydrogen.Tests;

public class StreamMappedPropertyTests {

	[Test]
	public void ActivationDoesntThrow([Values(0, 1, 3, 11)] int leftPadding, [Values(0, 1, 3, 11)] int rightPadding) {
		var serializer = PrimitiveSerializer<long>.Instance;
		var streamSize = leftPadding + serializer.ConstantSize + rightPadding;
		var buffer = new byte[streamSize];
		using var stream = new MemoryStream(buffer);
		Assert.That( () => new StreamMappedProperty<long>(stream, leftPadding, leftPadding + sizeof(long) + rightPadding, serializer), Throws.Nothing);
	}

	[Test]
	public void DefaultValue([Values(0, 1, 3, 11)] int leftPadding, [Values(0, 1, 3, 11)] int rightPadding, [Values] Endianness endianness) {
		const int LeftPaddingValue = 253;
		const int RightPaddingValue = 251;

		var serializer = PrimitiveSerializer<long>.Instance;
		var streamSize = leftPadding + serializer.ConstantSize + rightPadding;
		var buffer = new byte[streamSize];

		// Setup padding
		for(var i = 0; i < leftPadding; i++)
			buffer[i] = LeftPaddingValue;

		for (var i = 0 + leftPadding + serializer.ConstantSize; i < streamSize; i++)
			buffer[i] = RightPaddingValue;

		// Setup stream mapped property
		using var stream = new MemoryStream(buffer);
		var smp = new StreamMappedProperty<long>(stream, leftPadding, leftPadding + serializer.ConstantSize + rightPadding, serializer, endianess: endianness);

		// Check value is 0
		Assert.That( smp.Value, Is.EqualTo(0));

		// Check padding
		if (leftPadding > 0)
			Assert.That( buffer[0..leftPadding].All(x => x == LeftPaddingValue));
		if (rightPadding > 0)
			Assert.That( buffer[^rightPadding..].All(x => x == RightPaddingValue));
	}
	

	[Test]
	public void LoadsCorrectValue([Values(0, 1, 3, 11)] int leftPadding, [Values(0, 1, 3, 11)] int rightPadding, [Values(0L, 1L, 1111L, 123131231212312312L)] long value, [Values] Endianness endianness) {
		const int LeftPaddingValue = 253;
		const int RightPaddingValue = 251;

		var serializer = PrimitiveSerializer<long>.Instance;
		var streamSize = leftPadding + serializer.ConstantSize + rightPadding;
		var buffer = new byte[streamSize];

		// Setup padding
		for(var i = 0; i < leftPadding; i++)
			buffer[i] = LeftPaddingValue;

		for (var i = 0 + leftPadding + serializer.ConstantSize; i < streamSize; i++)
			buffer[i] = RightPaddingValue;

		// Write value to buffer directly
		var valueBytes = serializer.SerializeToBytes(value, endianness);
		valueBytes.CopyTo(buffer, leftPadding);

		// Setup stream mapped property
		using var stream = new MemoryStream(buffer);
		var smp = new StreamMappedProperty<long>(stream, leftPadding, leftPadding + serializer.ConstantSize + rightPadding, serializer, endianess: endianness);
		
		// Check loads correct value
		Assert.That( smp.Value, Is.EqualTo(value));

		// Check padding
		if (leftPadding > 0)
			Assert.That( buffer[0..leftPadding].All(x => x == LeftPaddingValue));
		if (rightPadding > 0)
			Assert.That( buffer[^rightPadding..].All(x => x == RightPaddingValue));
	}

	[Test]
	public void StoreCorrectValue([Values(0, 1, 3, 11)] int leftPadding, [Values(0, 1, 3, 11)] int rightPadding, [Values(0L, 1L, 1111L, 123131231212312312L)] long value, [Values] Endianness endianness) {
		const int LeftPaddingValue = 253;
		const int RightPaddingValue = 251;

		var serializer = PrimitiveSerializer<long>.Instance;
		var streamSize = leftPadding + serializer.ConstantSize + rightPadding;
		var buffer = new byte[streamSize];

		// Setup padding
		for(var i = 0; i < leftPadding; i++)
			buffer[i] = LeftPaddingValue;

		for (var i = 0 + leftPadding + serializer.ConstantSize; i < streamSize; i++)
			buffer[i] = RightPaddingValue;

		// Setup stream mapped property
		using var stream = new MemoryStream(buffer);
		var smp = new StreamMappedProperty<long>(stream, leftPadding, leftPadding + serializer.ConstantSize + rightPadding, serializer, endianess: endianness);
		
		// Assign value (should update buffer)
		smp.Value = value;

		// Check buffer is updated
		Assert.That( buffer[leftPadding..^rightPadding], Is.EqualTo(serializer.SerializeToBytes(value, endianness)));

		// Check property is updated
		Assert.That( smp.Value, Is.EqualTo(value));

		// Check padding
		if (leftPadding > 0)
			Assert.That( () => buffer[0..leftPadding].All(x => x == LeftPaddingValue));
		if (rightPadding > 0)
			Assert.That( () => buffer[^rightPadding..].All(x => x == RightPaddingValue));
	}


	[Test]
	public void ChangesValue([Values(0, 1, 3, 11)] int leftPadding, [Values(0, 1, 3, 11)] int rightPadding, [Values(0L, 1L, 1111L, 123131231212312312L)] long value, [Values] Endianness endianness) {
		const int LeftPaddingValue = 253;
		const int RightPaddingValue = 251;

		var serializer = PrimitiveSerializer<long>.Instance;
		var streamSize = leftPadding + serializer.ConstantSize + rightPadding;
		var buffer = new byte[streamSize];

		// Setup padding
		for(var i = 0; i < leftPadding; i++)
			buffer[i] = LeftPaddingValue;

		for (var i = 0 + leftPadding + serializer.ConstantSize; i < streamSize; i++)
			buffer[i] = RightPaddingValue;

		// Setup stream mapped property
		using var stream = new MemoryStream(buffer);
		var smp = new StreamMappedProperty<long>(stream, leftPadding, leftPadding + serializer.ConstantSize + rightPadding, serializer, endianess: endianness);

		// write initial value
		var initialValue = unchecked(value-1);
		smp.Value = initialValue;

		// Check buffer has initial value
		Assert.That( buffer[leftPadding..^rightPadding], Is.EqualTo(serializer.SerializeToBytes(initialValue, endianness)));

		// Set value
		smp.Value = value;

		// Check buffer is updated
		Assert.That( buffer[leftPadding..^rightPadding], Is.EqualTo(serializer.SerializeToBytes(value, endianness)));

		// Check property is updated
		Assert.That( smp.Value, Is.EqualTo(value));
		
		// Check padding
		if (leftPadding > 0)
			Assert.That( () => buffer[0..leftPadding].All(x => x == LeftPaddingValue));
		if (rightPadding > 0)
			Assert.That( () => buffer[^rightPadding..].All(x => x == RightPaddingValue));
	}

}
