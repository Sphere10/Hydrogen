using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components.Modal;

namespace VelocityNET.Presentation.Hydrogen.Services
{
    public interface IModalService
    {
        /// <summary>
        /// Initialize the modal service passing a reference to the modal component.
        /// </summary>
        /// <param name="component"></param>
        void Initialize(ModalHost component);

        /// <summary>
        /// Show the modal component of type,
        /// <typeparam name="T"> modal component to show. must implement modal component</typeparam>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"> parameters to supply to the modal component.</param>
        /// <returns></returns>
        Task<ModalResult> ShowAsync<T>(Dictionary<string, object>? parameters = null)
            where T : ModalComponentBase;
    }
}