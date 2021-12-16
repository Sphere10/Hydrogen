namespace Sphere10.Hydrogen.Core.Blockchain {
	public enum ApplyBlockResult {
		/// <summary>
		/// Block was appended to the blockchain successfully
		/// </summary>
		Success,

		/// <summary>
		/// Block was not appended to the blockchain due to invalid data
		/// </summary>
		InvalidBlock,

		/// <summary>
		/// Block was not appended because it did not satisfy the leader consensus rule (i.e. not enough work in PoW, or wrong staker in PoS, etc)
		/// </summary>
		InvalidLeader
	}
}
