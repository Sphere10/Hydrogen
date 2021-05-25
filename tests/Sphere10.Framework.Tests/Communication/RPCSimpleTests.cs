using System;
using System.Threading;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Newtonsoft.Json.Linq;

namespace Sphere10.Framework.Tests {

	//Anonymous API functions
	[RpcAPIService("")]
	public class TestAnonymousApi1 {
		[RpcAPIMethod("GetWorkItem")]
		[RpcAPIMethod]
		public string GetWork(string user, string nonce1) { return user.ToUpper() + "." + nonce1.ToUpper(); }
	}
	//Anonymous API functions
	public class TestAnonymousApi2 {
		[RpcAPIMethod("GetMoreWorkItem")]
		[RpcAPIMethod]
		public string GetMoreWork(string w, string n) { return w.ToUpper() + "." + n.ToUpper(); }
	}

	//test RPC parameter object
	public class TestObject {
		public int iVal = 0;
		public string sVal = "";
		public float[] fArray;
	}

	//Simple Class with RPC methodes
	[RpcAPIService("ClassMember")]
	public class TestChildClass {
		public uint TestValue { get; set; }

		[RpcAPIMethod("AddValue")]
		[RpcAPIMethod]
		public int AddInt(int a, int b) { return a + b; }

		[RpcAPIMethod]
		public uint AddUInt(uint a, uint b) { return a + b; }

		[RpcAPIMethod]
		public float AddFloat(float a, float b) { return a + b; }

		[RpcAPIMethod]
		public double AddDouble(double a, double b) { return a + b; }

		[RpcAPIMethod]
		public string ConcatString(string a, string b) { return a + b; }

		[RpcAPIMethod]
		public void ExplicitArguments([RpcAPIArgument("arg1")] uint argumentA) { TestValue = argumentA; }

		[RpcAPIMethod]
		public uint NoArgsWithRet() { TestValue = 123456789; return 987654321; }

		[RpcAPIMethod]
		public void NoArgs() { TestValue = 77777777; }

		[RpcAPIMethod]
		public object GetTestObject(TestObject bp) { return new TestObject { iVal = bp.iVal + 1, fArray = bp.fArray.Append(1).ToArray(), sVal = bp.sVal + "1" }; }

		[RpcAPIMethod]
		public object[] GetTestObjectArray(TestObject bp) { return new TestObject[] { new TestObject { iVal = bp.iVal + 1, fArray = bp.fArray.Append(1).ToArray(), sVal = bp.sVal + "1" }, new TestObject { iVal = bp.iVal + 2, fArray = bp.fArray.Append(2).ToArray(), sVal = bp.sVal + "2" } }; }

	}

	//Simple Class with RPC methodes
	[RpcAPIService("array")]
	public class TestArrayClass {
		public uint TestValue { get; set; }

		[RpcAPIMethod]
	 	public object[][] ToArrayOfArrayOfIntAndStrings(int a, string b) { return new object[][] { new object[] { a+1, b }, new object [] { b, a+2 } }; }

		[RpcAPIMethod]
		public int[] ToArrayInt(int a, int b) { return new int[] { a, b }; }

		[RpcAPIMethod]
		public uint[] ToArrayUInt(uint a, uint b) { return new uint[] { a, b }; }

		[RpcAPIMethod]
		public float[] ToArrayFloat(float a, float b) { return new float[] { a, b }; }

		[RpcAPIMethod]
		public double[] ToArrayDouble(double a, double b) { return new double[] { a, b }; }

		[RpcAPIMethod]
		public string[] ToArrayString(string a, string b) { return new string[] { a, b }; }

		[RpcAPIMethod]
		public int[] AddArrayInt(int a, int[] b) { return b.Select(i => i + a).ToArray(); }

		[RpcAPIMethod]
		public uint[] AddArrayUInt(uint a, uint[] b) { return b.Select(i => i + a).ToArray(); }

		[RpcAPIMethod]
		public float[] AddArrayFloat(float a, float[] b) { return b.Select(i => i + a).ToArray(); }

		[RpcAPIMethod]
		public double[] AddArrayDouble(double a, double[] b) { return b.Select(i => i + a).ToArray(); }

