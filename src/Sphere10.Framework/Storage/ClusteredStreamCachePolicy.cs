namespace Sphere10.Framework {
	public enum ClusteredStreamCachePolicy {
		/// <summary>
		///  Clusters next/prev pointers are always read from root stream
		/// </summary>
		None,

		/// <summary>
		/// Cluster locations are remembered as as they're read from root stream
		/// </summary>
		Remember,

		/// <summary>
		/// Clusters locations are pre-read on open
		/// </summary>
		Scan

	}
}
