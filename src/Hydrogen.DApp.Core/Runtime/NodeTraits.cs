using System;

namespace Sphere10.Hydrogen.Core.Runtime {
	
	/// <summary>
	/// Describes the characteristics of a Hydrogen node.
	/// </summary>
	[Flags]
	public enum NodeTraits {
		/// <summary>
		/// Stores minimal data for light-client functionality.
		/// </summary>
		Light,

		/// <summary>
		/// Fully validating node, stores all data for consensus.
		/// </summary>
		Full,

		/// <summary>
		/// Archival node, stores the full history of network data.
		/// </summary>
		Archival,

		/// <summary>
		/// Mining server, can be used to mine blocks by mining clients.
		/// </summary>
		MiningServer,

		/// <summary>
		/// GUI server, permits GUI app to connect to for data.
		/// </summary>
		GUIServer,

		/// <summary>
		/// Node is hosted by a Hydrogen Host, which permits auto-upgrading.
		/// </summary>
		Hosted
	}

}
