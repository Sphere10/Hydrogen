using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared.Plugins;
using VelocityNET.Presentation.Blazor.WidgetGallery.Widgets;

namespace VelocityNET.Presentation.Blazor.WidgetGallery
{

    /// <summary>
    /// 
    /// </summary>
    public class WidgetsBlock : IAppBlock
    {
        public WidgetsBlock()
        {
            AppBlockPages = new List<IAppBlockPage>
            {
                new EntityGrid()
            };
        }
        
        public string Name { get; } = "Widgets";

        public IEnumerable<IAppBlockPage> AppBlockPages { get; }
    }
}