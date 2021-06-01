using System;

namespace Sphere10.Framework.Communications {
    public class ProtocolException : SoftwareException {
		public ProtocolException(BadDataType dataType, ProtocolChannel channel) 
			: base($"Protocol error: {dataType}") {
			DataErrorType = dataType;
			Channel = channel;
		}

		public ProtocolException(BadDataType dataType, ProtocolChannel channel, Exception innerException) 
			: base($"Protocol error: {dataType}", innerException) {
			DataErrorType = dataType;
			Channel = channel;
		}


        public BadDataType DataErrorType { get; init; }

        public ProtocolChannel Channel { get; init; }
	}
    

}
