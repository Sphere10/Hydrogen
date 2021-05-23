using System;
using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {
	public class DayOfWeekScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {

		[XmlAttribute]
		public DayOfWeek DayOfWeek { get; set; }

		[XmlAttribute]
		public long TimeOfDay;

	}
}
