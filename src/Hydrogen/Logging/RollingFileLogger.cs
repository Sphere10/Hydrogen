// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Hydrogen;

/// <summary>
/// Logs to a file and after a size threshold it is archived. Only a a fixed number of archived log files are are kept. The filenames of archived log files are post-fixed with "_yyyyMMddHHmmss" timestamp. 
/// </summary>
/// <example>
/// web.log
/// web_20210914120111.log
/// web_20211014110100.log
/// </example>
public class RollingFileLogger : LoggerBase, IDisposable {
	public const int DefaultMaxFiles = 10;
	public const int DefaultMaxFileSizeB = 1000000;
	private readonly string _directory;
	private readonly string _fileName;
	private FileAppendTextWriter _textWriter;
	private int _currentFileSizeBytes;
	private readonly int _effectiveMaxSize;


	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="logFilePath">Path of log file.</param>
	/// <param name="maxFiles">Maximum number of files to keep in the directory. As new log files are created, older ones are deleted.</param>
	/// <param name="maxFileSize">The maximum byte-size of a log-file before further logging is rolled-over into a new log-file.</param>
	/// <param name="encoding">Text-encoding for file. Default is ASCII.</param>
	public RollingFileLogger(string logFilePath, int maxFiles = DefaultMaxFiles, int maxFileSize = DefaultMaxFileSizeB, Encoding encoding = default) {
		encoding ??= Encoding.ASCII;
		Guard.ArgumentInRange(maxFiles, 1, 9999, nameof(maxFiles));
		Guard.ArgumentInRange(maxFileSize, 32, int.MaxValue, nameof(maxFileSize));
		Guard.ArgumentNotNullOrEmpty(logFilePath, nameof(logFilePath));
		_directory = Tools.FileSystem.GetParentDirectoryPath(logFilePath);
		_fileName = Path.GetFileName(logFilePath);
		LogFilePath = logFilePath;
		Encoding = encoding;
		MaxFiles = maxFiles;
		MaxFileSize = maxFileSize;
		_effectiveMaxSize = MaxFileSize - Encoding.GetByteCount(Environment.NewLine);
		Options = LogOptions.VerboseProfile;
		OpenLogFile(logFilePath, encoding);
	}


	/// <summary>
	/// It is necessary to have an encoding specified so that the number of bytes a message will require can be calculated to maintain
	/// a max file size.
	/// </summary>
	public Encoding Encoding { get; }

	/// <summary>
	/// Template for log file
	/// </summary>
	public string LogFilePath { get; }

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

		var messageSize = Encoding.GetByteCount(message);
		if (messageSize > _effectiveMaxSize) {
			// Clip message since too large
			message = message.Substring(0, _effectiveMaxSize / Encoding.GetBytes(new[] { '0' }).Length);
			messageSize = Encoding.GetByteCount(message);
			GC.Collect();
		}

		if (_currentFileSizeBytes + messageSize > _effectiveMaxSize) {
			ArchiveLogFile();
		}

		_textWriter.WriteLine(message);
		_textWriter.Flush();
		_currentFileSizeBytes += messageSize;
	}

	/// <summary>
	/// Flushes and releases the underlying file writer.
	/// </summary>
	public void Dispose() {
		_textWriter.Close();
	}

	private void OpenLogFile(string logFilePath, Encoding encoding) {
		_textWriter = new FileAppendTextWriter(logFilePath, encoding, true);
		_currentFileSizeBytes = File.Exists(logFilePath) ? (int)Tools.FileSystem.GetFileSize(logFilePath) : 0;
		if (_currentFileSizeBytes > _effectiveMaxSize)
			ArchiveLogFile();
	}

	private void ArchiveLogFile() {
		// Rename current log to archive filename
		var fileExt = Path.GetExtension(_fileName);
		var archiveFileName = $"{Path.GetFileNameWithoutExtension(_fileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExt}";
		if (File.Exists(LogFilePath))
			Tools.FileSystem.RenameFile(LogFilePath, archiveFileName);

		// Recreate log as empty file
		Tools.FileSystem.CreateBlankFile(LogFilePath);
		_currentFileSizeBytes = 0;

		// Delete old files
		var numArchives = MaxFiles - 1; // take 1 since log file itself included in MaxFiles
		var archivesToDelete = Directory
			.GetFiles(
				_directory,
				$"{Path.GetFileNameWithoutExtension(_fileName)}_*{fileExt}",
				SearchOption.TopDirectoryOnly
			)
			.OrderByDescending(x => x)
			.Skip(numArchives);
		archivesToDelete.ForEach(File.Delete);
	}

}
