using System;
using Sphere10.Hydrogen.Presentation.Models;

namespace Sphere10.Hydrogen.Presentation.Events {

    public class DataSourceChangedEventArgs : EventArgs {
        private Uri Server { get; }

        public DataSourceChangedEventArgs(Uri server) {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }
    }

}