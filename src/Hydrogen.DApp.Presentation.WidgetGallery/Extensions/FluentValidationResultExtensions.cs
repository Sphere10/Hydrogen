using FluentValidation.Results;
using Hydrogen;

namespace Hydrogen.DApp.Presentation.WidgetGallery.Extensions {
    public static class FluentValidationResultExtensions {

        public static Result ToResult(this ValidationResult validationResult) {
            if (validationResult.IsValid) {
                return Result.Valid;
            } else {
                var result = new Result();
                foreach (var error in validationResult.Errors) {
                    result.AddError(error.ErrorMessage);
                }
                return result;
            }
        }

    }
}
