//-----------------------------------------------------------------------
// <copyright file="AppSettingAttribute.cs" company="Sphere 10 Software">
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

using System.ComponentModel;
using System.Configuration;

namespace Sphere10.Framework.Application {
	public class AppSettingAttribute : DefaultValueAttribute {

		public AppSettingAttribute(string key)
            : base(GetAppSetting(key)) {
			Key = key;
		}

		public string Key { get; }

		public override object Value => GetAppSetting(Key);

        private static string GetAppSetting(string key) 
            => ConfigurationManager.AppSettings[key];
    }
}

