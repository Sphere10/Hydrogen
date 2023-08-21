// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using NUnit.Framework;

namespace Hydrogen.Tests;

public class BoundedStreamTests {

	[Test]
	public void SeekBehaviour_Absolute() {
		var inner = new MemoryStream();
		var bounded = new BoundedStream(inner, 1, 1);
		Assert.That(bounded.UseRelativeAddressing, Is.False);
		
		Assert.That(() => bounded.Seek(0, SeekOrigin.Begin), Is.EqualTo(1));
		Assert.That(() => bounded.Position, Is.EqualTo(1));
		Assert.That(() => inner.Position, Is.EqualTo(1));

		Assert.That(() => bounded.Seek(1, SeekOrigin.Begin), Is.EqualTo(2));
		Assert.That(() => bounded.Position, Is.EqualTo(2));
		Assert.That(() => inner.Position, Is.EqualTo(2));

		// Allowed to seek post-boundary
		Assert.That(() => bounded.Seek(2, SeekOrigin.Begin), Is.EqualTo(3));
		Assert.That(() => bounded.Position, Is.EqualTo(3));
		Assert.That(() => inner.Position, Is.EqualTo(3));

	}

	[Test]
	public void SeekBehaviour_Relative() {
		var inner = new MemoryStream();
		var bounded = new BoundedStream(inner, 1, 1) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);
		
		Assert.That(() => bounded.Seek(0, SeekOrigin.Begin), Is.EqualTo(0));
		Assert.That(() => bounded.Position, Is.EqualTo(0));
		Assert.That(() => inner.Position, Is.EqualTo(1));

		Assert.That(() => bounded.Seek(1, SeekOrigin.Begin), Is.EqualTo(1));
		Assert.That(() => bounded.Position, Is.EqualTo(1));
		Assert.That(() => inner.Position, Is.EqualTo(2));

