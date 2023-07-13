// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Web.AspNetCore;

/// <summary>
/// Describes a message to a user. Currently used within the MVC pipeline (BPM) to pass warning messages in TransferResults.
/// </summary>
public sealed class UserMessage {
	public string Message { get; set; }
	public UserMessageType MessageType { get; set; }

	public UserMessage() {
		Message = string.Empty;
		MessageType = UserMessageType.Information;
	}

	public UserMessage(string message, UserMessageType messageType) {
		Message = message;
		MessageType = messageType;
	}

}
