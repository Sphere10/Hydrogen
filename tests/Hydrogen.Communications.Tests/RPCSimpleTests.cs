// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// HS 2021-11-14: DISABLED until this piece is resumed once core is fleshed out

//using System;
//using System.Threading;
//using System.Collections.Generic;
//using NUnit.Framework;
//using System.IO;
//using System.Net.Sockets;
//using System.Net;
//using System.Linq;
//using System.Text;
//using Hydrogen;
//using Hydrogen.Communications.RPC;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Hydrogen.Communications;
//using Newtonsoft.Json.Converters;

//namespace Hydrogen.Tests {
//	public enum FreeEnum { First, Second, Third };

//	//Anonymous API functions
//	[RpcAPIService("")]
//	public class TestAnonymousApi1 {
//		[RpcAPIMethod("GetWorkItem")]
//		[RpcAPIMethod]
//		public string GetWork(string user, string nonce1) { return user.ToUpper() + "." + nonce1.ToUpper(); }
//	}
//	//Anonymous API functions
//	public class TestAnonymousApi2 {
//		[RpcAPIMethod("GetMoreWorkItem")]
//		[RpcAPIMethod]
//		public string GetMoreWork(string w, string n) { return w.ToUpper() + "." + n.ToUpper(); }
//	}

//	//test RPC parameter object
//	public class TestObject {
//		public int iVal = 0;
//		public string sVal = "";
//		public float[] fArray;
//		[JsonConverter(typeof(ByteArrayHexConverter))]
//		public byte[] bytesArray;
//		public FreeEnum enumVal = FreeEnum.First;
//		[JsonConverter(typeof(StringEnumConverter))]
//		public FreeEnum enumVal2 = FreeEnum.First;
//		public Dictionary<string, int> dictionary;
//	}

//	public class TestObjectHex
//	{
//		[JsonConverter(typeof(HexadecimalValueConverterReader))]
//		public int hexInt = 0;
//		[JsonConverter(typeof(HexadecimalValueConverterReader))]
//		public uint hexUInt = 0;
//		[JsonConverter(typeof(HexadecimalValueConverterReader))]
//		public UInt64 hexUInt64 = 0;
//		[JsonConverter(typeof(HexadecimalValueConverterReader))]
//		public Int64 hexInt64 = 0;
//	}

//	public class TestObjectWithBytesArray
//	{
//		[JsonConverter(typeof(ByteArrayHexConverter))]
//		public byte[] bytesArray;
//	}

//	//Simple Class with RPC methodes
//	[RpcAPIService("ClassMember")]
//	public class TestChildClass {
//		public uint TestValue { get; set; }

//		[RpcAPIMethod("AddValue")]
//		[RpcAPIMethod]
//		public int AddInt(int a, int b) { return a + b; }

//		[RpcAPIMethod]
//		public uint AddUInt(uint a, uint b) { return a + b; }

//		[RpcAPIMethod]
//		public float AddFloat(float a, float b) { return a + b; }

//		[RpcAPIMethod]
//		public double AddDouble(double a, double b) { return a + b; }

//		[RpcAPIMethod]
//		public string ConcatString(string a, string b) { return a + b; }

//		[RpcAPIMethod]
//		public void ExplicitArguments([RpcAPIArgument("arg1")] uint argumentA) { TestValue = argumentA; }

//		[RpcAPIMethod]
//		public uint NoArgsWithRet() { TestValue = 123456789; return 987654321; }

//		[RpcAPIMethod]
//		public void NoArgs() { TestValue = 77777777; }

//		[RpcAPIMethod]
//		public object GetTestObject(TestObject bp) 
//		{ 
//			return new TestObject { 
//				iVal = bp.iVal + 1,
//				fArray = bp.fArray.Append(1).ToArray(),
//				sVal = bp.sVal + "1",
//				enumVal = bp.enumVal,
//				enumVal2 = bp.enumVal2,
//				bytesArray = bp.bytesArray.ToArray(),
//				dictionary = new Dictionary<string, int>(bp.dictionary)
//			}; 
//		}

//		[RpcAPIMethod]
//		public object[] GetTestObjectArray(TestObject bp) { return new TestObject[] { new TestObject { iVal = bp.iVal + 1, fArray = bp.fArray.Append(1).ToArray(), sVal = bp.sVal + "1" }, new TestObject { iVal = bp.iVal + 2, fArray = bp.fArray.Append(2).ToArray(), sVal = bp.sVal + "2" } }; }

//		[RpcAPIMethod]
//		public object GetObjectWithBytesArray(TestObjectWithBytesArray bp) { return new TestObjectWithBytesArray { bytesArray = new byte[] { bp.bytesArray[3], bp.bytesArray[2], bp.bytesArray[1], bp.bytesArray[0]} }; }

