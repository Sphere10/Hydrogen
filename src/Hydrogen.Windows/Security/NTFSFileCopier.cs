// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Linq;

namespace Hydrogen.Windows.Security;

public class NTFSFileCopier {
	private bool _cleanDestination;
	private bool _copyData;
	private bool _copySecurity;
	private bool _copyOwnership;
	private bool _copyTimestamps;
	private bool _copyShares;
	private bool _resolveDanglingSIDs;
	private bool _automaticallyImportLocalAccounts;
	private string _importedUsersPassword;
	private bool _forceOwnershipOnAccessDenied;
	private bool _continueOnAllErrors;
	private string _remoteHostName;
	private bool _verifyData;
	private int _retryCount;
	private SidNameResolver _resolver;
	private SidTranslator _sidTranslator;
	private IActionObserver _actionObserver;
	private SecurityIdentifier _administratorsSID;
	private ShareCopier _shareCopier;
	private NTHost _remoteHost = null;
	private List<NTShare> _remoteHostShares = null;

	public NTFSFileCopier(
		bool cleanDestination,
		bool copyData,
		bool verifyData,
		int retryCount,
		bool copySecurity,
		bool copyOwnership,
		bool copyTimestamps,
		bool copyShares,
		bool resolveDanglingSIDs,
		bool automaticallyImportLocalAccounts,
		string importedUsersPassword,
		bool forceOwnershipOnAccessDenied,
		bool continueOnAllErrors,
		string remoteHostName,
		IActionObserver actionObserver) {
		CleanDestination = cleanDestination;
		CopyData = copyData;
		VerifyData = verifyData;
		RetryCount = retryCount;
		CopySecurity = copySecurity;
		CopyOwnership = copyOwnership;
		CopyTimestamps = copyTimestamps;
		CopyShares = copyShares;
		ResolveDanglingSIDs = resolveDanglingSIDs;
		AutomaticallyImportLocalAccounts = automaticallyImportLocalAccounts;
		ImportedUsersPassword = importedUsersPassword;
		ForceOwnershipOnAccessDenied = forceOwnershipOnAccessDenied;
		ContinueOnAllErrors = continueOnAllErrors;
		RemoteHostName = !string.IsNullOrEmpty(remoteHostName) ? remoteHostName : string.Empty;
		ActionObserver = actionObserver;
		Resolver = string.IsNullOrEmpty(remoteHostName) ? new SidNameResolver() : new SidNameResolver(remoteHostName);
		SidTranslator = new SidTranslator(true, ImportedUsersPassword, ActionObserver);
		_administratorsSID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
	}

	public bool CleanDestination {
		get { return _cleanDestination; }
		set { _cleanDestination = value; }
	}

	public bool CopyData {
		get { return _copyData; }
		set { _copyData = value; }
	}

	public bool CopySecurity {
		get { return _copySecurity; }
		set { _copySecurity = value; }
	}

	public bool CopyOwnership {
		get { return _copyOwnership; }
		set { _copyOwnership = value; }
	}

	public bool CopyTimestamps {
		get { return _copyTimestamps; }
		set { _copyTimestamps = value; }
	}

	public bool CopyShares {
		get { return _copyShares; }
		set { _copyShares = value; }
	}

	public bool VerifyData {
		get { return _verifyData; }
		set { _verifyData = value; }
	}

	public int RetryCount {
		get { return _retryCount; }
		set { _retryCount = value; }
	}

	public bool ResolveDanglingSIDs {
		get { return _resolveDanglingSIDs; }
		set { _resolveDanglingSIDs = value; }
	}

	public bool AutomaticallyImportLocalAccounts {
		get { return _automaticallyImportLocalAccounts; }
		set { _automaticallyImportLocalAccounts = value; }
	}

	public string ImportedUsersPassword {
		get { return _importedUsersPassword; }
		set { _importedUsersPassword = value; }
	}

	public bool ForceOwnershipOnAccessDenied {
		get { return _forceOwnershipOnAccessDenied; }
		set { _forceOwnershipOnAccessDenied = value; }
	}

	public bool ContinueOnAllErrors {
		get { return _continueOnAllErrors; }
		set { _continueOnAllErrors = value; }
	}

	public IActionObserver ActionObserver {
		get { return _actionObserver; }
		set { _actionObserver = value; }
	}

	public SidNameResolver Resolver {
		get { return _resolver; }
		set { _resolver = value; }
	}

	public SidTranslator SidTranslator {
		get { return _sidTranslator; }
		set { _sidTranslator = value; }
	}

	public ShareCopier ShareCopier {
		get { return _shareCopier; }
		set { _shareCopier = value; }
	}

	public SecurityIdentifier AdministratorsSID {
		get { return _administratorsSID; }
		set { _administratorsSID = value; }
	}

	public string RemoteHostName {
		get { return _remoteHostName; }
		set { _remoteHostName = value; }
	}

	public NTHost RemoteHost {
		get {
			lock (this) {
				if (_remoteHost == null) {
					_remoteHost = new NTHost(RemoteHostName);
				}
			}
			return _remoteHost;
		}
	}

