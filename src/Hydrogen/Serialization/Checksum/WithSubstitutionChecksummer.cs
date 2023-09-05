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
