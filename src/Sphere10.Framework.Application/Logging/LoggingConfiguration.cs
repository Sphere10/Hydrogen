//-----------------------------------------------------------------------
// <copyright file="LoggingConfiguration.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Application {
	public class LoggingConfiguration {		
		public string Directory { get; set; }
		public string ApplicationName { get; set; }
		public int MaxLogFiles { get; set; }
		public int MaxLogFileSize { get; set; }
		public bool EnableDebug { get; set; }
		public bool EnableInfo { get; set; }
		public bool EnableWarning { get; set; }
		public bool EnableError { get; set; }
	}
}
