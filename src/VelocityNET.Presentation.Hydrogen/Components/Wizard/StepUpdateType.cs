namespace VelocityNET.Presentation.Hydrogen.Components.Wizard {
    /// <summary>
    /// Operations for updating wizard steps
    /// </summary>
    public enum StepUpdateType {
        /// <summary>
        /// Inject the step after the current step, before the next step.
        /// </summary>
        Inject,

        /// <summary>
        /// Replace all steps after current step with the new step
        /// </summary>
        ReplaceAllNext,

        /// <summary>
        /// Replace all steps from the beginning with the new step
        /// </summary>
        ReplaceAll,

        /// <summary>
        /// Removes all steps after the current step matching any of the types in the parameter collection.
        /// </summary>
        RemoveNext
    }
}