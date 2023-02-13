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

namespace Hydrogen.Application;

public class ProductUsageServices : IProductUsageServices {

	private readonly UsageSettings _userSettings;
	private readonly UsageSettings _systemSettings;

	public ProductUsageServices(ISettingsServices settingsServices) {
		SettingsServices = settingsServices;
		_userSettings = SettingsServices.UserSettings.Get<UsageSettings>();
		_systemSettings = SettingsServices.SystemSettings.Get<UsageSettings>();
	}

	public ISettingsServices SettingsServices { get; set; }

	public ProductUsageInformation ProductUsageInformation => new ProductUsageInformation {
		DaysUsedBySystem = (int)Math.Ceiling(DateTime.UtcNow.Subtract(this.FirstUTCTimeExecutedBySystem.GetValueOrDefault(DateTime.UtcNow)).TotalDays),
		DaysUsedByUser = (int)Math.Ceiling(DateTime.UtcNow.Subtract(this.FirstUTCTimeExecutedByUser.GetValueOrDefault(DateTime.UtcNow)).TotalDays),
		FirstUsedDateBySystemUTC = this.FirstUTCTimeExecutedBySystem.GetValueOrDefault(DateTime.UtcNow),
		FirstUsedDateByUserUTC = this.FirstUTCTimeExecutedByUser.GetValueOrDefault(DateTime.UtcNow),
		NumberOfUsesBySystem = NumberOfUsesBySystem,
		NumberOfUsesByUser = NumberOfUsesByUser
	};

	protected virtual DateTime? FirstUTCTimeExecutedByUser => _userSettings.FirstRunDate;

	protected virtual int NumberOfUsesByUser => _userSettings.NumberOfRuns;

	protected virtual DateTime? FirstUTCTimeExecutedBySystem => _systemSettings.FirstRunDate;

	protected virtual int NumberOfUsesBySystem => _systemSettings.NumberOfRuns;

	public void IncrementUsageByOne() {
		_userSettings.FirstRunDate ??= DateTime.UtcNow;
		_userSettings.NumberOfRuns++;
		_systemSettings.FirstRunDate ??= DateTime.UtcNow;
		_systemSettings.NumberOfRuns++;
		_userSettings.Save();
		_systemSettings.Save();
	}

	public class UsageSettings : SettingsObject {
		[DefaultValue(null)]
		public DateTime? FirstRunDate { get; set; }

		[DefaultValue(0)]
		public int NumberOfRuns { get; set; }
	}




}


