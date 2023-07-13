// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace AbstractProtocol.AnonymousPipeComplex;

public enum AppProtocolMessageType {
	Sync,
	Ack,
	Verack,
	Ping,
	Pong,
	RequestListFolder,
	FileDescriptor,
	DirectoryDescriptor,
	FolderContents,
	RequestFilePart,
	FilePart,
	NotifyLayer2Message,
	NotifyNewBlock,
	NotifyNewTransaction,
}
