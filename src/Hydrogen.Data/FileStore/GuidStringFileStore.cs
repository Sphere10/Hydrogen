// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

/// <summary>
/// Same as a <see cref="GuidFileStore"/> but uses string representation of Guids as the file key.
/// </summary>
public class GuidStringFileStore : KeyTransformedFileStore<Guid, string> {

	public GuidStringFileStore(string baseDirectory) :
		this(baseDirectory, (guid) => guid.ToString().Trim("{}".ToCharArray()), Guid.Parse) {
	}

	public GuidStringFileStore(string baseDirectory, Func<Guid, string> fromGuid, Func<string, Guid> toGuid, string fileExtension = null)
		: base(new GuidFileStore(baseDirectory) { FileExtension = fileExtension ?? string.Empty }, fromGuid, toGuid) {
	}

	public string FileExtension => ((GuidFileStore)InternalFileStore).FileExtension;


}
