using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sphere10.Framework.Communications.RPC {
	//Declare a communication endpoint for simmple messaging
	public interface IEndPoint {
		public string			GetDescription();
		public ulong			GetUID();
		public IEndPoint		WaitForMessage();
		public EndpointMessage	ReadMessage(); 
		public void				WriteMessage(EndpointMessage message);
		public bool				IsOpened();
		public void				Start();
		public void				Stop();
	}
}