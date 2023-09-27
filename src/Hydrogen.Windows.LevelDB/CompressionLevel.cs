// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.LevelDB;

/// <summary>
/// DB contents are stored in a set of blocks, each of which holds a
/// sequence of key,value pairs.  Each block may be compressed before
/// being stored in a file. The following enum describes which
/// compression method (if any) is used to compress a block.
/// </summary>
public enum CompressionLevel {
	NoCompression = 0,
	SnappyCompression = 1
}