	public List<NTShare> RemoteHostShares {
		get {
			lock (this) {
				if (_remoteHostShares == null) {
					_remoteHostShares = new List<NTShare>(RemoteHost.GetShares());
				}
			}
			return _remoteHostShares;
		}
	}

	public void CopyDirectory(string sourceDir, string destParentDirectory) {

		#region Validation

		if (!DirectoryExists(sourceDir)) {
			ActionObserver.NotifyError("Source directory '{0}' does not exist", sourceDir);
			return;
		}

		if (!DirectoryExists(destParentDirectory)) {
			ActionObserver.NotifyError("Destination parent directory '{0}' does not exist", destParentDirectory);
			return;
		}

		#endregion

		string destDir = ConstructDestinationDirectoryName(sourceDir, destParentDirectory);
		bool isVeryLongPath = IsVeryLongDirectoryPath(destDir);
		ActionObserver.NotifyAction("Copying", "Directory", sourceDir, destDir);

		try {
			if (CopyData) {
				if (DirectoryExists(destDir) && CleanDestination) {
					DeleteDirectory(destDir);
				}

				// create directory on destination if not exists
				if (!DirectoryExists(destDir)) {
					CreateDirectoryInternal(destDir);
				}
			}

			// if directory not exists, then since not copying data just bailout
			if (!DirectoryExists(destDir)) {
				return;
			}

			// if the directory path is long, then warn user that shares, security, timestamps for itself and nested objects will not be copied
			if (isVeryLongPath) {
				ActionObserver.NotifyWarning("Directory path is too long. All shares, security and timestamp information for this directory and nested directories will NOT be copied. Path is '{0}'", destDir);
			}

			// Recursively call to copy sub directories first
			foreach (string sourceSubDir in GetSubDirectories(sourceDir)) {
				CopyDirectory(sourceSubDir, destDir);
			}

			// copy share details
			try {
				if (CopyShares) {
					if (!isVeryLongPath) {

						#region See if source directory defines a share

						bool foundShare = false;
						NTShare sourceShareToCopy = null;
						foreach (NTShare share in RemoteHostShares) {
							string sourceLocalPath =
								sourceDir.IsUNCPath() ? RemoteHost.ResolveUNCPathToLocalPath(sourceDir) : sourceDir;

							if (Path.Equals(
								    share.ServerPath.ToUpper(),
								    sourceLocalPath.ToUpper()
							    )) {
								foundShare = true;
								sourceShareToCopy = share;
								break;
							}
						}

						#endregion

						#region Copy share if source dir defines a share

						if (foundShare) {
							try {
								ActionObserver.NotifyAction("Copying", "Share", sourceShareToCopy.FullName, destDir);
								CopyableShare copyableShare = new CopyableShare(
									sourceShareToCopy,
									destDir
								);
								ShareCopier shareCopier = new ShareCopier(
									this.Resolver,
									this.SidTranslator,
									true,
									this.ActionObserver
								);
								shareCopier.CopyShare(copyableShare);
							} catch (Exception failure) {
								ActionObserver.NotifyActionFailed("Copying", "Share", sourceShareToCopy.FullName, destDir, failure.ToDisplayString());
							}
						}

						#endregion

					}
				}
			} catch (Exception error) {
				ActionObserver.NotifyError("An unexpected error occured when trying to copy shares. {0}", error.ToDisplayString());
			}

			// copy file data
			string[] sourceFiles = GetDirectoryFiles(sourceDir);


			if (CopyData) {
				foreach (string sourceFile in sourceFiles) {
					string destFileName = ConstructDestinationFileName(sourceFile, destDir);
					CopyFile(sourceFile, destFileName);

					// verify file data
					if (VerifyData) {
						if (!IsVeryLongFilePath(destFileName)) {
							if (!VerifyFile(sourceFile, destFileName)) {
								int attempt = 0;
								while (++attempt < RetryCount) {
									ActionObserver.NotifyWarning("Retrying copy procedure. Attempt {0} of {1}. Source = '{2}' Dest = '{3}", attempt, RetryCount, sourceFile, destFileName);
									if (FileExists(destFileName)) {
										DeleteFile(destFileName);
									}
									CopyFile(sourceFile, destFileName);
									if (VerifyFile(sourceFile, destFileName)) {
										ActionObserver.NotifyWarning("Files are now identical. Attempt {0} succeeded. Source = '{1}' Dest = '{2}", attempt, sourceFile, destFileName);
										break;
									}
								}
							}
						}
					}
				}
			}

			// if dest dir is very long, ignore copying all other properties
			if (isVeryLongPath) {
				return;
			}

			// copy file and directory timestamps
			if (CopyTimestamps) {
				CopyDirectoryTimestamp(sourceDir, destDir);
				// copy file timestamps
				foreach (string sourceFile in sourceFiles) {
					string destFile = ConstructDestinationFileName(sourceFile, destDir);
					if (!IsVeryLongFilePath(destFile)) {
						if (FileExists(destFile)) {
							CopyFileTimestamps(sourceFile, destFile);
						} else {
							ActionObserver.NotifyWarning("Not copying file timestamp as destination file '{0}' did not exist", destFile);
						}
					}
				}
			}

			// copy file and directory attributes
			foreach (string sourceFile in sourceFiles) {
				string destFileName = ConstructDestinationFileName(sourceFile, destDir);
				if (!IsVeryLongFilePath(destFileName)) {
					if (FileExists(destFileName)) {
						CopyFileAttributes(sourceFile, destFileName);
					} else {
						ActionObserver.NotifyWarning(
							"Not copying file attributes as destination file '{0}' did not exist.",
							destFileName
						);
					}
				}
			}

			CopyDirectoryAttributes(sourceDir, destDir);

			// copy file security
			if (CopySecurity) {
				// copy file security
				foreach (string sourceFile in sourceFiles) {
					string destFile = ConstructDestinationFileName(sourceFile, destDir);
					if (!IsVeryLongFilePath(destFile)) {
						if (FileExists(destFile)) {
							CopyFileSecurity(sourceFile, destFile);
						} else {
							ActionObserver.NotifyWarning("Not copying file security as destination file '{0}' did not exist", destFile);
						}
					}
				}
			}

			// copy file ownership again (necessary by empirical tests)
			if (CopyOwnership) {
				// copy file ownership
				foreach (string sourceFile in sourceFiles) {
					string destFile = ConstructDestinationFileName(sourceFile, destDir);
					if (!IsVeryLongFilePath(destFile)) {
						if (FileExists(destFile)) {
							CopyFileOwner(sourceFile, destFile);
						} else {
							ActionObserver.NotifyWarning("Not copying file ownership as destination file '{0}' did not exist", destFile);
						}
					}
				}
			}


			// copy directory security
			if (CopySecurity) {
				CopyDirectorySecurity(sourceDir, destDir);
			}

			// copy directory ownership again (necessary by empirical tests)
			if (CopyOwnership) {
				CopyDirectoryOwnership(sourceDir, destDir);
			}


		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "Directory", sourceDir, destDir, e.ToDisplayString());
		}
	}

	public void CopyFileAttributes(string sourceFile, string destFile) {
		Debug.Assert(FileExists(sourceFile));
		Debug.Assert(FileExists(destFile));
		ActionObserver.NotifyAction("Copy", "File Attributes", sourceFile, destFile);
		try {
			File.SetAttributes(
				destFile,
				File.GetAttributes(
					sourceFile
				)
			);
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "File Ownership", sourceFile, destFile, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}

	public void CopyDirectoryAttributes(string sourceDir, string destDir) {
		Debug.Assert(DirectoryExists(sourceDir));
		Debug.Assert(DirectoryExists(destDir));
		ActionObserver.NotifyAction("Copy", "Directory Attributes", sourceDir, destDir);
		try {

			DirectoryInfo srcInfo = new DirectoryInfo(sourceDir);
			DirectoryInfo destInfo = new DirectoryInfo(destDir);

			destInfo.Attributes = srcInfo.Attributes;
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "Directory Attributes", sourceDir, destDir, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}

	public void CopyFileOwner(string sourceFile, string destFile) {
		Debug.Assert(FileExists(sourceFile));
		Debug.Assert(FileExists(destFile));

		ActionObserver.NotifyAction("Copy", "File Ownership", sourceFile, destFile);

		void CopyOwnership() {
			var sourceFileInfo = new FileInfo(sourceFile);
			var destFileInfo = new FileInfo(destFile);
			FileSecurity sourceSecurity = sourceFileInfo.GetAccessControl();
			FileSecurity destSecurity = destFileInfo.GetAccessControl();
			SecurityIdentifier translatedSID = TranslateSID(
				(SecurityIdentifier)sourceSecurity.GetOwner(typeof(SecurityIdentifier))
			);
			SecurityIdentifier currentSID = (SecurityIdentifier)destSecurity.GetOwner(typeof(SecurityIdentifier));
			if (currentSID != translatedSID) {
				destSecurity.SetOwner(translatedSID);
			}

			destFileInfo.SetAccessControl(destSecurity);
		}

		try {
			CopyOwnership();
		} catch (UnauthorizedAccessException ue) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					ActionObserver.NotifyWarning("Failed copying file ownership, will attempt to force ownership. Source = '{0}' Destination = '{1}' ", sourceFile, destFile);
					GiveLocalAdministratorsOwnershipOfFile(destFile);
					GrantFullAccessToFile(destFile);
					CopyOwnership();
					ActionObserver.NotifyWarning("Succeeded in copying file ownership from '{0}'", sourceFile);
				} catch (Exception e) {
					ActionObserver.NotifyActionFailed("Copy", "File Ownership", sourceFile, destFile, e.ToDisplayString());
				}
			} else {
				ActionObserver.NotifyActionFailed("Copy", "File Ownership", sourceFile, destFile, ue.ToDisplayString());
			}
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "File Ownership", sourceFile, destFile, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}

	public void CopyDirectoryOwnership(string sourceDir, string destDir) {
		// http://www.perlmonks.org/?node_id=70562
		Debug.Assert(DirectoryExists(sourceDir));
		Debug.Assert(DirectoryExists(destDir));

		ActionObserver.NotifyAction("Copying", "Directory Ownership", sourceDir, destDir);

		void CopyOwnership() {
			var sourceDirInfo = new DirectoryInfo(sourceDir);
			var destDirInfo = new DirectoryInfo(destDir);
			DirectorySecurity sourceSecurity = sourceDirInfo.GetAccessControl();
			DirectorySecurity destSecurity = destDirInfo.GetAccessControl();

			SecurityIdentifier translatedSID = TranslateSID(
				(SecurityIdentifier)sourceSecurity.GetOwner(typeof(SecurityIdentifier))
			);
			SecurityIdentifier currentSID = (SecurityIdentifier)destSecurity.GetOwner(typeof(SecurityIdentifier));
			// only copy if different
			if (currentSID != translatedSID) {
				destSecurity.SetOwner(translatedSID);
			}
			destDirInfo.SetAccessControl(destSecurity);
		}

		try {
			CopyOwnership();

		} catch (UnauthorizedAccessException uae) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					ActionObserver.NotifyWarning("Failed copying directory ownership, will attempt to force ownership. Source = '{0}' Destination = '{1}' ", sourceDir, destDir);
					GiveLocalAdministratorsOwnershipOfDirectory(destDir);
					GrantFullAccessToDirectory(destDir);
					CopyOwnership();
					ActionObserver.NotifyWarning("Succeeded in copying directory ownership from '{0}'", sourceDir);
				} catch (Exception e) {
					ActionObserver.NotifyActionFailed("Copy", "Directory Security", sourceDir, destDir, e.ToDisplayString());
				}
			} else {
				ActionObserver.NotifyActionFailed("Copy", "Directory Security", sourceDir, destDir, uae.ToDisplayString());
			}
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "Directory Ownership", sourceDir, destDir, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}

	}

	public void CopyDirectorySecurity(string sourceDir, string destDir) {
		Debug.Assert(DirectoryExists(sourceDir));
		Debug.Assert(DirectoryExists(destDir));

		ActionObserver.NotifyAction("Copy", "Directory Security", sourceDir, destDir);

		try {
			var sourceDirInfo = new DirectoryInfo(sourceDir);
			var destDirInfo = new DirectoryInfo(destDir);
			destDirInfo.SetAccessControl(TranslateDirectoryACL(sourceDirInfo.GetAccessControl()));
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copy", "Directory Security", sourceDir, destDir, e.ToDisplayString());
		}
	}


	/// <summary>
	/// Copies a file between locations.
	/// </summary>
	/// <param name="sourceFile">Full path of source file.</param>
	/// <param name="destFile">Full path of destination file. Parent directory must exist.</param>
	public void CopyFile(string sourceFile, string destFile) {
		Debug.Assert(FileExists(sourceFile));
		ActionObserver.NotifyAction("Copying", "File", sourceFile, destFile);

		if (IsVeryLongDirectoryPath(destFile)) {
			ActionObserver.NotifyWarning("File path is too long. Only data will be copied. No security, timestamp or attributes will be copied. File is '{0}'", destFile);
		}

		try {
			CopyFileInternal(sourceFile, destFile);
		} catch (UnauthorizedAccessException) {

			if (ForceOwnershipOnAccessDenied) {
				// strategy to force ownership:
				//  1. take ownership of parent directory
				//  2. grant full access to parent directory
				//  3. take ownership of file
				//  4. grant full access to file
				try {
					GiveLocalAdministratorsOwnershipOfDirectory(Path.GetDirectoryName(sourceFile));
					GrantFullAccessToDirectory(Path.GetDirectoryName(sourceFile));
					GiveLocalAdministratorsOwnershipOfFile(sourceFile);
					GrantFullAccessToFile(sourceFile);
					CopyFileInternal(sourceFile, destFile);
				} catch (Exception e1) {
					ActionObserver.NotifyActionFailed("Copying", "File", sourceFile, destFile, e1.ToDisplayString());
					if (!ContinueOnAllErrors) {
						throw;
					}
				}
			} else if (!ContinueOnAllErrors) {
				throw;

			}
		} catch (Exception e2) {
			ActionObserver.NotifyActionFailed("Copying", "File", sourceFile, destFile, e2.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}

	}

	public bool VerifyFile(string sourceFile, string destFile) {
		ActionObserver.NotifyAction("Verify", "File", sourceFile, destFile);
		bool retval = false;
		try {
			using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read)) {
				using (FileStream destStream = new FileStream(destFile, FileMode.Open, FileAccess.Read)) {
					retval = Tools.Streams.CompareFileStreams(sourceStream, destStream);
					if (!retval) {
						ActionObserver.NotifyError("Files are not identical. Source = '{0}' Dest = '{1}'", sourceFile, destFile);
					}
				}
			}
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Verify", "File", sourceFile, destFile, e.ToDisplayString());
		}
		return retval;
	}

	public void CopyFileSecurity(string sourceFile, string destFile) {
		Debug.Assert(FileExists(sourceFile));
		Debug.Assert(FileExists(destFile));
		ActionObserver.NotifyAction("Copying", "File Security", sourceFile, destFile);
		try {
			var sourceFileInfo = new FileInfo(sourceFile);
			var destFileInfo = new FileInfo(destFile);
			destFileInfo.SetAccessControl(TranslateFileACL(sourceFileInfo.GetAccessControl()));
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "File Security", sourceFile, destFile, e.ToDisplayString());
		}
	}

	public void CopyFileTimestamps(string sourceFile, string destFile) {
		Debug.Assert(FileExists(sourceFile));
		Debug.Assert(FileExists(destFile));
		ActionObserver.NotifyAction("Copying", "File Timestamps", sourceFile, destFile);
		try {
			File.SetCreationTime(destFile, File.GetCreationTime(sourceFile));
			File.SetLastAccessTime(destFile, File.GetLastAccessTime(sourceFile));
			File.SetLastWriteTime(destFile, File.GetLastWriteTime(sourceFile));
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "File Timestamps", sourceFile, destFile, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}

	public void CopyDirectoryTimestamp(string sourceDir, string destDir) {
		Debug.Assert(DirectoryExists(sourceDir));
		Debug.Assert(DirectoryExists(destDir));
		ActionObserver.NotifyAction("Copying", "Directory Timestamps", sourceDir, destDir);
		try {
			Directory.SetCreationTime(destDir, Directory.GetCreationTime(sourceDir));
			Directory.SetLastAccessTime(destDir, Directory.GetLastAccessTime(sourceDir));
			Directory.SetLastWriteTime(destDir, Directory.GetLastWriteTime(sourceDir));
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Copying", "Directory Timestamps", sourceDir, destDir, e.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}

	public void DeleteDirectory(string directory) {
		Debug.Assert(DirectoryExists(directory));
		ActionObserver.NotifyAction("Deleting", "Directory", directory, string.Empty);

		#region Delete child directories recursively

		foreach (string subDir in GetSubDirectories(directory)) {

			#region Delete directory

			try {
				DeleteDirectory(subDir);
			} catch (UnauthorizedAccessException e0) {
				if (ForceOwnershipOnAccessDenied) {
					try {
						GiveLocalAdministratorsOwnershipOfDirectory(directory);
						GrantFullAccessToDirectory(directory);
						try {
							DirectoryInfo dinfo = new DirectoryInfo(directory);
							dinfo.Attributes = FileAttributes.Normal;
						} catch {
						}
						File.SetAttributes(directory, FileAttributes.Normal);
						DeleteDirectoryInternal(directory);
					} catch (Exception e1) {
						ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e1.ToDisplayString());
						if (!ContinueOnAllErrors) {
							throw;
						}
					}
				} else {
					ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e0.ToDisplayString());
					if (!ContinueOnAllErrors) {
						throw;
					}
				}
			} catch (Exception e2) {
				ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e2.ToDisplayString());
				if (!ContinueOnAllErrors) {
					throw;
				}
			}

			#endregion

		}

		#endregion

		#region Delete member files

		foreach (string file in GetDirectoryFiles(directory)) {
			DeleteFile(file);
		}

		#endregion

		#region Delete actual directory

		try {
			ActionObserver.NotifyAction("Deleting", "Directory", directory, string.Empty);
			DeleteDirectoryInternal(directory);
		} catch (UnauthorizedAccessException e0) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					GiveLocalAdministratorsOwnershipOfDirectory(directory);
					GrantFullAccessToDirectory(directory);
					File.SetAttributes(directory, FileAttributes.Normal);
					DeleteDirectoryInternal(directory);
				} catch (Exception e1) {
					ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e1.ToDisplayString());
					if (!ContinueOnAllErrors) {
						throw;
					}
				}
			} else {
				ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e0.ToDisplayString());
				if (!ContinueOnAllErrors) {
					throw;
				}
			}
		} catch (Exception e2) {
			ActionObserver.NotifyActionFailed("Deleting", "Directory", directory, string.Empty, e2.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}

		#endregion

	}

	public void DeleteFile(string file) {
		try {
			ActionObserver.NotifyAction("Deleting", "File", file, string.Empty);
			DeleteFileInternal(file);
		} catch (UnauthorizedAccessException e0) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					GiveLocalAdministratorsOwnershipOfFile(file);
					GrantFullAccessToFile(file);
					File.SetAttributes(file, FileAttributes.Normal);
					DeleteFileInternal(file);
				} catch (Exception e1) {
					ActionObserver.NotifyActionFailed("Deleting", "File", file, string.Empty, e1.ToDisplayString());
					if (!ContinueOnAllErrors) {
						throw;
					}
				}
			} else {
				ActionObserver.NotifyActionFailed("Deleting", "File", file, string.Empty, e0.ToDisplayString());
				if (!ContinueOnAllErrors) {
					throw;
				}
			}
		} catch (Exception e2) {
			ActionObserver.NotifyActionFailed("Deleting", "File", file, string.Empty, e2.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
	}


	public void GrantFullAccessToDirectory(string directory) {
		ActionObserver.NotifyAction("Granting Full Access", "Directory", directory, string.Empty);
		try {
			ActionObserver.NotifyWarning(
				"Granting '{0}' full access to '{1}'",
				WindowsIdentity.GetCurrent().Owner.Translate(typeof(NTAccount)).Value,
				directory
			);

			Debug.Assert(DirectoryExists(directory));
			var directoryInfo = new DirectoryInfo(directory);
			DirectorySecurity security = directoryInfo.GetAccessControl();
			security.SetAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().Owner, FileSystemRights.FullControl, AccessControlType.Allow));
			directoryInfo.SetAccessControl(security);
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Granting Full Access", "Directory", directory, string.Empty, e.ToDisplayString());
		}
	}

	public void GiveLocalAdministratorsOwnershipOfDirectory(string directory) {
		Debug.Assert(DirectoryExists(directory));
		ActionObserver.NotifyAction("Take Ownership", "Directory", directory, string.Empty);
		try {
			ActionObserver.NotifyWarning("Granting Administrators group ownership of '{0}'", directory);
			IdentityReference administrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			// first try to give it the normal way 
			try {
				var directoryInfo = new DirectoryInfo(directory);
				DirectorySecurity security = directoryInfo.GetAccessControl(AccessControlSections.Owner);

				// if owner is not administrators group, make sure it is
				if (security.GetOwner(typeof(SecurityIdentifier)) != administrators) {
					security.SetOwner(administrators);
					directoryInfo.SetAccessControl(security);
				}
			} catch (UnauthorizedAccessException) {
				// give it a new ACL altogether
				ActionObserver.NotifyWarning("Cannot change ownership, resetting entire directory security '{0}'", directory);
				DirectorySecurity newSecurity = new DirectorySecurity();
				newSecurity.SetOwner(administrators);
				var directoryInfo = new DirectoryInfo(directory);
				directoryInfo.SetAccessControl(newSecurity);
			}

		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Take Ownership", "Directory", directory, string.Empty, e.ToDisplayString());
		}
	}

	public void GrantFullAccessToFile(string file) {
		Debug.Assert(FileExists(file));
		ActionObserver.NotifyAction("Granting Full Access", "File", file, string.Empty);
		try {
			ActionObserver.NotifyWarning(
				"Granting '{0}' full access to '{1}'",
				WindowsIdentity.GetCurrent().Owner.Translate(typeof(NTAccount)).Value,
				file
			);
			var fileInfo = new FileInfo(file);
			FileSecurity security = fileInfo.GetAccessControl();
			security.SetAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().Owner, FileSystemRights.FullControl, AccessControlType.Allow));
			fileInfo.SetAccessControl(security);
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Granting Full Access", "File", file, string.Empty, e.ToDisplayString());
		}
	}

	public void GiveLocalAdministratorsOwnershipOfFile(string file) {
		Debug.Assert(FileExists(file));
		ActionObserver.NotifyAction("Take Ownership", "File", file, string.Empty);
		try {
			ActionObserver.NotifyWarning("Granting Administrators group ownership of '{0}'", file);

			// first try to give it the normal way 
			try {
				File.SetAttributes(file, FileAttributes.Normal);
				var fileInfo = new FileInfo(file);
				FileSecurity security = fileInfo.GetAccessControl(AccessControlSections.Owner);

				// if owner is not administrators group, make sure it is
				if (security.GetOwner(typeof(SecurityIdentifier)) != AdministratorsSID) {
					security.SetOwner(AdministratorsSID);
					fileInfo.SetAccessControl(security);
				}
			} catch (UnauthorizedAccessException) {
				// that didnt work, give it a new ACL altogether
				ActionObserver.NotifyWarning("Cannot change ownership, resetting entire file security '{0}'", file);
				FileSecurity newSecurity = new FileSecurity();
				newSecurity.SetOwner(AdministratorsSID);
				var fileInfo = new FileInfo(file);
				fileInfo.SetAccessControl(newSecurity);
			}
		} catch (Exception e) {
			ActionObserver.NotifyActionFailed("Take Ownership", "File", file, string.Empty, e.ToDisplayString());
		}
	}

	#region Auxillary methods

	private static string ConstructDestinationDirectoryName(string sourceDir, string destParentDirectory) {
		string destDir = string.Empty;
		if (destParentDirectory.EndsWith("\\")) {
			destDir = string.Format(
				"{0}{1}",
				destParentDirectory,
				sourceDir.GetLeafDirectory()
			);
		} else {
			destDir = string.Format(
				"{0}{1}{2}",
				destParentDirectory,
				Path.DirectorySeparatorChar,
				sourceDir.GetLeafDirectory()
			);
		}
		return destDir;
	}

	private static string ConstructDestinationFileName(string sourceFile, string destParentDir) {
		string destFileName = string.Empty;
		if (destParentDir.EndsWith("\\")) {
			destFileName = string.Format(
				"{0}{1}",
				destParentDir,
				Path.GetFileName(sourceFile)
			);
		} else {
			destFileName = string.Format(
				"{0}{1}{2}",
				destParentDir,
				Path.DirectorySeparatorChar,
				Path.GetFileName(sourceFile)
			);
		}
		return destFileName;
	}


	private void GetDirectoryFiles(string directory, out string[] legalFilenames, out string[] veryLongFilenames) {
		List<string> legalFilenamesList = new List<string>();
		List<string> veryLongFilenamesList = new List<string>();
		legalFilenames = veryLongFilenames = new string[0];
		string[] filenames = GetDirectoryFiles(directory);
		foreach (string filename in filenames) {
			if (IsVeryLongFilePath(filename)) {
				veryLongFilenamesList.Add(filename);
			} else {
				legalFilenamesList.Add(filename);
			}
		}
		legalFilenames = legalFilenamesList.ToArray();
		veryLongFilenames = veryLongFilenamesList.ToArray();
	}

	private bool FileExists(string path) {
		if (!path.IsUNCPath() && IsVeryLongDirectoryPath(path)) {
			return UnicodeNamedFile.Exists(path);
		}
		return File.Exists(path);
	}

	private bool DirectoryExists(string path) {
		if (!path.IsUNCPath() && IsVeryLongDirectoryPath(path)) {
			return UnicodeNamedDirectory.Exists(path);
		}
		return Directory.Exists(path);
	}


	private bool IsVeryLongDirectoryPath(string path) {
		return path.Length > 248;
	}

	private bool IsVeryLongFilePath(string path) {
		return path.Length > 260;
	}


	private string[] GetDirectoryFiles(string directory) {
		string[] files = new string[0];
		try {
			files = GetDirectoryFilesInternal(directory);
		} catch (UnauthorizedAccessException) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					GiveLocalAdministratorsOwnershipOfDirectory(directory);
					GrantFullAccessToDirectory(directory);
					files = GetDirectoryFilesInternal(directory);
				} catch {
					if (!ContinueOnAllErrors) {
						throw;
					}
				}
			} else if (!ContinueOnAllErrors) {
				throw;
			}
		} catch (Exception) {
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
		return files;
	}

	private string[] GetSubDirectories(string directory) {
		string[] subDirectories = new string[0];
		try {
			subDirectories = GetDirectoryDirectoriesInternal(directory);
		} catch (UnauthorizedAccessException) {
			if (ForceOwnershipOnAccessDenied) {
				try {
					GiveLocalAdministratorsOwnershipOfDirectory(directory);
					GrantFullAccessToDirectory(directory);
					subDirectories = Directory.GetDirectories(directory);
				} catch (Exception e) {
					if (!ContinueOnAllErrors) {
						throw;
					} else {
						ActionObserver.NotifyError("Failed to enumerate subdirectories under '{0}'. {1}.'", directory, e.ToDisplayString());
					}
				}
			} else if (!ContinueOnAllErrors) {
				throw;
			}
		} catch (Exception error) {
			ActionObserver.NotifyError("Failed to enumerate subdirectories under '{0}'. {1}.", directory, error.ToDisplayString());
			if (!ContinueOnAllErrors) {
				throw;
			}
		}
		return subDirectories;
	}


	private void CopyFileInternal(string sourceFile, string destFile) {
		if (IsVeryLongFilePath(sourceFile) || IsVeryLongFilePath(destFile)) {
			UnicodeNamedFile.Copy(sourceFile, destFile);
		} else {
			File.Copy(sourceFile, destFile, true);
			File.SetAttributes(destFile, FileAttributes.Normal);
		}
	}

	private string[] GetDirectoryFilesInternal(string path) {
		if (IsVeryLongDirectoryPath(path)) {
			return UnicodeNamedDirectory.GetFiles(path);
		} else {
			return Directory.GetFiles(path);
		}
	}

	private string[] GetDirectoryDirectoriesInternal(string path) {
		if (IsVeryLongDirectoryPath(path)) {
			return UnicodeNamedDirectory.GetDirectories(path);
		} else {
			return Directory.GetDirectories(path);
		}
	}

	private void CreateDirectoryInternal(string directoryPath) {
		if (IsVeryLongDirectoryPath(directoryPath)) {
			UnicodeNamedDirectory.Create(directoryPath);
		} else {
			Directory.CreateDirectory(directoryPath);
		}
	}

	private void DeleteDirectoryInternal(string path) {
		if (IsVeryLongDirectoryPath(path)) {
			UnicodeNamedDirectory.Delete(path);
		} else {
			Directory.Delete(path);
		}
	}

	private void DeleteFileInternal(string file) {
		if (IsVeryLongFilePath(file)) {
			UnicodeNamedFile.Delete(file);
		} else {
			File.Delete(file);
		}
	}

	/// <summary>
	/// Translates directory ACL but puts owner as Administrators
	/// </summary>
	/// <param name="sourceSecurity"></param>
	/// <returns></returns>
	private DirectorySecurity TranslateDirectoryACL(DirectorySecurity sourceSecurity) {
		DirectorySecurity destSecurity = new DirectorySecurity();
		Dictionary<string, string> danglingSids = new Dictionary<string, string>();

		// 1. Get sddl string
		string sourceSDDL = sourceSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);

		// 2. Gather table of sid translations
		List<AuthorizationRule> authorizationRules = new List<AuthorizationRule>();
		authorizationRules.AddRange(sourceSecurity.GetAccessRules(true, false, typeof(NTAccount)).Cast<AuthorizationRule>().ToArray());
		authorizationRules.AddRange(sourceSecurity.GetAuditRules(true, false, typeof(NTAccount)).Cast<AuthorizationRule>().ToArray());
		danglingSids = AssembleSidTranslationTable(authorizationRules);

		// apply dest sid on new file
		string destSDDL = sourceSDDL;
		foreach (string danglingSID in danglingSids.Keys) {
			destSDDL = destSDDL.Replace(danglingSID, danglingSids[danglingSID]);
		}

		destSecurity.SetSecurityDescriptorSddlForm(destSDDL);

		destSecurity.SetOwner(AdministratorsSID);
		return destSecurity;
	}

	private FileSecurity TranslateFileACL(FileSecurity sourceSecurity) {
		FileSecurity destSecurity = new FileSecurity();
		Dictionary<string, string> danglingSids = new Dictionary<string, string>();

		// 1. Get sddl string
		string sourceSDDL = sourceSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);

		// 2. Gather table of sid translations
		List<AuthorizationRule> authorizationRules = new List<AuthorizationRule>();
		authorizationRules.AddRange(sourceSecurity.GetAccessRules(true, false, typeof(NTAccount)).Cast<AuthorizationRule>().ToArray());
		authorizationRules.AddRange(sourceSecurity.GetAuditRules(true, false, typeof(NTAccount)).Cast<AuthorizationRule>().ToArray());
		danglingSids = AssembleSidTranslationTable(authorizationRules);

		// apply dest sid on new file
		string destSDDL = sourceSDDL;
		foreach (string danglingSID in danglingSids.Keys) {
			destSDDL = destSDDL.Replace(danglingSID, danglingSids[danglingSID]);
		}

		destSecurity.SetSecurityDescriptorSddlForm(destSDDL);

		destSecurity.SetOwner(AdministratorsSID);

		return destSecurity;
	}

	private Dictionary<string, string> AssembleSidTranslationTable(IEnumerable<AuthorizationRule> authorizationRules) {
		var danglingSids = new Dictionary<string, string>();
		foreach (var authorizationRule in authorizationRules) {
			// if we encounter a SID and not an account name, it means it could not be resolved
			if (authorizationRule.IdentityReference is SecurityIdentifier) {
				// rule references a dangling id, try to resolve it to remote machine name
				var remoteSID = authorizationRule.IdentityReference.Value;
				string remoteName;
				string remoteHost;
				WinAPI.ADVAPI32.SidNameUse remoteNameUse;
				if (Resolver.TryResolve(remoteSID, out remoteHost, out remoteName, out remoteNameUse)) {

					// 2.1 Translate dangling user to equivalent name on local server
					if (!danglingSids.ContainsKey(remoteSID)) {
						string translatedName;

						// translate to a local name
						if (!SidTranslator.TryTranslate(
							    remoteHost,
							    remoteName,
							    remoteNameUse,
							    out translatedName)) {

							// couldn't translate it, or import anything, so just default to administrators
							ActionObserver.NotifyWarning("Unable to translate/import remote {0} '{1}\\{2}'", remoteNameUse, remoteHost, remoteName);
							translatedName = "Administrators";
						}

						var localAccount = new NTAccount(
							translatedName
						);

						var translatedUserSid = localAccount.Translate(
							typeof(SecurityIdentifier)
						).Value;

						ActionObserver.NotifyAction("Translating", "SID", remoteSID, translatedUserSid);
						if (!danglingSids.ContainsKey(remoteSID)) {
							danglingSids.Add(remoteSID, translatedUserSid);
						}
					}

				} else {
					// replace this SID with administrators
					var localAccount = new NTAccount("Administrators");
					string translatedUserSid = localAccount.Translate(
						typeof(SecurityIdentifier)
					).Value;


					ActionObserver.NotifyWarning("Danging SID '{0}' identified, using Administrators group sid", remoteSID);
					if (!danglingSids.ContainsKey(remoteSID)) {
						danglingSids.Add(remoteSID, translatedUserSid);
					}

				}

			}
		}
		return danglingSids;
	}

	private SecurityIdentifier TranslateSID(SecurityIdentifier sid) {
		var translatedSID = sid;
		if (!this.Resolver.IsSIDResolvableWithoutLookup(sid)) {
			// it is a remote SID, so try to resolve it
			string remoteHost, remoteName;
			WinAPI.ADVAPI32.SidNameUse remoteNameUse;

			// resolved it to remote name
			if (_resolver.TryResolve(sid.Value, out remoteHost, out remoteName, out remoteNameUse)) {
				// translate remote name to a local name
				string translatedName;
				if (SidTranslator.TryTranslate(
					    remoteHost,
					    remoteName,
					    remoteNameUse,
					    out translatedName)) {
					// translated to local or domain account, so get that SID
					translatedSID = (SecurityIdentifier)(new NTAccount(translatedName)).Translate(typeof(SecurityIdentifier));

					//Log.Info("Translated SID '{0}' to '{1}'", sid.Value, translatedSID.Value);
				} else {
					// default to administrators account
					ActionObserver.NotifyWarning("Danging SID '{0}' identified, using Administrators group sid", sid.ToString());
					translatedSID = AdministratorsSID;
					//Log.Info("Defaulted SID '{0}' to Administrators group", sid.Value);
				}
			}
		}
		return translatedSID;
	}

	#endregion

}
