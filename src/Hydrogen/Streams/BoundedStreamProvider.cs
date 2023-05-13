// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen {

	public class BoundedStreamProvider : IStreamProvider {

		public BoundedStreamProvider(Stream sourceStream, long startPostion, long endPosition, bool ownsStream = false) {
			OwnsStream = ownsStream;
			Stream =  new BoundedStream(sourceStream, startPostion, endPosition);
		}

		protected BoundedStream Stream { get; }

		protected bool OwnsStream { get; }

		public virtual Stream OpenReadStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream( Stream );
		}

		public virtual Stream OpenWriteStream() {
			Stream.Seek(Stream.MinAbsolutePosition, SeekOrigin.Begin);
			return new NonClosingStream( Stream );
		}

		public virtual void Dispose() {
			if (OwnsStream)
				Stream.Dispose();
		}
	}

}