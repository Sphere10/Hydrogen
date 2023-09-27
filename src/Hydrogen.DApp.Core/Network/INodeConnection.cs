// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VelocityNET.Network;

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
