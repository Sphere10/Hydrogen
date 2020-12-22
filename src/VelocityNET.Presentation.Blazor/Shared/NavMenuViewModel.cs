namespace VelocityNET.Presentation.Blazor.Shared
{
    public class NavMenuViewModel
    {
        private bool collapseNavMenu = true;

        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
