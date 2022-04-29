using System.Xml.Serialization;

namespace Hydrogen {


	public class DayOfMonthScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {
		
		[XmlAttribute]
		public int DayOfMonth { get; set; }

		[XmlAttribute]
		public long TimeOfDay;
	}


}
