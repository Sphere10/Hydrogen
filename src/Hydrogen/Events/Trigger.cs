using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen;
public class Trigger {

	public event EventHandlerEx Fired;

	public void Fire() {
		Fired?.Invoke();
	}

}
