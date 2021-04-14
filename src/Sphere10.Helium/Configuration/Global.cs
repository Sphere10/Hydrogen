namespace Sphere10.Helium.Configuration {
	public static class Global {
		public static int MaxAllowedCustomHeadersCount = 6;
		public static string TimeoutMessageId = "98BB8764-9AD1-45DC-9549-AD777A51BE8F-TimeoutId";
		public static int RouterQueueReadRatePerMinute = 60; /*60 messages per minute => 1 message per second*/
	}
}
