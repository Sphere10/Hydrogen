// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public class HydrogenJsonSerializer : IJsonSerializer {


	public string Serialize<T>(T obj) => Tools.Json.WriteToString(obj);

	public T Deserialize<T>(string json) => Tools.Json.ReadFromString<T>(json);

}
