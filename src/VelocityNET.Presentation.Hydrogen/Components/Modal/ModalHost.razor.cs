using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace VelocityNET.Presentation.Hydrogen.Components.Modal
{

    /// <summary>
    /// Modal host - a single instance of this component is rendered in the main layout. A child component / render template
    /// is shown inside the modal as the modal content on demand. 
    /// </summary>
    public sealed partial class ModalHost
    {
        /// <summary>
        /// Gets or sets the modal host content render fragment.
        /// </summary>
        [Parameter] private RenderFragment? Content { get; set; }

        /// <summary>
        /// Gets or sets the JS runtime object.
        /// </summary>
        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

        /// <summary>
        /// The hosted modal component instance.
        /// </summary>
        private ModalComponentBase? _modalComponent = null!;

        /// <summary>
        /// Gets the modal content index used by render builder.
        /// </summary>
        private int ContentIndex { get; set; } 

        /// <summary>
        /// Show the modal - modal host is made visible and an instance of component <typeparam name="T"></typeparam> is
        /// rendered in the modal host. This method when awaited will return once the modal has been closed. <see cref="ModalResult"/> return
        /// value is retrieved from ModalComponent and returned once finished.
        /// </summary>
        /// <param name="parameterView"> parameters to supply to new instance of component T</param>
        /// <typeparam name="T"> type of modal component to be rendered in the host</typeparam>
        /// <returns> modal result.</returns>
        public async Task<ModalResult> ShowAsync<T>(ParameterView? parameterView = null)
            where T : ModalComponentBase
        {
            Content = builder =>
            {
                builder.OpenComponent<T>(ContentIndex++);
                builder.AddComponentReferenceCapture(ContentIndex, o => _modalComponent = (ModalComponentBase) o);
                builder.CloseComponent();
            };

            StateHasChanged();

            await AwaitModalComponentRender();
            await _modalComponent?.SetParametersAsync(parameterView ?? ParameterView.Empty)!;

            await ToggleModalAsync();
            ModalResult result = await _modalComponent.ShowAsync();
            await ToggleModalAsync();

            _modalComponent = null;
            Content = null;

            return result;
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var objectReference = DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("initializeModal", objectReference);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Delays until the modal content component has been rendered and the component reference is no longer null.
        /// </summary>
        /// <returns> a task. modal content is rendered and ref available once complete</returns>
        private async Task AwaitModalComponentRender()
        {
            int attempts = 10;
            while (_modalComponent is null || attempts <= 0)
            {
                attempts--;
                await Task.Delay(5);
            }

            if (_modalComponent is null)
            {
                throw new InvalidOperationException("Modal content did not render in time.");
            }
            else
            {
                await _modalComponent.ModalRendered;
            }
        }

        /// <summary>
        /// Shows or hides the modal and its content based.
        /// </summary>
        /// <returns></returns>
        private async Task ToggleModalAsync() => await JsRuntime.InvokeAsync<object>("toggleModal");

        /// <summary>
        /// Event handler - when modal is closed e.g. dismiss button or click away this method is invoked
        /// via JS event handler.
        /// </summary>
        [JSInvokable("OnModalClosed")]
        public void OnModalClosed()
        {
            Content = null;
            _modalComponent?.OnClose();
            StateHasChanged();
        }
    }
}