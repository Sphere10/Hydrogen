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

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// Logs to a file then, after a parameterized byte size rolls over to a new log file, and so on. Keeps a limited number of files at any time. 
	/// </summary>
	public class RollingFileLogger : LoggerBase, IDisposable {
		private readonly string _extension = ".log";
		private FileAppendTextWriter _textWriter;
		private FileInfo _currentFile;
		private int _currentFileSizeBytes;

		/// <summary>
		/// It is necessary to have an encoding specified so that the number of bytes a message will require can be calculated to maintain
		/// a max file size.
		/// </summary>
		public static Encoding TextEncoding { get; set; } = Encoding.UTF8;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="directoryPath">Base directory where log-files reside.</param>
		/// <param name="logFileNameTemplate">Template log filename used to generate log files (include # for number sequence).</param>
		/// <param name="maxFiles">Maximum number of files to keep in the directory. As new log files are created, older ones are deleted.</param>
		/// <param name="maxFileSize">The maximum byte-size of a log-file before further logging is rolled-over into a new log-file.</param>
		/// <param name="createDirectoryIfMissing">Creates the <param name="directoryPath"></param> if it doesn't exist</param>
		/// <remarks>A <paramref name="logFileNameTemplate"/> parameter value of "ImageStudio_####.log" would result in log-files with paths "%Directory%/ImageStudio_0001.log", "%Directory%/ImageStudio_0002.log", etc</remarks>
		public RollingFileLogger(string directoryPath, string logFileNameTemplate, int maxFiles, int maxFileSize, bool createDirectoryIfMissing = false) {
			Guard.Argument(Tools.FileSystem.IsWellFormedDirectoryPath(directoryPath), nameof(directoryPath), "Directory path is not well formed.");
			Guard.ArgumentInRange(maxFiles, 1, 9999, nameof(maxFiles));
			Guard.ArgumentInRange(maxFileSize, 1, int.MaxValue, nameof(maxFileSize));
			Guard.ArgumentNotNullOrEmpty(logFileNameTemplate, nameof(logFileNameTemplate));
			Guard.Argument(Tools.FileSystem.IsWellFormedFileName(logFileNameTemplate), logFileNameTemplate, "Log file name template contains invalid characters.");
			var directoryPathExists = Directory.Exists(directoryPath);
			Guard.Argument(createDirectoryIfMissing || directoryPathExists, directoryPath, "Directory does not exist.");
			if (!directoryPathExists)
				Directory.CreateDirectory(directoryPath);

			DirectoryPath = directoryPath;
			LogFileNameTemplate = logFileNameTemplate;
			MaxFiles = maxFiles;
			MaxFileSize = maxFileSize;

			Options = LogOptions.DebugBuildDefaults;
		}

		/// <summary>
		/// Base directory where log files reside.
		/// </summary>
		public string DirectoryPath { get; }

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
		public int MaxFiles { get; }

		/// <summary>
		/// The maximum byte-size of a log file before further logging is rolled-over into a new log file.
		/// </summary>
		public int MaxFileSize { get; }

		/// <inheritdoc />
		protected override void Log(LogLevel logLevel, string message) {
			Guard.ArgumentNotNull(message, nameof(message));

			if (TextEncoding.GetByteCount(message) > MaxFileSize)
				throw new InvalidOperationException("Log message is larger than MaxFileSize, splitting message over multiple files is not supported.");

			LogInternal(message);
		}

		/// <summary>
		/// Log the message
		/// </summary>
		/// <param name="message"></param>
		private void LogInternal(string message) {
			Guard.ArgumentNotNull(message, nameof(message));
			message += Environment.NewLine;
			var bytesRequired = TextEncoding.GetByteCount(message);

			if (_currentFile is null
				|| !_currentFile.Exists
				|| _currentFileSizeBytes + bytesRequired > MaxFileSize) {

				_textWriter?.Close();
				var logfile = GetNewLogFileInfo();
				_textWriter = new FileAppendTextWriter(logfile.FullName, TextEncoding, true);
				_currentFile = logfile;
				_currentFileSizeBytes = 0;
			}

			_textWriter.Write(message);
			_textWriter.Flush();
			_currentFileSizeBytes += bytesRequired;
		}

		/// <summary>
		/// Retrieves <see cref="FileInfo"/> for the next log file in sequence after existing files if there are some.
		/// </summary>
		/// <param name="bytesRequired"></param>
		/// <returns></returns>
		private FileInfo GetNewLogFileInfo() {
			var searchPattern = GetLogFileSearchPattern();
			var files = Directory.GetFiles(DirectoryPath, searchPattern, SearchOption.TopDirectoryOnly)
				.OrderByDescending(x => x)
				.ToArray();

			var newFileNumber = 1;
			if (files.Any()) {
				if (files.Length >= MaxFiles) {
					var last = new FileInfo(files.Last());
					last.Delete();
				}

				newFileNumber = GetLogFileNumberFromFileName(_currentFile.Name) + 1;
			}

			var fileName = GetLogFileName(newFileNumber);
			return new FileInfo(Path.Combine(DirectoryPath, fileName));
		}

		/// <summary>
		/// For use when searching the directory for log files, creates the search pattern which is the log file name template, excluding
		/// log file sequence number component, and the extension. 
		/// </summary>
		/// <returns></returns>
		private string GetLogFileSearchPattern() {

			var nameIndex = LogFileNameTemplate.IndexOf('#') > 0 ? LogFileNameTemplate.IndexOf('#') : LogFileNameTemplate.Length;
			var fileNamePrefix = LogFileNameTemplate[..nameIndex];
			var searchPattern = fileNamePrefix + "*" + _extension;
			return searchPattern;
		}

		/// <summary>
		/// Uses the file name template to produce the name of a file with the given file sequence number.
		/// </summary>
		/// <param name="fileNumber"></param>
		/// <returns></returns>
		private string GetLogFileName(int fileNumber) {
			var fileNumberLength = LogFileNameTemplate.Count(x => x == '#');
			fileNumberLength = fileNumberLength > 0 ? fileNumberLength : 1;

			var fileNumberIndex = LogFileNameTemplate.IndexOf('#');
			fileNumberIndex = fileNumberIndex < 0 ? LogFileNameTemplate.Length : fileNumberIndex;

			var builder = new StringBuilder();

			builder.Append(LogFileNameTemplate.Insert(fileNumberIndex,
				fileNumber.ToString()
					.PadLeft(fileNumberLength, '0')).Replace("#", string.Empty));
			builder.Append(_extension);

			return builder.ToString();
		}

		/// <summary>
		/// Given a log file name based on the filename template, extracts the file number component.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private int GetLogFileNumberFromFileName(string fileName) {
			var fileNumberLength = LogFileNameTemplate.Count(x => x == '#');
			fileNumberLength = fileNumberLength > 0 ? fileNumberLength : 1;

			var fileNumberIndex = LogFileNameTemplate.IndexOf('#');
			fileNumberIndex = fileNumberIndex < 0 ? LogFileNameTemplate.Length : fileNumberIndex;

			return int.Parse(fileName.Substring(fileNumberIndex, fileNumberLength));
		}

		public void Dispose() {
			_textWriter.Close();
		}
	}
}
