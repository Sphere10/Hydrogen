using System;

namespace Sphere10.Framework {
	public interface IFragmentProvider {
		/// <summary>
		/// Fragment count
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Retrieve fragment at index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		Span<byte> GetFragment(int index);

		/// <summary>
		/// Requests new fragments for the requested space. Guaranteed allocation if returns true;
		/// </summary>
		/// <param name="bytes">Requested new space</param>
		/// <param name="newFragmentIndexes">New fragments</param>
		/// <returns></returns>
		bool TryRequestSpace(int bytes, out int[] newFragmentIndexes);

		/// <summary>
		/// Defensive release of space from fragments. Not guaranteed to release requested bytes.
		/// </summary>
		/// <param name="bytes">Number of bytes to release</param>
		/// <param name="releasedFragmentIndexes">Index of fragments released (should be right-most neighbourhood)</param>
		/// <returns>Number of bytes actually released</returns>
		int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes);
	}
}
