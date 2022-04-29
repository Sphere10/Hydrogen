//-----------------------------------------------------------------------
// <copyright file="UserDefaultsSettingsProvider.cs" company="Sphere 10 Software">
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

#if __IOS__
#define INCLUDE_FILE
#endif

#if MONOMAC
#define INCLUDE_FILE
#endif

#if INCLUDE_FILE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Foundation;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.iOS {

	public class UserDefaultsSettingsProvider : BaseSettingsProvider {
		private const string NullValueString = "NULL!8be93b18-cecb-4be0-8d05-d6c43123aa4e";
		
        
        public override bool ContainsSetting(Type settingsObjectType, object id = null) {
			Preconditions.CheckNotNull(settingsObjectType, "settingsObjectType");
			var key = new NSString(PropertyNameToKey(settingsObjectType, id, "ID"));
            var result = NSUserDefaults.StandardUserDefaults.ToDictionary().ContainsKey(key); 
		    return result;
		}


		protected override SettingsObject LoadInternal(Type settingsObjectType, object id = null) {
			var setting = Tools.Object.Create(settingsObjectType) as SettingsObject;
			setting.ID = id;
			foreach(var field in GetSettingsFields(settingsObjectType)) {
				field.SetValue(setting, LoadPropertyValue(settingsObjectType, id, field.Name, field.FieldType));
			}
				
			foreach(var property in GetSettingsProperties(settingsObjectType))
				property.SetValue(setting, LoadPropertyValue(settingsObjectType, id, property.Name, property.PropertyType));
			return setting;
		}

		protected override void SaveInternal(SettingsObject settings) {
			var settingType = settings.GetType();
			foreach(var field in GetSettingsFields(settings.GetType()))
				SavePropertyValue(settingType, settings.ID, field.Name, field.GetValue(settings));
				
			foreach(var property in GetSettingsProperties(settings.GetType()))
				SavePropertyValue(settingType, settings.ID, property.Name, property.GetValue(settings, null));

			NSUserDefaults.StandardUserDefaults.Synchronize();

		}

	
		public override void DeleteSetting(SettingsObject settings) {
			var settingType = settings.GetType();
			foreach(var field in GetSettingsFields(settingType)) {
				DeletePropertyValue(settingType, settings.ID, field.Name);
			}
				
			foreach(var property in GetSettingsProperties(settings.GetType()))
				DeletePropertyValue(settingType, settings.ID, property.Name);
		}

				
		public override void ClearSettings() {
			NSUserDefaults.ResetStandardUserDefaults();
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		#region Auxillary methods

		private string PropertyNameToKey(Type settingObjectType, object settingID, string propertyName) {
			var result = 
				string.Format(
					"{0}.{1}", 
					string.Format(
						"{0}{1}", 
						settingObjectType.FullName, 
						settingID != null ? "." + ConvertExChangeType<string>(settingID) : string.Empty), 
					propertyName
				);
		    return result;
		}

		private object LoadPropertyValue(Type settingObjectType, object settingID, string propertyName, Type expectedValueType) {
			var key = PropertyNameToKey(settingObjectType, settingID, propertyName);
			object storedValue = null;

			// Check if stored value is null
			var nullCheckValue = NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(key));
			if (nullCheckValue == null || nullCheckValue.ToString() == NullValueString)
				return null;

			switch (GetStorageClass(expectedValueType)) {
				case StorageClass.String:
					storedValue = NSUserDefaults.StandardUserDefaults.StringForKey(key);
					break;
				case StorageClass.Integer:
					storedValue = (int)(NSUserDefaults.StandardUserDefaults.IntForKey(key));
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
				case StorageClass.DateTime:
					storedValue = Tools.Parser.Parse<DateTime?>(NSUserDefaults.StandardUserDefaults.StringForKey(key));
					break;
                case StorageClass.Enum:
                    storedValue = Enum.Parse(expectedValueType, NSUserDefaults.StandardUserDefaults.StringForKey(key));
			        break;
				case StorageClass.XMLSerialized:
					storedValue = XmlProvider.ReadFromString(expectedValueType, NSUserDefaults.StandardUserDefaults.StringForKey(key));
					break;
				default:
					throw new Exception("Internal Error F1C3E9A8-BAD2-48AF-9533-E78DE234B353");
			}

			return Tools.Object.ChangeType(storedValue, expectedValueType);
		}

		private void SavePropertyValue(Type settingObjectType, object settingID, string propertyName, object value) {
			var key = PropertyNameToKey(settingObjectType, settingID, propertyName);
			if (value != null) {
				switch (GetStorageClass(value.GetType())) {
					case StorageClass.String:
						NSUserDefaults.StandardUserDefaults.SetString(ConvertExChangeType<string>(value), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					case StorageClass.Integer:
						NSUserDefaults.StandardUserDefaults.SetInt(ConvertExChangeType<int>(value), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					case StorageClass.Bool:
						NSUserDefaults.StandardUserDefaults.SetBool(ConvertExChangeType<bool>(value), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					case StorageClass.Float:
						NSUserDefaults.StandardUserDefaults.SetFloat(ConvertExChangeType<float>(value), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					case StorageClass.Double:
						NSUserDefaults.StandardUserDefaults.SetDouble(ConvertExChangeType<double>(value), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					case StorageClass.DateTime:
						NSUserDefaults.StandardUserDefaults.SetString(string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", ConvertExChangeType<DateTime>(value)), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
                    case StorageClass.Enum:
                        NSUserDefaults.StandardUserDefaults.SetString(value.ToString(), PropertyNameToKey(settingObjectType, settingID, propertyName));
				        break;
					case StorageClass.XMLSerialized:
						NSUserDefaults.StandardUserDefaults.SetString(XmlProvider.WriteToString(key), PropertyNameToKey(settingObjectType, settingID, propertyName));
						break;
					default:
						throw new Exception("Internal Error 01B66BCD-FDEB-4FCF-B32F-44B2C8A2A81C");
				}
			} else {
				NSUserDefaults.StandardUserDefaults.SetString(NullValueString, PropertyNameToKey(settingObjectType, settingID, propertyName));
			}
		}

		private void DeletePropertyValue(Type settingObjectType, object settingID, string propertyName) {
			NSUserDefaults.StandardUserDefaults.RemoveObject(PropertyNameToKey(settingObjectType, settingID, propertyName));
		}

		private StorageClass GetStorageClass(Type type) {
			var retval = StorageClass.XMLSerialized;
			TypeSwitch.DoType(type,
				TypeSwitch.Case<string>(() => retval = StorageClass.String),
				TypeSwitch.Case<bool>(() => retval = StorageClass.Bool),
				TypeSwitch.Case<bool?>(() => retval = StorageClass.Bool),
			    TypeSwitch.Case<byte>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<byte?>(() => retval = StorageClass.Integer),
                TypeSwitch.Case<sbyte>(() => retval = StorageClass.Integer),
			    TypeSwitch.Case<sbyte?>(() => retval = StorageClass.Integer),
			    TypeSwitch.Case<short>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<short?>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<ushort>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<ushort?>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<int>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<int?>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<uint>(() => retval = StorageClass.Integer),
				TypeSwitch.Case<uint?>(() => retval = StorageClass.Integer),
				/*TypeSwitch.Case<long>((x) => {throw new NotSupportedException(string.Format("Settings of type {0} not supported", x.GetType().Name)); }),
				TypeSwitch.Case<long?>((x) => {throw new NotSupportedException(string.Format("Settings of type {0} not supported", x.GetType().Name)); }),
				TypeSwitch.Case<ulong>((x) => {throw new NotSupportedException(string.Format("Settings of type {0} not supported", x.GetType().Name)); }),
				TypeSwitch.Case<ulong?>((x) => {throw new NotSupportedException(string.Format("Settings of type {0} not supported", x.GetType().Name)); }),*/
				TypeSwitch.Case<float>(() => retval = StorageClass.Float),
				TypeSwitch.Case<float?>(() => retval = StorageClass.Float),
				TypeSwitch.Case<double>(() => retval = StorageClass.Double),
				TypeSwitch.Case<double?>(() => retval = StorageClass.Double),
                TypeSwitch.Case<decimal>(() => retval = StorageClass.Double),
				TypeSwitch.Case<decimal?>(() => retval = StorageClass.Double),
				TypeSwitch.Case<DateTime>(() => retval = StorageClass.DateTime),
				TypeSwitch.Case<DateTime?>(() => retval = StorageClass.DateTime),
                TypeSwitch.Case<Enum>(() => retval = StorageClass.Enum),
				TypeSwitch.Default(() => retval = StorageClass.XMLSerialized)
			);
			return retval;
		}

		private IEnumerable<FieldInfo> GetSettingsFields(Type settingsObjectType) {
			return settingsObjectType.GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.SetField).Where(f => Attribute.IsDefined(f, typeof(DefaultValueAttribute)));
		}

		private IEnumerable<PropertyInfo> GetSettingsProperties(Type settingsObjectType) {
			return settingsObjectType.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty).Where(p => Attribute.IsDefined(p, typeof(DefaultValueAttribute)) && p.GetIndexParameters().Length == 0);
		}

		public T ConvertExChangeType<T>(object value) {
			return (T)Convert.ChangeType(value, typeof(T));
		}

		private enum StorageClass {
			String,
			Integer,
			Float,
			Double,
			Bool,
			DateTime,
			Enum,
            XMLSerialized
		}

		#endregion
	}
}


#endif
