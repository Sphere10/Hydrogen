using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sphere10.Framework.Web.AspNetCore {
    public class FormActionAttribute : ActionFilterAttribute {
        public const string OmitFormTag = "OmitFormElement";
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var controller = (Controller)filterContext.Controller;
            controller.ViewData[OmitFormTag] = true;
        }
    }
}