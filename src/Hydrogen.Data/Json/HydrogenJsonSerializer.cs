namespace Hydrogen.Data;

public class HydrogenJsonSerializer : IJsonSerializer {


	public string Serialize<T>(T obj) => Tools.Json.WriteToString(obj);

	public  T Deserialize<T>(string json) => Tools.Json.ReadFromString<T>(json);

}
