
namespace Sphere10.Helium.Router {
	/// <summary>
	/// This is the Router's specific configuration.
	/// </summary>
	public record RouterConfigDto {

		public int MaxRouterInputListCount { get; init; } = 100;

		public int WaitBeforePuttingMessageBackInQueueSec { get; init; } = 10;
	}
}
