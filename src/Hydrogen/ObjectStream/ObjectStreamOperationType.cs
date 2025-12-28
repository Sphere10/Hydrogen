// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Identifies the type of operation performed against an <see cref="ObjectStream"/>.
/// </summary>
public enum ObjectStreamOperationType {
	/// <summary>
	/// Read an existing item without mutation.
	/// </summary>
	Read,
	/// <summary>
	/// Append a new item to the end of the stream.
	/// </summary>
	Add,
	/// <summary>
	/// Insert a new item at a specific position.
	/// </summary>
	Insert,
	/// <summary>
	/// Replace an existing item.
	/// </summary>
	Update,
	/// <summary>
	/// Delete an item (not supported by all APIs).
	/// </summary>
	Remove,
	/// <summary>
	/// Mark an item as reaped without removing its slot.
	/// </summary>
	Reap
}
