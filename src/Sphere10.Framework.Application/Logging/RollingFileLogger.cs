//-----------------------------------------------------------------------
// <copyright file="RollingFileLogger.cs" company="Sphere 10 Software">
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

using System.Configuration;
using System.IO;
using Sphere10.Framework;

namespace Sphere10.Framework.Application {
    public class RollingFileLogger : DecoratedLogger {

	    public RollingFileLogger() : base(CreateLogger("Logging")) {		    
	    }

	    public static ILogger CreateLogger(string sectionName) {
		    var section = ConfigurationManager.GetSection("LoggingConfiguration") as LoggingConfiguration;
		    if (section == null) return new NoOpLogger();
		    var logger = new FileAppendLogger(Path.Combine(section.Directory, section.ApplicationName + ".log"), true) {Options = 0};
		    if (section.EnableDebug)
				logger.Options = logger.Options | LogOptions.DebugEnabled;
			if (section.EnableInfo)
			    logger.Options = logger.Options | LogOptions.InfoEnabled;
			if (section.EnableWarning)
				logger.Options = logger.Options | LogOptions.WarningEnabled;
			if (section.EnableError)
				logger.Options = logger.Options | LogOptions.ErrorEnabled;
		    return logger;
	    }
	}
}
