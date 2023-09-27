// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;
using System.Linq;


namespace Hydrogen.Windows.Security;

public class ShareCopier {
	public ShareCopier() : this(null, false, string.Empty, false, true, SecurityTool.DefaultActionObserver) {
	}

	public ShareCopier(IActionObserver observer) : this(null, false, string.Empty, false, true, observer) {
	}

	public ShareCopier(string sourceHost, bool importUsersAndGroups, string defaultUserPassword, bool autoResolveShareNameConflicts, bool renameOldShareWhenCopyingToSameMachine, IActionObserver observer)
		: this(
			string.IsNullOrEmpty(sourceHost) ? new SidNameResolver() : new SidNameResolver(sourceHost),
			new SidTranslator(importUsersAndGroups, defaultUserPassword, observer),
			autoResolveShareNameConflicts,
			observer
		) {
	}

	public ShareCopier(SidNameResolver nameResolver, SidTranslator sidTranslator, bool autoResolveShareNameConflicts, IActionObserver observer) {
		Resolver = nameResolver;
		SidTranslator = sidTranslator;
		AutoResolveShareNameConflicts = autoResolveShareNameConflicts;
		ActionObserver = observer;
	}

	public void CopyShare(NTShare sourceShare, string destPath) {
		CopyShare(new CopyableShare(sourceShare, destPath));
	}

	public void CopyShare(CopyableShare copyableShare) {

		#region Validation

		if (!Directory.Exists(copyableShare.DestPath)) {
			ActionObserver.NotifyError("Cannot copy share '{0}' to '{1}' as the destination path does not exist", copyableShare.SourceShare.FullName, copyableShare.DestPath);
			return;
		}

		#endregion

		NTShare sourceShare = copyableShare.SourceShare;
		string destPath = copyableShare.DestPath;

		ActionObserver.NotifyAction("Copying", "Share", sourceShare.FullName, destPath);
		try {
			NTShare destShare;

			// try to load share at destination server (assumed to be localhost), by name
			if (!NTHost.CurrentMachine.TryGetShare(sourceShare.Name, out destShare)) {
				// one doesn't exist so create one
				destShare = NTHost.CurrentMachine.CreateShare(
					sourceShare.Name,
					sourceShare.Description,
					sourceShare.Type,
					destPath
				);
			} else {
				// make sure we loaded one pointing to same directory as we expect
				if (!Path.Equals(destShare.ServerPath, copyableShare.DestPath)) {
					// in this branch, a different share of the same name already exists 
					if (AutoResolveShareNameConflicts) {
						string newName = ResolveShareNameConflict(NTHost.CurrentMachine, sourceShare.Name);
						ActionObserver.NotifyWarning(
							"Copying share '{0}\\{1}' as '{2}\\{3}' to avoid name conflict",
							copyableShare.SourceShare.Host,
							copyableShare.SourceShare.Name,
							NTHost.CurrentMachine.Host,
							newName
						);

						// we want to auto-create a share name
						destShare = NTHost.CurrentMachine.CreateShare(
							newName,
							sourceShare.Description,
							sourceShare.Type,
							destPath
						);
					} else {
						// in this branch, bail out of share copying as share name already taken
						ActionObserver.NotifyError(
							"Cannot copy the share '{0}\\{1}' to folder '{2}' as it will conflict with '{3}\\{4}' defined for folder '{5}'",
							copyableShare.SourceShare.Host,
							copyableShare.SourceShare.Name,
							copyableShare.DestPath,
							destShare.Host,
							destShare.Name,
							destShare.ServerPath
						);
					}
					return;
				}
			}

			// copy details
			destShare.Name = sourceShare.Name;
			destShare.Description = sourceShare.Description;
			destShare.MaxUses = sourceShare.MaxUses;
			destShare.Password = sourceShare.Password;
			destShare.Permissions = sourceShare.Permissions;
			destShare.Reserved = sourceShare.Reserved;
			destShare.ServerPath = destPath;
			destShare.Update();

			CopyShareSecurity(sourceShare, destShare);

			// create new share
		} catch (Exception error) {
			ActionObserver.NotifyActionFailed("Copying", "Share", copyableShare.SourceShare.FullName, copyableShare.DestPath, error.Message);
		}
	}

	public string ResolveShareNameConflict(NTHost host, string existingName) {
		while (host.ContainsShare(existingName)) {
			existingName += " Copy";
		}
		return existingName;
	}

	public void CopyShareSecurity(NTShare sourceShare, NTShare destShare) {
		ActionObserver.NotifyAction("Copying", "Share Security", sourceShare.FullName, destShare.FullName);
		try {
			ShareSecurity sourceSecurity = sourceShare.GetAccessControl();
			NTShare.SetAccessControl(
				destShare.FullName,
				TranslateShareACL(
					sourceShare.GetAccessControl()
				)
			);
		} catch (Exception error) {
			ActionObserver.NotifyActionFailed("Copying", "Share Security", sourceShare.FullName, destShare.FullName, error.Message);
		}
	}

	private ShareSecurity TranslateShareACL(ShareSecurity sourceSecurity) {
		ShareSecurity destSecurity = new ShareSecurity();
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
					var translatedUserSid = localAccount.Translate(
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

	#region Properties

	public bool AutoResolveShareNameConflicts { get; set; }


	public SidNameResolver Resolver { get; set; }

	public SidTranslator SidTranslator { get; set; }

	public IActionObserver ActionObserver { get; set; }

	#endregion

}
