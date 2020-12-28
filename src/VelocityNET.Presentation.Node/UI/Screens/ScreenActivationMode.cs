namespace VelocityNET.Presentation.Node.UI {

	/// <summary>
	/// Determine when the screen is created and destroyed
	/// </summary>
	public enum ScreenLifetime {
		/// <summary>
		/// Screen is created on application startup and destroyed on application shutdown
		/// </summary>
		Application,

		/// <summary>
		/// Screen is created on first show and destroyed on application shutdown
		/// </summary>
		LazyLoad,

		/// <summary>
		/// Screen is created on when shown and destroyed not shown
		/// </summary>
		WhenVisible,
		
	}

}
