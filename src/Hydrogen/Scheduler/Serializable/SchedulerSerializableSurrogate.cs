using System.Xml.Serialization;

namespace Hydrogen {
	[XmlRoot("Scheduler")]
	public class SchedulerSerializableSurrogate {

		[XmlElement("Job")]
		public JobSerializableSurrogate[] Jobs { get; set; }
	}
}
