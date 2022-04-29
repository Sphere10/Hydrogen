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
using Sphere10.Framework;

namespace Sphere10.Framework.Windows {
    public class EventLogLogger : ILogger {
        private System.Diagnostics.EventLog _eventLog;
        private readonly string _source;
        private readonly string _logName;



        public EventLogLogger(string sourceName, string logName = "Application") {
            Options = LogOptions.DebugBuildDefaults;
            _source = sourceName;
            _logName = logName;
            if (!EventLog.SourceExists(_source)) {
                EventLog.CreateEventSource(new EventSourceCreationData(_source, _logName));
            }
        }

        public LogOptions Options { get; set; }
        public void Debug(string message) {
            if (Options.HasFlag(LogOptions.DebugEnabled)) {
                EventLog.WriteEntry(_source, "DEBUG: " + message, EventLogEntryType.Information);   
            }
        }

        public void Info(string message) {
            if (Options.HasFlag(LogOptions.InfoEnabled)) {
                EventLog.WriteEntry(_source, message, EventLogEntryType.Information);
            }
        }

        public void Warning(string message) {
            if (Options.HasFlag(LogOptions.WarningEnabled)) {
                EventLog.WriteEntry(_source, message, EventLogEntryType.Warning);
            }
        }

        public void Error(string message) {
            if (Options.HasFlag(LogOptions.ErrorEnabled)) {
                EventLog.WriteEntry(_source, message, EventLogEntryType.Error);
            }
        }
    }
}