		// Allowed to seek post-boundary
		Assert.That(() => bounded.Seek(2, SeekOrigin.Begin), Is.EqualTo(2));
		Assert.That(() => bounded.Position, Is.EqualTo(2));
		Assert.That(() => inner.Position, Is.EqualTo(3));
	}

	
	[Test]
	public void Consistency_Absolute_1() {
		var memoryStream = new MemoryStream();
		var bounded = new BoundedStream(memoryStream, 1, 2);
		Assert.That(bounded.UseRelativeAddressing, Is.False);

		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { }));
		
		Assert.That(()=>bounded.WriteByte(0), Throws.TypeOf<StreamOutOfBoundsException>());
		memoryStream.WriteByte(0);
		Assert.That(bounded.Position, Is.EqualTo(1));
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { }));

		bounded.WriteByte(1);
		Assert.That(bounded.Position, Is.EqualTo(2));
		Assert.That(bounded.Length, Is.EqualTo(1));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1 }));

		bounded.WriteByte(2);
		Assert.That(bounded.Position, Is.EqualTo(3));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));

		Assert.That(() => bounded.WriteByte(3), Throws.TypeOf<StreamOutOfBoundsException>());

	}

	[Test]
	public void Consistency_Absolute_2() {
		var memoryStream = new MemoryStream(new byte[] {9,9,9,9});
		var bounded = new BoundedStream(memoryStream, 1, 2);
		Assert.That(bounded.UseRelativeAddressing, Is.False);

		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 9, 9 }));
		
		Assert.That(()=>bounded.WriteByte(0), Throws.TypeOf<StreamOutOfBoundsException>());
		memoryStream.WriteByte(0);
		Assert.That(bounded.Position, Is.EqualTo(1));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 9, 9 }));

		bounded.WriteByte(1);
		Assert.That(bounded.Position, Is.EqualTo(2));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 9 }));

		bounded.WriteByte(2);
		Assert.That(bounded.Position, Is.EqualTo(3));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));

		Assert.That(() => bounded.WriteByte(3), Throws.TypeOf<StreamOutOfBoundsException>());
	}

	[Test]
	public void Consistency_Relative_1() {
		var memoryStream = new MemoryStream();
		var bounded = new BoundedStream(memoryStream, 1, 2) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);

		Assert.That(bounded.Position, Is.EqualTo(-1));
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { }));

		Assert.That(()=>bounded.WriteByte(0), Throws.TypeOf<StreamOutOfBoundsException>());
		memoryStream.WriteByte(0);
		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { }));

		bounded.WriteByte(1);
		Assert.That(bounded.Position, Is.EqualTo(1));
		Assert.That(bounded.Length, Is.EqualTo(1));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1 }));

		bounded.WriteByte(2);
		Assert.That(bounded.Position, Is.EqualTo(2));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));

		Assert.That(() => bounded.WriteByte(3), Throws.TypeOf<StreamOutOfBoundsException>());
	}


	[Test]
	public void Consistency_Relative_2() {
		var memoryStream = new MemoryStream(new byte[] {9,9,9,9});
		var bounded = new BoundedStream(memoryStream, 1, 2) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);

		Assert.That(bounded.Position, Is.EqualTo(-1));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 9, 9 }));

		Assert.That(()=>bounded.WriteByte(0), Throws.TypeOf<StreamOutOfBoundsException>());
		memoryStream.WriteByte(0);
		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 9, 9 }));

		bounded.WriteByte(1);
		Assert.That(bounded.Position, Is.EqualTo(1));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 9 }));

		bounded.WriteByte(2);
		Assert.That(bounded.Position, Is.EqualTo(2));
		Assert.That(bounded.Length, Is.EqualTo(2));
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));

		Assert.That(() => bounded.WriteByte(3), Throws.TypeOf<StreamOutOfBoundsException>());
	}


	[Test]
	public void Empty_Absolute_1() {
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 0, 0);
		Assert.That(bounded.UseRelativeAddressing, Is.False);
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.Position, Is.EqualTo(0));
	}

	[Test]
	public void Empty_Absolute_2() {
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 1, 0);
		Assert.That(bounded.UseRelativeAddressing, Is.False);
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.Position, Is.EqualTo(0));
	}

	[Test]
	public void Empty_Relative_1() {
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 0, 0) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.Position, Is.EqualTo(0));
	}

	[Test]
	public void Empty_Relative_2() {
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 1, 0) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);
		Assert.That(bounded.Length, Is.EqualTo(0));
		Assert.That(bounded.Position, Is.EqualTo(-1));
	}


	[Test]
	public void OverwriteFull_Absolute() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream(rng.NextBytes(100));
		var bounded = new BoundedStream(stream, 0, 100);
		Assert.That(bounded.UseRelativeAddressing, Is.False);
		bounded.Position = 0;
		bounded.Write(rng.NextBytes(100));
		Assert.That(bounded.Position, Is.EqualTo(100));
	}

	[Test]
	public void OverwriteFull_Relative() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream(rng.NextBytes(100));
		var bounded = new BoundedStream(stream, 0, 100) { UseRelativeAddressing = true };
		Assert.That(bounded.UseRelativeAddressing, Is.True);
		bounded.Position = 0;
		bounded.Write(rng.NextBytes(100));
		Assert.That(bounded.Position, Is.EqualTo(100));
	}

	[Test]
	public void InnerSetLength_Does_Not_Change_LogicalLength([Values] bool useRelativeAddressing) {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 0, 100) { AllowInnerResize = true, UseRelativeAddressing = useRelativeAddressing};
		Assert.That(bounded.Length, Is.EqualTo(0));
		stream.SetLength(200);
		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(100));
	}

	[Test]
	public void SetLength_Sets_Correct_InnerLength_Absolute() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 100, 100) { AllowInnerResize = true, UseRelativeAddressing = false};
		Assert.That(bounded.UseRelativeAddressing, Is.False);
		Assert.That(bounded.Length, Is.EqualTo(0));
		bounded.SetLength(50);
		Assert.That(bounded.Position, Is.EqualTo(0));
		Assert.That(bounded.Length, Is.EqualTo(50));
		Assert.That(stream.Length, Is.EqualTo(150));
	}

	[Test]
	public void SetLength_Sets_Correct_InnerLength_Relative() {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		var bounded = new BoundedStream(stream, 100, 100) { AllowInnerResize = true, UseRelativeAddressing = true};
		Assert.That(bounded.UseRelativeAddressing, Is.True);
		Assert.That(bounded.Length, Is.EqualTo(0));
		bounded.SetLength(50);
		Assert.That(bounded.Position, Is.EqualTo(-100));
		Assert.That(bounded.Length, Is.EqualTo(50));
		Assert.That(stream.Length, Is.EqualTo(150));
	}

	[Test]
	public void ToArray_0([Values]bool useRelative) {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		for(byte i = 0; i < 8; i++) {
			stream.WriteByte(i);
			// 01234567
		}
		var bounded = new BoundedStream(stream, 3, 0) { UseRelativeAddressing = useRelative, AllowInnerResize = false };

		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { }));
	}


	[Test]
	public void ToArray_1([Values]bool useRelative) {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		for(byte i = 0; i < 8; i++) {
			stream.WriteByte(i);
			// 01234567
		}
		var bounded = new BoundedStream(stream, 3, 2) { UseRelativeAddressing = useRelative, AllowInnerResize = false };

		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 3, 4 }));
	}


	[Test]
	public void ToArray_2([Values]bool useRelative) {
		var rng = new Random(31337);
		using Stream stream = new MemoryStream();
		for(byte i = 0; i < 8; i++) {
			stream.WriteByte(i);
			// 01234567
		}

		var bounded = new BoundedStream(stream, 3, 4) { UseRelativeAddressing = useRelative, AllowInnerResize = false };

		// Note: Position at 7 means cursor is at end of 6 and start of 7. Reading entire array will thus not read 7.
		Assert.That(bounded.ToArray(), Is.EqualTo(new byte[] { 3, 4, 5, 6 }));
	}


}
