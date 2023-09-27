// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public class AppointmentDropEvent : AppointmentEvent {
	public AppointmentColumn TargetColumn { get; internal set; }
	public DateTime StartTime { get; internal set; }
	public DateTime EndTime { get; internal set; }
}
