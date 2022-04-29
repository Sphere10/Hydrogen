//-----------------------------------------------------------------------
// <copyright file="StandardProductUsageProvider.cs" company="Sphere 10 Software">
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
using System.ComponentModel;

namespace Hydrogen.Application {

	public class StandardProductUsageServices : IProductUsageServices {

		private readonly UserSettings _userSettings;
		private readonly SystemSettings _systemSettings;

		public class UserSettings : SettingsObject {
			[DefaultValue(null)]
			public DateTime? FirstTimeExecutedByUser { get; set; }

			[DefaultValue(0)]
			public int NumberOfUsesByUser { get; set; }
		}

		public class SystemSettings : SettingsObject {
			[DefaultValue(null)]
			public DateTime? FirstTimeExecutedBySystem { get; set; }

			[DefaultValue(0)]
			public int NumberOfUsesBySystem { get; set; }
		}


		public StandardProductUsageServices(IConfigurationServices configurationServices) {
			ConfigurationServices = configurationServices;
			_userSettings = ConfigurationServices.UserSettings.Get<UserSettings>();
			_systemSettings = ConfigurationServices.SystemSettings.Get<SystemSettings>();
		}

		public IConfigurationServices ConfigurationServices { get; set; }

		public ProductUsageInformation ProductUsageInformation => new ProductUsageInformation {
			DaysUsedBySystem = (int)Math.Ceiling(DateTime.UtcNow.Subtract(this.FirstUTCTimeExecutedBySystem.GetValueOrDefault(DateTime.UtcNow)).TotalDays),
			DaysUsedByUser = (int)Math.Ceiling(DateTime.UtcNow.Subtract(this.FirstUTCTimeExecutedByUser.GetValueOrDefault(DateTime.UtcNow)).TotalDays),
			FirstUsedDateBySystemUTC = this.FirstUTCTimeExecutedBySystem.GetValueOrDefault(DateTime.UtcNow),
			FirstUsedDateByUserUTC = this.FirstUTCTimeExecutedByUser.GetValueOrDefault(DateTime.UtcNow),
			NumberOfUsesBySystem = NumberOfUsesBySystem,
			NumberOfUsesByUser = NumberOfUsesByUser
		};

		protected virtual DateTime? FirstUTCTimeExecutedByUser => _userSettings.FirstTimeExecutedByUser;

		protected virtual int NumberOfUsesByUser => _userSettings.NumberOfUsesByUser;

		protected virtual DateTime? FirstUTCTimeExecutedBySystem => _systemSettings.FirstTimeExecutedBySystem;

		protected virtual int NumberOfUsesBySystem => _systemSettings.NumberOfUsesBySystem;

		public void IncrementUsageByOne() {
			_userSettings.FirstTimeExecutedByUser = _userSettings.FirstTimeExecutedByUser ?? DateTime.UtcNow;
			_userSettings.NumberOfUsesByUser++;
			_systemSettings.FirstTimeExecutedBySystem = _systemSettings.FirstTimeExecutedBySystem ?? DateTime.UtcNow;
			_systemSettings.NumberOfUsesBySystem++;
			_userSettings.Save();
			_systemSettings.Save();
		}
	}
}

