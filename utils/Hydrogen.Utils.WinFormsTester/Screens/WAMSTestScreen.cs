// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen.CryptoEx;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

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
		unchecked {
			bytes[0] += 1;
		} // ensure false result
		var count = 0;
		var startTime = DateTime.Now;
		if (!_runningKeyMatchingTests) {
			var task = new Task(() => {
				_runningKeyMatchingTests = true;
				while (_runningKeyMatchingTests) {
					Parallel.For(0, 1000, (x) => { wams.IsPublicKey(privateKey, bytes); });
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
					Parallel.For(0, 1000, (x) => { wams.VerifyDigest(signature, messageDigest, publicKeyRawBytes); });
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
		const int totalSigs = 1000;

		//| OTS | CHF bits | Winternitz | Private Key Size | Public Key Length | Signature Length |
		//	| ---- | -------- | ---------- | ---------------- | ----------------- | ---------------- |
		//	| WAMS | 128 | 4 |                  |                   |                  |


		var referenceConfig = new WAMS(0, 8, CHF.SHA2_256);

		foreach (var chf in new[] { CHF.Blake2b_128_Fast, CHF.Blake2b_160_Fast, CHF.Blake2b_256_Fast })
		foreach (var w in new[] { 2, 4, 8 })
		foreach (var h in new[] { 0, 8, 16 })
		foreach (var amsots in new[] { AMSOTS.WOTS, AMSOTS.WOTS_Sharp }) {
			var wams = amsots == AMSOTS.WOTS ? (AMS)new WAMS(h, w, chf) : new WAMSSharp(h, w, chf);
			var privateKey = await Task.Run(() => wams.GeneratePrivateKey());
			var messageDigest = Hashers.Hash(wams.Config.OTS.HashFunction, Encoding.ASCII.GetBytes("Testing bla bla bla"));
			var publicKey = await Task.Run(() => wams.DerivePublicKeyForBatch(privateKey, (ulong)0));
			var signature = await Task.Run(() => wams.SignDigest(privateKey, messageDigest, 0, 0));
			Guard.Ensure(wams.VerifyDigest(signature, messageDigest, publicKey));
			var signTime = await Tools.Threads.MeasureDuration(() => {
				for (var i = 0; i < totalSigs; i++)
					wams.SignDigest(privateKey, messageDigest, 0, 0);
			});
			var verifyTime = await Tools.Threads.MeasureDuration(() => {
				var sig = wams.SignDigest(privateKey, messageDigest, 0, 0);
				for (var i = 0; i < totalSigs; i++)
					wams.VerifyDigest(sig, messageDigest, publicKey);
			});
			var sigTps = totalSigs / signTime.TotalSeconds;
			var verTps = totalSigs / verifyTime.TotalSeconds;
			_outputLogger.Info($"| {amsots.GetDescription()} | {Hashers.GetDigestSizeBytes(chf) * 8} | {w} | {h} | {publicKey.RawBytes.Length} | {signature.Length} | {sigTps:.} | {verTps:.}");
		}


	}

	private void _registerStandardHashers_Click(object sender, EventArgs e) {
		Hashers.RegisterDefaultAlgorithms();
	}

	private void _regFastButton_Click(object sender, EventArgs e) {
		Hashers.Register(CHF.Blake2b_512_Fast, () => new Blake2bFastAdapter(64));
		Hashers.Register(CHF.Blake2b_384_Fast, () => new Blake2bFastAdapter(48));
		Hashers.Register(CHF.Blake2b_256_Fast, () => new Blake2bFastAdapter(32));
		Hashers.Register(CHF.Blake2b_128_Fast, () => new Blake2bFastAdapter(16));
		Hashers.Register(CHF.Blake2b_224_Fast, () => new Blake2bFastAdapter(28));
		Hashers.Register(CHF.Blake2b_160_Fast, () => new Blake2bFastAdapter(20));
		Hashers.Register(CHF.Blake2s_256_Fast, () => new Blake2sFastAdapter(32));
		Hashers.Register(CHF.Blake2s_224_Fast, () => new Blake2sFastAdapter(28));
		Hashers.Register(CHF.Blake2s_160_Fast, () => new Blake2sFastAdapter(20));
		Hashers.Register(CHF.Blake2s_128_Fast, () => new Blake2sFastAdapter(16));
		//HashLibAdapter.RegisterHashLibHashers();
	}
}
