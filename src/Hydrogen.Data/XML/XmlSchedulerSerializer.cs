// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class XmlSchedulerSerializer : ISchedulerSerializer {

	public XmlSchedulerSerializer(string filepath) {
		FilePath = filepath;
	}

	public string FilePath { get; init; }

	public void Serialize(SchedulerSerializableSurrogate scheduler) {
		Tools.Xml.WriteToFile(FilePath, scheduler);
	}

	public SchedulerSerializableSurrogate Deserialize() {
		return Tools.Xml.ReadFromFile<SchedulerSerializableSurrogate>(FilePath);
	}
}
