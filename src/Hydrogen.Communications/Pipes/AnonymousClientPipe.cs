// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO.Pipes;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public sealed class AnonymousClientPipe : AnonymousPipe {

	public AnonymousClientPipe(AnonymousPipeEndpoint endpoint) {
		Guard.ArgumentNotNull(endpoint, nameof(endpoint));
		this.Endpoint = endpoint;
	}

	public override CommunicationRole LocalRole => CommunicationRole.Client;

	protected override async Task<(AnonymousPipeEndpoint endpoint, PipeStream readStream, PipeStream writeStream)> OpenPipeInternal() {
		var readPipe = new AnonymousPipeClientStream(PipeDirection.In, Endpoint.WriterHandle);
		var writePipe = new AnonymousPipeClientStream(PipeDirection.Out, Endpoint.ReaderHandle);
		return (Endpoint, readPipe, writePipe);
	}

}
