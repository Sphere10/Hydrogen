using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace VelocityNET.Presentation.Hydrogen.Components.Wizard
{
    /// <summary>
    /// Wizard!
    /// </summary>
    public interface IWizard<TModel> : IWizard
    {
        TModel Model { get; }
    }

    public interface IWizard
    {
        string Title { get; }
        
        Type CurrentStep { get; }

        bool HasNext { get; }

        bool HasPrevious { get; }
        
        void UpdateSteps(StepUpdateType updateType, IEnumerable<Type> steps);

        Result<bool> Next();

        Result<bool> Previous();

        Task<Result<bool>> FinishAsync();

        Task<Result<bool>> CancelAsync();
    }
}