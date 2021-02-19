﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Presentation2.Logic.Wizard
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

        WizardOptions Options { get; set; }

        void UpdateSteps(StepUpdateType updateType, IEnumerable<Type> steps);

        Result<bool> Next();

        Result<bool> Previous();

        Task<Result<bool>> FinishAsync();

        Task<Result<bool>> CancelAsync();
    }

}