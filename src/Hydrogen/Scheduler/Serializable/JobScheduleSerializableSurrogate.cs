using System;
using System.Xml.Serialization;

namespace Hydrogen {
	public abstract class JobScheduleSerializableSurrogate {

		[XmlAttribute]
		public string LastStartTime { get; set; }

		[XmlAttribute]
		public string LastEndTime { get; set; }

		[XmlAttribute]
		public string EndDate { get; set; }

		[XmlAttribute]
		public ReschedulePolicy ReschedulePolicy { get; set; }

		[XmlAttribute]
		public uint IterationsRemaining { get; set; }

		[XmlAttribute]
		public uint IterationsExecuted { get; set; }

		[XmlAttribute]
		public string TotalIterations { get; set; }

		[XmlAttribute]
		public string InitialStartTime { get; set; }
	}
}
