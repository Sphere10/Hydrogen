//-----------------------------------------------------------------------
// <copyright file="HttpContextExtensions.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;

namespace Sphere10.Framework.Web.AspNetCore {
    public static class HttpContextExtensions {
        public const string UserMessagesKey = "UserMessages_449E67E9-F455-454a-86A7-1E49CB8F7A9B";

        public static Collection<UserMessage> GetUserMessages(this HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            // if context doesn't have the user messages collection, lazily create it
            if (!context.Items.ContainsKey(UserMessagesKey) || context.Items[UserMessagesKey].GetType() != typeof(Collection<UserMessage>)) {
                lock (context) {
                    // Uses double-checked locking to avoid race condition (thread safe)
                    if (!context.Items.ContainsKey(UserMessagesKey) || context.Items[UserMessagesKey].GetType() != typeof(Collection<UserMessage>)) {
                        context.Items[UserMessagesKey] = new Collection<UserMessage>();
                    }
                }
            }
            return context.Items[UserMessagesKey] as Collection<UserMessage>;
        }
    }
}
