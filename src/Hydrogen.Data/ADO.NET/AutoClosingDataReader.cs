// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


using System.Data;

namespace Hydrogen.Data;

public class AutoClosingDataReader : DataReaderDecorator {
	protected readonly IDbCommand InternalCommand;

	public AutoClosingDataReader(IDataReader dataReader, IDbCommand command) : base(dataReader) {
		InternalCommand = command;

	}

	public override void Dispose() {
		if (!InternalReader.IsClosed) {
			Tools.Exceptions.ExecuteIgnoringException(InternalReader.Close);
			InternalReader.Dispose();
		}

		if (InternalCommand != null) {
			Tools.Exceptions.ExecuteIgnoringException(InternalCommand.Dispose);
		}

	}


}
