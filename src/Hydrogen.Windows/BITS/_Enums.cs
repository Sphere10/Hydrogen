// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.BITS;

public enum JobOwner {
	CurrentUser = 0,
	AllUsers = 1,
}


public enum JobPriority {
	ForeGround = 0,
	High = 1,
	Normal = 2,
	Low = 3,
}


public enum JobState {
	Queued = 0,
	Connecting = 1,
	Transferring = 2,
	Suspended = 3,
	Error = 4,
	TransientError = 5,
	Transferred = 6,
	Acknowledged = 7,
	Cancelled = 8,
}


public enum JobType {
	Download,
	Upload,
	UploadReply,
	Unknown, // not available in BITS API     
}


public enum ProxyUsage {
	Preconfig,
	NoProxy,
	Override,
	AutoDetect,
}


public enum ErrorContext {
	None = 0,
	Unknown = 1,
	General_Queue_Manager = 2,
	Queue_Manager_Notification = 3,
	Local_File = 4,
	Remote_File = 5,
	General_Transport = 6,
	Remote_Application = 7,
}


/// <summary>
/// The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used for proxy or server user authentication requests.
/// </summary>
public enum AuthenticationTarget {
	/// <summary>
	/// Use credentials for server requests.
	/// </summary>
	Server = 1,

	/// <summary>
	/// Use credentials for proxy requests. 
	/// </summary>
	Proxy = 2,
}


/// <summary>
/// The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to use when a proxy or server requests user authentication.
/// </summary>
public enum AuthenticationScheme {
	/// <summary>
	/// Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.
	/// </summary>
	Basic = 1,

	/// <summary>
	/// Digest is a challenge-response scheme that uses a server-specified data string for the challenge. 
	/// </summary>
	Digest,

	/// <summary>
	/// Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the 
	/// user for authentication in a Windows network environment. 
	/// </summary>
	Ntlm,

	/// <summary>
	/// Simple and Protected Negotiation protocol (Snego) is a challenge-response scheme that negotiates 
	/// with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol and NTLM
	/// </summary>
	Negotiate,

	/// <summary>
	/// Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites. 
	/// </summary>
	Passport
}


[Flags()]
public enum NotificationFlags {
	/// <summary>
	/// All of the files in the job have been transferred.
	/// </summary>
	JobTransferred = 1,

	/// <summary>
	/// An error has occurred
	/// </summary>
	JobErrorOccured = 2,

	/// <summary>
	/// Event notification is disabled. BITS ignores the other flags.
	/// </summary>
	NotificationDisabled = 4,

	/// <summary>
	/// The job has been modified. For example, a property value changed, the state of the job changed, or progress is made transferring the files.
	/// </summary>
	JobModified = 8,
}
