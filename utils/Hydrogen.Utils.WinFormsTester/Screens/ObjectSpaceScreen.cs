// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
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
		using var appSpace = new DemoObjectSpace(path);

		appSpace.Commit();
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

	public class DemoObjectSpace : ObjectSpace {

		public DemoObjectSpace(string file, FileAccessMode accessMode = FileAccessMode.Default)
			: base(BuildFileDefinition(file), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default, accessMode) {
		}

		public IRepository<Account, long> Accounts => throw new NotImplementedException();

		public IRepository<Account, long> AccountsByName => throw new NotImplementedException();

		public IRepository<Identity, long> Identities => throw new NotImplementedException();

		public IRepository<Identity, long> IdentitiesByKey => throw new NotImplementedException();

		private static HydrogenFileDescriptor BuildFileDefinition(string filePath) 
			=> HydrogenFileDescriptor.From(
				filePath, 
				8192, 
				Tools.Memory.ToBytes(50, MemoryMetric.Megabyte), 
				512, 
				StreamContainerPolicy.Default
			);

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


