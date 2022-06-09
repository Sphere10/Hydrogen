using Hydrogen.DApp.Node.UI;

namespace Hydrogen.DApp.Node.UI {

	[Title("WebBrowser")]
	[Lifetime(ScreenLifetime.WhenVisible)]
	[MenuLocation(AppMenu.WebBrowser, "WebBrowser", 1)]
	public class WebBrowserScreen : MockScreen {
	
			[Title("Config")]
			public class StartWebBrowserScreen  {
		}


		public void StartBrowser() {

	}

	}
}