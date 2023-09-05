// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen;

public static class HydrogenDefaults {

	// Streams
	public const int DefaultBufferOperationBlockSize = 32768; 
	public const int TransactionalPageBufferOperationBlockSize = 262144;
	public const int OptimalCompressWriteBlockSize = 8192;

	// Paged
	public const int MaxMemoryPerCollection = int.MaxValue; // Use all available memory


	// Transactional
	public const int TransactionalPageSize = 1 << 18; // 256kb

	// Clustered
	public const int ClusterSize = 256; // 256b
	public const long RecordCacheSize = 1 << 20; // 1mb


	// Serialization
	public const Endianness Endianness = Hydrogen.Endianness.LittleEndian;

}
