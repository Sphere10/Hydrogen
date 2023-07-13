// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace DevAge.Configuration;

[Serializable]
public class ConfigurationException : DevAgeApplicationException {
	public ConfigurationException(string p_strErrDescription) :
		base(p_strErrDescription) {
	}
	public ConfigurationException(string p_strErrDescription, Exception p_InnerException) :
		base(p_strErrDescription, p_InnerException) {
	}
#if !MINI
	protected ConfigurationException(System.Runtime.Serialization.SerializationInfo p_Info, System.Runtime.Serialization.StreamingContext p_StreamingContext) :
		base(p_Info, p_StreamingContext) {
	}
#endif
}
