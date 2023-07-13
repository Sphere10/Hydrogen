// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Security.AccessControl;
using System.Security.Principal;

namespace Hydrogen.Windows.Security;

public sealed class ShareAccessRule : AccessRule {
	// Methods
	public ShareAccessRule(IdentityReference identity, ShareRights shareRights, AccessControlType type)
		: this(identity, AccessMaskFromRights(shareRights, type), false, InheritanceFlags.None, PropagationFlags.None, type) {
	}

	public ShareAccessRule(string identity, ShareRights shareRights, AccessControlType type)
		: this(new NTAccount(identity), AccessMaskFromRights(shareRights, type), false, InheritanceFlags.None, PropagationFlags.None, type) {
	}

	public ShareAccessRule(IdentityReference identity, ShareRights shareRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		: this(identity, AccessMaskFromRights(shareRights, type), false, inheritanceFlags, propagationFlags, type) {
	}

	public ShareAccessRule(string identity, ShareRights shareRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		: this(new NTAccount(identity), AccessMaskFromRights(shareRights, type), false, inheritanceFlags, propagationFlags, type) {
	}

	internal ShareAccessRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags, type) {
	}

	internal static int AccessMaskFromRights(ShareRights shareRights, AccessControlType controlType) {
		return (int)shareRights;
	}

	internal static ShareRights RightsFromAccessMask(int accessMask) {
		return (ShareRights)accessMask;
	}

	// Properties
	public ShareRights ShareRights => RightsFromAccessMask(base.AccessMask);
}


public sealed class ShareAuditRule : AuditRule {
	// Methods
	public ShareAuditRule(IdentityReference identity, ShareRights shareRights, AuditFlags flags)
		: this(identity, shareRights, InheritanceFlags.None, PropagationFlags.None, flags) {
	}

	public ShareAuditRule(string identity, ShareRights shareRights, AuditFlags flags)
		: this(new NTAccount(identity), shareRights, InheritanceFlags.None, PropagationFlags.None, flags) {
	}

	public ShareAuditRule(IdentityReference identity, ShareRights shareRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		: this(identity, AccessMaskFromRights(shareRights), false, inheritanceFlags, propagationFlags, flags) {
	}

	public ShareAuditRule(string identity, ShareRights shareRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		: this(new NTAccount(identity), AccessMaskFromRights(shareRights), false, inheritanceFlags, propagationFlags, flags) {
	}

	internal ShareAuditRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags, flags) {
	}

	private static int AccessMaskFromRights(ShareRights shareRights) {
		return ShareAccessRule.AccessMaskFromRights(shareRights, AccessControlType.Allow);
		;
	}

	// Properties
	public ShareRights ShareRights {
		get { return ShareAccessRule.RightsFromAccessMask(base.AccessMask); }
	}
}
