using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Sphere10.Framework.Scheduler.Serializable {


	public class DayOfMonthScheduleSerializableSurrogate : JobScheduleSerializableSurrogate {
		
		[XmlAttribute]
		public int  DayOfMonth { get; set; }

		[XmlAttribute]
		public string TimeOfDay;   // HH:mm:ss
	}


}
