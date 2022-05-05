//-----------------------------------------------------------------------
// <copyright file="AndroidSettingsProvider.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Android.App;
using System.IO;
using Hydrogen.Application;

namespace Hydrogen.Android {

	public class AndroidSettingsProvider : DirectorySettingsProvider {
		private const string AppSettingsFolderName = "ApplicationSettings";
		public AndroidSettingsProvider()
			: base(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppSettingsFolderName)) {
		}
	}
}


