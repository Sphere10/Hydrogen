using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Internal;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Services;

namespace VelocityNET.Presentation.Hydrogen.Loader
{

    public class ModalService : IModalService
    {
        private IComponentFactory ComponentFactory { get; }
        
        private Modal? ModalInstance { get; set; }

        public ModalService(IComponentFactory componentFactory)
        {
            ComponentFactory = componentFactory ?? throw new ArgumentNullException(nameof(componentFactory));
        }

        public void Initialize(Modal component)
        {
            ModalInstance = component ?? throw new ArgumentNullException(nameof(component));
        }

        public async Task<ModalResult> ShowAsync<T>() where T : ModalComponent
        {
            if (ModalInstance is null)
            {
                throw new InvalidOperationException("Modal service is not initialized, no modal component");
            }

            T component = (T)ComponentFactory.Activate(typeof(T));
            
            return await ModalInstance.ShowAsync(component);
        }
    }
}