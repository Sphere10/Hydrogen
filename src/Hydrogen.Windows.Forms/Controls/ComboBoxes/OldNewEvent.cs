//-----------------------------------------------------------------------
// <copyright file="OldNewEvent.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms
{
    public class OldNewEventArgs<T> : EventArgs
    {
        public OldNewEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public T OldValue
        {
            get { return this.m_oldValue; }
            protected set { this.m_oldValue = value; }
        }
        public T NewValue
        {
            get { return this.m_newValue; }
            protected set { this.m_newValue = value; }
        }

        T m_oldValue = default(T);
        T m_newValue = default(T);
    }

    public delegate void OldNewEventHandler<T>(object sender, OldNewEventArgs<T> e);
}
