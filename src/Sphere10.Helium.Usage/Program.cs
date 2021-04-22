using System;
using System.IO;
using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Message;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Usage {
	public class Program {

		public static void Main(string[] args) {
			Console.WriteLine("Hello World!");

			var tempQueueName = "Temp_AB3CB3F9-3EBC-46B3-877D-14AB5A7A7FD2_1";
			var localQueueName = "Local_6B86A05B-9F84-4358-9284-3E56E601B2C6_1";

			var strGuid = "997D1367-E7B0-46F0-B0A1-686DC0F15945";
			var sameGuid = new Guid(strGuid);

			var message = new TestMessage1 {
				Id = strGuid,
				Aa1 = "Hello please work! 1",
				Aa2 = "Hello please work! 2",
				Aa3 = "Hello please work! 3",
				Aa4 = "Hello please work! 4"
			};

			var queueTempPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "a");
			if (!Directory.Exists(queueTempPath))
				Directory.CreateDirectory(queueTempPath);

			var queueConfig = new QueueConfigDto {
				Path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), tempQueueName),
				TempDirPath = queueTempPath,
				FileId = sameGuid,
				TransactionalPageSizeBytes = 1 << 17, /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
				MaxStorageSizeBytes = 1 << 21, /*2097152 ~ 2MB*/
				FileMemoryCacheBytes = 1 << 20, /*1048576 ~ 1MB*/
				ClusterSize = 1 << 9, /*132071 ~ 128 KB TODO: 512 bytes*/
				MaxItems = 5,
				ReadOnly = false
			};

			var localQueue = new LocalQueue(queueConfig); //10 params (TransactionalList) -1 for serializer  = 9 params
			if (localQueue.RequiresLoad)
				localQueue.Load();

			localQueue.AddMessageToQueue(message);

			Console.WriteLine("END!");
		}

		//public static void Main(string[] args) {
		//	var message = new TestMessage1 { FirstName = "stuff", Id = "1234" };

		//	var config = new BusConfiguration {
		//		EndpointType = EnumEndpointType.SendAndForget,
		//		IsPersisted = false
		//	};

		//	var bus = new BusSetup().Create(config);

		//	bus.SendAndForget("FarAway", message);
		//}
	}

	[Serializable]
	public record TestMessage1 : IMessage {
		public string Id { get; set; }
		public string Aa1 { get; init; }
		public string Aa2 { get; init; }
		public string Aa3 { get; init; }
		public string Aa4 { get; init; }
	}

	//public record TestMessage1 : IMessage {
	//	public string FirstName { get; init; }
	//	public string Id { get; set; }
	//}
}
