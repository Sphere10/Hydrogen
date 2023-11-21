// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Hydrogen.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public static class HydrogenFrameworkExtensions {

	public static void StartWinFormsApplication<TMainForm>(this HydrogenFramework framework, Size? size = null, HydrogenFrameworkOptions options = HydrogenFrameworkOptions.Default)
		where TMainForm : class, IMainForm {
		HydrogenFramework.Instance.StartFramework(serviceCollection => serviceCollection.AddMainForm<TMainForm>(), options);
		framework.StartWinFormsApplication(size);
	}

	public static void StartWinFormsApplication(this HydrogenFramework framework, Size? size = null) {


		if (!framework.IsStarted)
			framework.StartFramework();
		var mainForm = HydrogenFramework.Instance.ServiceProvider.GetService<IMainForm>();
		if (!(mainForm is Form)) {
			throw new SoftwareException("Registered IMainForm is not a WinForms Form");
		}
		if (mainForm is IBlockManager) {
			var blockManager = mainForm as IBlockManager;
			var blocks = HydrogenFramework.Instance.ServiceProvider.GetServices<IApplicationBlock>().OrderBy(b => b.Position);
			blocks.ForEach(blockManager.RegisterBlock);
		}

		if (size != null)
			((Form)mainForm).Size = size.Value;

		System.Windows.Forms.Application.Run(mainForm as Form);
	}

}
