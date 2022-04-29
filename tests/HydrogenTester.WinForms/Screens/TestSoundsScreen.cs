//-----------------------------------------------------------------------
// <copyright file="TestSoundsForm.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using Sphere10.Framework;
using Sphere10.Framework.Windows.Forms;


namespace Sphere10.FrameworkTester.WinForms {
	public partial class TestSoundsScreen : ApplicationScreen {
		public TestSoundsScreen() {
			InitializeComponent();
		}

		private void PlaySound(Stream sound) {
			DirectSound ds = new DirectSound();

			ds.SetCooperativeLevel(this.Handle, CooperativeLevel.Priority);


			WaveFormat format =  WaveFormat.CreateCustomFormat(
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
	}
}