		[RpcAPIMethod]
		public string[] AddArrayString(string a, string[] b) { return b.Select(i => a + "." + i).ToArray(); }
	}

	public class TestClass{
		public TestChildClass classMember;
		public TestClass() { classMember = new TestChildClass(); }
	}

	[RpcAPIService("Api")]
	public class TestApi : ApiService {
		[RpcAPIMethod]
		public string AddStrings([RpcAPIArgument("s1")] string str1, [RpcAPIArgument("s2")] string str2) { return str1 + str2; }

		[RpcAPIMethod]
		public uint Add2Diff([RpcAPIArgument("arg1")] uint argumentA, [RpcAPIArgument("arg2")] int argumentB) { return argumentA + (uint)argumentB; }

		[RpcAPIMethod]
		public string DirtyJson() { return "BadJson{\a\v{"; }
	}


	[TestFixture]
	[Category("RPC")]
	public class RPC {
		protected string TestRpcException<TRetType>(JsonRpcClient client, string funcName, object[] args) {
			string err = "";
			try {
				client.RemoteCall<TRetType>(funcName, args);
			} catch (Exception e) {
				err = e.Message;
			}
			return err;
		}
		protected string TestRpcExceptionBatch(JsonRpcClient client, string funcName, object[] args) {
			string err = "";
			object[] batchResult = null;
			try {
				ApiBatchCallDescriptor batch = new ApiBatchCallDescriptor();
				batch.Call("ClassMember.addint", 11, -1);
				batch.Call(funcName, args);				
                batch.Call("ClassMember.AddValue", 11, 2);
				batchResult = client.RemoteCall(batch);
			} catch (Exception e) {
				err = e.Message;
			}
			//get 2nd entry that supposed to contain exception
			return batchResult[1] == null ? "null" : (batchResult[1] as JsonRpcException).ToString();
		}

