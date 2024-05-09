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
		//using var appSpace = new DemoObjectSpace(path);
		
		var secret = "MyPassword";

		var dss = DSS.ECDSA_SECP256k1;
		var privateKey = Signers.CreatePrivateKey(dss, secret.ToAsciiByteArray());
		var publicKey = Signers.DerivePublicKey(dss, privateKey, 0);

		var identity = new Identity {
			DSS = DSS.ECDSA_SECP256k1,
			Key = publicKey.RawBytes
		};

		var account = new Account {
			Identity = identity,
			Name = "Savings",
			Quantity = 0
		};

		//appSpace.Commit();
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

	//public class DemoObjectSpace : ObjectSpace {

	//	public DemoObjectSpace(string file, FileAccessMode accessMode = FileAccessMode.Default)
	//		: base(BuildFileDefinition(file), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default, accessMode) {
	//	}

	//	//public IRepository<Account, long> Accounts => throw new NotImplementedException();

	//	//public IRepository<Account, long> AccountsByName => throw new NotImplementedException();

	//	//public IRepository<Identity, long> Identities => throw new NotImplementedException();

	//	//public IRepository<Identity, long> IdentitiesByKey => throw new NotImplementedException();

	//	private static HydrogenFileDescriptor BuildFileDefinition(string filePath) 
	//		=> HydrogenFileDescriptor.From(
	//			filePath, 
	//			8192, 
	//			Tools.Memory.ToBytes(50, MemoryMetric.Megabyte), 
	//			512, 
	//			ClusteredStreamsPolicy.Default
	//		);

	//	private static ObjectSpaceDefinition BuildSpaceDefinition() {
	//		var definition = new ObjectSpaceDefinition {
	//			Dimensions = new ObjectSpaceDefinition.DimensionDefinition[] {
	//				new() { 
	//					ObjectType = typeof(Account),
	//					Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
	//						new() {
	//							Type = ObjectSpaceDefinition.IndexType.RecyclableIndexStore,
	//						},
	//						new() {
	//							Type = ObjectSpaceDefinition.IndexType.UniqueKey,
	//							Member = Tools.Mapping.GetMember<Account, string>(x => x.Name),
	//						}
	//					}
	//				},
	//				new() { 
	//					ObjectType = typeof(Identity),
	//					Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
	//						new() {
	//							Type = ObjectSpaceDefinition.IndexType.RecyclableIndexStore,
	//						},
	//						new() {
	//							Type = ObjectSpaceDefinition.IndexType.UniqueKey,
	//							Member = Tools.Mapping.GetMember<Identity, byte[]>(x => x.Key),
	//						}
	//					}
	//				},


	//			}
	//		};


	//		return definition;
	//	}
	//}

	public class Account {

		[UniqueIndex]
		public string Name { get; set; }

		public decimal Quantity { get; set; }

		public Identity Identity { get; set; }

	}

	public class Identity {

		public DSS DSS { get; set; }

		[UniqueIndex]
		public byte[] Key { get; set; }
	}


	public class ScreenSettings : SettingsObject {
		public string FilePath { get; set; }
	}
}


