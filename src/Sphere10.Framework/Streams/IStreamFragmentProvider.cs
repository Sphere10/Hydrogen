using System;

namespace Sphere10.Framework {
	
	public interface IStreamFragmentProvider {
		
		/// <summary>
		/// Length of fragment bytes
		/// </summary>
		long Length { get; }
		
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
		/// Retrieves the fragment which the global position / index corresponds to. 
		/// </summary>
		/// <param name="position"> index of position in total fragment space</param>
		/// <param name="fragment"> the fragment that</param>
		/// <returns> fragment index, or -1 if index outside of fragment space. if valid fragment, the corresponding position in fragment also returned.</returns>
		(int fragmentIndex, int fragmentPosition) GetFragment(long position, out Span<byte> fragment);

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
		/// <param name="releasedFragmentIndexes">Index of fragments released (should be right-most neighborhood)</param>
		/// <returns>Number of bytes actually released</returns>
		int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes);

		/// <summary>
		/// Update an existing fragment with the span bytes from the specified position.
		/// </summary>
		/// <param name="fragmentIndex"> fragment index</param>
		/// <param name="fragmentPosition"> fragment position</param>
		/// <param name="updateSpan"> span of bytes to update the fragment with</param>
		void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan);
	}
}
