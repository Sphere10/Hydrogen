using FluentValidation;
using Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Models;

namespace Sphere10.Hydrogen.Presentation.WidgetGallery.Widgets.Validators {
    public class NewWidgetModelValidator : AbstractValidator<NewWidgetModel> {
        public NewWidgetModelValidator() {
            RuleFor(x => x.Price).NotNull().GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}