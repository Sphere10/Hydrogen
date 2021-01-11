using System;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Events
{

    public class DataSourceChangedEventArgs : EventArgs
    {
        private Server Server { get; }

        public DataSourceChangedEventArgs(Server server)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }
    }

}