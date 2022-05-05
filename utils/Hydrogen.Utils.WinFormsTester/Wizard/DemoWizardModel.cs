﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Utils.WinFormsTester.Wizard {
	public class DemoWizardModel {

		public string Name { get; set; }

		public int Age { get; set; }


		public static DemoWizardModel Default => new DemoWizardModel { Name = string.Empty, Age = 0 };

	}
}
