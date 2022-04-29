using System;
using System.Xml.Serialization;

namespace Hydrogen {
	public class DayOfWeekScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {

		[XmlAttribute]
		public DayOfWeek DayOfWeek { get; set; }

		[XmlAttribute]
		public long TimeOfDay;

	}
}
