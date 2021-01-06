using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{
    public sealed partial class Modal
    {
        [Parameter] 
        public RenderFragment Content { get; set; } = null!;
        
        [Inject] 
        private IJSRuntime JsRuntime { get; set; } = null!;

        public async Task<ModalResult> ShowAsync<T>(T component) where T : ModalComponentBase
        {
            Content = RenderComponent(component);
            StateHasChanged();

            await JsRuntime.InvokeAsync<object>("toggleModal");
            ModalResult result = await component.ShowAsync();
            await JsRuntime.InvokeAsync<object>("toggleModal");
            
            return result;
        }

        //https://stackoverflow.com/a/48551735/66988
        private static FieldInfo? GetPrivateField(Type? t, string name)
        {
            const BindingFlags bf = BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

            FieldInfo? fi;
            while ((fi = t?.GetField(name, bf)) == null && (t = t?.BaseType) != null)
            {
            }

            return fi;
        }

        private RenderFragment RenderComponent(ComponentBase instance)
        {
            var fragmentField = GetPrivateField(instance.GetType(), "_renderFragment");

            var value = (RenderFragment) fragmentField.GetValue(instance);

            return value;
        }
    }
}