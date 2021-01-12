using System.Collections.Generic;

namespace VelocityNET.Presentation.Hydrogen.ViewModels
{
    public class PagedTableViewModel<T> : ComponentViewModelBase
    {
        public IEnumerable<T> Items { get; set; }
    }
}