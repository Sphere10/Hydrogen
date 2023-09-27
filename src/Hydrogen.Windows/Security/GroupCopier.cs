// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;
using System.Linq;

namespace Hydrogen.Windows.Security;

public class GroupCopier {
	bool _copyLocalMembership;
	bool _importLocalUsers;
	bool _copyDomainMembership;
	string _defaultUserPassword;
	bool _applyMembershipDeletions;
	IActionObserver _actionObserver;

	public GroupCopier(bool copyLocalMembership, bool importLocalUsers, bool copyDomainMembership, string defaultUserPassword, bool applyMembershipDeletions)
		: this(copyLocalMembership, importLocalUsers, copyDomainMembership, defaultUserPassword, applyMembershipDeletions, null) {
	}

	public GroupCopier(bool copyLocalMembership, bool importLocalUsers, bool copyDomainMembership, string defaultUserPassword, bool applyMembershipDeletions, IActionObserver actionObserver) {
		Debug.Assert(defaultUserPassword != null);
		_copyLocalMembership = copyLocalMembership;
		_importLocalUsers = importLocalUsers;
		_copyDomainMembership = copyDomainMembership;
		_defaultUserPassword = defaultUserPassword;
		_applyMembershipDeletions = applyMembershipDeletions;
		_actionObserver = (actionObserver == null) ? SecurityTool.DefaultActionObserver : actionObserver;
	}


	public void CopyGroup(NTLocalGroup[] groups) {
		foreach (NTLocalGroup group in groups) {
			CopyRemoteGroupToLocalMachine(group);
		}
	}

	public NTLocalGroup CopyRemoteGroupToLocalMachine(NTLocalGroup remoteGroup) {
		ActionObserver.NotifyAction("Querying", "Group", remoteGroup.Name, string.Empty);
		NTLocalGroup localGroup = null;
		if (!NTHost.CurrentMachine.TryGetLocalGroup(remoteGroup.Name, out localGroup)) {
			ActionObserver.NotifyAction("Creating", "Group", NTHost.CurrentMachine.Name + '\\' + remoteGroup.Name, string.Empty);
			localGroup = NTHost.CurrentMachine.CreateLocalGroup(
				remoteGroup.Name,
				remoteGroup.Description
			);
		} else {
			ActionObserver.NotifyAction("Copying", "Group", localGroup.FullName, remoteGroup.FullName);
			localGroup.Description = remoteGroup.Description;
			localGroup.Update();
		}

		Debug.Assert(localGroup != null);

		NTObject[] localMembers = localGroup.GetMembers();
		if (_applyMembershipDeletions) {
			foreach (NTObject obj in localMembers) {
				ActionObserver.NotifyAction("Deleting", "Group Membership", localGroup.FullName, obj.FullName);
				localGroup.DeleteMember(obj);
			}
		}

		if (_copyLocalMembership) {
			CopyLocalGroupMembership(remoteGroup, localGroup, localMembers);
		}

		if (_copyDomainMembership) {
			CopyRemoteGroupMembership(remoteGroup, localGroup, localMembers);
		}

		return localGroup;
	}

	public void CopyLocalGroupMembership(NTLocalGroup sourceRemoteGroup, NTLocalGroup destinationLocalGroup, NTObject[] destinationLocalGroupMembers) {
		ActionObserver.NotifyAction("Copy", "Local Group Membership", sourceRemoteGroup.FullName, destinationLocalGroup.FullName);
		TextBank existingMembers = new TextBank(
			from obj in destinationLocalGroupMembers
			where obj is NTLocalObject
			select (object)obj
		);

		UserCopier userCopier = new UserCopier(
			false,
			false,
			DefaultUserPassword,
			false,
			ActionObserver
		);

		foreach (NTLocalUser remoteUser in sourceRemoteGroup.GetLocalMembers()) {
			NTLocalUser localUser = null;

			// find a user by the same name of local machine
			if (!NTHost.CurrentMachine.TryGetLocalUser(remoteUser.Name, out localUser)) {
				// import the user if required
				if (_importLocalUsers) {
					localUser = userCopier.CopyRemoteUserToLocalMachine(remoteUser);
				}
			}

			if (localUser != null) {
				if (!existingMembers.ContainsText(localUser.Name)) {
					destinationLocalGroup.AddLocalMember(localUser);
				}
			}
		}
	}

	public void CopyRemoteGroupMembership(NTLocalGroup sourceRemoteGroup, NTLocalGroup destinationLocalGroup, NTObject[] destinationLocalGroupMembers) {
		ActionObserver.NotifyAction("Copy", "Remote Group Membership", sourceRemoteGroup.FullName, destinationLocalGroup.FullName);
		TextBank existingMembers = new TextBank(
			from obj in destinationLocalGroupMembers
			where obj is NTObject
			select (object)obj
		);


		#region debug region

		NTRemoteObject[] tmpObjects = sourceRemoteGroup.GetRemoteMembers();
		if (tmpObjects == null) {
			tmpObjects = new NTRemoteObject[0];
		}
		ActionObserver.NotifyInformation("Detected a total of {0} members for remote local group {1}", tmpObjects.Length, sourceRemoteGroup.FullName);
		foreach (NTRemoteObject remoteObject in tmpObjects) {
			ActionObserver.NotifyInformation("Member '{0}' its type is {1}", remoteObject.FullName, remoteObject.GetType().FullName);
		}

		#endregion


		NTRemoteObject[] remoteObjects = sourceRemoteGroup.GetRemoteMembers();
		if (remoteObjects == null) {
			remoteObjects = new NTRemoteObject[0];
		}
		ActionObserver.NotifyInformation("Detected a total of {0} AD members for remote local group {1}", remoteObjects.Length, sourceRemoteGroup.FullName);
		foreach (NTRemoteObject remoteObject in remoteObjects) {
			ActionObserver.NotifyInformation("AD Member '{0}' its is {1}", remoteObject.FullName, remoteObject.GetType().FullName);
		}



		foreach (NTRemoteObject remoteObject in remoteObjects) {
			string remoteObjectName = remoteObject.Domain + "\\" + remoteObject.Name;
			if (!existingMembers.ContainsText(remoteObjectName)) {
				destinationLocalGroup.AddMember(remoteObjectName);
			}
		}
	}

	public bool CopyLocalMembership {
		get { return _copyLocalMembership; }
		set { _copyLocalMembership = value; }
	}

	public bool ImportLocalUsers {
		get { return _importLocalUsers; }
		set { _importLocalUsers = value; }
	}

	public bool CopyDomainMembership {
		get { return _copyDomainMembership; }
		set { _copyDomainMembership = value; }
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

}
