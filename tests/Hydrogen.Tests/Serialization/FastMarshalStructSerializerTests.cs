// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Hydrogen.Tests;

[Ignore("Fails on Github")]
public class FastMarshalStructSerializerTests {

	[Test]
	public void Test1([Range(0, 100)] int seed) {
		var rng = new Random(seed);
		RunTest(Struct0.Gen(rng));
		RunTest(Struct1.Gen(rng));
		RunTest(Struct2.Gen(rng));
		RunTest(Struct3.Gen(rng));
		RunTest(Struct4.Gen(rng));
		RunTest(Struct5.Gen(rng));

		void RunTest<T>(T item) where T : struct {
			var serializer = new FastMarshalStructSerializer<T>();
			var bytes = serializer.SerializeBytesLE(item);
			var deserialized = serializer.DeserializeBytesLE(bytes);
			Assert.That(deserialized, Is.EqualTo(item));
		}
	}


	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct Struct0 : IEquatable<Struct0> {
		public char D;
		public static Struct0 Gen(Random rng)
			=> new() {
				D = rng.NextAnsiChar()
			};

		public bool Equals(Struct0 other) {
			return D == other.D;
		}
		public override bool Equals(object obj) {
			return obj is Struct0 other && Equals(other);
		}
		public override int GetHashCode() {
			return D.GetHashCode();
		}
	}


	internal struct Struct1 : IEquatable<Struct1> {

		private byte A;
		public short B { get; set; }
		internal int C;
		public byte[] D;

		public static Struct1 Gen(Random rng)
			=> new() {
				A = (byte)rng.Next(0, 256),
				B = (short)rng.Next(short.MinValue, short.MaxValue + 1),
				C = rng.Next(),
				D = Tools.Maths.Gamble(0.9) ? rng.NextBytes(rng.Next(0, 101)) : null,
			};


		public bool Equals(Struct1 other) {
			return A == other.A && C == other.C && (D?.SequenceEqual(other.D) ?? other.D is null) && B == other.B;
		}

		public override bool Equals(object obj) {
			return obj is Struct1 other && Equals(other);
		}
		public override int GetHashCode() {
			return HashCode.Combine(A, C, D, B);
		}
	}


	[StructLayout(LayoutKind.Sequential)]
	internal struct Struct2 : IEquatable<Struct2> {
		private byte A;
		public short B { get; set; }
		internal int C;
		internal byte[] D;

		public static Struct2 Gen(Random rng)
			=> new() {
				A = (byte)rng.Next(0, 256),
				B = (short)rng.Next(short.MinValue, short.MaxValue + 1),
				C = rng.Next(),
				D = Tools.Maths.Gamble(0.9) ? rng.NextBytes(rng.Next(0, 101)) : null,
			};


		public bool Equals(Struct2 other) {
			return A == other.A && C == other.C && (D?.SequenceEqual(other.D) ?? other.D is null) && B == other.B;
		}
		public override bool Equals(object obj) {
			return obj is Struct2 other && Equals(other);
		}
		public override int GetHashCode() {
			return HashCode.Combine(A, C, D, B);
		}
	}


	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal struct Struct3 : IEquatable<Struct3> {
		private byte A;
		public short B { get; set; }
		internal int C;
		internal byte[] D;

		public static Struct3 Gen(Random rng)
			=> new() {
				A = (byte)rng.Next(0, 256),
				B = (short)rng.Next(short.MinValue, short.MaxValue + 1),
				C = rng.Next(),
				D = Tools.Maths.Gamble(0.9) ? rng.NextBytes(rng.Next(0, 101)) : null,
			};


		public bool Equals(Struct3 other) {
			return A == other.A && C == other.C && (D?.SequenceEqual(other.D) ?? other.D is null) && B == other.B;
		}
		public override bool Equals(object obj) {
			return obj is Struct3 other && Equals(other);
		}
		public override int GetHashCode() {
			return HashCode.Combine(A, C, D, B);
		}
	}


	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct Struct4 : IEquatable<Struct4> {
		private byte A;
		public short B { get; set; }
		internal int C;
		internal byte[] D;

		public static Struct4 Gen(Random rng)
			=> new() {
				A = (byte)rng.Next(0, 256),
				B = (short)rng.Next(short.MinValue, short.MaxValue + 1),
				C = rng.Next(),
				D = Tools.Maths.Gamble(0.9) ? rng.NextBytes(rng.Next(0, 101)) : null,
			};


		public bool Equals(Struct4 other) {
			return A == other.A && C == other.C && (D?.SequenceEqual(other.D) ?? other.D is null) && B == other.B;
		}
		public override bool Equals(object obj) {
			return obj is Struct4 other && Equals(other);
		}
		public override int GetHashCode() {
			return HashCode.Combine(A, C, D, B);
		}
	}


	[StructLayout(LayoutKind.Explicit)]
	internal struct Struct5 : IEquatable<Struct5> {
		[FieldOffset(0)] private byte A; // 0

		[FieldOffset(1)] public short B; // 12

		[FieldOffset(3)] internal int C; // 3456

		[FieldOffset(7)] internal byte D; // 7

		//[FieldOffset(9)] 
		//internal string E; // 8

		public static Struct5 Gen(Random rng)
			=> new() {
				A = (byte)rng.Next(0, 256),
				B = (short)rng.Next(short.MinValue, short.MaxValue + 1),
				C = rng.Next(),
				D = (byte)rng.Next(0, 256),
				//E = Tools.Maths.Gamble(0.9) ? rng.NextString(0, 10) : null
			};


		public bool Equals(Struct5 other) {
			return A == other.A && B == other.B && C == other.C && D == other.D;
		}
		public override bool Equals(object obj) {
			return obj is Struct5 other && Equals(other);
		}
		public override int GetHashCode() {
			return HashCode.Combine(A, B, C, D);
		}
	}
}
