//-----------------------------------------------------------------------
// <copyright file="SchedulerTest.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;
using System.Linq;
using System.IO;
using Hydrogen;


namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class SchedulerTest {

        [Test]
        public async Task StartOn_LocalTime() {
	        var count = 0;
	        Action action = () => count++;
	        var job = JobBuilder.For(action).RunOnce(DateTime.Now.Add(TimeSpan.FromSeconds(1))).Build();
	        var scheduler = new Scheduler();
			scheduler.AddJob(job);
			scheduler.Start();
	        await Task.Delay(TimeSpan.FromSeconds(1.1));
			scheduler.Stop();
			Assert.AreEqual(1, count);
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
			Assert.AreEqual(1, count);
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
			Assert.AreEqual(1, count);   // does not start straight away
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
			Assert.AreEqual(1 + 5, count);   // Starts straight away
		}

		[Test]
		public async Task AwaitsCorrectly_Complex() {
			var count1 = 0;
			var count2 = 0;
			var count3 = 0;
			var count4 = 0;
			var count5 = 0;
			var count6 = 0;
			var count7 = 0;
			var count8 = 0;
			var count9 = 0;
			Action action1 = () => count1++;
			Action action2 = () => count2++;
			Action action3 = () => count3++;
			Action action4 = () => count4++;
			Action action5 = () => count5++;
			Action action6 = () => count6++;
			Action action7 = () => count7++;
			Action action8 = () => count8++;
			Action action9 = () => count9++;
			var job1 = JobBuilder.For(action1).Repeat.OnInterval(TimeSpan.FromSeconds(1)).Build();
			var job2 = JobBuilder.For(action2).Repeat.OnInterval(TimeSpan.FromSeconds(2)).Build();
			var job3 = JobBuilder.For(action3).Repeat.OnInterval(TimeSpan.FromSeconds(3)).Build();
			var job4 = JobBuilder.For(action4).Repeat.OnInterval(TimeSpan.FromSeconds(4)).Build();
			var job5 = JobBuilder.For(action5).Repeat.OnInterval(TimeSpan.FromSeconds(5)).Build();
			var job6 = JobBuilder.For(action6).Repeat.OnInterval(TimeSpan.FromSeconds(6)).Build();
			var job7 = JobBuilder.For(action7).Repeat.OnInterval(TimeSpan.FromSeconds(7)).Build();
			var job8 = JobBuilder.For(action8).Repeat.OnInterval(TimeSpan.FromSeconds(8)).Build();
			var job9 = JobBuilder.For(action9).Repeat.OnInterval(TimeSpan.FromSeconds(9)).Build();
			var scheduler = new Scheduler();
			scheduler.AddJob(job1);
			scheduler.AddJob(job2);
			scheduler.AddJob(job3);
			scheduler.AddJob(job4);
			scheduler.AddJob(job5);
			scheduler.AddJob(job6);
			scheduler.AddJob(job7);
			scheduler.AddJob(job8);
			scheduler.AddJob(job9);
			scheduler.Start();
			var secondsToTest = 20;
			await Task.Delay(TimeSpan.FromSeconds(secondsToTest).Add(TimeSpan.FromMilliseconds(920)));
			scheduler.Stop();
			Assert.AreEqual(secondsToTest / 1, count1);
			Assert.AreEqual(secondsToTest / 2, count2);
			Assert.AreEqual(secondsToTest / 3, count3);
			Assert.AreEqual(secondsToTest / 4, count4);
			Assert.AreEqual(secondsToTest / 5, count5);
			Assert.AreEqual(secondsToTest / 6, count6);
			Assert.AreEqual(secondsToTest / 7, count7);
			Assert.AreEqual(secondsToTest / 8, count8);
			Assert.AreEqual(secondsToTest / 9, count9);
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
			Assert.AreEqual(3, count);   // Starts straight away
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
			Assert.AreEqual(1, count);   // Starts straight away
		}

		[Test]
		public async Task ScheduleJob_OnIntervalJobWithEndDate_ShouldCompleteByEndDate()
		{
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
			Assert.AreEqual(JobStatus.Completed, job.Status);
			scheduler.Stop();

			Assert.AreEqual(executionCount, count);
		}

		[Test]
		public async Task ScheduleJob_OnIntervalJobWithEndDate_ShouldCompleteByInterval()
		{
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
			Assert.AreEqual(JobStatus.Completed, job.Status);
			scheduler.Stop();

			Assert.AreEqual(executionCount, count);
		}

		[Test]
		public void ScheduleJob_WithIncompatiblePolicies_ShouldFail()
		{
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
			Assert.Throws<InvalidOperationException>(() =>
			{
				asynchronousScheduler.AddJob(syncJob1);
			});

			// Adding async job to sync scheduler - should fail.
			Assert.Throws<InvalidOperationException>(() =>
			{
				synchronousScheduler.AddJob(asyncJob1);
			});
		}

		[Test]
		public async Task ScheduleJob_JobThrowsException_ShouldCallErrorHandler()
		{
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

			Assert.IsTrue(failException != null && failException.Message == SchedulerTestErrorJob.Errormessage);
		}

		[Test]
		public void SerializeScheduler_ToFromSurrogate_ShouldBeEqual()
		{
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

		[Test]
		public void SerializeScheduler_ToFromXmlFile_ShouldBeEqual()
        {
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

			var path = Path.GetTempFileName();
			try
			{
				var serializer = new XmlSchedulerSerializer(path);

				// Convert scheduler to XML and save to a file.
				var surrogate = scheduler.ToSerializableSurrogate();
				serializer.Serialize(surrogate);

				// Load the XML from the file into a surrogate.
				var convertedSurrogate = serializer.Deserialize();

				// Convert the surrogate back to the scheduler.
				var convertedScheduler = new Scheduler(schedulerPolicy);
				convertedScheduler.FromSerializableSurrogate(convertedSurrogate);

				// Compare the two schedulers - should be the same.
				CompareSchedulers(scheduler, convertedScheduler);
			}
			finally
			{
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					File.Delete(path);
				}
			}
		}

		private static void CompareSchedulers(Scheduler scheduler, Scheduler convertedScheduler)
		{
			// Compare the two schedulers.
			foreach (var job in scheduler.GetJobs())
            {
				// Compare jobs.
				var convertedJob = convertedScheduler.GetJobs().FirstOrDefault(x => x.Name == job.Name);
				Assert.IsNotNull(convertedJob);
				Assert.AreEqual(job.Policy, convertedJob.Policy);
				Assert.AreEqual(job.Status, convertedJob.Status);
				Assert.AreEqual(job.Schedules.Count(), convertedJob.Schedules.Count());

				// Compare schedules.
				var schedules = job.Schedules.ToList();
				var convertedSchedules = convertedJob.Schedules.ToList();

				for (var i = 0; i < schedules.Count; i++)
                {
					var schedule = schedules[i];
					var convertedSchedule = convertedSchedules[i];

					Assert.IsNotNull(convertedSchedule);
					Assert.AreEqual(Truncate(schedule.LastStartTime), Truncate(convertedSchedule.LastStartTime));
					Assert.AreEqual(Truncate(schedule.LastEndTime), Truncate(convertedSchedule.LastEndTime));
					Assert.AreEqual(Truncate(schedule.EndDate), Truncate(convertedSchedule.EndDate));
					Assert.AreEqual(schedule.ReschedulePolicy, convertedSchedule.ReschedulePolicy);
					Assert.AreEqual(schedule.IterationsRemaining, convertedSchedule.IterationsRemaining);
					Assert.AreEqual(schedule.IterationsExecuted, convertedSchedule.IterationsExecuted);
				}
			}

			// Removes frational seconds from a DateTime (e.g. 2000-01-01 00:00:00.12345 becomes 2000-01-01 00:00:00).
			static DateTime? Truncate(DateTime? date)
			{
                return date.HasValue
					? new DateTime(date.Value.Ticks - (date.Value.Ticks % TimeSpan.TicksPerSecond), date.Value.Kind)
					: null;
            }
        }

		/// <summary>
		/// Scheduler job that will throw an exception when executed.
		/// </summary>
		public class SchedulerTestErrorJob : ISchedulerJob
		{
			public const string Errormessage = "FAIL";

			public void Execute(IJob job)
			{
				throw new Exception(Errormessage);
			}
		}

		/// <summary>
		/// Scheduler job that does nothing when executed.
		/// </summary>
		public class SchedulerTestNopJob : ISchedulerJob
		{
			public void Execute(IJob job)
			{
			}
		}
	}
}
