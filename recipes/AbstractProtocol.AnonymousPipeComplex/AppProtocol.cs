// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System;
using System.IO;
using System.Linq;
using Hydrogen.Communications;

namespace AbstractProtocol.AnonymousPipeComplex;

public class AppProtocol {
	public static Protocol Build() {
		return new ProtocolBuilder()
			.ConfigureHandshake(hb => hb
				.UseThreeWay()
				.InitiatedBy(CommunicationRole.Client)
				.HandleWith<Sync, Ack, Verack>(InitiateHandshake, ReceiveHandshake, VerifyHandshake, AcknowledgeHandshake)
			)
			.AddRequestResponse<Ping, Pong>(Ping, HandlePong)
			.AddRequestResponse<RequestListFolder, FolderContents>(ListFolder, PrintFolderContents)
			.ConfigureRequest<RequestFilePart>(rb => rb.HandleRequestWith(GetFilePart).HandleResponseWith(SaveFilePart))
			.AddCommand<NotifyNewTransaction>(HandleNewTransaction)
			.AddCommand<NotifyNewBlock>(HandleNewBlock)
			.ConfigureCommand<NotifyLayer2Message>(cb => cb.HandleWith(HandleNewLayer2Message))
			.ConfigureSerialization( sf => sf
				.SetMinTypeCode(1000)
				.RegisterAutoBuild<Ping>()
				.RegisterAutoBuild<Pong>()
			)
			.AutoBuildSerializers()
			.Build();
	}

	private static Sync InitiateHandshake(ProtocolOrchestrator orchestrator) {
		// can populate details inside sync
		return new Sync {
			ClientID = "client name",
			Timestamp = DateTime.UtcNow
		};
	}

	private static HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, Sync handshake, out Ack ack) {
		// here we handle the handshake on server, we can check handshake for compliance
		ack = new Ack {
			ServerID = "server name",
			Timestamp = DateTime.UtcNow
		};

		return HandshakeOutcome.Accepted;
	}

	private static HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, Sync handshake, Ack response,
	                                                out Verack verack) {
		// here the client
		// can finalize the handshake (for 2-way)
		verack = new Verack();
		return HandshakeOutcome.Accepted;
	}


	private static bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, Sync handshake, Ack response,
	                                         Verack verack) {
		// unncessary
		return true;
	}


	private static void HandlePong(ProtocolOrchestrator orchestrator, Ping request, Pong response) {
		SystemLog.Info($"Handled Ping: Request: {request}, Respose {response} ");
	}

	private static FolderContents ListFolder(ProtocolOrchestrator orchestrator, RequestListFolder request) {
		SystemLog.Info($"Handled ListFolder: OriginalRequest - {request}");
		var files = Directory.GetFiles(request.Folder).Select(f => new FileDescriptor(new FileInfo(f)))
			.Cast<Descriptor>();
		var subDirs = Directory.GetDirectories(request.Folder)
			.Select(d => new DirectoryDescriptor(new DirectoryInfo(d))).Cast<Descriptor>();
		return new FolderContents {
			Items = files.Union(subDirs).ToArray()
		};
	}

	private static void PrintFolderContents(ProtocolOrchestrator orchestrator, RequestListFolder request,
	                                        FolderContents response) {
		SystemLog.Info($"PrintFolderContents: Request: {request}, Respose {response} ");
	}

	private static void GetFilePart(ProtocolOrchestrator orchestrator, RequestListFolder request,
	                                FolderContents response) {
		SystemLog.Info($"Handled GetFilePart: OriginalRequest - {request} Response - {response}");
	}

	private static FilePart GetFilePart(ProtocolOrchestrator orchestrator, RequestFilePart request) {
		SystemLog.Info($"Handled GetFilePart: OriginalRequest - {request}");
		return new FilePart {
			Data = Tools.FileSystem.GetFilePart(request.Filename, request.Offset, request.Length)
		};
	}

	private static void SaveFilePart(ProtocolOrchestrator orchestrator, RequestFilePart request, FilePart response) {
		SystemLog.Info($"Handled SaveFilePart: OriginalRequest - {request} Response - {response}");
	}

	private static Pong Ping(ProtocolOrchestrator orchestrator, Ping ping) {
		SystemLog.Info("Handled Ping");
		return new Pong();
	}

	private static void HandleNewTransaction(ProtocolOrchestrator orchestrator, NotifyNewTransaction message) {
		SystemLog.Info("Handled NewTransaction");
	}

	private static void HandleNewBlock(ProtocolOrchestrator orchestrator, NotifyNewBlock message) {
		SystemLog.Info("Handled NewBlock");
	}

	private static void HandleNewLayer2Message(ProtocolOrchestrator orchestrator, NotifyLayer2Message message) {
		SystemLog.Info("Handled Layer2Message");
	}
}
