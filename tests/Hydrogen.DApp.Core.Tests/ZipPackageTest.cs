using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hydrogen;
using Hydrogen.DApp.Core.Storage;

namespace VelocityNET.Processing.Tests.Core {
    public class ZipPackageTest {

        [Test]
        public void CreatePackageOnFirstWrite() {
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));

            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = new ZipPackage(filename);
                using (var scope = package.EnterWriteScope()) {
                    Assert.IsTrue(File.Exists(filename));
                }
            }
        }


        [Test]
        public void Create_TextFile() {
            var textData = "alpha beta gamma";
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));

            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = new ZipPackage(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllText("alpha.txt", textData);
                }

                using (package.EnterReadScope()) {
                    Assert.AreEqual(1, package.GetKeys().Count());
                    var value = package.ReadAllText("alpha.txt");
                    Assert.AreEqual(textData, value);
                }
            }
        }


        [Test]
        public void Create_BinFile() {
            var binData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));
            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = new ZipPackage(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllBytes("alpha.bin", binData);
                }

                using (package.EnterReadScope()) {
                    Assert.AreEqual(1, package.GetKeys().Count());
                    var value = package.ReadAllBytes("alpha.bin");
                    Assert.AreEqual(binData, value);
                }
            }
        }


        [Test]
        public void Update_1() {
            var textData = "alpha beta gamma";
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));

            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = new ZipPackage(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllText("alpha.txt", "old data");
                }

                using (package.EnterWriteScope()) {
                    package.WriteAllText("alpha.txt", textData);
                }

                using (package.EnterReadScope()) {
                    Assert.AreEqual(1, package.GetKeys().Count());
                    var value = package.ReadAllText("alpha.txt");
                    Assert.AreEqual(textData, value);
                }
            }
        }

        [Test]
        public void Update_2() {
            var textData = "alpha beta gamma";
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));

            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = new ZipPackage(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllText("alpha1.txt", "old data");
                }

                using (package.EnterWriteScope()) {
                    package.WriteAllText("alpha1.txt", textData);
                    package.WriteAllText("alpha2.txt", textData);
                }

                using (package.EnterReadScope()) {
                    Assert.AreEqual(2, package.GetKeys().Count());
                    Assert.AreEqual(textData, package.ReadAllText("alpha1.txt"));
                    Assert.AreEqual(textData, package.ReadAllText("alpha2.txt"));
                }
            }
        }


        [Test]
        public void Complex() {
            var file1Name = "root.bin";
            var file2Name = "sub.txt";
            var file3Name = "sub.bin";
            var folder1Name = "folder1";
            var folder2Name = "folder2";
            var textData = "alpha beta gamma";
            var binData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));
            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = ZipPackage.Create(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllBytes($"{file1Name}", binData);
                    package.WriteAllText($"{folder1Name}/{file2Name}", textData);
                }
                using (package.EnterWriteScope()) {
                    Assert.AreEqual(2, package.GetKeys().Count());
                    Assert.Contains(file1Name, package.GetKeys().ToArray());
                    Assert.Contains($"{folder1Name}/{file2Name}", package.GetKeys().ToArray());
                    package.WriteAllBytes($"{file2Name}", binData);
                    package.WriteAllText($"{folder2Name}/{file2Name}", textData);
                }

                using (package.EnterReadScope()) {
                    Assert.AreEqual(4, package.GetKeys().Count());
                    Assert.Contains(file1Name, package.GetKeys().ToArray());
                    Assert.Contains($"{folder1Name}/{file2Name}", package.GetKeys().ToArray());
                    Assert.Contains(file2Name, package.GetKeys().ToArray());
                    Assert.Contains($"{folder2Name}/{file2Name}", package.GetKeys().ToArray());
                }
            }
        }


        [Test]
        public void Extract() {
            var filename_bin = "root.bin";
            var filename_txt = "root.txt";
            var child_dir_1 = "folder1";
            var child_dir_2 = "folder2";
            var textData = "alpha beta gamma";
            var binData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var filename = Tools.FileSystem.GetTempFileName(false);
            Assert.IsTrue(!File.Exists(filename));
            using (new ActionScope(endAction: () => File.Delete(filename))) {
                var package = ZipPackage.Create(filename);
                using (package.EnterWriteScope()) {
                    package.WriteAllBytes($"{filename_bin}", binData);
                    package.WriteAllText($"{filename_txt}", textData);

                    package.WriteAllBytes($"{child_dir_1}/{filename_bin}", binData);
                    package.WriteAllText($"{child_dir_1}/{filename_txt}", textData);

                    package.WriteAllBytes($"{child_dir_2}/{filename_bin}", binData);
                    package.WriteAllText($"{child_dir_2}/{filename_txt}", textData);

                }
                using (package.EnterReadScope()) {
                    var tmpDir = Tools.FileSystem.GetTempEmptyDirectory(true);
                    using (new ActionScope(endAction: () => Tools.FileSystem.DeleteDirectory(tmpDir))) {
                        package.ExtractTo(tmpDir);

                        // root
                        var files = Tools.FileSystem.GetFiles(tmpDir).ToArray();
                        Assert.AreEqual(2, files.Length);
                        Assert.Contains(filename_bin, files);
                        Assert.Contains(filename_txt, files);
                        Assert.AreEqual(binData, File.ReadAllBytes(Path.Combine(tmpDir, filename_bin)));
                        Assert.AreEqual(textData, File.ReadAllText(Path.Combine(tmpDir, filename_txt)));

                        var dirs = Tools.FileSystem.GetSubDirectories(tmpDir).ToArray();
                        Assert.AreEqual(2, dirs.Length);
                        Assert.Contains(child_dir_1, dirs);
                        Assert.Contains(child_dir_2, dirs);

                        // subdir 1
                        var subDir1 = Path.Combine(tmpDir, child_dir_1);
                        files = Tools.FileSystem.GetFiles(subDir1).ToArray();
                        Assert.AreEqual(2, files.Length);
                        Assert.Contains(filename_bin, files);
                        Assert.Contains(filename_txt, files);
                        Assert.AreEqual(0, Tools.FileSystem.GetSubDirectories(subDir1).Count());
                        Assert.AreEqual(binData, File.ReadAllBytes(Path.Combine(tmpDir, subDir1, filename_bin)));
                        Assert.AreEqual(textData, File.ReadAllText(Path.Combine(tmpDir, subDir1, filename_txt)));


                        // subdir 2
                        var subDir2 = Path.Combine(tmpDir, child_dir_2);
                        files = Tools.FileSystem.GetFiles(subDir2).ToArray();
                        Assert.AreEqual(2, files.Length);
                        Assert.Contains(filename_bin, files);
                        Assert.Contains(filename_txt, files);
                        Assert.AreEqual(0, Tools.FileSystem.GetSubDirectories(subDir2).Count());
                        Assert.AreEqual(binData, File.ReadAllBytes(Path.Combine(tmpDir, subDir2, filename_bin)));
                        Assert.AreEqual(textData, File.ReadAllText(Path.Combine(tmpDir, subDir2, filename_txt)));

                    }

                }

            }
        }
    }
}