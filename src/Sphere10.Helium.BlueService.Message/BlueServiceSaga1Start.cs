using Sphere10.Helium.Message;

namespace Sphere10.Helium.BlueService.Message {
	public record BlueServiceSaga1Start : ICommand {
		public string Id { get; set; }
	}
}