//		[RpcAPIMethod]
//		public object[] GetTestArrayOfTuple(string a, int b) { return new Tuple<string, int>[] { new Tuple<string, int>(a, b), new Tuple<string, int>(a + a, b + b), new Tuple<string, int>(a + a + a, b + b + b) }; }
//		[RpcAPIMethod]
//		public Dictionary<string, int> GetTestDictionary(Dictionary<string, int> d) { 
//			string a = d["a"].ToString();
//			int b = d["b"];  
//			return new Dictionary<string, int> { { a, b }, { a+a, b+b }, { a +a+a, b+b+b } }; }
//		}

//	//Simple Class with RPC methodes
//	[RpcAPIService("array")]
//	public class TestArrayClass {
//		public uint TestValue { get; set; }

//		[RpcAPIMethod]
//		public object[][] ToArrayOfArrayOfIntAndStrings(int a, string b) { return new object[][] { new object[] { a + 1, b }, new object[] { b, a + 2 } }; }

//		[RpcAPIMethod]
//		public int[] ToArrayInt(int a, int b) { return new int[] { a, b }; }

//		[RpcAPIMethod]
//		public uint[] ToArrayUInt(uint a, uint b) { return new uint[] { a, b }; }

//		[RpcAPIMethod]
//		public float[] ToArrayFloat(float a, float b) { return new float[] { a, b }; }

//		[RpcAPIMethod]
//		public double[] ToArrayDouble(double a, double b) { return new double[] { a, b }; }

//		[RpcAPIMethod]
//		public string[] ToArrayString(string a, string b) { return new string[] { a, b }; }
//		[RpcAPIMethod]
//		public byte[] ToByteArray(byte a, byte[] b) {  return new byte[] { (byte)(b[0] + a), (byte)(b[1] + a), (byte)(b[2] + a), (byte)(b[3] + a) }; }
//		[RpcAPIMethod]
//		public int[] AddArrayInt(int a, int[] b) { return b.Select(i => i + a).ToArray(); }

//		[RpcAPIMethod]
//		public uint[] AddArrayUInt(uint a, uint[] b) { return b.Select(i => i + a).ToArray(); }

//		[RpcAPIMethod]
//		public float[] AddArrayFloat(float a, float[] b) { return b.Select(i => i + a).ToArray(); }

//		[RpcAPIMethod]
//		public double[] AddArrayDouble(double a, double[] b) { return b.Select(i => i + a).ToArray(); }

//		[RpcAPIMethod]
//		public string[] AddArrayString(string a, string[] b) { return b.Select(i => a + "." + i).ToArray(); }
//	}


//	public class TestClass{
//		public TestChildClass classMember;
//		public TestClass() { classMember = new TestChildClass(); }
//	}

//	[RpcAPIService("Api")]
//	public class TestApi : ApiService {
//		public enum FixedEnum { Big = 0x11, Medium = 0x22, Small = 0x33 };

//		[RpcAPIMethod]
//		public FixedEnum TestEnum(FreeEnum selector) { 
//			return selector switch { 
//				FreeEnum.First => FixedEnum.Big, 
//				FreeEnum.Second => FixedEnum.Medium, 
//				FreeEnum.Third => FixedEnum.Small,
//				_ => FixedEnum.Small
//			}; 
//		}

//		[RpcAPIMethod]
//		public string AddStrings([RpcAPIArgument("s1")] string str1, [RpcAPIArgument("s2")] string str2) { return str1 + str2; }

//		[RpcAPIMethod]
//		public uint Add2Diff([RpcAPIArgument("arg1")] uint argumentA, [RpcAPIArgument("arg2")] int argumentB) { return argumentA + (uint)argumentB; }

//		[RpcAPIMethod]
//		public string DirtyJson() { return "BadJson{\a\v{"; }

//		[RpcAPIMethod]
//		public TestObjectHex GetTestObjectHex() { return new TestObjectHex { hexUInt = 999999, hexInt = 88888, hexInt64 = 0x75553333999CCCC1, hexUInt64 = 0xFFFFAAAABBBB3333 }; }
//		[RpcAPIMethod]
//		public TestObjectHex GetTestObjectHex2(TestObjectHex o) { return new TestObjectHex { hexUInt = o.hexUInt+1, hexInt = o.hexInt+1, hexInt64 = o.hexInt64+1, hexUInt64 = o.hexUInt64 +1}; }
//	}

