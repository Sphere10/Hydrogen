//-----------------------------------------------------------------------
// <copyright file="ApplicationLifecycleExtensions.cs" company="Sphere 10 Software">
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
using Sphere10.Framework.Application;

namespace Sphere10.Framework.Windows.Forms {
    public static class Sphere10FrameworkExtensions {

        public static void StartWinFormsApplication(this Sphere10Framework framework) {
            if (!framework.IsStarted)
                framework.StartFramework();
            var mainForm = ComponentRegistry.Instance.Resolve<IMainForm>();
            if (!(mainForm is Form)) {
                throw new SoftwareException("Registered IMainForm is not a WinForms Form");
            }
            if (mainForm is IBlockManager) {
                var blockManager = mainForm as IBlockManager;
                var blocks = ComponentRegistry.Instance.ResolveAll<IApplicationBlock>().OrderBy(b => ComponentRegistryExtensions.BlockPositions[b.GetType()]);
                blocks.ForEach(blockManager.RegisterBlock);
            }
            System.Windows.Forms.Application.Run(mainForm as Form);
        }

        public static void StartWinFormsApplication<TMainForm>(this Sphere10Framework framework)
            where TMainForm : class, IMainForm {
            ComponentRegistry.Instance.RegisterMainForm<TMainForm>();
            framework.StartWinFormsApplication();
        }

        public static void EndWinFormsApplication(this Sphere10Framework applicationLifecycle, out bool abort, out string abortReason) {
            applicationLifecycle.EndFramework(out abort, out abortReason);
        }
    }
}
