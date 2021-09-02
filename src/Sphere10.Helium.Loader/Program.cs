using System;
using System.Collections.Generic;
using Sphere10.Framework;
using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Message;
using Sphere10.Helium.PluginFramework;
using Sphere10.Helium.Router;
using Sphere10.Helium.TestPlugin1;

namespace Sphere10.Helium.Loader {
	/// <summary>
	/// ********
	/// THE NODE
	/// ********
	/// 
	/// IMPORTANT: This simulates the Sphere10 node and shows how the HeliumFramework and HeliumPluginLoader are used.
	/// This Project will be deleted once Helium is integrated into Hydrogen and Helium runs within the Node.
	///
	/// IMPORTANT: Design Decisions
	/// An assembly (dll, Project consisting of Handlers and Sagas) is a Plugin.
	/// 1) ALL Plugins with relative paths listed in the pluginsRelativePathArray will be loaded and will be enabled when the Node starts up.
	///    enabled in this context means: ALL handlers, Timeouts etc of the Plugin will work as per normal.
	/// 2) If you DON'T want a Plugin to load at start-up then remove the Plug-in's relative path from the pluginsRelativePathArray.
	/// 3) Any Plugin can be disabled that is: PluginAssemblyHandlerDto.IsEnabled = false at any time while the Node is running.
	/// 4) Any Plugin that was disabled can be enabled at any time while the Node is running.
	/// </summary>
	public class Program {
		private static IRouter _router; //This will be made public when integrated into Sphere10.Framework
		private static int _messageNumber;
		private const int MessageCount = 7;

		public static void Main(string[] args) {
			var logger = new ConsoleLogger();
			var pluginsRelativePathArray = GetPluginsToBeLoadedList();
			var endpointSettings = new EndPointSettings {
				FlushLocalQueueOnStartup = true,
				FlushPrivateQueueOnStartup = true
			};

			IHeliumPluginLoader heliumPluginLoader = new HeliumPluginLoader(logger);
			heliumPluginLoader.LoadPlugins(pluginsRelativePathArray);

			var heliumFramework = heliumPluginLoader.GetHeliumFramework();
			heliumFramework.ModeOfOperation = EnumModeOfOperationType.HydrogenMode;
			heliumFramework.StartHeliumFramework(endpointSettings);
			heliumFramework.LoadHandlerTypes(heliumPluginLoader.PluginAssemblyHandlerList);
			_router = heliumFramework.Router;

			Console.WriteLine("======================================");
			Console.WriteLine("Send messages to Helium");
			Console.WriteLine("To exit, press Ctrl + C");
			Console.WriteLine("A) To send a single message: Alt A.");
			Console.WriteLine($"B) To send a List of {MessageCount} messages synchronously : Alt B.");
			Console.WriteLine("======================================");

			ConsoleKeyInfo cki;
			do {
				cki = Console.ReadKey();

				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'a' || cki.KeyChar == 'A')) {
					Console.WriteLine("-----------------------------------------------------------------------------------");
					Console.WriteLine(" Sending a single message now.");
					SendSingleMessageToLocalQueue();
					Console.WriteLine("-----------------------------------------------------------------------------------");
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'b' || cki.KeyChar == 'B')) {
					Console.WriteLine("-----------------------------------------------------------------------------------");
					Console.WriteLine($" Sending {MessageCount} messages now.");
					SendListOfMessagesToLocalQueue();
					Console.WriteLine("-----------------------------------------------------------------------------------");
				}
				if ((cki.Modifiers & ConsoleModifiers.Alt) != 0 && (cki.KeyChar == 'c' || cki.KeyChar == 'C')) {

				}

			} while (cki.Key != ConsoleKey.Escape);
		}

		private static string[] GetPluginsToBeLoadedList() {
			return new[] {
					@"Sphere10.Helium.TestPlugin1\bin\Debug\net5.0\Sphere10.Helium.TestPlugin1.dll",
					@"Sphere10.Helium.TestPlugin2\bin\Debug\net5.0\Sphere10.Helium.TestPlugin2.dll",
					@"Sphere10.Helium.TestPlugin3\bin\Debug\net5.0\Sphere10.Helium.TestPlugin3.dll"
			};
		}

		private static void SendSingleMessageToLocalQueue() {
			var message = CreateTestMessage();

			_router.InputMessage(message);
		}

		private static void SendListOfMessagesToLocalQueue() {

			var messageList = new List<IMessage>();

			for (var i = 0; i < MessageCount; i++) {
				var message = CreateTestMessage();
				messageList.Add(message);
			}

			_router.InputMessageList(messageList);
		}

		private static IMessage CreateTestMessage() {
			var message = new BlueHandlerMessage {
				Id = Guid.NewGuid().ToString(),
				StartDateTime = DateTime.Now.Ticks,
				MessageNumber = _messageNumber++,
				MessageName = nameof(BlueHandlerMessage)
			};

			return message;
		}
	}
}