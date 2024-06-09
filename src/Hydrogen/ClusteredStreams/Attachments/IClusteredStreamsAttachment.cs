// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// An auxillary component that attaches to a reserved stream within a <see cref="ClusteredStreams"/>.
/// </summary>
public interface IClusteredStreamsAttachment {

	public string AttachmentID { get; }

	ClusteredStreams Streams { get; }

	bool IsAttached { get; }

	void Attach();

	void Detach();

	void Flush();
}
