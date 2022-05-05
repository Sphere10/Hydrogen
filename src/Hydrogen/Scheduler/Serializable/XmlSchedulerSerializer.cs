namespace Hydrogen {
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
}
