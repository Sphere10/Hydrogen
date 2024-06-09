// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Net.NetworkInformation;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Hydrogen;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Network {

	public static string GetMimeType(string filePathOrExt)
		=> TryGetMimeType(filePathOrExt, out var mimeType) ? mimeType : throw new InvalidOperationException($"Unable to determine mimetype for {filePathOrExt}");

	public static bool TryGetMimeType(string filePathOrExt, out string mimeType)
		=> MimeHelper.TryGetMimeTypeFromFileExt(filePathOrExt, out mimeType);

	// TODO: needs refactoring like GetMacAddressses below
	public static IPAddress GetNetworkAddress() {

		// HS 2019-02-26: Android specifics refactored out in NET STANDARD 2
		foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces()) {
			switch (netInterface.NetworkInterfaceType) {
				case NetworkInterfaceType.Loopback:
				case NetworkInterfaceType.Tunnel:
				case NetworkInterfaceType.Unknown:
					continue;
			}
			foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses) {
				if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
					var ipAddress = addrInfo.Address; // use ipAddress as needed ... 
					return ipAddress;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Finds the first alpha-numerically sorted MAC address out of all MAC Addresses (that aren't loopbacks, tunnels or unknown).
	/// </summary>
	/// <remarks> This can be used as system identifier and handles cases of disabling/enabling NIC's, adding new ones. Will return different only
	/// if NIC is removed.</remarks>
	/// <returns>The MAC address.</returns>
	public static string GetMacAddressOrDefault() => GetMacAddresses(false, true).OrderBy(x => x).FirstOrDefault();

	public static IEnumerable<string> GetMacAddresses(bool onlyRunning = false, bool ignoreInternalNics = true) {
		var nics = NetworkInterface.GetAllNetworkInterfaces().AsEnumerable();

		if (onlyRunning)
			nics = nics.Where(x => x.OperationalStatus == OperationalStatus.Up);
		if (ignoreInternalNics)
			nics = nics.Where(x => !x.NetworkInterfaceType.IsIn(NetworkInterfaceType.Loopback, NetworkInterfaceType.Tunnel, NetworkInterfaceType.Unknown));

		return nics
			.Select(x => x.GetPhysicalAddress().GetAddressBytes())
			.Select(ToMacAddressString).Distinct();
	}

	public static string ToMacAddressString(byte[] bytes) {
		var stringBuilder = new StringBuilder();
		for (int i = 0; i < bytes.Length; i++) {
			// Display the physical address in hexadecimal.
			stringBuilder.AppendFormat("{0}", bytes[i].ToString("X2"));
			// Insert a hyphen after each byte, unless we are at the end of the
			// address.
			if (i != bytes.Length - 1) {
				stringBuilder.AppendFormat("-");
			}
		}
		return stringBuilder.ToString();
	}

}
