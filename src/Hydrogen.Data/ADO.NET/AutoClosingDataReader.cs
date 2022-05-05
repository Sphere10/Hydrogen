//-----------------------------------------------------------------------
// <copyright file="AutoClosingDataReader.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Hydrogen.Data {
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
}
