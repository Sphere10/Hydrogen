using System;
using System.Collections.Generic;
using System.Text;

namespace VelocityNET.Core.Configuration {
	public interface IConfiguration {

		public TimeSpan RTTInterval { get;  }

		public TimeSpan NewMinerBlockTime { get; }

		public TimeSpan DAAsertRelaxationTime { get; }

	}


}
