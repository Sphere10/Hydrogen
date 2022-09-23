//-----------------------------------------------------------------------
// <copyright file="ConfigurationException.cs" company="Sphere 10 Software">
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

namespace DevAge.Configuration
{
	[Serializable]
	public class ConfigurationException : DevAgeApplicationException
	{
		public ConfigurationException(string p_strErrDescription):
			base(p_strErrDescription)
		{
		}
		public ConfigurationException(string p_strErrDescription, Exception p_InnerException):
			base(p_strErrDescription, p_InnerException)
		{
		}
#if !MINI
		protected ConfigurationException(System.Runtime.Serialization.SerializationInfo p_Info, System.Runtime.Serialization.StreamingContext p_StreamingContext): 
			base(p_Info, p_StreamingContext)
		{
		}
#endif
	}
}
