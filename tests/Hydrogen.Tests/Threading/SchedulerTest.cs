// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;
using System.Linq;
using NUnit.Framework.Legacy;


namespace Hydrogen.Tests;

[TestFixture]
[NonParallelizable]
public class SchedulerTest {

	[SetUp]
	public void Setup() {
		Assume.That(!Tools.NUnit.IsGitHubAction, "Test fixture is disabled on GitHub Actions");
	}

	[Test]
	public async Task StartOn_LocalTime() {
		var count = 0;
		Action action = () => count++;
		var job = JobBuilder.For(action).RunOnce(DateTime.Now.Add(TimeSpan.FromSeconds(1))).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(1.5));
		scheduler.Stop();
		ClassicAssert.AreEqual(1, count);
	}

	[Test]
	public async Task StartOn_UtcTime() {
		var count = 0;
		Action action = () => count++;
		var job = JobBuilder.For(action).RunOnce(DateTime.UtcNow.Add(TimeSpan.FromSeconds(1))).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(1.1));
		scheduler.Stop();
		ClassicAssert.AreEqual(1, count);
	}

	[Test]
	public async Task RepeatWithoutStartTime() {
		var count = 0;
		Action action = () => count++;
		var job = JobBuilder.For(action).Repeat.OnInterval(TimeSpan.FromSeconds(1)).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(1.1));
		scheduler.Stop();
		ClassicAssert.AreEqual(1, count); // does not start straight away
	}


	[Test]
	public async Task AwaitsCorrectly_Simple() {
		var count = 0;
		Action action = () => count++;
		var job = JobBuilder.For(action).Repeat.OnInterval(DateTime.Now, TimeSpan.FromSeconds(1)).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(5.8));
		scheduler.Stop();
		ClassicAssert.AreEqual(1 + 5, count); // Starts straight away
	}

	[Test]
	public async Task AwaitsCorrectly_Complex() {
		const int ToleranceMS = 25;
		const int Job1FreqMS = 100;
		const int Job2FreqMS = 200;
		const int Job3FreqMS = 500;
		var count1Date = DateTime.Now;
		var count1 = new Statistics();
		var count2Date = DateTime.Now;
		var count2 = new Statistics();
		var count3Date = DateTime.Now;
		var count3 = new Statistics();

		void UpdateCount(Statistics countStats, ref DateTime lastCountTime) {
			var now = DateTime.Now;
			countStats.AddDatum(now.Subtract(lastCountTime).TotalMilliseconds);
			lastCountTime = now;
		}

		var job1 = JobBuilder.For(() => UpdateCount(count1, ref count1Date)).Repeat.OnInterval(TimeSpan.FromMilliseconds(Job1FreqMS)).Build();
		var job2 = JobBuilder.For(() => UpdateCount(count2, ref count2Date)).Repeat.OnInterval(TimeSpan.FromMilliseconds(Job2FreqMS)).Build();
		var job3 = JobBuilder.For(() => UpdateCount(count3, ref count3Date)).Repeat.OnInterval(TimeSpan.FromMilliseconds(Job3FreqMS)).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job1);
		scheduler.AddJob(job2);
		scheduler.AddJob(job3);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(5));
		scheduler.Stop();
		Assert.That(count1.Mean, Is.InRange(Job1FreqMS - ToleranceMS, Job1FreqMS + ToleranceMS));
		Assert.That(count2.Mean, Is.InRange(Job2FreqMS - ToleranceMS, Job2FreqMS + ToleranceMS));
		Assert.That(count3.Mean, Is.InRange(Job3FreqMS - ToleranceMS, Job3FreqMS + ToleranceMS));
	}


	[Test]
	public async Task RescheduleOnStart() {

		// Schedule = every 0.1 second
		// Run for 1 second
		// Execution takes 1.1 seconds
		// Total runs should be 1
		var count = 0;
		int workerThreads, portThreads;
		ThreadPool.GetMaxThreads(out workerThreads, out portThreads);
		ThreadPool.SetMaxThreads(100, 100);
		var runTimes = new List<DateTime>();
		Action action = () => {
			runTimes.Add(DateTime.Now);
			count++;
			Thread.Sleep(10000);
		};
		var job = JobBuilder.For(action).Repeat.OnInterval(DateTime.Now, TimeSpan.FromSeconds(1), ReschedulePolicy.OnStart).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(3.8));
		scheduler.Stop();
		ThreadPool.SetMaxThreads(workerThreads, portThreads);
		ClassicAssert.AreEqual(3, count); // Starts straight away
	}

	[Test]
	public async Task RescheduleOnFinish() {
		// Schedule = every 0.1 second
		// Run for 1 second
		// Execution takes 1.1 seconds
		// Total runs should be 1
		var runTimes = new List<DateTime>();
		var count = 0;
		Action action = () => {
			runTimes.Add(DateTime.Now);
			count++;
			Thread.Sleep(1100);
		};
		var job = JobBuilder.For(action).Repeat.OnInterval(TimeSpan.FromSeconds(0.1)).Build();
		var scheduler = new Scheduler();
		scheduler.AddJob(job);
		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds(1));
		scheduler.Stop();
		ClassicAssert.AreEqual(1, count); // Starts straight away
	}

	[Test]
	public async Task ScheduleJob_OnIntervalJobWithEndDate_ShouldCompleteByEndDate() {
		// Schedule a job:
		// - to run for 1 second, then repeat each second (no iteration count specified).
		// - with an end date 4 seconds from now.
		//
		// Job should run for 4 seconds and be executed 4 times (once each second).
		const int intervalCount = 3;
		const int intervalSeconds = 1;

		var executionCount = intervalCount + 1;
		var endDate = DateTime.Now.Add(TimeSpan.FromSeconds(executionCount * intervalSeconds));

		var count = 0;
		var job = JobBuilder
			.For(() => count++)
			.RunOnce(DateTime.Now)
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(intervalSeconds), endDate: endDate)
			.Build();

		var scheduler = new Scheduler();
		scheduler.AddJob(job);

		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds((executionCount * intervalSeconds) + 1));
		ClassicAssert.AreEqual(JobStatus.Completed, job.Status);
		scheduler.Stop();

		ClassicAssert.AreEqual(executionCount, count);
	}

	[Test]
	public async Task ScheduleJob_OnIntervalJobWithEndDate_ShouldCompleteByInterval() {
		// Schedule a job:
		// - to run for 1 second, then repeat 2 times (1 second between each execution).
		// - with an end date 1 day from now.
		//
		// Job should run for 3 seconds and be executed 3 times (once each second).
		const int intervalSeconds = 1;
		const int totalIterations = 2;

		var executionCount = totalIterations + 1;
		var endDate = DateTime.Now.Add(TimeSpan.FromDays(1));

		var count = 0;
		var job = JobBuilder
			.For(() => count++)
			.RunOnce(DateTime.Now)
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(intervalSeconds), totalIterations: totalIterations, endDate: endDate)
			.Build();

		var scheduler = new Scheduler();
		scheduler.AddJob(job);

		scheduler.Start();
		await Task.Delay(TimeSpan.FromSeconds((executionCount * intervalSeconds) + 1));
		ClassicAssert.AreEqual(JobStatus.Completed, job.Status);
		scheduler.Stop();

		ClassicAssert.AreEqual(executionCount, count);
	}

	[Test]
	public void ScheduleJob_WithIncompatiblePolicies_ShouldFail() {
		var asyncJob1 = JobBuilder
			.For(() => { })
			.Called("AsyncJob1")
			.RunOnce(DateTime.Now)
			.RunAsyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1))
			.Build();

		var syncJob1 = JobBuilder
			.For(() => { })
			.Called("SyncJob1")
			.RunOnce(DateTime.Now)
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1))
			.Build();

		var asynchronousScheduler = new Scheduler();
		var synchronousScheduler = new Scheduler(SchedulerPolicy.ForceSyncronous);

		// Adding sync job to async scheduler - should fail.
		Assert.Throws<InvalidOperationException>(() => { asynchronousScheduler.AddJob(syncJob1); });

		// Adding async job to sync scheduler - should fail.
		Assert.Throws<InvalidOperationException>(() => { synchronousScheduler.AddJob(asyncJob1); });
	}

	[Test]
	public async Task ScheduleJob_JobThrowsException_ShouldCallErrorHandler() {
		Exception failException = null;

		var job = JobBuilder
			.For(typeof(SchedulerTestErrorJob))
			.RunOnce(DateTime.Now)
			.Build();

		var scheduler = new Scheduler();
		scheduler.OnJobError = (job, ex) => failException = ex;
		scheduler.AddJob(job);
		scheduler.Start();

		await Task.Delay(TimeSpan.FromSeconds(2));
		scheduler.Stop();

		ClassicAssert.IsTrue(failException != null && failException.Message == SchedulerTestErrorJob.Errormessage);
	}

	[Test]
	public void SerializeScheduler_ToFromSurrogate_ShouldBeEqual() {
		var schedulerPolicy = SchedulerPolicy.ForceSyncronous;

		// Create a scheduler and add jobs to it.
		var scheduler = new Scheduler(schedulerPolicy);

		var job1 = JobBuilder
			.For(typeof(SchedulerTestNopJob))
			.Called("SyncJob1")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Build();
		scheduler.AddJob(job1);

		var job2 = JobBuilder
			.For(typeof(SchedulerTestNopJob))
			.Called("SyncJob2")
			.RunOnce(DateTime.Now.AddMinutes(1))
			.RunSyncronously()
			.Repeat
			.OnInterval(TimeSpan.FromSeconds(1), endDate: DateTime.Now.AddMinutes(2))
			.Build();
		scheduler.AddJob(job2);

		// Convert the scheduler to a surrogate.
		var surrogate = scheduler.ToSerializableSurrogate();

		// Convert the surrogate back to the scheduler.
		var convertedScheduler = new Scheduler(schedulerPolicy);
		convertedScheduler.FromSerializableSurrogate(surrogate);

		// Compare the two schedulers - should be the same.
		CompareSchedulers(scheduler, convertedScheduler);
	}

	public static void CompareSchedulers(Scheduler scheduler, Scheduler convertedScheduler) {
		// Compare the two schedulers.
		foreach (var job in scheduler.GetJobs()) {
			// Compare jobs.
			var convertedJob = convertedScheduler.GetJobs().FirstOrDefault(x => x.Name == job.Name);
			ClassicAssert.IsNotNull(convertedJob);
			ClassicAssert.AreEqual(job.Policy, convertedJob.Policy);
			ClassicAssert.AreEqual(job.Status, convertedJob.Status);
			ClassicAssert.AreEqual(job.Schedules.Count(), convertedJob.Schedules.Count());

			// Compare schedules.
			var schedules = job.Schedules.ToList();
			var convertedSchedules = convertedJob.Schedules.ToList();

			for (var i = 0; i < schedules.Count; i++) {
				var schedule = schedules[i];
				var convertedSchedule = convertedSchedules[i];

				ClassicAssert.IsNotNull(convertedSchedule);
				ClassicAssert.AreEqual(Truncate(schedule.LastStartTime), Truncate(convertedSchedule.LastStartTime));
				ClassicAssert.AreEqual(Truncate(schedule.LastEndTime), Truncate(convertedSchedule.LastEndTime));
				ClassicAssert.AreEqual(Truncate(schedule.EndDate), Truncate(convertedSchedule.EndDate));
				ClassicAssert.AreEqual(schedule.ReschedulePolicy, convertedSchedule.ReschedulePolicy);
				ClassicAssert.AreEqual(schedule.IterationsRemaining, convertedSchedule.IterationsRemaining);
				ClassicAssert.AreEqual(schedule.IterationsExecuted, convertedSchedule.IterationsExecuted);
			}
		}

		// Removes frational seconds from a DateTime (e.g. 2000-01-01 00:00:00.12345 becomes 2000-01-01 00:00:00).
		static DateTime? Truncate(DateTime? date) {
			return date.HasValue
				? new DateTime(date.Value.Ticks - (date.Value.Ticks % TimeSpan.TicksPerSecond), date.Value.Kind)
				: null;
		}
	}


}
