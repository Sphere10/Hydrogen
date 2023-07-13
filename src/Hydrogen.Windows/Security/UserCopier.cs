// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;


namespace Hydrogen.Windows.Security;

public class UserCopier {
	bool _copyGroupMembership;
	bool _importLocalGroups;
	string _defaultUserPassword;
	bool _applyMembershipDeletions;
	IActionObserver _actionObserver;


	public UserCopier(bool copyGroupMembership, bool importLocalGroups, string defaultPassword, bool applyMembershipDeletions)
		: this(copyGroupMembership, importLocalGroups, defaultPassword, applyMembershipDeletions, SecurityTool.DefaultActionObserver) {
	}

	public UserCopier(bool copyGroupMembership, bool importLocalGroups, string defaultPassword, bool applyMembershipDeletions, IActionObserver actionObserver) {
		CopyGroupMembership = copyGroupMembership;
		ImportLocalGroups = importLocalGroups;
		DefaultUserPassword = defaultPassword;
		ApplyMembershipDeletions = applyMembershipDeletions;
		_actionObserver = (actionObserver == null) ? SecurityTool.DefaultActionObserver : actionObserver;
	}

	public bool CopyGroupMembership {
		get { return _copyGroupMembership; }
		set { _copyGroupMembership = value; }
	}

	public bool ImportLocalGroups {
		get { return _importLocalGroups; }
		set { _importLocalGroups = value; }
	}

	public string DefaultUserPassword {
		get { return _defaultUserPassword; }
		set { _defaultUserPassword = value; }
	}

	public bool ApplyMembershipDeletions {
		get { return _applyMembershipDeletions; }
		set { _applyMembershipDeletions = value; }
	}

	public IActionObserver ActionObserver {
		get { return _actionObserver; }
		set { _actionObserver = value; }
	}

	public NTLocalUser CopyRemoteUserToLocalMachine(NTLocalUser remoteUser) {

		#region Validation

		Debug.Assert(remoteUser.Host.ToUpper() != NTHost.CurrentMachine.Name.ToUpper());
		if (remoteUser.Host.ToUpper() == NTHost.CurrentMachine.Name.ToUpper()) {
			throw new WindowsException(
				"Cannot copy user '{0}\\{1}' onto itself",
				remoteUser.Host,
				remoteUser.Name
			);
		}

		#endregion


		NTLocalUser localUser;

		ActionObserver.NotifyAction("Querying", "User", remoteUser.Name, string.Empty);
		if (!NTHost.CurrentMachine.TryGetLocalUser(remoteUser.Name, out localUser)) {
			ActionObserver.NotifyAction("Creating", "User", NTHost.CurrentMachine.Name + '\\' + remoteUser.Name, string.Empty);
			localUser = NTHost.CurrentMachine.CreateLocalUser(
				remoteUser.Name,
				DefaultUserPassword
			);
		}

		CopyUserData(remoteUser, localUser);

		if (_copyGroupMembership) {
			CopyUserMembership(remoteUser, localUser);
		}

		return localUser;
	}

	public void CopyUserData(NTLocalUser sourceRemoteUser, NTLocalUser destLocalUser) {
		ActionObserver.NotifyAction("Copying", "User", sourceRemoteUser.FullName, destLocalUser.FullName);
		destLocalUser.FullName = sourceRemoteUser.FullName;
		destLocalUser.AccountExpires = sourceRemoteUser.AccountExpires;
		destLocalUser.CodePage = sourceRemoteUser.CodePage;
		destLocalUser.Comment = sourceRemoteUser.Comment;
		destLocalUser.CountryCode = sourceRemoteUser.CountryCode;
		destLocalUser.Description = sourceRemoteUser.Description;
		destLocalUser.Flags = sourceRemoteUser.Flags;
		destLocalUser.HomeDirectory = sourceRemoteUser.HomeDirectory;
		destLocalUser.LogonHours = sourceRemoteUser.LogonHours;
		destLocalUser.LogonServer = sourceRemoteUser.LogonServer;
		destLocalUser.MaxStorage = sourceRemoteUser.MaxStorage;
		destLocalUser.ScriptPath = sourceRemoteUser.ScriptPath;
		destLocalUser.UnitsPerWeek = sourceRemoteUser.UnitsPerWeek;
		destLocalUser.Workstations = sourceRemoteUser.Workstations;
		destLocalUser.Update();
	}

	public void CopyUserMembership(NTLocalUser sourceRemoteUser, NTLocalUser destLocalUser) {
		ActionObserver.NotifyAction("Copying", "User Membership", sourceRemoteUser.FullName, destLocalUser.FullName);
		TextBank existingMembers = new TextBank(destLocalUser.GetMembership());
		GroupCopier groupCopier = new GroupCopier(false, false, true, DefaultUserPassword, false, ActionObserver);

		foreach (NTLocalGroup remoteGroup in sourceRemoteUser.GetMembership()) {

			if (!existingMembers.ContainsText(remoteGroup.Name)) {

				NTLocalGroup localGroup = null;

				// find a user by the same name of local machine
				if (!NTHost.CurrentMachine.TryGetLocalGroup(remoteGroup.Name, out localGroup)) {
					if (_importLocalGroups) {
						groupCopier.CopyRemoteGroupToLocalMachine(remoteGroup);
					}
				}

				if (localGroup != null) {
					if (!existingMembers.ContainsText(localGroup.Name)) {
						destLocalUser.AddMembership(localGroup);
					}
				}

			}
		}



	}

}
