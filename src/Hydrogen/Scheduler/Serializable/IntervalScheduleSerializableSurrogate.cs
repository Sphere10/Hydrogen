using System.Xml.Serialization;

namespace Hydrogen {
	public class IntervalScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {
		
		[XmlAttribute]
		public long RepeatIntervalMS;

	}
}
