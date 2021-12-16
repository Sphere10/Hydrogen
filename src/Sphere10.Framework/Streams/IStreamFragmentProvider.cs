using System;

namespace Sphere10.Framework {

	public interface IStreamFragmentProvider {
		
		/// <summary>
		/// Number of bytes requested minus total released (not necessarily allocated).
		/// </summary>
		long TotalBytes { get; }
		
		/// <summary>
		/// Fragment count
		/// </summary>
		int FragmentCount { get; }

		/// <summary>
		/// Retrieve fragment at index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		Span<byte> GetFragment(int index);

		/// <summary>
		/// Maps a logical <see cref="position"/> to a fragment and a local index therein.
		/// </summary>
		/// <param name="position">logical position being resolved</param>
		/// <param name="fragment"> the fragment that the position resides in</param>
		/// <returns> fragment index (or -1 if index outside of fragment space). if valid fragment, the corresponding position in fragment also returned.</returns>
		(int fragmentIndex, int fragmentPosition) MapLogicalPositionToFragment(long position, out Span<byte> fragment);

		/// <summary>
		/// Requests new fragments for the requested space. Guaranteed allocation if returns true;
		/// </summary>
		/// <param name="bytes">Requested new space</param>
		/// <param name="newFragments">New fragments</param>
		/// <returns></returns>
		bool TryRequestSpace(int bytes, out int[] newFragments);

		/// <summary>
		/// Defensive release of space from fragments. Not guaranteed to release requested bytes.
		/// </summary>
		/// <param name="bytes">Number of bytes to release</param>
		/// <param name="releasedFragments">Index of fragments released (should be right-most neighborhood)</param>
		/// <returns>Number of bytes actually released</returns>
		int ReleaseSpace(int bytes, out int[] releasedFragments);

		/// <summary>
		/// Update an existing fragment with the span bytes from the specified position.
		/// </summary>
		/// <param name="fragmentIndex"> fragment index</param>
		/// <param name="fragmentPosition"> fragment position</param>
		/// <param name="updateSpan"> span of bytes to update the fragment with</param>
		void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan);
	}
}
