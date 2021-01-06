using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;

namespace VelocityNET.Presentation.Hydrogen.Services
{
    public interface IModalService
    {
        /// <summary>
        /// Initialize the modal service passing a reference to the modal component.
        /// </summary>
        /// <param name="component"></param>
        void Initialize(Modal component);

        /// <summary>
        /// Show the modal component of type,
        /// <typeparam name="T"> modal component to show. must implement modal component</typeparam>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<ModalResult> ShowAsync<T>() where T : ModalComponent;
    }
}