		[Test]
		public void TestServiceManager() {
			try {
				var anonymousAPI1 = new TestAnonymousApi1();
				var anonymousAPI2 = new TestAnonymousApi2();
				var classMemberTest = new TestClass();
				var arrayClass = new TestArrayClass();
				var apiTest = new TestApi();
				ApiServiceManager.RegisterService(anonymousAPI1);
				ApiServiceManager.RegisterService(anonymousAPI2);
				ApiServiceManager.RegisterService(arrayClass);
				ApiServiceManager.RegisterService(classMemberTest.classMember);
				ApiServiceManager.RegisterService(apiTest);

				//test GetService
				Assert.AreNotEqual(ApiServiceManager.GetService("ClassMember"), null);
				Assert.AreNotEqual(ApiServiceManager.GetService("classmember"), null);
				Assert.AreEqual(ApiServiceManager.GetService("Api"), apiTest);
				Assert.AreEqual(ApiServiceManager.GetService("_VOID_"), null);
				Assert.AreNotEqual(ApiServiceManager.GetService("array"), null);

				ApiService service = ApiServiceManager.GetService("classmember");

				//Test GetServiceFromMethod
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.AddValue"), service);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.addvalue"), service);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("classmember.ExplicitArguments"), service);

				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.AddValue"), apiTest);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.addvalue"), apiTest);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.ExplicitArguments"), apiTest);

				//Test ApiService
				Assert.IsFalse(service.IsApi(classMemberTest));
				Assert.IsTrue(service.IsApi(classMemberTest.classMember));
				Assert.IsFalse(service.IsApi(this));
				Assert.AreNotEqual(service.GetMethod("classmember.AddValue"), null);
				Assert.AreEqual(service.GetMethod("AddValue"), null);
				Assert.AreEqual(service.GetMethod("_VOID_"), null);

				//Test MethodDescriptors
				var method = service.GetMethod("classmember.ExplicitArguments");
				Assert.AreEqual(method.MethodName, "classmember.explicitarguments");
				Assert.AreEqual(method.Arguments.Count, 1);
				Assert.AreEqual(method.Arguments[0].Item1, "arg1");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(uint));
				Assert.AreEqual(method.ReturnType, typeof(void));

				method = service.GetMethod("classmember.AddValue");
				Assert.AreEqual(method.MethodName, "classmember.addvalue");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "a");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(int));
				Assert.AreEqual(method.Arguments[1].Item1, "b");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(int));
				Assert.AreEqual(method.ReturnType, typeof(int));

				//Test api MethodDescriptors
				service = ApiServiceManager.GetService("api");
				method = service.GetMethod("Api.AddStrings");
				Assert.AreEqual(method.MethodName, "api.addstrings");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "s1");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(string));
				Assert.AreEqual(method.Arguments[1].Item1, "s2");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(string));
				Assert.AreEqual(method.ReturnType, typeof(string));

				method = service.GetMethod("Api.Add2Diff");
				Assert.AreEqual(method.MethodName, "api.add2diff");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "arg1");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(uint));
				Assert.AreEqual(method.Arguments[1].Item1, "arg2");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(int));
				Assert.AreEqual(method.ReturnType, typeof(uint));

				//Test anonymous api
				var anonService = ApiServiceManager.GetService("");
				Assert.AreNotEqual(ApiServiceManager.GetService(""), null);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("getworkitem"), anonService);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("getwork"), anonService);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("GetMoreWorkItem"), anonService);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("GetMoreWork"), anonService);
				
				method = anonService.GetMethod("GetWork");
				Assert.AreEqual(method.MethodName, "getwork");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "user");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(string));
				Assert.AreEqual(method.Arguments[1].Item1, "nonce1");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(string));
				Assert.AreEqual(method.ReturnType, typeof(string));

				method = anonService.GetMethod("GetMoreWork");
				Assert.AreEqual(method.MethodName, "getmorework");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "w");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(string));
				Assert.AreEqual(method.Arguments[1].Item1, "n");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(string));
				Assert.AreEqual(method.ReturnType, typeof(string));
								
				method = ApiServiceManager.GetService("array").GetMethod("array.AddArrayUInt");
				Assert.AreEqual(method.MethodName, "array.addarrayuint");
				Assert.AreEqual(method.Arguments.Count, 2);
				Assert.AreEqual(method.Arguments[0].Item1, "a");
				Assert.AreEqual(method.Arguments[0].Item2, typeof(uint));
				Assert.AreEqual(method.Arguments[1].Item1, "b");
				Assert.AreEqual(method.Arguments[1].Item2, typeof(uint[]));
				Assert.AreEqual(method.ReturnType, typeof(uint[]));

				//test unreg+reg+unred
				ApiServiceManager.UnregisterService("classmember");
				Assert.AreEqual(ApiServiceManager.GetService("ClassMember"), null);
				ApiServiceManager.UnregisterService("classmember");
				Assert.AreEqual(ApiServiceManager.GetService("ClassMember"), null);

				ApiServiceManager.UnregisterService("api");
				Assert.AreEqual(ApiServiceManager.GetService("api"), null);
				ApiServiceManager.RegisterService(apiTest);
				Assert.AreEqual(ApiServiceManager.GetServiceFromMethod("api.AddValue"), apiTest);
				ApiServiceManager.UnregisterService(apiTest as object);
				Assert.AreEqual(ApiServiceManager.GetService("api"), null);
				ApiServiceManager.UnregisterService("");

				ApiServiceManager.UnregisterService(anonymousAPI1);
				ApiServiceManager.UnregisterService(anonymousAPI2);
				ApiServiceManager.UnregisterService(arrayClass);
				ApiServiceManager.UnregisterService(classMemberTest.classMember);
				ApiServiceManager.UnregisterService(apiTest);
				ApiServiceManager.UnregisterService("");
			}
			catch (Exception e) {
				Assert.Fail(e.ToString());
			}
		}

		[Test]
		public void TestRPCCalls() {
			//----------------------------------------------------
			// Server side
			var anonymousAPI1 = new TestAnonymousApi1();
			var anonymousAPI2 = new TestAnonymousApi2();
			var arrayClass = new TestArrayClass();
			var classMemberTest = new TestClass();
			var apiTest = new TestApi();
			JsonRpcServer server = null;
			try {
				//Register various apis
				ApiServiceManager.RegisterService(anonymousAPI1);
				ApiServiceManager.RegisterService(anonymousAPI2);
				ApiServiceManager.RegisterService(classMemberTest.classMember);
				ApiServiceManager.RegisterService(apiTest);
				ApiServiceManager.RegisterService(arrayClass);
				//Start server()	
				server = new JsonRpcServer(new TcpEndPointListener(true, 27000, 5));
				server.Start();
				Thread.Sleep(250);
			} catch (Exception e) {
				Assert.Fail(e.ToString());
			}

			//----------------------------------------------------
			// Client side
			try {
				var client = new JsonRpcClient(new TcpEndPoint("127.0.0.1", 27000));

				//Test array of random types
				Assert.AreEqual(client.RemoteCall<object[][]>("array.ToArrayOfArrayOfIntAndStrings", 13, "Bob"), new object[][] { new object[] { 14, "Bob" }, new object[] { "Bob", 15 } });

				//Test objects and array of objects
				TestObject to = client.RemoteCall<TestObject>("ClassMember.GetTestObject", new TestObject { iVal = 199, fArray = new float[] { 8 }, sVal = "allo" });
				Assert.AreEqual(to.iVal, 199 + 1);
				Assert.AreEqual(to.fArray, new float[] { 8, 1 });
				Assert.AreEqual(to.sVal, "allo1");
				TestObject[] toa = client.RemoteCall<TestObject[]>("ClassMember.GetTestObjectArray", new TestObject { iVal = 199, fArray = new float[] { 8 }, sVal = "allo" });
				Assert.AreEqual(toa[0].iVal, 199 + 1);
				Assert.AreEqual(toa[0].fArray, new float[] { 8, 1 });
				Assert.AreEqual(toa[0].sVal, "allo1");
				Assert.AreEqual(toa[1].iVal, 199 + 2);
				Assert.AreEqual(toa[1].fArray, new float[] { 8, 2 });
				Assert.AreEqual(toa[1].sVal, "allo2");

				//Test array in return values
				Assert.AreEqual(client.RemoteCall<string[]>("array.AddArrayString", "A", new string[] { "1", "3", "7", "9" }), new string[] { "A.1", "A.3", "A.7", "A.9" });
				Assert.AreEqual(client.RemoteCall<Int64[]>("array.AddArrayInt", 10, new int[] { 1, 3, 7, 9 }), new int[] { 11, 13, 17, 19 });
				Assert.AreEqual(client.RemoteCall<UInt64[]>("array.AddArrayUInt", 100, new int[] { 1, 3, 7, 9 }), new uint[] { 101, 103, 107, 109 });
				Assert.AreEqual(client.RemoteCall<float[]>("array.AddArrayFloat", 10.5, new float[] { 1, 3, 7, 9 }), new float[] { 11.5f, 13.5f, 17.5f, 19.5f });
				Assert.AreEqual(client.RemoteCall<double[]>("array.AddArrayDouble", 100.8, new double[] { 1, 3, 7, 9 }), new double[] { 101.8, 103.8, 107.8, 109.8 });
				Assert.AreEqual(client.RemoteCall<string[]>("array.ToArrayString", "Sponge", "Bob"), new string[] { "Sponge", "Bob" });
				Assert.AreEqual(client.RemoteCall<double[]>("array.ToArrayDouble", 666.777, 88.99), new double[] { 666.777, 88.99 });
				Assert.AreEqual(client.RemoteCall<float[]>("array.ToArrayFloat", 123.555, 12.12), new float[] { 123.555f, 12.12f });
				Assert.AreEqual(client.RemoteCall<UInt64[]>("array.ToArrayUInt", 422, 5), new uint[] { 422, 5 });
				Assert.AreEqual(client.RemoteCall<Int64[]>("array.ToArrayInt", 123, 1), new uint[] { 123, 1 });

				//tsts normal calls with return value
				Assert.AreEqual(client.RemoteCall<int>("ClassMember.addint", 11, -1), 10);
				Assert.AreEqual(client.RemoteCall<int>("ClassMember.AddValue", 11, 2), 13);
				Assert.AreEqual(client.RemoteCall<uint>("ClassMember.AddUInt", 5, 5), 10);
				Assert.AreEqual(client.RemoteCall<float>("ClassMember.AddFloat", 198.0099999, 0.0000001), (float)198.01);
				Assert.AreEqual(client.RemoteCall<double>("ClassMember.AddDouble", (double)float.MaxValue, ((double)float.MaxValue) / 2), (((double)float.MaxValue) + ((double)float.MaxValue) / 2));
				Assert.AreEqual(client.RemoteCall<string>("api.addstrings", "ham", "burger"), "hamburger");
				Assert.AreEqual(client.RemoteCall<uint>("api.Add2Diff", 199, -9), 190);

				//Test method with no return value and no args
				classMemberTest.classMember.TestValue = 7;
				client.RemoteCall("ClassMember.ExplicitArguments", 2);
				Assert.AreEqual(classMemberTest.classMember.TestValue, 2);
				classMemberTest.classMember.TestValue = 17;
				client.RemoteCall<Void>("ClassMember.ExplicitArguments", 12);
				Assert.AreEqual(classMemberTest.classMember.TestValue, 12);
				classMemberTest.classMember.TestValue = 8;
				Assert.AreEqual(client.RemoteCall<uint>("ClassMember.NoArgsWithRet"), 987654321);
				Assert.AreEqual(classMemberTest.classMember.TestValue, 123456789);
				classMemberTest.classMember.TestValue = 9;
				client.RemoteCall("ClassMember.NoArgs");
				Assert.AreEqual(classMemberTest.classMember.TestValue, 77777777);

				//Test int overflow
				Assert.AreEqual(client.RemoteCall<int>("ClassMember.AddUInt", System.UInt32.MaxValue, 2), 1);

				//test anonymous/nameless api
				Assert.AreEqual(client.RemoteCall<string>("getwork", "rad", "ical"), "RAD.ICAL");
				Assert.AreEqual(client.RemoteCall<string>("getmorework", "Abs", "Olut"), "ABS.OLUT");

				//Test batch calls
				classMemberTest.classMember.TestValue = 156;
				ApiBatchCallDescriptor batch = new ApiBatchCallDescriptor();
				batch.Call<int>("ClassMember.addint", 11, -1);
                batch.Call<int>("ClassMember.AddValue", 11, 2);
				batch.Call<uint>("ClassMember.AddUInt", 5, 5);
				batch.Call<float>("ClassMember.AddFloat", 198.0099999, 0.0000001);
				batch.Call<double>("ClassMember.AddDouble", (double)float.MaxValue, ((double)float.MaxValue) / 2);
				batch.Call<string>("api.addstrings", "ham", "burger");
				batch.Call<uint>("api.Add2Diff", 199, -9);
				batch.Call("ClassMember.ExplicitArguments", 877);
				batch.Call<object[][]>("array.ToArrayOfArrayOfIntAndStrings", 13, "Bob");
				batch.Call<float[]>("array.AddArrayFloat", 10.5, new float[] { 1, 3, 7, 9 });
				batch.Call<TestObject[]>("ClassMember.GetTestObjectArray", new TestObject { iVal = 199, fArray = new float[] { 8 }, sVal = "allo" });
				object[] batchResult = client.RemoteCall(batch);
				//eval
				Assert.AreEqual(batchResult[0], 10);
				Assert.AreEqual(batchResult[1], 13);
				Assert.AreEqual(batchResult[2], 10);
				Assert.AreEqual(batchResult[3], (float)198.01);
				Assert.AreEqual(batchResult[4], (((double)float.MaxValue) + ((double)float.MaxValue) / 2)  );
				Assert.AreEqual(batchResult[5], "hamburger");
				Assert.AreEqual(batchResult[6], 190);
				Assert.AreEqual(classMemberTest.classMember.TestValue, 877);
				Assert.AreEqual(batchResult[8], new object[][] { new object[] { 14, "Bob" }, new object[] { "Bob", 15 } });
				Assert.AreEqual(batchResult[9], new float[] { 11.5f, 13.5f, 17.5f, 19.5f });
				var objArray = (TestObject[])batchResult[10];
				Assert.AreEqual(objArray[0].iVal, 199 + 1);
				Assert.AreEqual(objArray[0].fArray, new float[] { 8, 1 });
				Assert.AreEqual(objArray[0].sVal, "allo1");
				Assert.AreEqual(objArray[1].iVal, 199 + 2);
				Assert.AreEqual(objArray[1].fArray, new float[] { 8, 2 });
				Assert.AreEqual(objArray[1].sVal, "allo2");


				//Test "Arguments exception cought : {ex.ToString()}"  / Wrong argument counts 
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.AddValue.");
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.NoArgs.");

				//Test "The method {methodName}does not exist"
				Assert.AreEqual(TestRpcException<string>(client, "_VOID_", new object[] { 1 }), "RPC error -2: The method _VOID_ does not exist.");
				//Test "Wrong argument type in {methodName}. Arguments are {sig}"
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { "s", "s" }), "RPC error -5: Wrong argument type in method ClassMember.AddValue.");
				//Test "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method"
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { }), "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method");
			
				//Test BATCH "Arguments exception cought : {ex.ToString()}"  / Wrong argument counts 
				Assert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.AddValue", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.AddValue.");
				Assert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.NoArgs", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.NoArgs.");

				//Test BATCH "The method {methodName}does not exist"
				Assert.AreEqual(TestRpcExceptionBatch(client, "_VOID_", new object[] { 1 }), "RPC error -2: The method _VOID_ does not exist.");
				//Test BATCH "Wrong argument type in {methodName}. Arguments are {sig}"
				Assert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.AddValue", new object[] { "s", "s" }), "RPC error -5: Wrong argument type in method ClassMember.AddValue.");
				//Test BATCH "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method"
				Assert.AreEqual(TestRpcExceptionBatch(client, "ClassMember.NoArgs", new object[] { }), "null");


			} catch (Exception e) {
				Assert.Fail(e.ToString());
			}

			//close everything
			server.Stop();
			ApiServiceManager.UnregisterService(anonymousAPI1);
			ApiServiceManager.UnregisterService(anonymousAPI2);
			ApiServiceManager.UnregisterService(classMemberTest.classMember);
			ApiServiceManager.UnregisterService(apiTest);
			ApiServiceManager.UnregisterService(arrayClass);
			ApiServiceManager.UnregisterService("");
		}


		[Test]
		public void TestTcpSecurityPoliciy_TooManyConnections()
		{
			const int TestMaxConnections = 8;
			const int TestMaxThreads = 32;

			// Server side
			var apiTest = new TestApi();
			JsonRpcServer server = null;
			try
			{
				//Register various apis
				ApiServiceManager.RegisterService(apiTest);
				//Start server()
				TcpSecurityPolicies.MaxSimultanousConnecitons = TestMaxConnections;
				server = new JsonRpcServer(new TcpEndPointListener(true, 27000, 5));
				server.Start();
				Thread.Sleep(250);
			}
			catch (Exception e)
			{
				Assert.Fail(e.ToString());
			}

			//----------------------------------------------------
			// Client side
			try
			{
				int failed = 0;
				Thread[] threadsArray = new Thread[TestMaxThreads];
				for (int i = 0; i < TestMaxThreads; i++)
					threadsArray[i] = new Thread((i) =>
					{
						Thread.CurrentThread.Name = $"test{i}";
						try { 
							var client = new JsonRpcClient(new TcpEndPoint("127.0.0.1", 27000));
							Assert.AreEqual(client.RemoteCall<uint>("api.Add2Diff", 199, -9), 190);
						}
						catch (Exception e) {
							Interlocked.Increment(ref failed);
						}
					});
				for (int i = 0; i < TestMaxThreads; i++)
					threadsArray[i].Start(i);
				for (int i = 0; i < TestMaxThreads; i++)
					threadsArray[i].Join();

				//no need for a big number here. On fast computer, it can be very low. 1 is enought to tell the policy works.
				Assert.IsTrue(failed > 2); 
			}
			catch (Exception e) {
				Assert.Fail(e.ToString());
			}

			//close everything
			server.Stop();
			ApiServiceManager.UnregisterService(apiTest);
		}
	}

}
