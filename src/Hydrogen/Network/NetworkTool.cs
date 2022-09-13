//-----------------------------------------------------------------------
// <copyright file="NetworkTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Net.NetworkInformation;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Hydrogen;
using System;

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Network {

		public static string GetMimeType(string filePathOrExt)
			=> TryGetMimeType(filePathOrExt, out var mimeType) ? mimeType : throw new InvalidOperationException($"Unable to determine mimetype for {filePathOrExt}");

		public static bool TryGetMimeType(string filePathOrExt, out string mimeType)
			=> MimeHelper.TryGetMimeTypeFromFileExt(filePathOrExt, out mimeType);
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
		/// Finds the MAC address of the NIC with maximum speed.
		/// </summary>
		/// <returns>The MAC address.</returns>
		public static string GetMacAddress() {
			// HS 2019-02-26: Android specifics refactored out in NET STANDARD 2
			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
				switch (nic.NetworkInterfaceType) {
					case NetworkInterfaceType.Loopback:
					case NetworkInterfaceType.Tunnel:
					case NetworkInterfaceType.Unknown:
						continue;
				}

				foreach (var addrInfo in nic.GetIPProperties().UnicastAddresses) {
					if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork) {
						var ipAddress = addrInfo.Address; // use ipAddress as needed ... 
						return ToMacAddressString(nic.GetPhysicalAddress().GetAddressBytes());
					}
				}
			}

			return null;
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

}


