// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Media;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using Hydrogen.Windows.Forms;


namespace Hydrogen.Utils.WinFormsTester;

public partial class TestSoundsScreen : ApplicationScreen {
	public TestSoundsScreen() {
		InitializeComponent();
	}

	private void PlaySound(Stream sound) {
		DirectSound ds = new DirectSound();

		ds.SetCooperativeLevel(this.Handle, CooperativeLevel.Priority);


		WaveFormat format = WaveFormat.CreateCustomFormat(
			WaveFormatEncoding.Pcm,
			44100,
			2,
			4 * 44100,
			4,
			16
		);

		SoundBufferDescription primaryDesc = new SoundBufferDescription();
		primaryDesc.Format = format;
		primaryDesc.Flags = BufferFlags.GlobalFocus;
		primaryDesc.BufferBytes = 8 * 4 * 44100;
		PrimarySoundBuffer pBuffer = new PrimarySoundBuffer(ds, primaryDesc);

		SoundBufferDescription secondDesc = new SoundBufferDescription();
		secondDesc.Format = format;
		secondDesc.Flags = BufferFlags.GlobalFocus | BufferFlags.ControlPositionNotify | BufferFlags.GetCurrentPosition2;
		secondDesc.BufferBytes = 8 * 4 * 44100;

		SecondarySoundBuffer secondBuffer = new SecondarySoundBuffer(ds, secondDesc);
		secondBuffer.Write(sound.ReadAll(), 0, LockFlags.None);
		secondBuffer.Play(0, PlayFlags.None);



	}

	private void button1_Click(object sender, EventArgs e) {
		PlaySound(Resources.LeftClickDown);

	}

	private void button2_Click(object sender, EventArgs e) {
		PlaySound(Resources.LeftClickUp);
	}

	private void button3_Click(object sender, EventArgs e) {
		PlaySound(Resources.RightClickDown);
	}

	private void button4_Click(object sender, EventArgs e) {
		PlaySound(Resources.RightClickUp);
	}

	private void _systemPlayerButton_Click(object sender, EventArgs e) {
		try {
			using var leftClickDownSoundPlayer = new SoundPlayer(Resources.LeftClickDown);
			leftClickDownSoundPlayer.Play();
		} catch (Exception ex) {
			ExceptionDialog.Show(ex);
		}
	}
}
