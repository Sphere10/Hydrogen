namespace Hydrogen.DApp.Presentation2.Logic.Modal
{

    /// <summary>
    /// Modal options
    /// </summary>
    public class ModalOptions
    {
        /// <summary>
        /// Modal size.
        /// </summary>
        public ModalSize Size { get; init; } = ModalSize.Medium;
    }

    /// <summary>
    /// Modal size
    /// </summary>
    public enum ModalSize
    {
        Small,
        Medium,
        Large
    }
}