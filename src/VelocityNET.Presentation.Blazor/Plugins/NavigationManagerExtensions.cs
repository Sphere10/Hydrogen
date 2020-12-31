using Microsoft.AspNetCore.Components;

namespace VelocityNET.Presentation.Blazor.Plugins
{

    public static class NavigationManagerExtensions
    {
        /// <summary>
        /// Returns the path relative to the base including a initial slash as per page route constraints.
        /// </summary>
        /// <param name="navigationManager"></param>
        /// <param name="path"> path to convert</param>
        /// <returns> path relative to base.</returns>
        public static string ToBaseRelativePathWithSlash(this NavigationManager navigationManager, string path)
        {
            string baseRealtivePath = navigationManager.ToBaseRelativePath(path);
            return "/" + baseRealtivePath;
        }
    }

}