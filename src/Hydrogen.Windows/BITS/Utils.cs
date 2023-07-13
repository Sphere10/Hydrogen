// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Hydrogen.Windows.BITS;

class Utils {
	internal static string GetName(string SID) {
		const int size = 255;
		uint cbUserName = size;
		uint cbDomainName = size;
		IntPtr ptrSid;
		var userName = new StringBuilder(size);
		var domainName = new StringBuilder(size);
		if (WinAPI.ADVAPI32.ConvertStringSidToSidW(SID, out ptrSid)) {
			WinAPI.ADVAPI32.SidNameUse psUse;
			if (WinAPI.ADVAPI32.LookupAccountSidW(string.Empty, ptrSid, userName, ref cbUserName, domainName, ref cbDomainName, out psUse)) {
				return String.Format("{0}\\{1}", domainName, userName);
			}
		}
		return string.Empty;
	}

	internal static FILETIME DateTime2FileTime(DateTime dateTime) {
		long fileTime = 0;
		if (dateTime != DateTime.MinValue) //Checking for MinValue
			fileTime = dateTime.ToFileTimeUtc();
		var resultingFileTime = new FILETIME();
		resultingFileTime.dwLowDateTime = (uint)(fileTime & 0xFFFFFFFF);
		resultingFileTime.dwHighDateTime = (uint)(fileTime >> 32);
		return resultingFileTime;
	}

	internal static DateTime FileTime2DateTime(FILETIME fileTime) {
		if (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0) //Checking for MinValue
			return DateTime.MinValue;

		var dateTime = (((long)fileTime.dwHighDateTime) << 32) + fileTime.dwLowDateTime;
		return DateTime.FromFileTimeUtc(dateTime);
	}
}
