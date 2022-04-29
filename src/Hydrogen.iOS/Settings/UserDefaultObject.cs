//-----------------------------------------------------------------------
// <copyright file="UserDefaultObject.cs" company="Sphere 10 Software">
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

// This needs to be refactored into UserDefaultSettingsProvider
#if __IOS__
	#define INCLUDE_FILE
#endif

#if MONOMAC
	#define INCLUDE_FILE
#endif

#if INCLUDE_FILE
using System;
using System.ComponentModel;
using System.Reflection;
using Foundation;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.iOS {


	public abstract class UserDefaultObject : SettingsObject {
		public UserDefaultObject(object instanceID) {
			base.ID = instanceID;
			if (ContainsKey(PropertyNameToKey("InstanceID")))
				Load();
		}

		public override void Load() {
			foreach (var f in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField)) {
				foreach (Attribute attr in f.GetCustomAttributes(true)) {
					if (attr is DefaultValueAttribute) {
						var dv = (DefaultValueAttribute)attr;
						f.SetValue(this, LoadPropertyValue(f.Name, f.FieldType));
					}
				}
			}
			
			foreach (var p in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty)) {
				if (p.GetIndexParameters().Length == 0) {
					foreach (Attribute attr in p.GetCustomAttributes(true)) {
						if (attr is DefaultValueAttribute) {
							var dv = (DefaultValueAttribute)attr;

							if (p.Name == "Domain") {
								var xxx = LoadPropertyValue(p.Name, p.PropertyType);
							}

							p.SetValue(this, LoadPropertyValue(p.Name, p.PropertyType), null);
						}
					}
				}
			}

		}
		
		public override void Save() {
			foreach (var f in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField)) {
				foreach (Attribute attr in f.GetCustomAttributes(true)) {
					if (attr is DefaultValueAttribute) {
						var dv = (DefaultValueAttribute)attr;
						SavePropertyValue(f.Name, f.GetValue(this));
					}
				}
			}
			
			foreach (var p in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty)) {
				if (p.GetIndexParameters().Length == 0) {
					foreach (Attribute attr in p.GetCustomAttributes(true)) {
						if (attr is DefaultValueAttribute) {
							var dv = (DefaultValueAttribute)attr;
							SavePropertyValue(p.Name, p.GetValue(this, null));
						}
					}
				}
			}
			SavePropertyValue("InstanceID", base.ID);
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		private bool ContainsKey(string key) {
			return NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(key)) != null;
		}

		private string PropertyNameToKey(string name) {
			return string.Format ("{0}.{1}", base.ID, name);
		}

		private T LoadPropertyValue<T>(string propertyName) {
			return (T)LoadPropertyValue(propertyName, typeof(T));
		}

		private object LoadPropertyValue(string propertyName, Type expectedValueType) {
			var key = PropertyNameToKey(propertyName);
			object storedValue = null;
			switch(GetStorageClass(expectedValueType)) {
				case StorageClass.String:
					storedValue = NSUserDefaults.StandardUserDefaults.StringForKey(key);
					break;
				case StorageClass.Integer:
					storedValue = NSUserDefaults.StandardUserDefaults.IntForKey(key);
					break;
				case StorageClass.Bool:
					storedValue = NSUserDefaults.StandardUserDefaults.BoolForKey(key);
					break;
				case StorageClass.Float:
					storedValue = NSUserDefaults.StandardUserDefaults.FloatForKey(key);
					break;
				case StorageClass.Double:
					storedValue = NSUserDefaults.StandardUserDefaults.DoubleForKey(key);
					break;
				case StorageClass.XMLSerialized:
					storedValue = XmlProvider.ReadFromString(expectedValueType, NSUserDefaults.StandardUserDefaults.StringForKey(key));
					break;
				default:
					throw new Exception("Internal Error F1C3E9A8-BAD2-48AF-9533-E78DE234B353");
			}

			object result;
			if (storedValue.GetType() != expectedValueType) {
				var converter = TypeDescriptor.GetConverter(expectedValueType);
				result = converter.ConvertFrom(storedValue);
			} else {
				result = storedValue;
			}
			return result;
		}

		private void SavePropertyValue(string propertyName, object value) {
			var key = PropertyNameToKey(propertyName);
			switch(GetStorageClass(value.GetType())) {
			case StorageClass.String:
				NSUserDefaults.StandardUserDefaults.SetString(ConvertExChangeType<string>(value), PropertyNameToKey(propertyName));
				break;
			case StorageClass.Integer:
				NSUserDefaults.StandardUserDefaults.SetInt(ConvertExChangeType<int>(value), PropertyNameToKey(propertyName));
				break;
			case StorageClass.Bool:
				NSUserDefaults.StandardUserDefaults.SetBool(ConvertExChangeType<bool>(value), PropertyNameToKey(propertyName));
				break;
			case StorageClass.Float:
				NSUserDefaults.StandardUserDefaults.SetFloat(ConvertExChangeType<float>(value), PropertyNameToKey(propertyName));
				break;
			case StorageClass.Double:
				NSUserDefaults.StandardUserDefaults.SetDouble(ConvertExChangeType<double>(value), PropertyNameToKey(propertyName));
				break;
			case StorageClass.XMLSerialized:
				NSUserDefaults.StandardUserDefaults.SetString(XmlProvider.WriteToString(key), PropertyNameToKey(propertyName));
				break;
			default:
				throw new Exception("Internal Error 01B66BCD-FDEB-4FCF-B32F-44B2C8A2A81C");
			}
		}


		private StorageClass GetStorageClass(Type type) {
			var retval = StorageClass.XMLSerialized;
			TypeSwitch.DoType(type,
				TypeSwitch.Case<string>(() => retval = StorageClass.String),
				TypeSwitch.Case<int>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<bool>(() => retval = StorageClass.Bool),
				TypeSwitch.Case<float>(() => retval = StorageClass.Float),
			    TypeSwitch.Case<double>(() => retval = StorageClass.Double),
			    TypeSwitch.Default(() => retval = StorageClass.XMLSerialized)
			);
			return retval;
		}

		private enum StorageClass {
			String,
			Integer,
			Float,
			Double,
			Bool,
			XMLSerialized
		}


		public T ConvertExChangeType<T>(object value)  {
			return (T)Convert.ChangeType(value, typeof(T));
		}
	}
	
	/* EXAMPLE
	 
	 public class Settings : UserDefaultObject {
		private static Settings _instance = null;
		private static volatile object _threadLock = new object();

		private Settings () : base("ServerDetails")	{
		}

		public static Settings Instance { 
			get {
				if (_instance == null) {
					lock(_threadLock) {
						if (_instance == null) {
							_instance = new Settings();
						}
					}
				}
				return _instance;
			}
		}

		[DefaultValue("")]
		public string LastKnownUsername { get; set; }

		[DefaultValue("http")]
		public string Scheme { get; set; }

		[DefaultValue("")]
		public string Domain { get; set; }

		[DefaultValue(80)]
		public int Port { get; set; }

		[DefaultValue(true)]
		public bool ForceServerLogon { get; set; }

		[DefaultValue(false)]
		public bool HasValidServer { get; set; }

		[DefaultValue(0)]
		public int LoadCount { get; set; }

		public string ServerUrl { 
			get {
				return string.Format ("{0}://{1}:{2}/Services/DataSync_1_0", Scheme, Domain, Port);
			}
		}
	} */
}

#endif