//	[TestFixture]
//	[Category("RPC")]
//	public class RPC {
//		protected string TestRpcException<TRetType>(JsonRpcClient client, string funcName, object[] args) {
//			string err = "";
//			try {
//				client.RemoteCall<TRetType>(funcName, args);
//			} catch (Exception e) {
//				err = e.Message;
//			}
//			return err;
//		}
//		protected string TestRpcExceptionBatch(JsonRpcClient client, string funcName, object[] args) {
//			string err = "";
//			object[] batchResult = null;
//			try {
//				ApiBatchCallDescriptor batch = new ApiBatchCallDescriptor();
//				batch.Call("ClassMember.addint", 11, -1);
//				batch.Call(funcName, args);				
//                batch.Call("ClassMember.AddValue", 11, 2);
//				batchResult = client.RemoteCall(batch);
//			} catch (Exception e) {
//				err = e.Message;
//			}
//			//get 2nd entry that supposed to contain exception
//			return batchResult[1] == null ? "null" : (batchResult[1] as JsonRpcException).ToString();
//		}

//		[Test]
//		public void TestRPCServiceManager() {
//			try {
//				var anonymousAPI1 = new TestAnonymousApi1();
//				var anonymousAPI2 = new TestAnonymousApi2();
//				var classMemberTest = new TestClass();
//				var arrayClass = new TestArrayClass();
//				var apiTest = new TestApi();
//				ApiServiceManager.RegisterService(anonymousAPI1);
//				ApiServiceManager.RegisterService(anonymousAPI2);
//				ApiServiceManager.RegisterService(arrayClass);
//				ApiServiceManager.RegisterService(classMemberTest.classMember);
//				ApiServiceManager.RegisterService(apiTest);

//				//test GetService
//				ClassicAssert.AreNotEqual(ApiServiceManager.GetService("ClassMember"), null);
//				ClassicAssert.AreNotEqual(ApiServiceManager.GetService("classmember"), null);
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("Api"), apiTest);
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("_VOID_"), null);
//				ClassicAssert.AreNotEqual(ApiServiceManager.GetService("array"), null);

//				ApiService service = ApiServiceManager.GetService("classmember");

//				//Test GetServiceFromMethod
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.AddValue"), service);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.addvalue"), service);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.ExplicitArguments"), service);

//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.AddValue"), apiTest);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.addvalue"), apiTest);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.ExplicitArguments"), apiTest);

//				//Test ApiService
//				ClassicAssert.IsFalse(service.IsApi(classMemberTest));
//				ClassicAssert.IsTrue(service.IsApi(classMemberTest.classMember));
//				ClassicAssert.IsFalse(service.IsApi(this));
//				ClassicAssert.AreNotEqual(service.GetMethod("classmember.AddValue"), null);
//				ClassicAssert.AreEqual(service.GetMethod("AddValue"), null);
//				ClassicAssert.AreEqual(service.GetMethod("_VOID_"), null);

//				//Test MethodDescriptors
//				var method = service.GetMethod("classmember.ExplicitArguments");
//				ClassicAssert.AreEqual(method.MethodName, "classmember.explicitarguments");
//				ClassicAssert.AreEqual(method.Arguments.Count, 1);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "arg1");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(uint));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(void));

//				method = service.GetMethod("classmember.AddValue");
//				ClassicAssert.AreEqual(method.MethodName, "classmember.addvalue");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "a");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(int));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "b");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(int));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(int));

//				//Test api MethodDescriptors
//				service = ApiServiceManager.GetService("api");
//				method = service.GetMethod("Api.AddStrings");
//				ClassicAssert.AreEqual(method.MethodName, "api.addstrings");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "s1");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "s2");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(string));

//				method = service.GetMethod("Api.Add2Diff");
//				ClassicAssert.AreEqual(method.MethodName, "api.add2diff");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "arg1");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(uint));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "arg2");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(int));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(uint));

//				//Test anonymous api
//				var anonService = ApiServiceManager.GetService("");
//				ClassicAssert.AreNotEqual(ApiServiceManager.GetService(""), null);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("getworkitem"), anonService);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("getwork"), anonService);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("GetMoreWorkItem"), anonService);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("GetMoreWork"), anonService);

//				method = anonService.GetMethod("GetWork");
//				ClassicAssert.AreEqual(method.MethodName, "getwork");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "user");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "nonce1");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(string));

//				method = anonService.GetMethod("GetMoreWork");
//				ClassicAssert.AreEqual(method.MethodName, "getmorework");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "w");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "n");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(string));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(string));

