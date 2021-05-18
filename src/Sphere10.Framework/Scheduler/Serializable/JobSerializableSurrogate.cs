using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {
	public class JobSerializableSurrogate {

		[XmlElement("Interval", typeof(IntervalScheduleSerializableSurrogate))]
		[XmlElement("DayOfWeek", typeof(DayOfWeekScheduleSerializableSurrogate))]
		[XmlElement("DayOfMonth", typeof(DayOfMonthScheduleSerializableSurrogate))]
		public JobScheduleSerializableSurrogate[] Jobs { get; set; }

	}
}
