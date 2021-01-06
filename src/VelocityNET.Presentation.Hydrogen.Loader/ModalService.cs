using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using VelocityNET.Presentation.Hydrogen.Components;
using VelocityNET.Presentation.Hydrogen.Components.Modal;
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

        public async Task<ModalResult> ShowAsync<T>(Dictionary<string, object>? parameters = null) where T : ModalComponentBase
        {
            if (ModalInstance is null)
            {
                throw new InvalidOperationException("Modal service is not initialized, no modal component");
            }

            T component = (T)ComponentFactory.Activate(typeof(T));
            if (parameters is not null)
            {
                ParameterView parameterView = ParameterView.FromDictionary(parameters);
                parameterView.SetParameterProperties(component);
            }

            return await ModalInstance.ShowAsync(component);
        }
    }
}