//				method = ApiServiceManager.GetService("array").GetMethod("array.AddArrayUInt");
//				ClassicAssert.AreEqual(method.MethodName, "array.addarrayuint");
//				ClassicAssert.AreEqual(method.Arguments.Count, 2);
//				ClassicAssert.AreEqual(method.Arguments[0].Item1, "a");
//				ClassicAssert.AreEqual(method.Arguments[0].Item2, typeof(uint));
//				ClassicAssert.AreEqual(method.Arguments[1].Item1, "b");
//				ClassicAssert.AreEqual(method.Arguments[1].Item2, typeof(uint[]));
//				ClassicAssert.AreEqual(method.ReturnType, typeof(uint[]));

//				//test unreg+reg+unred
//				ApiServiceManager.UnregisterService("classmember");
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("ClassMember"), null);
//				ApiServiceManager.UnregisterService("classmember");
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("ClassMember"), null);

//				ApiServiceManager.UnregisterService("api");
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("api"), null);
//				ApiServiceManager.RegisterService(apiTest);
//				ClassicAssert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.AddValue"), apiTest);
//				ApiServiceManager.UnregisterService(apiTest as object);
//				ClassicAssert.AreEqual(ApiServiceManager.GetService("api"), null);
//				ApiServiceManager.UnregisterService("");

//				ApiServiceManager.UnregisterService(anonymousAPI1);
//				ApiServiceManager.UnregisterService(anonymousAPI2);
//				ApiServiceManager.UnregisterService(arrayClass);
//				ApiServiceManager.UnregisterService(classMemberTest.classMember);
//				ApiServiceManager.UnregisterService(apiTest);
//				ApiServiceManager.UnregisterService("");
//			}
//			catch (Exception e) {
//				Assert.Fail(e.ToString());
//			}
//		}

//		[Test]
//		public void TestRPCCalls() {
//			//----------------------------------------------------
//			// Server side
//			var anonymousAPI1 = new TestAnonymousApi1();
//			var anonymousAPI2 = new TestAnonymousApi2();
//			var arrayClass = new TestArrayClass();
//			var classMemberTest = new TestClass();
//			var apiTest = new TestApi();
//			JsonRpcServer server = null;
//			try {
//				//Register various apis
//				ApiServiceManager.RegisterService(anonymousAPI1);
//				ApiServiceManager.RegisterService(anonymousAPI2);
//				ApiServiceManager.RegisterService(classMemberTest.classMember);
//				ApiServiceManager.RegisterService(apiTest);
//				ApiServiceManager.RegisterService(arrayClass);
//				//Start server()	
//				server = new JsonRpcServer(new TcpEndPointListener(true, 27001, 5), JsonRpcConfig.Default);
//				server.Start();
//				Thread.Sleep(250);
//			} catch (Exception e) {
//				Assert.Fail(e.ToString());
//			}

//			//----------------------------------------------------
//			// Client side
//			try
//			{
//				var client = new JsonRpcClient(new TcpEndPoint("127.0.0.1", 27001), JsonRpcConfig.Default);

//				//Test objects and array of objects
//				TestObject to = client.RemoteCall<TestObject>("ClassMember.GetTestObject", new TestObject
//				{
//					iVal = 199,
//					fArray = new float[] { 8 },
//					sVal = "allo",
//					bytesArray = new byte[] { 9, 2, 3, 8, 9, 4, 5, 6, 7, 8, 1, 9, 3, 7, 6, 5 },
//					enumVal = FreeEnum.Third,
//					enumVal2 = FreeEnum.Second,
//					dictionary = new Dictionary<string, int> { { "zzz", 111 }, { "YYY", 222 }, { "TTT", 333 } },
//				});
//				ClassicAssert.AreEqual(to.iVal, 199 + 1);
//				ClassicAssert.AreEqual(to.fArray, new float[] { 8, 1 });
//				ClassicAssert.AreEqual(to.sVal, "allo1");
//				ClassicAssert.AreEqual(to.enumVal, FreeEnum.Third);
//				ClassicAssert.AreEqual(to.enumVal2, FreeEnum.Second);
//				ClassicAssert.AreEqual(to.bytesArray, new byte[] { 9, 2, 3, 8, 9, 4, 5, 6, 7, 8, 1, 9, 3, 7, 6, 5 });
//				ClassicAssert.AreEqual(to.dictionary, new Dictionary<string, int> { { "zzz", 111 }, { "YYY", 222 }, { "TTT", 333 } });
//				ClassicAssert.AreEqual(client.RemoteCall<Dictionary<string, int>>("classMember.GetTestDictionary", new Dictionary<string, int> { { "a", 8 }, { "b", 10 } }), new Dictionary<string, int> { { "8", 10 }, { "88", 20 }, { "888", 30 } });
//				ClassicAssert.AreEqual(client.RemoteCall<Tuple<string, int>[]>("classMember.GetTestArrayOfTuple", "x", 1), new Tuple<string, int>[] { new Tuple<string, int>("x", 1), new Tuple<string, int>("xx", 2), new Tuple<string, int>("xxx", 3) });

