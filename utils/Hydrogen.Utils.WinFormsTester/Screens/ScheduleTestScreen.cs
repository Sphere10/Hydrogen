// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Windows.Forms;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ScheduleTestScreen : ApplicationScreen {
	private readonly TextBoxWriter _textWriter;
	private readonly ILogger _logger;

	private Scheduler _asynchronousScheduler = null;
	private Scheduler _synchronousScheduler = null;

	public ScheduleTestScreen() {
		InitializeComponent();

		_textWriter = new TextBoxWriter(textBox1);
		_logger = new ThreadIdLogger(new TimestampLogger(new MulticastLogger(
			new TextWriterLogger(_textWriter),
			new ConsoleLogger())));
	}

	private void Log(string text) {
		textBox1.BeginInvoke((MethodInvoker)delegate() {
			_logger.Info($"{DateTime.Now:HH:mm:ss.fff} {text}");
			//System.Diagnostics.Debug.WriteLine(text);
		});
	}

	private void _job1Button_Click(object sender, EventArgs e) {
		var job1 =
			JobBuilder.For(() => _logger.Info("Executed Job 1"))
				.Called("Job1")
				.RunOnce(DateTime.Now)
				.RunAsyncronously()
				.Repeat
				.OnInterval(TimeSpan.FromSeconds(1))
				.Build();

		var job2 =
			JobBuilder.For(() => _logger.Info("Executed Job 2"))
				.Called("Job2")
				.RunOnce(DateTime.Now)
				.RunAsyncronously()
				.Repeat
				.OnInterval(TimeSpan.FromSeconds(2))
				.Build();

		Hydrogen.Scheduler.Asynchronous.JobStatusChanged +=
			(job, fromStatus, toStatus) => _textWriter.WriteLine("{0}: {1} -> {2}", job.Name, fromStatus, toStatus);
		Hydrogen.Scheduler.Asynchronous.AddJob(job1);
		Hydrogen.Scheduler.Asynchronous.AddJob(job2);
	}

	private void TestButton_Click(object sender, EventArgs e) {
		var job2 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob2")
			.RunOnce(DateTime.Now)
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1))
			.Build();

		var job3 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob3")
			.RunOnce(DateTime.Now)
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1))
			.Build();

		if (_synchronousScheduler == null) {
			_synchronousScheduler = new Scheduler(SchedulerPolicy.ForceSyncronous, _logger);

			_synchronousScheduler.OnBeforeJobBegin = (job) => Log($"{job.Name} [OnBeforeJobBegin]");
			_synchronousScheduler.OnAfterJobEnd = (job) => Log($"{job.Name} [OnAfterJobEnd]");
			_synchronousScheduler.OnJobError = (job, ex) => Log($"{job.Name} [OnJobError] {ex.Message}");

			_synchronousScheduler.AddJob(job2);
			_synchronousScheduler.AddJob(job3);

			_synchronousScheduler.Start();
		}
	}

	private void SyncStartButton_Click(object sender, EventArgs e) {
		if (_synchronousScheduler != null) {
			Log("Starting (synchronous)");
			_synchronousScheduler.Start();
		} else {
			Log("Scheduler not created yet");
		}
	}

	private void SyncStopButton_Click(object sender, EventArgs e) {
		if (_synchronousScheduler != null) {
			Log("Stopping (synchronous)");
			_synchronousScheduler.Stop();
		} else {
			Log("Scheduler not created yet");
		}
	}

	private void SyncSerializeButton_Click(object sender, EventArgs e) {
		var scheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError | SchedulerPolicy.ForceSyncronous, _logger);

		var job1 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob1")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Build();
		scheduler.AddJob(job1);

		var job2 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob2")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1), endDate: DateTime.Now.AddMinutes(2))
			.Build();
		scheduler.AddJob(job2);

		var surrogate = scheduler.ToSerializableSurrogate();

		var newScheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError | SchedulerPolicy.ForceSyncronous, _logger);
		newScheduler.FromSerializableSurrogate(surrogate);
	}

	private void XmlSerializeButton_Click(object sender, EventArgs e) {
		var scheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError | SchedulerPolicy.ForceSyncronous, _logger);

		var job1 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob1")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Build();
		scheduler.AddJob(job1);

		var job2 = JobBuilder
			.For(typeof(TestJob))
			.Called("SyncJob2")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1), endDate: DateTime.Now.AddMinutes(2))
			.Build();
		scheduler.AddJob(job2);

		var path = Path.GetTempFileName();
		try {
			var serializer = new XmlSchedulerSerializer(path);
			serializer.Serialize(scheduler.ToSerializableSurrogate());

			var s = File.ReadAllText(path);
			Log(s);
		} finally {
			if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
				File.Delete(path);
			}
		}
	}

	private void AsyncTestButton_Click(object sender, EventArgs e) {
		var job1 = JobBuilder
			.For(() => Log("Executed Async Job 1"))
			.Called("AsyncJob1")
			.RunOnce(DateTime.Now)
			.RunAsyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1))
			.Build();

		var job2 = JobBuilder
			.For(() => Log("Executed Async Job 2"))
			.Called("AsyncJob2")
			.RunOnce(DateTime.Now)
			.RunAsyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(2))
			.Build();

		if (_asynchronousScheduler == null) {
			_asynchronousScheduler = new Scheduler(SchedulerPolicy.DontThrow | SchedulerPolicy.RemoveJobOnError, _logger);

			_asynchronousScheduler.OnBeforeJobBegin = (job) => Log($"{job.Name} [OnBeforeJobBegin]");
			_asynchronousScheduler.OnAfterJobEnd = (job) => Log($"{job.Name} [OnAfterJobEnd]");
			_asynchronousScheduler.OnJobError = (job, ex) => Log($"{job.Name} [OnJobError] {ex.Message}");
			_asynchronousScheduler.JobStatusChanged += (job, fromStatus, toStatus) => Log($"{job.Name}: {fromStatus} -> {toStatus}");

			_asynchronousScheduler.AddJob(job1);
			_asynchronousScheduler.AddJob(job2);

			_asynchronousScheduler.Start();
		}
	}

	private void AsyncStartButton_Click(object sender, EventArgs e) {
		if (_asynchronousScheduler != null) {
			Log("Starting (asynchronous)");
			_asynchronousScheduler.Start();
		} else {
			Log("Scheduler not created yet");
		}
	}

	private void AsyncStopButton_Click(object sender, EventArgs e) {
		if (_asynchronousScheduler != null) {
			Log("Stopping (asynchronous)");
			_asynchronousScheduler.Stop();
		} else {
			Log("Scheduler not created yet");
		}
	}
}


public class TestJob : ISchedulerJob {
	public void Execute(IJob job) {
		job.Log.Debug("TestJob BEGIN");
		System.Threading.Thread.Sleep(500);
		job.Log.Debug("TestJob END");
	}
}
