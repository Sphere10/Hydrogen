using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
