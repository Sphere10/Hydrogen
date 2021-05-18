using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Scheduler.Serializable {
	public interface ISchedulerSerializer {

		void Serialize(SchedulerSerializableSurrogate scheduler);

		SchedulerSerializableSurrogate Deserialize();
	}


}
