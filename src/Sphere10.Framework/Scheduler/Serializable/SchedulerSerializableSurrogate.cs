using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {
	[XmlRoot("Scheduler")]
	public class SchedulerSerializableSurrogate {

		[XmlElement("Job")]
		public JobSerializableSurrogate[] Jobs { get; set; }
	}
}
