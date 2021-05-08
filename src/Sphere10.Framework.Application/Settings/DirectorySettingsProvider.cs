//-----------------------------------------------------------------------
// <copyright file="DirectorySettingsProvider.cs" company="Sphere 10 Software">
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
using System.IO;

namespace Sphere10.Framework.Application {
	public class DirectorySettingsProvider : BaseSettingsProvider {

		public DirectorySettingsProvider(string directory, DirectorySettingsProviderPolicy policy = DirectorySettingsProviderPolicy.Default) {
			BaseDirectory = directory;
			Policy = policy;
		}

		public string BaseDirectory { get; protected set; }

		public DirectorySettingsProviderPolicy Policy { get; protected set; }

		public override bool ContainsSetting(Type settingsObjectType, object id = null) {
			return File.Exists(DetermineFilepath(settingsObjectType, id));
		}

		public override void DeleteSetting(SettingsObject settings) {
			var filename = DetermineFilepath(settings);
			if (File.Exists(filename))
				File.Delete(filename);
		}

		public override void ClearSettings() {
			if (Directory.Exists(BaseDirectory)) {
				foreach(var filePath in Directory.GetFiles(BaseDirectory)) {
					if (Path.GetExtension(filePath).ToUpper() == ".SETTING")
						File.Delete(filePath);
				}
			}
		}


		protected override SettingsObject LoadInternal(Type settingsObjectType, object id = null) {
            return Tools.Xml.DeepReadFromFile<SettingsObject>(DetermineFilepath(settingsObjectType, id));
		}

		protected override void SaveInternal(SettingsObject settings) {
			if (!Directory.Exists(BaseDirectory)) {
				Directory.CreateDirectory(BaseDirectory);
			}
			var filePath = DetermineFilepath(settings);
			var fileExists = File.Exists(filePath);
			try
            {
                Tools.Xml.DeepWriteToFile(DetermineFilepath(settings), settings);		
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
				Policy.HasFlag(DirectorySettingsProviderPolicy.UseFullyQualifiedTypeNameInFilename) ?
				settingsObjectType.FullName :
				settingsObjectType.GetShortName();

			if (id != null)
				filename = CombineExtensionlessFilenameWithID(filename, id.ToString());

			return AddFileExtension(filename);
		}

		protected virtual string CombineExtensionlessFilenameWithID(string extensionlessFilename, string id) {
			return extensionlessFilename + "_" + id.ToPathSafe();
		}

		protected virtual string AddFileExtension(string partialFilename) {
			return partialFilename + ".setting";
		}

		#endregion

	}

	[Flags]
	public enum DirectorySettingsProviderPolicy {
		CreateDirectoryIfNotExists				= 1 << 0,
		UseFullyQualifiedTypeNameInFilename		= 1 << 1,
		Default									= CreateDirectoryIfNotExists
	}
}
