// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Text;

namespace Hydrogen;

public class EncryptedStringAttribute : Attribute {
	public const string DefaultPepper = "00000000";

	public static string ApplicationSharedSecret = null;

	public EncryptedStringAttribute() {
		Policy = EncryptionSaltPolicy.Custom;
		Pepper = DefaultPepper;
	}

	public EncryptionSaltPolicy Policy { get; set; }

	public string Pepper { get; set; }

	public virtual string Encrypt(string value) {
		CheckSecret();
		if (value == null)
			return null;
		var salt = CalculateSalt() ?? string.Empty;
		var pepper = Pepper ?? string.Empty;
		return Tools.Crypto.EncryptStringAES(value, ApplicationSharedSecret, salt + pepper);
	}

	public virtual string Decrypt(string value) {
		CheckSecret();
		if (value == null)
			return null;
		var salt = CalculateSalt() ?? string.Empty;
		var pepper = Pepper ?? string.Empty;
		return Tools.Crypto.DecryptStringAES(value, ApplicationSharedSecret, salt + pepper);
	}

	protected string CalculateSalt() {
		var salt = new StringBuilder();

		if (Policy.HasFlag(EncryptionSaltPolicy.None))
			salt.Append(string.Empty);

		if (Policy.HasFlag(EncryptionSaltPolicy.MACAddress)) {
			salt.Append(GetMacAddress());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.ExecutableFileCreationTime)) {
			salt.Append(GetExecutableFileCreationTime());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.MachineName)) {
			salt.Append(GetMachineName());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.UserName)) {
			salt.Append(GetUserName());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.OperatingSystemVersion)) {
			salt.Append(GetOperatingSystemVersion());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.ProcessorCount)) {
			salt.Append(GetProcessorCount());
		}

		if (Policy.HasFlag(EncryptionSaltPolicy.Custom)) {
			salt.Append(GetCustomSaltValue());
		}

		return salt.ToString();
	}

	protected virtual string GetMacAddress() {
		return Tools.Network.GetMacAddressOrDefault();
	}

	protected virtual string GetExecutableFileCreationTime() {
		return string.Format("{0:yyyy-MM-dd HH:mm:ss fff}", File.GetCreationTimeUtc(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
	}

	protected virtual string GetMachineName() {
		return Environment.MachineName;
	}

	protected virtual string GetUserName() {
		return Environment.UserName;
	}

	protected virtual string GetOperatingSystemVersion() {
		return Environment.OSVersion.ToString();
	}

	protected virtual string GetProcessorCount() {
		return Environment.ProcessorCount.ToString();
	}

	protected virtual string GetCustomSaltValue() {
		return Pepper;
	}

	private void CheckSecret() {
		if (string.IsNullOrEmpty(ApplicationSharedSecret))
			throw new InvalidOperationException("Application secret was not set");
	}

}
