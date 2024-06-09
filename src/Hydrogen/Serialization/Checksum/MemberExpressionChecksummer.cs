// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ProjectionChecksummer<TItem, TMember> : IItemChecksummer<TItem> {
	
	private readonly Func<TItem, TMember> _projection;
	private readonly IItemChecksummer<TMember> _memberChecksummer;

	public ProjectionChecksummer(Func<TItem, TMember> projection, IItemChecksummer<TMember> memberChecksummer) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(memberChecksummer, nameof(memberChecksummer));
		_projection = projection;
		_memberChecksummer = memberChecksummer;
	}

	public int CalculateChecksum(TItem item) => _memberChecksummer.CalculateChecksum(_projection.Invoke(item));
}
