using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen {
	public interface ISchedulerSerializer {

		void Serialize(SchedulerSerializableSurrogate scheduler);

		SchedulerSerializableSurrogate Deserialize();
	}


}
