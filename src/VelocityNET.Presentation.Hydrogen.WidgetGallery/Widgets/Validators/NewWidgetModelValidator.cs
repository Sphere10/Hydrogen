using FluentValidation;
using VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Validators {
    public class NewWidgetModelValidator : AbstractValidator<NewWidgetModel> {
        public NewWidgetModelValidator() {
            RuleFor(x => x.Price).NotNull().GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}