// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Web.AspNetCore;

public class HydrogenLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider {

	public HydrogenLoggerProvider(Hydrogen.ILogger hydrogenLogger) {
		HydrogenLogger = hydrogenLogger;
	}

	protected ILogger HydrogenLogger { get; }

	public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
		=> new MicrosoftExtensionsLoggerAdapter(HydrogenLogger);

	public void Dispose() {
	}

}
