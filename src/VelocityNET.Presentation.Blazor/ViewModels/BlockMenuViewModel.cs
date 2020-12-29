using System;
using System.Collections.Generic;
using VelocityNET.Presentation.Blazor.Shared;
using VelocityNET.Presentation.Blazor.Shared.Plugins;

namespace VelocityNET.Presentation.Blazor.ViewModels
{
    public class BlockMenuViewModel : ComponentViewModelBase
    {
        public IEnumerable<IAppBlock> AppBlocks { get; }
        public BlockMenuViewModel(IEnumerable<IAppBlock> appBlocks)
        {
            AppBlocks = appBlocks ?? throw new ArgumentNullException(nameof(appBlocks));
        }
    }
}