//				//Test array of random types
//				ClassicAssert.AreEqual(client.RemoteCall<object[][]>("array.ToArrayOfArrayOfIntAndStrings", 13, "Bob"), new object[][] { new object[] { 14, "Bob" }, new object[] { "Bob", 15 } });
//				ClassicAssert.AreEqual(client.RemoteCall<byte[]>("array.ToByteArray", 9, new byte[] { (byte)0, (byte)10, (byte)20, (byte)30 }), new byte[] { (byte)9, (byte)19, (byte)29, (byte)39 });
//				ClassicAssert.AreEqual(client.RemoteCall<TestObjectWithBytesArray>("ClassMember.GetObjectWithBytesArray", new TestObjectWithBytesArray { bytesArray = new byte[] { 1, 3, 7, 9 } }).bytesArray, new byte[] { 9, 7, 3, 1 });

//				TestObject[] toa = client.RemoteCall<TestObject[]>("ClassMember.GetTestObjectArray", new TestObject { iVal = 199, fArray = new float[] { 8 }, sVal = "allo" });
//				ClassicAssert.AreEqual(toa[0].iVal, 199 + 1);
//				ClassicAssert.AreEqual(toa[0].fArray, new float[] { 8, 1 });
//				ClassicAssert.AreEqual(toa[0].sVal, "allo1");
//				ClassicAssert.AreEqual(toa[1].iVal, 199 + 2);
//				ClassicAssert.AreEqual(toa[1].fArray, new float[] { 8, 2 });
//				ClassicAssert.AreEqual(toa[1].sVal, "allo2");

//				//Test array in return values
//				ClassicAssert.AreEqual(client.RemoteCall<string[]>("array.AddArrayString", "A", new string[] { "1", "3", "7", "9" }), new string[] { "A.1", "A.3", "A.7", "A.9" });
//				ClassicAssert.AreEqual(client.RemoteCall<Int64[]>("array.AddArrayInt", 10, new int[] { 1, 3, 7, 9 }), new int[] { 11, 13, 17, 19 });
//				ClassicAssert.AreEqual(client.RemoteCall<UInt64[]>("array.AddArrayUInt", 100, new int[] { 1, 3, 7, 9 }), new uint[] { 101, 103, 107, 109 });
//				ClassicAssert.AreEqual(client.RemoteCall<float[]>("array.AddArrayFloat", 10.5, new float[] { 1, 3, 7, 9 }), new float[] { 11.5f, 13.5f, 17.5f, 19.5f });
//				ClassicAssert.AreEqual(client.RemoteCall<double[]>("array.AddArrayDouble", 100.8, new double[] { 1, 3, 7, 9 }), new double[] { 101.8, 103.8, 107.8, 109.8 });
//				ClassicAssert.AreEqual(client.RemoteCall<string[]>("array.ToArrayString", "Sponge", "Bob"), new string[] { "Sponge", "Bob" });
//				ClassicAssert.AreEqual(client.RemoteCall<double[]>("array.ToArrayDouble", 666.777, 88.99), new double[] { 666.777, 88.99 });
//				ClassicAssert.AreEqual(client.RemoteCall<float[]>("array.ToArrayFloat", 123.555, 12.12), new float[] { 123.555f, 12.12f });
//				ClassicAssert.AreEqual(client.RemoteCall<UInt64[]>("array.ToArrayUInt", 422, 5), new uint[] { 422, 5 });
//				ClassicAssert.AreEqual(client.RemoteCall<Int64[]>("array.ToArrayInt", 123, 1), new uint[] { 123, 1 });

//				//tsts normal calls with return value
//				ClassicAssert.AreEqual(client.RemoteCall<int>("ClassMember.addint", 11, -1), 10);
//				ClassicAssert.AreEqual(client.RemoteCall<int>("ClassMember.AddValue", 11, 2), 13);
//				ClassicAssert.AreEqual(client.RemoteCall<uint>("ClassMember.AddUInt", 5, 5), 10);
//				ClassicAssert.AreEqual(client.RemoteCall<float>("ClassMember.AddFloat", 198.0099999, 0.0000001), (float)198.01);
//				ClassicAssert.AreEqual(client.RemoteCall<double>("ClassMember.AddDouble", (double)float.MaxValue, ((double)float.MaxValue) / 2), (((double)float.MaxValue) + ((double)float.MaxValue) / 2));
//				ClassicAssert.AreEqual(client.RemoteCall<string>("api.addstrings", "ham", "burger"), "hamburger");
//				ClassicAssert.AreEqual(client.RemoteCall<uint>("api.Add2Diff", 199, -9), 190);

