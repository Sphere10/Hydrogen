using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests {

    public class TestNavigationManager : NavigationManager {
        public TestNavigationManager() {
            Initialize("http://localhost/", "http://localhost/");
        }

        public TestNavigationManager(string baseUri = null, string uri = null) {
            Initialize(baseUri ?? "http://localhost/", uri ?? baseUri ?? "http://localhost/");
        }

        public new void Initialize(string baseUri, string uri) {
            base.Initialize(baseUri, uri);
        }

        protected override void NavigateToCore(string uri, bool forceLoad) {
            Uri = BaseUri + uri.TrimStart('/');
            NotifyLocationChanged(forceLoad);
        }
    }

}