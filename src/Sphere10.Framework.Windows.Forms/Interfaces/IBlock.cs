//-----------------------------------------------------------------------
// <copyright file="IBlock.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Sphere10.Framework.Application;

namespace Sphere10.Framework.Windows.Forms {

#warning Add Option Dialogs
#warning Add 8x8 special icon (outlook bar bottom)

    public interface IApplicationBlock : IDisposable {
        string Name { get; }
        IMenu[] Menus { get; }
        Image Image32x32 { get; }
        Image Image8x8 { get; }
        string HelpFileCHM { get; }
        bool ShowInMenuStrip { get; }
        bool ShowInToolStrip { get; }

        Type DefaultScreen { get; }
    }

}
