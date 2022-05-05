namespace Hydrogen.DApp.Presentation.Plugins {
    /// <summary>
    /// Denotes an item that can be routed/navigated to.
    /// </summary>
    public interface IRoutablePage {
        /// <summary>
        /// Gets the routable page url for this app
        /// </summary>
        public string Route { get; }
    }
}