// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.IO;
using Hydrogen.Application;
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class SettingsTest : ApplicationScreen {
	private readonly TextWriter _outputWriter;
	public SettingsTest() {
		InitializeComponent();
		_outputWriter = new TextBoxWriter(_outputTextBox);
	}

	private async void _test1Button_Click(object sender, EventArgs e) {

		var userSettings = UserSettings.Get<Test1Settings>();
		await _outputWriter.WriteLineAsync($"USER SETTINGS");
		await _outputWriter.WriteLineAsync($"DBMSType: {userSettings.DBMSType}");
		await _outputWriter.WriteLineAsync($"ConnectionString: {userSettings.ConnectionString}");
		await _outputWriter.WriteLineAsync($"ActiveLanguage: {userSettings.ActiveLanguage}");
		await _outputWriter.WriteLineAsync($"FirstTimeRun: {userSettings.FirstTimeRun}");
		await _outputWriter.WriteLineAsync($"RunCount: {userSettings.RunCount}");
		await _outputWriter.WriteLineAsync($"FirstRunDate: {userSettings.FirstRunDate}");
		await _outputWriter.WriteLineAsync($"Secret: {userSettings.Secret}");
		await _outputWriter.WriteLineAsync("");

		var globalSettings = GlobalSettings.Get<Test1Settings>();
		await _outputWriter.WriteLineAsync($"Global SETTINGS");
		await _outputWriter.WriteLineAsync($"DBMSType: {globalSettings.DBMSType}");
		await _outputWriter.WriteLineAsync($"ConnectionString: {globalSettings.ConnectionString}");
		await _outputWriter.WriteLineAsync($"ActiveLanguage: {globalSettings.ActiveLanguage}");
		await _outputWriter.WriteLineAsync($"FirstTimeRun: {globalSettings.FirstTimeRun}");
		await _outputWriter.WriteLineAsync($"RunCount: {globalSettings.RunCount}");
		await _outputWriter.WriteLineAsync($"FirstRunDate: {globalSettings.FirstRunDate}");
		await _outputWriter.WriteLineAsync($"Secret: {globalSettings.Secret}");
		await _outputWriter.WriteLineAsync("");

		userSettings.RunCount++;
		userSettings.Secret = $"Hello! {userSettings.RunCount}";

		globalSettings.RunCount++;
		globalSettings.Secret = $"Hello! {userSettings.RunCount}";

		userSettings.Save();
		globalSettings.Save();
	}
}


public class Test1Settings : SettingsObject {

	[DefaultValue(DBMSType.Sqlite)] public DBMSType DBMSType { get; set; }


	[DefaultValue(null)] public string ConnectionString { get; set; }


	[DefaultValue("en")] public string ActiveLanguage { get; set; }

	[DefaultValue(true)] public bool FirstTimeRun { get; set; }

	[DefaultValue(0)] public int RunCount { get; set; }

	[DefaultDate(true, 0, 0, 0, 0, 0, 0, 0)]
	public DateTime FirstRunDate { get; set; }

	[EncryptedString] public string Secret { get; set; } = "Hello!";

}
