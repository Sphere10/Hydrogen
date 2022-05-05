using System.Xml.Serialization;

namespace Hydrogen {
	public class JobSerializableSurrogate {
		public string JobType { get; set; }
		public string Name { get; set; }
		public JobStatus Status { get; set; }
		public JobPolicy Policy { get; set; }

		[XmlElement("Interval", typeof(IntervalScheduleSerializableSurrogate))]
		[XmlElement("DayOfWeek", typeof(DayOfWeekScheduleSerializableSurrogate))]
		[XmlElement("DayOfMonth", typeof(DayOfMonthScheduleSerializableSurrogate))]
		public JobScheduleSerializableSurrogate[] Schedules { get; set; }

	}
}
