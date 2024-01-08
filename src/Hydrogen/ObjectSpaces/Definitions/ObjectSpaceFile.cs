// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.ObjectSpaces;

public class ObjectSpaceFile {
	public string FilePath { get; set; }

	public string PageFileDir { get; set; }

	public long PageSize { get; set; }

	public long MaxMemory { get; set; }

	public long ClusterSize { get; set; }

	public StreamContainerPolicy ContainerPolicy { get; set; }

}
