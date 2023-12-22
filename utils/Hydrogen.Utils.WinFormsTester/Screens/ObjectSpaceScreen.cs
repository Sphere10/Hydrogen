// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using Hydrogen.Application;
using Hydrogen.ObjectSpaces;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ObjectSpaceScreen : ApplicationScreen {
	private ScreenSettings _settings;

	public ObjectSpaceScreen() {
		InitializeComponent();
	}

	protected override void InitializeUIPrimingData() {
		base.InitializeUIPrimingData();
		LoadSettings();
	}

	private void DoConsensusSpaceDemo(string path) {
		using var appSpace = new MockAppDataSpace(path);
		appSpace.Load();
	}

	private void SaveSettings() {
		_settings.FilePath = _objectSpacePathControl.Path;
		_settings.Save();
	}

	private void LoadSettings() {
		_settings = UserSettings.Get<ScreenSettings>();
		_objectSpacePathControl.Path = _settings.FilePath;
	}

	private void _buildButton_Click(object sender, EventArgs e) {
		try {
			SaveSettings();
			DoConsensusSpaceDemo(_objectSpacePathControl.Path);
	
		} catch (Exception ex) {
			ExceptionDialog.Show(ex);
		}
	}

	public class MockAppDataSpace : ObjectSpace {

		public MockAppDataSpace(string file)
			: base(BuildFileDefinition(file), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default) {
		}

		public IRepository<long, Account> Accounts { get; }

		public IRepository<string, Account> AccountsByName { get; }

		public IRepository<long, Identity> Identities { get; }

		public IRepository<long, Identity> IdentitiesByKey { get; }

		private static ObjectSpaceFile BuildFileDefinition(string filePath) {
			Guard.ArgumentNotNull(filePath, nameof(filePath));
			var parentPath = Tools.FileSystem.GetParentDirectoryPath(filePath);
			var txnDir = Path.Combine(parentPath, ".txn");
			if (!Path.Exists(txnDir))
				Tools.FileSystem.CreateDirectory(txnDir);

			return new ObjectSpaceFile {
				FilePath = filePath,
				PageFileDir = txnDir,
				PageSize = 8192,
				MaxMemory = Tools.Memory.ToBytes(50, MemoryMetric.Megabyte),
				ClusterSize = 512,
				ContainerPolicy = StreamContainerPolicy.Default
			};
		}

		private static ObjectSpaceDefinition BuildSpaceDefinition() {
			var definition = new ObjectSpaceDefinition {
				Containers = new ObjectSpaceDefinition.ContainerDefinition[] {
					new() { 
						ObjectType = typeof(Account),
						Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
							new() {
								Type = ObjectSpaceDefinition.IndexType.FreeIndexStore,
								ReservedStreamIndex = 0,
							},
							new() {
								Type = ObjectSpaceDefinition.IndexType.UniqueKey,
								KeyMember = Tools.Mapping.GetMember<Account, string>(x => x.Name),
								ReservedStreamIndex = 1
							}
						}
					},
					new() { 
						ObjectType = typeof(Identity),
						Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
							new() {
								Type = ObjectSpaceDefinition.IndexType.FreeIndexStore,
								ReservedStreamIndex = 0,
							},
							new() {
								Type = ObjectSpaceDefinition.IndexType.UniqueKey,
								KeyMember = Tools.Mapping.GetMember<Identity, byte[]>(x => x.Key),
								ReservedStreamIndex = 0
							}
						}
					},


				}
			};


			return definition;
		}
	}

	public class Account {
		
		[UniqueProperty]
		public string Name { get; set; }
		
		public decimal Quantity { get; set; }

		public Identity Identity { get; set; }

	}

	public class Identity { 

		public DSS DSS { get; set; }

		[UniqueProperty]
		public byte[] Key { get; set; }
	}


	public class ScreenSettings : SettingsObject {
		public string FilePath { get; set; }
	}
}

