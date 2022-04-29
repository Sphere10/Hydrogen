//-----------------------------------------------------------------------
// <copyright file="PCLTimer.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen {
	
	internal sealed class CustomTimer : IDisposable {
		private CancellationTokenSource _cancellationTokenSource;
		private int _interval;

		internal delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

		internal CustomTimer() {
			_cancellationTokenSource = null;
			_interval = 100;
		}

		public bool Enabled { get; set; }

		public ElapsedEventHandler Elapsed { get; set; }

		public double Interval {
			get => _interval;
			set {
				_interval = (int)Math.Round(value);
				if (Enabled) {
					Stop();
					Start();
				}
			}
		}

		public void Start() {
			if (_cancellationTokenSource == null)
				_cancellationTokenSource = new CancellationTokenSource();
			 
			Task
				.Delay((int)Interval, _cancellationTokenSource.Token)
				.ContinueWith((t, s) => {
						if (Enabled && Elapsed != null) {

							Elapsed(this, new ElapsedEventArgs(DateTime.Now));
						}
					},
					null,
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default
				);


		}

		public void Stop() {
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource = null;
		}

		public bool AutoReset { get; set; }

		public void Dispose() {
			_cancellationTokenSource?.Cancel();
		}

		public class ElapsedEventArgs : EventArgs {
			internal ElapsedEventArgs(DateTime signalTime) {
				SignalTime = signalTime;
			}

			internal ElapsedEventArgs(int low, int high) {
				var fileTime = (long)((((ulong)high) << 32) | (((ulong)low) & 0xffffffff));
				this.SignalTime = DateTime.FromFileTime(fileTime);
			}

			public DateTime SignalTime { get; }
		}

	}

	
}
