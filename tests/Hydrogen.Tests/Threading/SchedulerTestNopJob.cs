namespace Hydrogen.Tests;

/// <summary>
/// Scheduler job that does nothing when executed.
/// </summary>
public class SchedulerTestNopJob : ISchedulerJob
{
	public void Execute(IJob job)
	{
	}
}
