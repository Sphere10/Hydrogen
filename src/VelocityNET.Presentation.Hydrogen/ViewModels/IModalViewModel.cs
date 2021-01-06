using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Components;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    public interface IModalViewModel
    {
        Task<ModalResult> ShowAsync();
    }
}