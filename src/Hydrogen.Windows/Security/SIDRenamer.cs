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
using System.Security.Principal;
using System.IO;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace Hydrogen.Windows.Security;

public class SIDRenamer {
	List<Tuple<SecurityIdentifier, SecurityIdentifier>> _replaceList;
	IActionObserver _actionObserver;
	string _hostname;

	public SIDRenamer(string hostname, List<Tuple<SecurityIdentifier, SecurityIdentifier>> list)
		: this(hostname, list, SecurityTool.DefaultActionObserver) {
	}

	public SIDRenamer(
		string hostname,
		List<Tuple<SecurityIdentifier, SecurityIdentifier>> list,
		IActionObserver actionObserver) {
		Debug.Assert(hostname != null);
		Debug.Assert(list != null);
		Debug.Assert(actionObserver != null);
		ReplaceList = list;
		ActionObserver = actionObserver;
		Hostname = hostname;
	}

	public string Hostname {
		get { return _hostname; }
		set { _hostname = value; }
	}

	public List<Tuple<SecurityIdentifier, SecurityIdentifier>> ReplaceList {
		get { return _replaceList; }
		set { _replaceList = value; }
	}

	public IActionObserver ActionObserver {
		get { return _actionObserver; }
		set { _actionObserver = value; }
	}

	#region Replace

	public void ReplaceEntireRegistry() {
		if (Tools.WinTool.KeyExists(Hostname, "HKEY_CLASSES_ROOT")) {
			ReplaceRegistry("HKEY_CLASSES_ROOT", true);
		}
		if (Tools.WinTool.KeyExists(Hostname, "HKEY_CURRENT_USER")) {
			ReplaceRegistry("HKEY_CURRENT_USER", true);
		}

		if (Tools.WinTool.KeyExists(Hostname, "HKEY_LOCAL_MACHINE")) {
			ReplaceRegistry("HKEY_LOCAL_MACHINE", true);
		}

		if (Tools.WinTool.KeyExists(Hostname, "HKEY_USERS")) {
			ReplaceRegistry("HKEY_USERS", true);
		}

		if (Tools.WinTool.KeyExists(Hostname, "HKEY_CURRENT_CONFIG")) {
			ReplaceRegistry("HKEY_CURRENT_CONFIG", true);
		}

		if (Tools.WinTool.KeyExists(Hostname, "HKEY_DYN_DATA")) {
			ReplaceRegistry("HKEY_DYN_DATA", true);
		}
	}

	public void ReplaceRegistry(string rootKey, bool recursive) {
		ActionObserver.NotifyAction("Search/Replace SID", "Registry", rootKey, Hostname);
		try {
			RegistryKey key = Tools.WinTool.OpenKey(Hostname, rootKey);
			try {
				RegistrySecurity security = key.GetAccessControl(AccessControlSections.All);
				string sddl = security.GetSecurityDescriptorSddlForm(AccessControlSections.All);
				foreach (Tuple<SecurityIdentifier, SecurityIdentifier> item in ReplaceList) {
					string searchItem = item.Item1.ToString();
					string replaceItem = item.Item2.ToString();
					string newSddl = sddl.Replace(searchItem, replaceItem);
					if (newSddl != sddl) {
						ActionObserver.NotifyInformation(
							"RegistryKey '{0}' replaced '{1}' with '{2}'",
							rootKey,
							searchItem,
							replaceItem
						);
					}
					sddl = newSddl;
				}
				security.SetSecurityDescriptorSddlForm(sddl, AccessControlSections.All);
				key.SetAccessControl(security);
			} catch (Exception error) {
				ActionObserver.NotifyError("Unable to search SIDs on registry key '{0}' due to error '{1}'", rootKey, error.Message);
			}

			if (recursive) {
				foreach (string subKey in Tools.WinTool.GetSubKeys(Hostname, rootKey)) {
					if (Tools.WinTool.KeyExists(Hostname, subKey)) {
						ReplaceRegistry(subKey, recursive);
					} else {
						ActionObserver.NotifyWarning("Unable to access key '{0}'", subKey);
					}
				}
			}
		} catch (Exception error) {
			ActionObserver.NotifyError("Unable to search/replace SIDs on registry key '{0}' due to error='{1}'", rootKey, error.Message);
		}
	}

	public void ReplaceDirectory(string directory, bool recursive) {
		Debug.Assert(Directory.Exists(directory));
		ActionObserver.NotifyAction("Search/Replace SID", "Directory", directory, string.Empty);
		try {
			try {
				var directoryInfo = new DirectoryInfo(directory);
				DirectorySecurity security = directoryInfo.GetAccessControl(AccessControlSections.All);
				string sddl = security.GetSecurityDescriptorSddlForm(AccessControlSections.All);
				foreach (Tuple<SecurityIdentifier, SecurityIdentifier> item in ReplaceList) {
					string searchItem = item.Item1.ToString();
					string replaceItem = item.Item2.ToString();
					string newSddl = sddl.Replace(searchItem, replaceItem);
					if (newSddl != sddl) {
						ActionObserver.NotifyInformation(
							"Directory '{0}' replaced '{1}' with '{2}'",
							directory,
							searchItem,
							replaceItem
						);
					}
					sddl = newSddl;
				}
				security.SetSecurityDescriptorSddlForm(sddl, AccessControlSections.All);
				directoryInfo.SetAccessControl(security);

			} catch (Exception error) {
				ActionObserver.NotifyError("Unable to search SIDs on directory '{0}' due to error '{1}'", directory, error.Message);
			}

			try {
				// process files
				foreach (string file in Directory.GetFiles(directory)) {
					ReplaceFile(file);
				}
			} catch (Exception error) {
				ActionObserver.NotifyError("Unable to search SIDs on directory '{0}' due to error '{1}'", directory, error.Message);
			}

			// process sub directories first
			if (recursive) {
				foreach (string subDirectory in Directory.GetDirectories(directory)) {
					ReplaceDirectory(subDirectory, recursive);
				}
			}
		} catch (Exception error) {
			ActionObserver.NotifyError("Unable to search/replace SIDs on directory '{0}' due to '{1}'", directory, error.Message);

		}
	}

	public void ReplaceFile(string file) {
		Debug.Assert(File.Exists(file));
		ActionObserver.NotifyAction("Search/Replace SID", "File", file, string.Empty);
		try {
			var fileInfo = new FileInfo(file);
			FileSecurity security = fileInfo.GetAccessControl(AccessControlSections.All);
			string sddl = security.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			foreach (Tuple<SecurityIdentifier, SecurityIdentifier> item in ReplaceList) {
				string searchItem = item.Item1.ToString();
				string replaceItem = item.Item2.ToString();
				string newSddl = sddl.Replace(searchItem, replaceItem);
				if (newSddl != sddl) {
					ActionObserver.NotifyInformation(
						"File '{0}' replaced '{1}' with '{2}'",
						file,
						searchItem,
						replaceItem
					);
				}
				sddl = newSddl;
			}
			security.SetSecurityDescriptorSddlForm(sddl, AccessControlSections.All);
			fileInfo.SetAccessControl(security);
		} catch (Exception error) {
			ActionObserver.NotifyError("Unable to search SIDs on file '{0}' due to error '{1}'", file, error.Message);
		}

	}

	#endregion

}
