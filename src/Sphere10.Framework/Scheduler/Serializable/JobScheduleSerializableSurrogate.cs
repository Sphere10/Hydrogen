using System;
using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {
	public abstract class JobScheduleSerializableSurrogate {

		[XmlAttribute]
		public string LastStartTime { get; set; }

		[XmlAttribute]
		public string LastEndTime { get; set; }

		[XmlAttribute]
		public string NextStartTime { get; set; }

		[XmlAttribute]
		public ReschedulePolicy ReschedulePolicy { get; set; }

		[XmlAttribute]
		public uint IterationsRemaining { get; set; }

		[XmlAttribute]
		public uint IterationsExecuted { get; set; }
	}
}