//				//Test method with no return value and no args
//				classMemberTest.classMember.TestValue = 7;
//				client.RemoteCall("ClassMember.ExplicitArguments", 2);
//				ClassicAssert.AreEqual(classMemberTest.classMember.TestValue, 2);
//				classMemberTest.classMember.TestValue = 17;
//				client.RemoteCall<Void>("ClassMember.ExplicitArguments", 12);
//				ClassicAssert.AreEqual(classMemberTest.classMember.TestValue, 12);
//				classMemberTest.classMember.TestValue = 8;
//				ClassicAssert.AreEqual(client.RemoteCall<uint>("ClassMember.NoArgsWithRet"), 987654321);
//				ClassicAssert.AreEqual(classMemberTest.classMember.TestValue, 123456789);
//				classMemberTest.classMember.TestValue = 9;
//				client.RemoteCall("ClassMember.NoArgs");
//				ClassicAssert.AreEqual(classMemberTest.classMember.TestValue, 77777777);

//				//Test int overflow
//				ClassicAssert.AreEqual(client.RemoteCall<int>("ClassMember.AddUInt", System.UInt32.MaxValue, 2), 1);

//				//test anonymous/nameless api
//				ClassicAssert.AreEqual(client.RemoteCall<string>("getwork", "rad", "ical"), "RAD.ICAL");
//				ClassicAssert.AreEqual(client.RemoteCall<string>("getmorework", "Abs", "Olut"), "ABS.OLUT");

//				//test enum
//				ClassicAssert.AreEqual(client.RemoteCall<TestApi.FixedEnum>("api.TestEnum", FreeEnum.First), TestApi.FixedEnum.Big);
//				ClassicAssert.AreEqual(client.RemoteCall<TestApi.FixedEnum>("api.TestEnum", FreeEnum.Second), TestApi.FixedEnum.Medium);
//				ClassicAssert.AreEqual(client.RemoteCall<TestApi.FixedEnum>("api.TestEnum", FreeEnum.Third), TestApi.FixedEnum.Small);

//				//Test int as hex
//				var ohx = client.RemoteCall<TestObjectHex>("api.GetTestObjectHex");
//				ClassicAssert.AreEqual(ohx.hexUInt, 999999);
//				ClassicAssert.AreEqual(ohx.hexInt, 88888);
//				ClassicAssert.AreEqual(ohx.hexInt64, 0x75553333999CCCC1);
//				ClassicAssert.AreEqual(ohx.hexUInt64, 0xFFFFAAAABBBB3333);
//				var ohx2 = client.RemoteCall<TestObjectHex>("api.GetTestObjectHex2", new TestObjectHex { hexUInt = 1234567891, hexInt = 2147483641, hexInt64 = 163245617943825, hexUInt64 = 0xFFFFFFFFFFFFFFF });
//				ClassicAssert.AreEqual(ohx2.hexUInt, 1234567891 + 1);
//				ClassicAssert.AreEqual(ohx2.hexInt, 2147483641 + 1);
//				ClassicAssert.AreEqual(ohx2.hexInt64, 163245617943825 + 1);
//				ClassicAssert.AreEqual(ohx2.hexUInt64, 0xFFFFFFFFFFFFFFF + 1);

