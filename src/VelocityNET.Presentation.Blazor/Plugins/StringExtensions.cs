namespace VelocityNET.Presentation.Blazor.Plugins
{

    /// <summary>
    /// Extension methods to help with routing
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Given a relative path to the host, excluding the host name, determines the first
        /// segment of the path which should be the app's path. e.g. /myapp/page1?param=1 => /myapp
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string ToAppPathFromBaseRelativePath(this string relativePath)
        {
            int indexOfSegment = relativePath.IndexOf('/', 1);
            string appSegment = relativePath.Substring(0, indexOfSegment > 0 ? indexOfSegment : relativePath.Length);

            return appSegment;
        }
    }

}