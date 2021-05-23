using System;
using System.Threading;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;

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

		[Test]
		public void TestServiceManager() {
			try {
				var anonymousAPI1 = new TestAnonymousApi1();
				var anonymousAPI2 = new TestAnonymousApi2();
				var classMemberTest = new TestClass();
				var apiTest = new TestApi();
				ApiServiceManager.RegisterService(anonymousAPI1);
				ApiServiceManager.RegisterService(anonymousAPI2);
				ApiServiceManager.RegisterService(classMemberTest.classMember);
				ApiServiceManager.RegisterService(apiTest);

				//test GetService
				Assert.AreNotEqual(ApiServiceManager.GetService("ClassMember"), null);
				Assert.AreNotEqual(ApiServiceManager.GetService("classmember"), null);
				Assert.AreEqual(ApiServiceManager.GetService("Api"), apiTest);
				Assert.AreEqual(ApiServiceManager.GetService("_VOID_"), null);

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

			} catch (Exception e) {
				Assert.Fail(e.ToString());
			}
		}

		[Test]
		public void TestClientCalls() {
			//----------------------------------------------------
			// Server side
			var anonymousAPI1 = new TestAnonymousApi1();
			var anonymousAPI2 = new TestAnonymousApi2();
			var classMemberTest = new TestClass();
			var apiTest = new TestApi();
			JsonRpcServer server = null;
			try {
				//Register various apis
				ApiServiceManager.RegisterService(anonymousAPI1);
				ApiServiceManager.RegisterService(anonymousAPI2);
				ApiServiceManager.RegisterService(classMemberTest.classMember);
				ApiServiceManager.RegisterService(apiTest);
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

				Assert.AreEqual(client.RemoteCall<float>("ClassMember.AddFloat", 198.0099999, 0.0000001), (float)198.01);
				Assert.AreEqual(client.RemoteCall<double>("ClassMember.AddDouble", (double)float.MaxValue, ((double)float.MaxValue) / 2), (((double)float.MaxValue) + ((double)float.MaxValue) / 2));
				Assert.AreEqual(client.RemoteCall<string>("api.addstrings", "ham", "burger"), "hamburger");
				Assert.AreEqual(client.RemoteCall<uint>("api.Add2Diff", 199, -9), 190);

				//Test normal rpc call
				Assert.AreEqual(client.RemoteCall<int>("ClassMember.addint", 11, -1), 10);
				Assert.AreEqual(client.RemoteCall<int>("ClassMember.AddValue", 11, 2), 13);
				Assert.AreEqual(client.RemoteCall<uint>("ClassMember.AddUInt", 5, 5), 10);

				//Test method with no return value and no args
				classMemberTest.classMember.TestValue = 7;
				client.RemoteCall("ClassMember.ExplicitArguments", 2);
				Assert.AreEqual(classMemberTest.classMember.TestValue, 2);
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

				//Test "Arguments exception cought : {ex.ToString()}"  / Wrong argument counts 
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.AddValue.");
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { 1, 1, 1 }), "RPC error -3: Wrong argument count in method ClassMember.NoArgs.");

				//Test "The method {methodName}does not exist"
				Assert.AreEqual(TestRpcException<string>(client, "_VOID_", new object[] { 1 }), "RPC error -2: The method _VOID_ does not exist.");
				//Test "Wrong argument type in {methodName}. Arguments are {sig}"
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.AddValue", new object[] { "s", "s" }), "RPC error -5: Wrong argument type in method ClassMember.AddValue.");
				//Test "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method"
				Assert.AreEqual(TestRpcException<int>(client, "ClassMember.NoArgs", new object[] { }), "RPC method ClassMember.NoArgs does not return a value. Use non-templated RemoteCall to call this method");

			} catch (Exception e) {
				Assert.Fail(e.ToString());
			}

			//close everything
			server.Stop();
			ApiServiceManager.UnregisterService(classMemberTest.classMember);
			ApiServiceManager.UnregisterService(apiTest);
			ApiServiceManager.UnregisterService("");
		}
	}
}