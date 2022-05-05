//-----------------------------------------------------------------------
// <copyright file="CocoaTool.cs" company="Sphere 10 Software">
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
using System.Linq;
using MonoMac.AppKit;
using System.Collections.Generic;
using MonoMac.Foundation;
using System.Collections;
using System.Diagnostics;


namespace Hydrogen {

	public static class CocoaTool {

		public static NSComboBoxDataSource GenerateComboBoxDataSourceFromEnum<T>() {
			Debug.Assert(typeof(T).IsEnum);
			return GenerateComboBoxDataSourceFromEnum(typeof(T));
		}

		public static NSComboBoxDataSource GenerateComboBoxDataSourceFromEnum(Type enumType) {

			return 
				new ActionComboBoxDataSource(

					itemCount: 
						(cb) => Enum.GetNames(enumType).Length,

					objectValueForItem: 
						(cb, i) => (
							from v in DescriptionAttribute.GetValuesWithDescriptions(enumType, true)
							select v.Item2
						)
						.ElementAt(i)
						.ToNSString(),

					indexOfItem:
						(cb, s) => 
							string.IsNullOrWhiteSpace(s)
							? -1
							: (
								from v in DescriptionAttribute.GetValuesWithDescriptions(enumType, true)
								select v.Item2
							)
							.IndexOf(s)
				);


		}


		public static NSComboBoxDataSource GenerateComboBoxDataSource(IEnumerable<Tuple<int, string>> nameValues) {
			return 
				new ActionComboBoxDataSource(
					
					itemCount: 
						(cb) => nameValues.Count(),
					
					objectValueForItem: 
						(cb, i) =>  nameValues.ElementAt(i).Item2.ToNSString(),
					
					indexOfItem:
						(cb, s) => !string.IsNullOrWhiteSpace(s) ? nameValues.IndexOf(v => v.Item2 == s) : -1
			);
		}

		public static bool AskYN(string question, params object[] formatArgs) {
			return AskYN(string.Format(question, formatArgs));
		}

		public static bool AskYN(string question) {
			// http://stackoverflow.com/questions/2919826/how-do-i-implement-a-message-box-in-a-cocoa-application
			bool retval = false;
			NSAlert alert = new NSAlert();
			alert.MessageText = question;
			alert.AddButton("Yes");
			alert.AddButton("No");
			switch((NSAlertButtonReturn)alert.RunModal()) {
				case NSAlertButtonReturn.First:
					retval = true;
					break;
			}
			return retval;
		}
		
		public static SaveResponse AskSave(string message) {
			var retval = SaveResponse.Cancel;
			NSAlert alert = new NSAlert();
			alert.MessageText = message;
			alert.AddButton("Save");
			alert.AddButton("Cancel");
			alert.AddButton("Don't Save");
			switch((NSAlertButtonReturn)alert.RunModal()) {
				case NSAlertButtonReturn.First:
					retval = SaveResponse.Save;
					break;
				case NSAlertButtonReturn.Second:
					retval = SaveResponse.Cancel;
					break;
				case NSAlertButtonReturn.Third:
					retval = SaveResponse.DontSave;
					break;
			}
			return retval;
		}
		
		public static void ReportInfo (string message) {
			NSAlert alert = new NSAlert();
			alert.MessageText = message;
			alert.RunModal();
		}

		public static void ReportError (string message) {
			NSAlert alert = new NSAlert();
			alert.MessageText = message;
			alert.RunModal();
		}


		public static void ReportResult(Result result) {
			if (result.Success) {
				ReportInfo(result.ToString());
			} else {
				ReportError(result.ToString());
			}

		}

		public static  void ReportError(string message, params object[] formatArgs) {
			ReportError(string.Format(message, formatArgs));
		}
		
		public static  void ReportError (Exception exception) {
			ReportError(exception.ToDisplayString());
		}

		public static void Async<T> (Func<T> func, Action<T> result = null) {
			try {
				Tools.Lambda.ActionAsAsyncronous( () => {
					T value = default(T);
					value = func();
					if (result != null) {
						result(value);
					}
				}).Invoke();
			} catch (Exception error) {
				CocoaTool.ReportError(error);
			}
		}


		public static string GetNIBName<T>() where T : NSObject {
			var type = typeof(T);
			if (type == typeof(NSView) || type.IsSubclassOf(typeof(NSView)) || type == typeof(NSWindow) || type.IsSubclassOf(typeof(NSWindow))) {
				var registerAttributes = type.GetCustomAttributesOfType<RegisterAttribute>();
				if (!registerAttributes.Any()) {
					throw new SoftwareException("Unable to get NIB name for type '{0}' as it has no Cocoa registrations", type.Name);
				} else if (registerAttributes.Count() != 1) {
					throw new SoftwareException("Unable to get NIB name for type '{0}' as it has many Cocoa registrations", type.Name);
				}
				return ((RegisterAttribute)registerAttributes.Single()).Name;
			} else {
				throw new SoftwareException("Unable to get NIB name of type {0} as it is not an NSView or NSWindow", type.Name);
			}
		}


		public static NSView GetEventSourceFromSender(object sender) {
			NSView source = null;
			if (sender is NSView) 
				source = (NSView)sender;
			else if (sender is NSNotification) 
				source = (NSView)((NSNotification)sender).Object;
			return source;
		}

	}


}

