using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VelocityNET.Network {

	public interface IPeer {
		IPAddress IP { get; }
		int Port { get; }
	}


	public interface IPeerConnection {
        event EventHandler<EventArgs> DataReceived;
        event EventHandler<EventArgs> DataSent;
        event EventHandler<EventArgs> Disconnected;
        IPeer Remote { get; }
		DateTime ConnectedOn { get; }
		DateTime LastSentOn { get; }
		DateTime LastReceivedOn { get; }
		long TotalBytesSent { get; }
		long TotalBytesReceived { get; }
		Task SendBytes(Stream stream);
	}
}
