namespace VelocityNET.ProtocolObjects {

	public abstract class Identity {
		public ulong Number { get; set; }
		public string Name { get; set; }
		public IdentityType Type { get; set; }
		public DataType DataType { get; set; }
		public byte[] Data1 { get; set; }
		public byte[] Data2 { get; set; }
		public byte[] Data3 { get; set; }
		public byte[] Data4 { get; set; }
		public byte[] Data5 { get; set; }
		public byte[] Data6 { get; set; }
		public byte[] Data7 { get; set; }
		public byte[] Data8 { get; set; }
		public ulong Nonce { get; set; }
		public ulong Updates { get; set; }
	}

}