// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// An auxillary component that attaches to a reserved stream within a <see cref="StreamContainer"/>.
/// </summary>
public interface IStreamContainerAttachment {
	StreamContainer Streams { get; }

	long ReservedStreamIndex { get; }

	bool IsAttached { get; }

	void Attach();

	void Detach();
}
