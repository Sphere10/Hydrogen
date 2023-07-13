// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

public class LoggingConfiguration {
	public string LogFilePath { get; set; }
	public int MaxLogFiles { get; set; }
	public int MaxLogFileSize { get; set; }
	public bool EnableDebug { get; set; }
	public bool EnableInfo { get; set; }
	public bool EnableWarning { get; set; }
	public bool EnableError { get; set; }
}
