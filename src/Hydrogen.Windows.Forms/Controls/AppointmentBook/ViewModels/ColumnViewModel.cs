//-----------------------------------------------------------------------
// <copyright file="ColumnViewModel.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Windows.Forms.AppointmentBook {

	public sealed class ColumnViewModel {

		public ColumnViewModel(
			string name,
			AppointmentViewModel[] appointments,
			AppointmentColumn columnDataObject) {
			Name = name;
			Appointments = appointments;
			ColumnDataObject = columnDataObject;
		}

		public string Name { get; internal set; }

		public int Index { get; internal set; }

		public AppointmentViewModel[] Appointments { get; internal set; }

		public AppointmentColumn ColumnDataObject { get; internal set; }

	}
}
