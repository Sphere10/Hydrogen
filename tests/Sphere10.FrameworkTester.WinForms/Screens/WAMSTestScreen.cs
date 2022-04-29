using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Collections;
using Sphere10.Framework.Maths;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
	
	public partial class WAMSTestScreen : ApplicationScreen {
		private readonly ILogger _outputLogger;
		private bool _runningKeyMatchingTests;
		private bool _runningVerificationTests;

		public WAMSTestScreen() {
			InitializeComponent();
			_outputLogger = new TextWriterLogger(new TextBoxWriter(_outputTextBox));
			_hashAlgControl.EnumType = typeof(CHF);
			_hashAlgControl.SelectedEnum = CHF.SHA2_256;
		}

		private WAMSSharp CreateWAMS() => new WAMSSharp((int)_heightControl.Value, (int)_winternitzControl.Value, (CHF)_hashAlgControl.SelectedEnum);

		private async void _keyMatchingTestsButton_Click(object sender, EventArgs e) {
			var wams = CreateWAMS();
			var privateKey = wams.GeneratePrivateKey();
			var publicKey = await Task.Run(() => wams.DerivePublicKeyForBatch(privateKey, (ulong)0));
			var bytes = publicKey.RawBytes.ToArray();
			unchecked { bytes[0] += 1; } // ensure false result
			var count = 0;
			var startTime = DateTime.Now;
			if (!_runningKeyMatchingTests) {
				var task = new Task(() => {
					_runningKeyMatchingTests = true;
					while (_runningKeyMatchingTests) {
						Parallel.For(0, 1000, (x) => {
							wams.IsPublicKey(privateKey, bytes);
						});
						count += 1000;
						if (count % 1000 == 0) {
							var timeDiff = DateTime.Now.Subtract(startTime).TotalSeconds;
							if (timeDiff > 1) {
								var checksPerSecond = count / timeDiff;
								_outputLogger.Info($"Non-matching key checks per second = {checksPerSecond:##.###}");
								count = 0;
								startTime = DateTime.Now;
							}
						}
					}
				});
				task.Start();
				_keyMatchingTestsButton.Text = "Stop";
			} else {
				_runningKeyMatchingTests = false;
				_keyMatchingTestsButton.Text = "Key Matching";
			}
		}

		private async void _verificationSpeedButton_Click(object sender, EventArgs e) {
			var wams = CreateWAMS();
			var privateKey = await Task.Run(() => wams.GeneratePrivateKey());
			var messageDigest = Hashers.Hash(wams.Config.OTS.HashFunction, Encoding.ASCII.GetBytes("Testing bla bla bla"));
			var publicKey = await Task.Run(() => wams.DerivePublicKeyForBatch(privateKey, (ulong)0));
			var signature = await Task.Run(() => wams.SignDigest(privateKey, messageDigest, 0, 0));
			var publicKeyRawBytes = publicKey.RawBytes;
			var count = 0;
			var startTime = DateTime.Now;
			if (!_runningVerificationTests) {
				var task = new Task(() => {
					_runningVerificationTests = true;
					while (_runningVerificationTests) {
						Parallel.For(0, 1000, (x) => {
							wams.VerifyDigest(signature, messageDigest, publicKeyRawBytes);
						});
						count += 1000;
						if (count % 1000 == 0) {
							var timeDiff = DateTime.Now.Subtract(startTime).TotalSeconds;
							if (timeDiff > 1) {
								var checksPerSecond = count / timeDiff;
								_outputLogger.Info($"Signature verifications per second = {checksPerSecond:##.###}");
								count = 0;
								startTime = DateTime.Now;
							}
						}
					}
				});
				task.Start();
				_verificationSpeedButton.Text = "Stop";
			} else {
				_runningVerificationTests = false;
				_verificationSpeedButton.Text = "Verification Tests";
			}

		}

		private void _miscButton_Click(object sender, EventArgs e) {
		}

		private async void _miscButton2_Click(object sender, EventArgs e) {


			//| OTS | CHF bits | Winternitz | Private Key Size | Public Key Length | Signature Length |
			//	| ---- | -------- | ---------- | ---------------- | ----------------- | ---------------- |
			//	| WAMS | 128 | 4 |                  |                   |                  |


			var referenceConfig = new WAMS(0, 8, CHF.SHA2_256);
			 
			foreach (var chf in new[] { CHF.Blake2b_128, CHF.Blake2b_160, CHF.Blake2b_256 })
				foreach (var w in new[] { 2, 4, 8 })
					foreach (var h in new[] { 0, 8, 16 })
						foreach (var amsots in new[] { AMSOTS.WOTS, AMSOTS.WOTS_Sharp }) {
						var wams = amsots == AMSOTS.WOTS ? (AMS) new WAMS(h, w, chf) : new WAMSSharp(h, w, chf);
						var privateKey = await Task.Run(() => wams.GeneratePrivateKey());
						var messageDigest = Hashers.Hash(wams.Config.OTS.HashFunction, Encoding.ASCII.GetBytes("Testing bla bla bla"));
						var publicKey = await Task.Run(() => wams.DerivePublicKeyForBatch(privateKey, (ulong)0));
						var signature = await Task.Run(() => wams.SignDigest(privateKey, messageDigest, 0, 0));
						_outputLogger.Info($"| {amsots.GetDescription()} | {Hashers.GetDigestSizeBytes(chf) * 8} | {w} | {h} | {privateKey.RawBytes.Length} | {publicKey.RawBytes.Length} | {signature.Length} |");
					}

			
		}

		private void _registerStandardHashers_Click(object sender, EventArgs e) {
			Hashers.RegisterDefaultAlgorithms();
		}

		private void _regFastButton_Click(object sender, EventArgs e) {
			HashLibAdapter.RegisterHashLibHashers();
		}
	}
}