//				//Test batch calls
//				classMemberTest.classMember.TestValue = 156;
//				ApiBatchCallDescriptor batch = new ApiBatchCallDescriptor();
//				batch.Call<int>("ClassMember.addint", 11, -1);
//                batch.Call<int>("ClassMember.AddValue", 11, 2);
//				batch.Call<uint>("ClassMember.AddUInt", 5, 5);
//				batch.Call<float>("ClassMember.AddFloat", 198.0099999, 0.0000001);
//				batch.Call<double>("ClassMember.AddDouble", (double)float.MaxValue, ((double)float.MaxValue) / 2);
//				batch.Call<string>("api.addstrings", "ham", "burger");
//				batch.Call<uint>("api.Add2Diff", 199, -9);
//				batch.Call("ClassMember.ExplicitArguments", 877);
//				batch.Call<object[][]>("array.ToArrayOfArrayOfIntAndStrings", 13, "Bob");
//				batch.Call<float[]>("array.AddArrayFloat", 10.5, new float[] { 1, 3, 7, 9 });
//				batch.Call<TestObject[]>("ClassMember.GetTestObjectArray", new TestObject { iVal = 199, fArray = new float[] { 8 }, sVal = "allo" });
//				object[] batchResult = client.RemoteCall(batch);
//				//eval
//				ClassicAssert.AreEqual(batchResult[0], 10);
//				ClassicAssert.AreEqual(batchResult[1], 13);
//				ClassicAssert.AreEqual(batchResult[2], 10);
//				ClassicAssert.AreEqual(batchResult[3], (float)198.01);
//				ClassicAssert.AreEqual(batchResult[4], (((double)float.MaxValue) + ((double)float.MaxValue) / 2)  );
//				ClassicAssert.AreEqual(batchResult[5], "hamburger");
//				ClassicAssert.AreEqual(batchResult[6], 190);
//				ClassicAssert.AreEqual(classMemberTest.classMember.TestValue, 877);
//				ClassicAssert.AreEqual(batchResult[8], new object[][] { new object[] { 14, "Bob" }, new object[] { "Bob", 15 } });
//				ClassicAssert.AreEqual(batchResult[9], new float[] { 11.5f, 13.5f, 17.5f, 19.5f });
//				var objArray = (TestObject[])batchResult[10];
//				ClassicAssert.AreEqual(objArray[0].iVal, 199 + 1);
//				ClassicAssert.AreEqual(objArray[0].fArray, new float[] { 8, 1 });
//				ClassicAssert.AreEqual(objArray[0].sVal, "allo1");
//				ClassicAssert.AreEqual(objArray[1].iVal, 199 + 2);
//				ClassicAssert.AreEqual(objArray[1].fArray, new float[] { 8, 2 });
//				ClassicAssert.AreEqual(objArray[1].sVal, "allo2");


//				//Test "Arguments exception cought : {ex.ToString()}"  / Wrong argument counts 
//				ClassicAssert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.AddValue.");
//				ClassicAssert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.NoArgs.");

//				//Test "The method {methodName}does not exist"
//				ClassicAssert.AreEqual(TestRpcException<string>(client, "_VOID_", new object[] { 1 }), "RPC error -2: The method _VOID_ does not exist.");
//				//Test "Wrong argument type in {methodName}. Arguments are {sig}"
//				ClassicAssert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { "s", "s" }), "RPC error -5: Wrong argument type in method ClassMember.AddValue.");
//				//Test "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method"
//				ClassicAssert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { }), "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method");

//				//Test BATCH "Arguments exception cought : {ex.ToString()}"  / Wrong argument counts 
//				ClassicAssert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.AddValue", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.AddValue.");
//				ClassicAssert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.NoArgs", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.NoArgs.");

//				//Test BATCH "The method {methodName}does not exist"
//				ClassicAssert.AreEqual(TestRpcExceptionBatch(client, "_VOID_", new object[] { 1 }), "RPC error -2: The method _VOID_ does not exist.");
//				//Test BATCH "Wrong argument type in {methodName}. Arguments are {sig}"
//				ClassicAssert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.AddValue", new object[] { "s", "s" }), "RPC error -5: Wrong argument type in method ClassMember.AddValue.");
//				//Test BATCH "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method"
//				ClassicAssert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.NoArgs", new object[] { }), "null");


//			} catch (Exception e) {
//				Assert.Fail(e.ToString());
//			}

//			//close everything
//			server.Stop();
//			ApiServiceManager.UnregisterService(anonymousAPI1);
//			ApiServiceManager.UnregisterService(anonymousAPI2);
//			ApiServiceManager.UnregisterService(classMemberTest.classMember);
//			ApiServiceManager.UnregisterService(apiTest);
//			ApiServiceManager.UnregisterService(arrayClass);
//			ApiServiceManager.UnregisterService("");
//		}

//		[Test]
//		public void TestRPC_from_curl()
//		{
//			//----------------------------------------------------
//			// Server side
//			var apiTest = new TestApi();
//			var classMemberTest = new TestClass(); 
//			JsonRpcServer server = null;
//			try
//			{
//				//Register various apis
//				ApiServiceManager.RegisterService(apiTest);
//				ApiServiceManager.RegisterService(classMemberTest.classMember);
//				//Start server()	
//				server = new JsonRpcServer(new TcpEndPointListener(true, 27001, 5), JsonRpcConfig.Default);
//				server.Start();
//				Thread.Sleep(250);
//			}
//			catch (Exception e)
//			{
//				Assert.Fail(e.ToString());
//			}

//			Func<string, string> curl = argStr => { 
//				var clientTcp = new TcpEndPoint("127.0.0.1", 27001);
//				var curlMessage = new EndpointMessage("", clientTcp);
//				var result = new EndpointMessage("");

//				curlMessage.FromString(argStr);
//				clientTcp.WriteMessage(curlMessage);
//				return clientTcp.ReadMessage().ToString();
//			};

