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
using Sphere10.Framework.Scheduler;


namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class SchedulerTest {

        [Test]
        public async Task StartOn_LocalTime() {
	        var count = 0;
	        Action action = () => count++;
	        var job = JobBuilder.For(action).RunOnce(DateTime.Now.Add(TimeSpan.FromSeconds(1))).Build();
	        var scheduler = new Scheduler.Scheduler();
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
			var scheduler = new Scheduler.Scheduler();
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
			var scheduler = new Scheduler.Scheduler();
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
			var scheduler = new Scheduler.Scheduler();
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
			var scheduler = new Scheduler.Scheduler();
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
			await Task.Delay(TimeSpan.FromSeconds(secondsToTest).Add(TimeSpan.FromMilliseconds(800)));
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
			var scheduler = new Scheduler.Scheduler();
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
			var scheduler = new Scheduler.Scheduler();
			scheduler.AddJob(job);
			scheduler.Start();
			await Task.Delay(TimeSpan.FromSeconds(1));
			scheduler.Stop();
			Assert.AreEqual(1, count);   // Starts straight away
		}

	}

}
