//-----------------------------------------------------------------------
// <copyright file="IoCTestForm.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Windows.Forms;
using TinyIoC;

namespace Hydrogen.Utils.WinFormsTester {
	public partial class IoCTestScreen : ApplicationScreen {
		private readonly ILogger _logger;
		public IoCTestScreen() {
			InitializeComponent();
			_logger = new TextBoxLogger(_outputTextBox);
		}

		private void _testButton_Click(object sender, EventArgs e) {
			try {
				var container = new TinyIoCContainer();
				container.Register<IAlpha, Alpha>();
				container.Register<IBeta, Beta>();
				//container.Register<IGamma, Gamma>();

				var alpha = container.Resolve<IAlpha>();
				var beta = container.Resolve<IBeta>();
				var gamma = container.Resolve<IGamma>();
			} catch (Exception error) {
				_logger.LogException(error);
			}
		}
	}

	public interface IAlpha { }

	public interface IBeta { }

	public interface IGamma { }

	public class Alpha : IAlpha {
		public Alpha(IBeta beta) { }
	}

	public class Beta : IBeta {
		public Beta(IGamma gamma) { }
	}

	public class Gamma : IGamma {
	}

}