//			//----------------------------------------------------
//			// Client side - Simulate calls with curl
//			try
//			{
//				//test HexadecimalValueConverterReader reading/writing
//				string curlRes = curl("    {\"jsonrpc\":\"2.0\",\"method\":\"api.GetTestObjectHex2\",\"params\":[{\"hexInt\":0x7ffffff9,\"hexUInt\":\"0x499602d3\",\"hexUInt64\":\"0xfffffffffffffff\",\"hexInt64\":\"0x947895119511\"}],\"id\":2}   \n  ");
//				JsonResponse jres = JsonConvert.DeserializeObject<JsonResponse>(curlRes, new JsonSerializerSettings { Converters = { { new ByteArrayHexConverter() } } });
//				ClassicAssert.AreEqual(jres.Error, null);
//				int hexInt = (int)(jres.Result as JObject)["hexInt"];
//				uint hexUInt = (uint)(jres.Result as JObject)["hexUInt"];
//				Int64 hexInt64 = (Int64)(jres.Result as JObject)["hexInt64"];
//				UInt64 hexUInt64 = (UInt64)(jres.Result as JObject)["hexUInt64"];
//				ClassicAssert.AreEqual(hexInt, 0x7ffffff9 + 1);
//				ClassicAssert.AreEqual(hexUInt, 0x499602d3 + 1);
//				ClassicAssert.AreEqual(hexInt64, 0x947895119511 + 1);
//				ClassicAssert.AreEqual(hexUInt64, 0xfffffffffffffff + 1);

//				//test Newtonsoft.Json implicit hex reading
//				curlRes = curl("\n{ \"jsonrpc\":\"2.0\",\"method\":\"ClassMember.addint\",\"params\":[0xd,7],\"id\":1}\n");
//				jres = JsonConvert.DeserializeObject<JsonResponse>(curlRes, new JsonSerializerSettings { Converters = { { new ByteArrayHexConverter() } } });
//				ClassicAssert.AreEqual(jres.Error, null);
//				ClassicAssert.AreEqual((long)jres.Result, 20);
//			}
//			catch (Exception e)
//			{
//				Assert.Fail(e.ToString());
//			}

//			//close everything
//			server.Stop();
//			ApiServiceManager.UnregisterService(apiTest);
//			ApiServiceManager.UnregisterService(classMemberTest.classMember);
//			ApiServiceManager.UnregisterService(classMemberTest);
//			ApiServiceManager.UnregisterService("");
//		}


//		[Test]
//		public void TestTcpRPCSecurityPoliciy_TooManyConnections()
//		{
//			const int TestMaxConnections = 8;
//			const int TestMaxThreads = TestMaxConnections*3;

//			// Server side
//			var apiTest = new TestApi();
//			JsonRpcServer server = null;
//			try
//			{
//				//Register various apis
//				ApiServiceManager.RegisterService(apiTest);
//				//Start server()
//				TcpSecurityPolicies.MaxSimultanousConnecitons = TestMaxConnections;
//				server = new JsonRpcServer(new TcpEndPointListener(true, 27001, 5), JsonRpcConfig.Default);
//				server.Start();
//				Thread.Sleep(250);
//			}
//			catch (Exception e)
//			{
//				Assert.Fail(e.ToString());
//			}

//			//----------------------------------------------------
//			// Client side
//			try
//			{
//				int failed = 0;
//				Thread[] threadsArray = new Thread[TestMaxThreads];
//				//creating 3x more clients/connection than the limit
//				for (int i = 0; i < TestMaxThreads; i++)
//					threadsArray[i] = new Thread((i) =>
//					{
//						Thread.CurrentThread.Name = $"test{i}";
//						try {
//							var socket = new TcpClient("127.0.0.1", 27001);
//							socket.Client.ReceiveTimeout = 5000;
//							var client = new JsonRpcClient(new TcpEndPoint(socket), JsonRpcConfig.Default);
//							ClassicAssert.AreEqual(client.RemoteCall<uint>("api.Add2Diff", 199, -9), 190);
//							Thread.Sleep(250);
//						}
//						catch (Exception e) {
//							Interlocked.Increment(ref failed);
//						}
//					});
//				for (int i = 0; i < TestMaxThreads; i++)
//					threadsArray[i].Start(i);
//				for (int i = 0; i < TestMaxThreads; i++)
//					threadsArray[i].Join();

//				//must all occur under 250 ms
//				Assert.Greater(failed, TestMaxConnections*2); 
//			}
//			catch (Exception e) {
//				Assert.Fail(e.ToString());
//			}

//			//close everything
//			server.Stop();
//			ApiServiceManager.UnregisterService(apiTest);
//		}
//	}

//}


