//-----------------------------------------------------------------------
// <copyright file="SoundPlayerEx.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Hydrogen.Windows.Forms {

	public class SoundPlayerEx : IDisposable {
		private byte[] _bytesToPlay;
        GCHandle? _gcHandle;

		public SoundPlayerEx(Stream stream) {
			LoadStream(stream);
		}

		public void LoadStream(Stream stream) {
			byte[] bytesToPlay = new byte[stream.Length];
			stream.Read(bytesToPlay, 0, (int)stream.Length);
			this.BytesToPlay = bytesToPlay;
		}

		public void PlaySync() {
			FreeHandle();
			WinAPI.WINMM.SoundFlags flags = WinAPI.WINMM.SoundFlags.SND_SYNC;
			flags |= WinAPI.WINMM.SoundFlags.SND_MEMORY;
			WinAPI.WINMM.PlaySound(BytesToPlay, (UIntPtr)0, (uint)flags);
		}

		public void PlaySync(WinAPI.WINMM.SoundFlags flags) {
			FreeHandle();
			flags |= WinAPI.WINMM.SoundFlags.SND_SYNC;
			flags |= WinAPI.WINMM.SoundFlags.SND_MEMORY;
			WinAPI.WINMM.PlaySound(BytesToPlay, (UIntPtr)0, (uint)flags);
		}

		public void PlayASync() {
			PlayASync(WinAPI.WINMM.SoundFlags.SND_ASYNC);
		}
		
		public void PlayASync(WinAPI.WINMM.SoundFlags flags) {
			FreeHandle();
			flags |= WinAPI.WINMM.SoundFlags.SND_ASYNC;
			if (BytesToPlay != null) {
				_gcHandle = GCHandle.Alloc(BytesToPlay, GCHandleType.Pinned);
				flags |= WinAPI.WINMM.SoundFlags.SND_MEMORY;
				WinAPI.WINMM.PlaySound(_gcHandle.Value.AddrOfPinnedObject(), (UIntPtr)0, (uint)flags);
			} else {
				WinAPI.WINMM.PlaySound((byte[])null, (UIntPtr)0, (uint)flags);
			}
		}

		public byte[] BytesToPlay {
			get { return _bytesToPlay; }
			set {
				FreeHandle();
				_bytesToPlay = value;
			}
		}

		private void FreeHandle() {
			WinAPI.WINMM.PlaySound((byte[])null, (UIntPtr)0, (uint)0);
			if (_gcHandle != null) {
				_gcHandle.Value.Free();
				_gcHandle = null;
			}
		}

		#region IDisposable Members

		public void Dispose() {
			Dispose(true);
		}

		private void Dispose(bool disposing) {
			BytesToPlay = null;
			if (disposing) {
				GC.SuppressFinalize(this);
			}
		}

		~SoundPlayerEx() {
			Dispose(false);
		}

		#endregion
	}

}
