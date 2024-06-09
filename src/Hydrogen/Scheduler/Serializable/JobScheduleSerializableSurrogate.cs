// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Xml.Serialization;

namespace Hydrogen;

public abstract class JobScheduleSerializableSurrogate {

	[XmlAttribute] public string LastStartTime { get; set; }

	[XmlAttribute] public string LastEndTime { get; set; }

	[XmlAttribute] public string EndDate { get; set; }

	[XmlAttribute] public ReschedulePolicy ReschedulePolicy { get; set; }

	[XmlAttribute] public uint IterationsRemaining { get; set; }

	[XmlAttribute] public uint IterationsExecuted { get; set; }

	[XmlAttribute] public string TotalIterations { get; set; }

	[XmlAttribute] public string InitialStartTime { get; set; }
}
