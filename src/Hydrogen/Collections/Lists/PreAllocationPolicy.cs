namespace Hydrogen;

public enum PreAllocationPolicy {
	/// <summary>
	/// The initial block of pre-allocated items is used, never grown or reduced.
	/// </summary>
	Fixed,

	/// <summary>
	/// The Capacity is grown in fixed-sized blocks as needed and never reduced.
	/// </summary>
	ByBlock,

	/// <summary>
	/// The Capacity is grown (and reduced) to meet the item Count.
	/// </summary>
	MinimumRequired,

}
