using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hydrogen.Data;


public interface IJsonSerializer {
	string Serialize<T>(T value);

	T Deserialize<T>(string value);

}

