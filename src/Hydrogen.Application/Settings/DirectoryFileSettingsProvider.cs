// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen.Application;

public class DirectoryFileSettingsProvider : BaseSettingsProvider {

	private readonly IFuture<string> _baseDirectory;

	public DirectoryFileSettingsProvider(string directory, DirectorySettingsProviderPolicy policy = DirectorySettingsProviderPolicy.Default)
		: this(Tools.Values.Future.Explicit(directory), policy) {
	}

	public DirectoryFileSettingsProvider(IFuture<string> directory, DirectorySettingsProviderPolicy policy = DirectorySettingsProviderPolicy.Default) {
		_baseDirectory = directory;
		Policy = policy;
	}

	public string BaseDirectory => _baseDirectory.Value;

	public DirectorySettingsProviderPolicy Policy { get; protected set; }

	public override bool ContainsSetting(Type settingsObjectType, object id = null) {
		var path = DetermineFilepath(settingsObjectType, id);
		if (!File.Exists(path))
			return false;

		if (!Tools.FileSystem.IsFileEmpty(path))
			return true;

		File.Delete(path);
		return false;
	}

	public override void DeleteSetting(SettingsObject settings) {
		var filename = DetermineFilepath(settings);
		if (File.Exists(filename))
			File.Delete(filename);
		FireChanged();
	}

	public override void ClearSettings() {
		if (Directory.Exists(BaseDirectory)) {
			foreach (var filePath in Directory.GetFiles(BaseDirectory)) {
				if (Path.GetExtension(filePath).ToUpper() == ".SETTING")
					File.Delete(filePath);
			}
		}
		FireChanged();
	}

	protected override SettingsObject LoadInternal(Type settingsObjectType, object id = null) {
		var file = DetermineFilepath(settingsObjectType, id);
		if (Tools.Xml.IsXmlFile(file))
			return Tools.Xml.DeepReadFromFile<SettingsObject>(file);
		return (SettingsObject)Tools.Json.ReadFromFile(settingsObjectType, file);
	}

	protected override void SaveInternal(SettingsObject settings) {
		if (!Directory.Exists(BaseDirectory)) {
			Directory.CreateDirectory(BaseDirectory);
		}
		var filePath = DetermineFilepath(settings);
		var fileExists = File.Exists(filePath);
		try {
			Tools.Json.WriteToFile(filePath, settings);
		} catch {
			if (!fileExists && File.Exists(filePath)) {
				Tools.Exceptions.ExecuteIgnoringException(() => File.Delete(filePath));
			}
			throw;
		}
	}


	#region Auxillary methods

	protected string DetermineFilepath(SettingsObject settings) {
		return DetermineFilepath(settings.GetType(), settings.ID);
	}

	protected string DetermineFilepath(Type settingsObjectType, object id = null) {
		return Path.Combine(BaseDirectory, DetermineFilename(settingsObjectType, id));
	}

	protected virtual string DetermineFilename(SettingsObject settingsObject) {
		Guard.ArgumentNotNull(settingsObject, "settingsObject");
		return DetermineFilename(settingsObject.GetType(), settingsObject.ID);
	}

	protected virtual string DetermineFilename(Type settingsObjectType, object id) {
		Guard.ArgumentNotNull(settingsObjectType, "settingsObjectType");
		string filename = null;
		filename =
			Policy.HasFlag(DirectorySettingsProviderPolicy.UseFullyQualifiedTypeNameInFilename) ? settingsObjectType.FullName : settingsObjectType.GetShortName();

		if (id != null)
			filename = CombineExtensionlessFilenameWithID(filename, id.ToString());

		return AddFileExtension(filename);
	}

	protected virtual string CombineExtensionlessFilenameWithID(string extensionlessFilename, string id) {
		return extensionlessFilename + "_" + Tools.FileSystem.ToWellFormedFileName(id);
	}

	protected virtual string AddFileExtension(string partialFilename) {
		return partialFilename + ".setting";
	}

	#endregion

}
