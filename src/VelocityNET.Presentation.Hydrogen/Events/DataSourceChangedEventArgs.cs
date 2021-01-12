using System;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Events
{

    public class DataSourceChangedEventArgs : EventArgs
    {
        private Uri Server { get; }

        public DataSourceChangedEventArgs(Uri server)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }
    }

}