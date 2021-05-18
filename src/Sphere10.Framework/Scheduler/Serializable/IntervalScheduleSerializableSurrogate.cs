using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {
	public class IntervalScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {
		
		[XmlAttribute]
		public long RepeatIntervalMS;

	}
}
