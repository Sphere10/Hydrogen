// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// HS 2021-10-11: removed since implementation fixed and changed
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using AutoFixture;
//using AutoFixture.Kernel;
//using FluentAssertions;
//using NUnit.Framework;

//namespace Hydrogen.Tests.Logging
//{
//    public class RollingFileLoggerTests
//    {
//        private readonly Random _random = new(31337);

//        [Test]
//        public void ArgumentsAreValidated()
//        {
//            var dir = Tools.FileSystem.GetTempEmptyDirectory();
//            using (Tools.Scope.ExecuteOnDispose(() =>
//                Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
//            {
//                Action a1 = () => new RollingFileLogger("baz", "baz", 1, 1);
//                a1.Should().Throw<ArgumentException>("Directory must exist");

//                Action a2 = () => new RollingFileLogger(dir, string.Empty, 1, 1);
//                a2.Should().Throw<ArgumentException>("Log file name template cannot be an empty string");

//                Action a3 = () => new RollingFileLogger(dir, string.Empty, 1, 0);
//                a3.Should().Throw<ArgumentOutOfRangeException>("Max file size must be greater than 0");

//                Action a4 = () => new RollingFileLogger(dir, string.Empty, 0, 1);
//                a4.Should().Throw<ArgumentOutOfRangeException>("Max file counts must be greater than 0");

//                Action a5 = () => new RollingFileLogger(dir, new string(Path.GetInvalidFileNameChars()), 1, 1);
//                a5.Should().Throw<ArgumentException>(
//                    "Log file name template contains invalid file name / path characters");
//            }
//        }

//        [Test]
//        [TestCase("logfile", "logfile1.log")]
//        [TestCase("log##file", "log01file.log")]
//        [TestCase("logfile###", "logfile001.log")]
//        [TestCase("log file #", "log file 1.log")]
//        public void NewLogFileCreatedWithNameTemplate(string fileNameTemplate, string expectedFileName)
//        {
//            var dir = Tools.FileSystem.GetTempEmptyDirectory();
//            using (Tools.Scope.ExecuteOnDispose(() =>
//                Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
//            {
//                var logger = new RollingFileLogger(dir, fileNameTemplate, 1, 100);
//                var message = _random.NextString(1, 10);
//                logger.Info(message);

//                var directoryInfo = new DirectoryInfo(dir);
//                var files = directoryInfo.GetFiles("*.log");
//                var fileNames = files.Select(x => x.Name).Single();
//                fileNames.Should().Be(expectedFileName);
//            }
//        }

//        [Test]
//        public void OldestFileIsRemovedWhenMaxReached()
//        {
//            var dir = Tools.FileSystem.GetTempEmptyDirectory();
//            using (Tools.Scope.ExecuteOnDispose(() =>
//                Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
//            {
//                string fileNamePattern = "testlogger###_debug";
//                string fileName = "testlogger";
//                var message = _random.NextString(1, 10);
//                int messageSize = RollingFileLogger.TextEncoding.GetByteCount(message + System.Environment.NewLine);
//                var logger = new RollingFileLogger(dir, fileNamePattern, 3, messageSize*2);

//                // max file size is twice message size. resulting 3 files should be numbered half of log count
//                int logCount = 100;
//                for (int i = 0; i < logCount; i++)
//                {
//                    logger.Info(message);
//                }

//                var directoryInfo = new DirectoryInfo(dir);
//                var files = directoryInfo.GetFiles("*.log");
//                var fileNames = files.Select(x => x.Name);

//                fileNames.Should().BeEquivalentTo($"{fileName}048_debug.log", $"{fileName}049_debug.log",
//                    $"{fileName}050_debug.log");
//            }
//        }

//        [Test]
//        public void LogFileContentsIsCorrect()
//        {
//            var dir = Tools.FileSystem.GetTempEmptyDirectory();
//            using (Tools.Scope.ExecuteOnDispose(() =>
//                Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
//            {
//                string fileNamePattern = "testlogger###_debug";
//                string fileName = "testlogger";

//                int messagelength = 11;
//                int messageSize = RollingFileLogger.TextEncoding.GetMaxByteCount(messagelength);
//                var logger = new RollingFileLogger(dir, fileNamePattern, 3, messageSize * 10);

//                List<string> messages = new List<string>()
//                {
//                    _random.NextString(10),
//                    _random.NextString(10),
//                    _random.NextString(10),
//                    _random.NextString(10),
//                    _random.NextString(10),
//                };

//                for (int i = 0; i < messages.Count; i++)
//                {
//                    logger.Info(messages[i]);
//                }

//                var directoryInfo = new DirectoryInfo(dir);
//                var files = directoryInfo.GetFiles(fileName + "*");
//                var fileNames = files.Select(x => x.Name);

//                fileNames.Should().BeEquivalentTo($"{fileName}001_debug.log");


//                var contents = messages.Aggregate((x, y) => x + System.Environment.NewLine + y);

//                using var textReader = new StreamReader(files.Single().FullName);
//                var fileContents = textReader.ReadToEnd();

//                //contents from file has a whitespace at the end for some reason, trim it
//                fileContents.Trim().Should().Be(contents);
//            }
//        }
//    }
//}


