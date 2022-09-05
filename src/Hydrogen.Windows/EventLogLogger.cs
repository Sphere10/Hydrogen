//-----------------------------------------------------------------------
// <copyright file="EventLogLogger.cs" company="Sphere 10 Software">
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen;

namespace Hydrogen.Windows {
    public class EventLogLogger : LoggerBase {
	    private readonly string _source;
        private readonly string _logName;

        public EventLogLogger(string sourceName, string logName = "Application") {
            Options = LogOptions.VerboseProfile;
            _source = sourceName;
            _logName = logName;
            if (!EventLog.SourceExists(_source)) {
                EventLog.CreateEventSource(new EventSourceCreationData(_source, _logName));
            }
        }

        protected override void Log(LogLevel logLevel, string message) {
	        var eventLogEntryType = logLevel switch {
		        LogLevel.Debug => EventLogEntryType.Information,
		        LogLevel.Info => EventLogEntryType.Information,
		        LogLevel.Warning => EventLogEntryType.Warning,
		        LogLevel.Error => EventLogEntryType.Error,
		        _ => throw new NotSupportedException($"{logLevel}")
	        };
			EventLog.WriteEntry(_source, message, eventLogEntryType);
        }
		

    }
}
