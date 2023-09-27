// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Hydrogen.Windows.Security;

[Flags]
public enum ShareRights {
	FileRead = 0x1, // user has read access
	FileWrite = 0x2, // user has write access
	FileCreate = 0x4 // user has create access
}


public sealed class ShareSecurity : NativeObjectSecurity {


	public ShareSecurity()
		: base(true, ResourceType.LMShare) {
	}

	[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	public ShareSecurity(string name, AccessControlSections includeSections)
		: base(
			true,
			ResourceType.LMShare,
			name,
			includeSections,
			new NativeObjectSecurity.ExceptionFromErrorCode(ShareSecurity._HandleErrorCode),
			null) {

	}

	private static Exception _HandleErrorCode(int errorCode, string name, SafeHandle handle, object context) {
		return new WindowsException(errorCode, "Could not access share '{0}'", name);
	}

	public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type) {
		return new ShareAccessRule(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, type);
		//            return new RegistryAccessRule(identityReference, accessMask, isInherited, inheritanceFlags, propagationFlags, type);
	}

	public void AddAccessRule(ShareAccessRule rule) {
		base.AddAccessRule(rule);
	}

	//public void AddAuditRule(ShareAuditRule rule) {
	//    base.AddAuditRule(rule);
	//}

	public override AuditRule AuditRuleFactory(
		IdentityReference identityReference,
		int accessMask,
		bool isInherited,
		InheritanceFlags inheritanceFlags,
		PropagationFlags propagationFlags,
		AuditFlags flags
	) {
		return new ShareAuditRule(
			identityReference,
			accessMask,
			isInherited,
			inheritanceFlags,
			propagationFlags,
			flags
		);
	}

	internal AccessControlSections GetAccessControlSectionsFromChanges() {
		AccessControlSections none = AccessControlSections.None;
		if (base.AccessRulesModified) {
			none = AccessControlSections.Access;
		}
		if (base.AuditRulesModified) {
			none |= AccessControlSections.Audit;
		}
		if (base.OwnerModified) {
			none |= AccessControlSections.Owner;
		}
		if (base.GroupModified) {
			none |= AccessControlSections.Group;
		}
		return none;
	}


	public bool RemoveAccessRule(ShareAccessRule rule) {
		return base.RemoveAccessRule(rule);
	}

	public void RemoveAccessRuleAll(ShareAccessRule rule) {
		base.RemoveAccessRuleAll(rule);
	}

	public void RemoveAccessRuleSpecific(ShareAccessRule rule) {
		base.RemoveAccessRuleSpecific(rule);
	}

	public void ResetAccessRule(ShareAccessRule rule) {
		base.ResetAccessRule(rule);
	}

	public void SetAccessRule(ShareAccessRule rule) {
		base.SetAccessRule(rule);
	}


	internal void Persist(string fullPath) {
		base.WriteLock();
		try {
			AccessControlSections accessControlSectionsFromChanges = this.GetAccessControlSectionsFromChanges();
			base.Persist(fullPath, accessControlSectionsFromChanges);
		} finally {
			base.WriteUnlock();
		}
	}


	// Properties
	public override Type AccessRightType {
		get { return typeof(ShareRights); }
	}

	public override Type AccessRuleType {
		get { return typeof(ShareAccessRule); }
	}

	public override Type AuditRuleType {
		get { return typeof(ShareAuditRule); }
	}
}
