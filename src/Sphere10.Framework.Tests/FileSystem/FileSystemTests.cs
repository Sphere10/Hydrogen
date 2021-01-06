//-----------------------------------------------------------------------
// <copyright file="FileSystemTests.cs" company="Sphere 10 Software">
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
using NUnit.Framework;
using System.IO;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class FileSystemTests {

        [Test]
        public void AvailableFile_1() {
            var path = Path.GetTempPath();
            var file = Guid.NewGuid().ToStrictAlphaString();
            var ext = ".ext";
            var expectedPath = Path.Combine(path, file + ext);
            Assert.AreEqual(expectedPath, Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
        }

        [Test]
        public void AvailableFile_2() {
            var path = Path.GetTempPath();
            var file = Guid.NewGuid().ToStrictAlphaString();
            var ext = ".ext";
            var desiredPath = Path.Combine(path, file + ext);
            try {
                Tools.FileSystem.CreateBlankFile(desiredPath);
                Assert.AreEqual(Path.Combine(path, file + " 2" + ext), Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
            } finally {
                File.Delete(Path.Combine(path, file + ext));
            }
        }

        [Test]
        public void AvailableFile_3() {
            var path = Path.GetTempPath();
            var file = Guid.NewGuid().ToStrictAlphaString();
            var ext = ".ext";
            try {
                Tools.FileSystem.CreateBlankFile(Path.Combine(path, file + ext));
                Tools.FileSystem.CreateBlankFile(Path.Combine(path, file + " 2" + ext));
                Assert.AreEqual(Path.Combine(path, file + " 3" + ext), Tools.FileSystem.DetermineAvailableFileName(path, file + ext));
            } finally {
                File.Delete(Path.Combine(path, file + ext));
                File.Delete(Path.Combine(path, file + " 2" +ext));
            }
        }



    }
}
