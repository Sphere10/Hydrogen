//-----------------------------------------------------------------------
// <copyright file="IWizardScreen.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms {
    public interface IWizardScreen<T> {
        IWizard<T> Wizard { get; }
        Task Initialize();
        Task OnPresent();
        Task OnPrevious();
        Task OnNext();
        Task<Result> Validate();
    }
}
