﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VelocityNET.Presentation.Hydrogen.Models;

namespace VelocityNET.Presentation.Hydrogen.Services
{

    /// <summary>
    /// Service used to manage server configuration.
    /// </summary>
    public interface IServerConfigService
    {
        /// <summary>
        /// Raised when active server has been changed.
        /// </summary>
        event EventHandler<EventArgs> ActiveServerChanged;
        
        /// <summary>
        /// Raised when a new server is added
        /// </summary>
        event EventHandler<EventArgs>? NewServerAdded; 

        /// <summary>
        /// Gets the currently active server.
        /// </summary>
        Uri ActiveServer { get; }

        /// <summary>
        /// Gets the available servers
        /// </summary>
        public IEnumerable<Uri> AvailableServers { get; }

        /// <summary>
        /// Sets the active server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        Task SetActiveServerAsync(Uri server);

        /// <summary>
        /// Adds another server config. Should be persisted.
        /// </summary>
        /// <param name="server"> config</param>
        /// <returns></returns>
        Task AddServerAsync(Uri server);

        /// <summary>
        /// Validates the given server details
        /// </summary>
        /// <param name="server"> server</param>
        /// <returns> whether this is a valid server</returns>
        Task<bool> ValidateServerAsync(Uri server);
    }

}