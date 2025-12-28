// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Projection index that enforces uniqueness of projected keys across items.
/// </summary>
public interface IUniqueProjectionIndex<TKey> : IClusteredStreamsAttachment {

	IReadOnlyDictionary<TKey, long> Values { get ; }
}
