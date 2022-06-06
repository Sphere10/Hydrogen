using System;

namespace Hydrogen.Tests;

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
