//-----------------------------------------------------------------------
// <copyright file="UserMessage.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

namespace Sphere10.Framework.Web.AspNetCore {
    /// <summary>
    /// Describes a message to a user. Currently used within the MVC pipeline (BPM) to pass warning messages in TransferResults.
    /// </summary>
    public sealed class UserMessage
    {
        public string Message { get; set; }
        public UserMessageType MessageType { get; set; }

        public UserMessage()
        {
            Message = string.Empty;
            MessageType = UserMessageType.Information;
        }

        public UserMessage(string message, UserMessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }

    }
}
