﻿using System;

namespace Hydrogen {

	/// <summary>
	/// Used by <see cref="FragmentedStream"/> to map a set of non-contiguous byte arrays (fragments) into a logically contiguous stream of bytes.
	/// 
	/// For example, consider how a file is stored on a file-system over random clusters yet the OS assembles that data into a logical contiguous stream of bytes for applications.
	/// Similarly, a <see cref="FragmentedStream"/> can provide the client a logically contiguous stream of bytes when the underlying store is fragmented arbitrarily. Implementations
	/// of this handle the fragments for <see cref="FragmentedStream"/>.
	/// </summary>
	public interface IStreamFragmentProvider {
		
		/// <summary>
		/// Sum of all fragment bytes
		/// </summary>
		long TotalBytes { get; }
		
		/// <summary>
		/// Fragment count
		/// </summary>
		int FragmentCount { get; }

		/// <summary>
		/// Retrieves a fragment's content.
		/// </summary>
		/// <param name="fragmentIndex">fragment index</param>
		/// <returns></returns>
		/// <remarks></remarks>
		ReadOnlySpan<byte> GetFragment(int fragmentIndex);

		/// <summary>
		/// Update an existing fragment with the span bytes from the specified position.
		/// </summary>
		/// <param name="fragmentIndex">fragment index</param>
		/// <param name="fragmentPosition"> fragment position</param>
		/// <param name="updateSpan"> span of bytes to update the fragment with</param>
		void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan);

		/// <summary>
		/// Maps a logical stream position to a fragment and index within fragment.
		/// </summary>
		/// <param name="position">logical stream position being resolved</param>
		/// <param name="fragmentIndex">index of fragment that <see cref="position"/> resolves to</param>
		/// <param name="fragmentPosition">position within fragment at <see cref="fragmentIndex"/> that <see cref="position"/> resolves to</param>
		/// <returns>Whether <see cref="position"/> could be mapped to a fragment</returns>
		bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition);

		/// <summary>
		/// When the <see cref="FragmentedStream"/> resizes, the fragment provider needs to allocate (or deallocate) fragments.
		/// </summary>
		/// <param name="bytes">Requested new space</param>
		/// <param name="newFragments">New fragments</param>
		/// <returns>Whether fragment stores can accomodate total allocation</returns>
		bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments);

	}
}
