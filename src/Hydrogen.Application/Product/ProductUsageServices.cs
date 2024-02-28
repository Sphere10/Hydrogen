// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Hydrogen.Application;

public class ProductUsageServices : IProductUsageServices {
	private const string TamperCheckKey = "Alpha";
	private const string TamperCheckValue = "CBAEA9E8-B883-4C38-80A2-E70973D4C273";


	private readonly UsageSettings _userSettings;
	private readonly UsageSettings _systemSettings;

	private readonly IFuture<IDictionary<string, object>> _userPropertiesFuture;

	private readonly IFuture<IDictionary<string, object>> _systemPropertiesFuture;

	public ProductUsageServices(ISettingsServices settingsServices) {
		SettingsServices = settingsServices;
		_userSettings = SettingsServices.UserSettings.Get<UsageSettings>();
		_systemSettings = SettingsServices.SystemSettings.Get<UsageSettings>();
		_userPropertiesFuture = Tools.Values.Future.LazyLoad(() => TamperCheckLoad(_userSettings.EncryptedPropertiesJson));
		_systemPropertiesFuture = Tools.Values.Future.LazyLoad(() => TamperCheckLoad(_systemSettings.EncryptedPropertiesJson));
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

	protected virtual DateTime? FirstUTCTimeExecutedByUser {
		get => _userSettings.FirstRunDate;
		set {
			_userSettings.FirstRunDate = value;
			_userSettings.Save();
		}
	}

	protected virtual int NumberOfUsesByUser {
		get => _userSettings.NumberOfRuns;
		set {
			_userSettings.NumberOfRuns = value;
			_userSettings.Save();
		}
	}

	protected virtual DateTime? FirstUTCTimeExecutedBySystem {
		get => _systemSettings.FirstRunDate;
		set {
			_systemSettings.FirstRunDate = value;
			_systemSettings.Save();
		}
	}

	protected virtual int NumberOfUsesBySystem {
		get => _systemSettings.NumberOfRuns;
		set {
			_systemSettings.NumberOfRuns = value;
			_systemSettings.Save();
		}
	}

	public void IncrementUsageByOne() {
		_userSettings.FirstRunDate ??= DateTime.UtcNow;
		_userSettings.NumberOfRuns++;
		_userSettings.Save();
		_systemSettings.FirstRunDate ??= DateTime.UtcNow;
		_systemSettings.NumberOfRuns++;
		_systemSettings.Save();

	}

	public IDictionary<string, object> UserEncryptedProperties {
		get {
			var dict = new ObservableDictionary<string, object>(_userPropertiesFuture.Value);
			dict.Mutated += (_, _) => {
				_userSettings.EncryptedPropertiesJson = Tools.Json.WriteToString(_userPropertiesFuture.Value);
				_userSettings.Save();
			};
			return dict;
		}
	}

	public IDictionary<string, object> SystemEncryptedProperties {
		get {
			var dict = new ObservableDictionary<string, object>(_systemPropertiesFuture.Value);
			dict.Mutated += (_, _) => {
				_systemSettings.EncryptedPropertiesJson = Tools.Json.WriteToString(_systemPropertiesFuture.Value);
				_systemSettings.Save();
			};
			return dict;
		}
	}

	private IDictionary<string, object> TamperCheckLoad(string encryptedJson) {

		// Removed 2024-02-29: causes license failures on edge-case scenarios involving usage of system > 1 but no encrypted settings written before
		//if (string.IsNullOrWhiteSpace(encryptedJson))
		//	if (NumberOfUsesBySystem > 1)
		//		throw new ProductLicenseTamperedException();

		var dict = encryptedJson != null ? Tools.Json.ReadFromString<IDictionary<string, object>>(encryptedJson) : new Dictionary<string, object> { [TamperCheckKey] = TamperCheckValue };

		if (!dict.TryGetValue(TamperCheckKey, out var value) || !StringComparer.InvariantCulture.Equals(value, TamperCheckValue))
			throw new ProductLicenseTamperedException();

		return dict;
	}


	public class UsageSettings : SettingsObject {
		[DefaultValue(null)] public DateTime? FirstRunDate { get; set; }

		[DefaultValue(0)] public int NumberOfRuns { get; set; }

		[EncryptedString] public string EncryptedPropertiesJson { get; set; }
	}


}
