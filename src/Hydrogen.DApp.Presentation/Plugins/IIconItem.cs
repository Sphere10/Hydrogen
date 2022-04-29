namespace Sphere10.Hydrogen.Presentation.Plugins {

    /// <summary>
    /// Implementers of this interface have a font-awesome css icon to be displayed in lists etc.
    /// </summary>
    public interface IIconItem {
        /// <summary>
        /// Gets the icon font-awesome ccs classes for this app block.
        /// </summary>
        public string Icon { get; }
    }
}