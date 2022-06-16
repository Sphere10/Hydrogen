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
	[NonParallelizable]
	public class SchedulerXmlSerializerTest {

        

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
				SchedulerTest.CompareSchedulers(scheduler, convertedScheduler);
			}
			finally
			{
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					File.Delete(path);
				}
			}
		}

	
	}
}
