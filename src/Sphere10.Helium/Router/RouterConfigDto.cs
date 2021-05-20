using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Helium.Router {
	public record RouterConfigDto {

		public int MaxRouterInputListCount { get; init; } = 1000;

		public int WaitBeforePuttingMessageBackInQueueSec { get; init; } = 10;
	}
}
