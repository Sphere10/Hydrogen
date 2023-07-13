// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data.Exceptions;

public class NoSingleRecordException : SoftwareException {
	private const string UnknownFound = "Expected a single '{0}', found {1}";
	private const string NoneFound = "No matching record was found in '{0}'";
	private const string TooManyFound = "Too many matching records were found in '{0}'";

	private const string UnknownFoundID = "Expected a single record with ID '{0}' in '{1}', found {2}";
	private const string NoneFoundID = "No record with ID '{0}' was found in '{1}'";
	private const string TooManyFoundID = "Too many records with ID '{0}' were found in '{1}'";

	public NoSingleRecordException(string collectionName, long recordCount)
		: base(PickMessage(collectionName, recordCount)) {
	}

	public NoSingleRecordException(string collectionName, object requestedID, long recordCount) : base(PickMessageWithID(collectionName, requestedID, recordCount)) {
	}

	private static string PickMessage(string collectionName, long recordCount) {
		if (recordCount < 0)
			return UnknownFound.FormatWith(collectionName, recordCount);
		else if (recordCount == 0)
			return NoneFound.FormatWith(collectionName);
		else
			return TooManyFound.FormatWith(collectionName);
	}

	private static string PickMessageWithID(string collectionName, object requestedID, long recordCount) {
		if (recordCount < 0)
			return UnknownFoundID.FormatWith(requestedID, collectionName, recordCount);
		else if (recordCount == 0)
			return NoneFoundID.FormatWith(requestedID, collectionName);
		else
			return TooManyFoundID.FormatWith(requestedID, collectionName);
	}
}
