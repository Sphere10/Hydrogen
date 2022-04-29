using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Communications.RPC {

	public class TooManyConnectionsException : IllegalValueException {
		public TooManyConnectionsException(string message) : base(message) { }
	}

	//Implement basic security poplicies for TCP connections
	public class TcpSecurityPolicies {
		//Policiies configuration.
		//TODO: fetch from global config or general config file.
		public static int MaxSimultanousConnecitons = 200;
		public static int MaxSpoofStringPerClient = 5;
		public static int MaxSpoofStringPerClientOccurencesBeforeTotalBan = 3;
		public static int MaxClientConnectionPerSecond = 120;
		public static int MaxClientConnectionPerSecondOccurencesBeforeTotalBan = 3;

		//Handle global MaxConnectionCount
		private static int _connectionCount = 0;
		public enum MaxConnecitonPolicy { ConnectionOpen, ConnectionClose}
		//TODO: use ent/delegates on TcpClients
		static public void ValidateConnectionCount(MaxConnecitonPolicy policyAction) {
			int count = 0;
			if (policyAction == MaxConnecitonPolicy.ConnectionOpen)
				count = Interlocked.Increment(ref _connectionCount);
			else
				count = Interlocked.Decrement(ref _connectionCount);

			if (count > MaxSimultanousConnecitons)
				throw new TooManyConnectionsException($"Maximum of {MaxSimultanousConnecitons} peer reached");
		}

		//handle ip ban
		public enum AttackType { MessageSpoof, ConnectionFlod }
		static public void MonitorPotentialAttack(AttackType attack, IEndPoint client) {
			//TODO: implement policy for too many spoof and flood attack
		}

		//Remove all control characters
		static public byte[] RemoveControlCharacters(byte[] bytes, int bytesRead) {
			return Array.FindAll(bytes, c => c >= 32 );
		}

		//Detect spoofed buffer attack.
		static public void ValidateJsonQuality(byte[] bytes, int bytesRead) {
			for (int i = 0; i < bytesRead; i++) {
				byte c = bytes[i];
				//allowed control chars
				if (c == 9 || c == 10 || c == 13)
					continue;
				//illegal control chars
				if (c < 32)
					throw new IllegalValueException("Json text contain illegal characters");
			}
		}


	}
}