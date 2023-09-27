// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class WithSubstitutionChecksummer<TItem> : ItemChecksummerDecorator<TItem> {
	private readonly int _reserved;
	private readonly int _substitution;
	public WithSubstitutionChecksummer(IItemChecksummer<TItem> innerChecksummer, int reservedChecksumValue, int substitutionChecksumValue)
		: base(innerChecksummer) {
	}

	public int ReservedChecksum => _reserved;

	public int SubstitutionChecksum => _substitution;

	public override int CalculateChecksum(TItem item) {
		var checksum = base.CalculateChecksum(item);
		if (checksum == _reserved) {
			return _substitution;
		}
		return checksum;
	}	
}
