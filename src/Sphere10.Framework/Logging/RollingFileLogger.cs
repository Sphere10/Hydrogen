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

namespace Sphere10.Framework {

    /// <summary>
    /// Logs to a file then, after a paramterized byte size rolls over to a new log file, and so on. Keeps a limited number of files at any time. 
    /// </summary>
    public class RollingFileLogger : LoggerBase {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="directory">Base directory where log-files reside.</param>
        /// <param name="logFileNameTemplate">Template log filename used to generate log files (include # for number sequence).</param>
        /// <param name="maxFiles">Maximum number of files to keep in the directory. As new log files are created, older ones are deleted.</param>
        /// <param name="maxFileSize">The maximum byte-size of a log-file before further logging is rolled-over into a new log-file.</param>
        /// <remarks>A <paramref name="logFileNameTemplate"/> parameter value of "ImageStudio_####.log" would result in log-files with paths "%Directory%/ImageStudio_0001.log", "%Directory%/ImageStudio_0002.log", etc</remarks>
        public RollingFileLogger(string directory, string logFileNameTemplate, int maxFiles, int maxFileSize) {

        }

        /// <summary>
        /// Base directory where log files reside.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Template for log file
        /// </summary>
        /// <example>A <see cref="LogFileNameTemplate"/> value of "ImageStudio_####.log" would result in log-files with paths "%Directory%/ImageStudio_0001.log", "%Directory%/ImageStudio_0002.log", etc</example>
        public string LogFileNameTemplate { get; }

        /// <summary>
        /// Maximum number of files to keep in the directory. As new log files are created, older ones are deleted.
        /// </summary>
        /// <remarks>
        /// Maximum number is 9999.
        /// </remarks>
        public int MaxFiles { get;  }

        /// <summary>
        /// The maximum byte-size of a log file before further logging is rolled-over into a new log file.
        /// </summary>
        public int MaxFileSize { get; }

        protected override void Log(LogLevel logLevel, string message) {
            throw new System.NotImplementedException();
        }
    }
}
