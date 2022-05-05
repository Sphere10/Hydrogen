//-----------------------------------------------------------------------
// <copyright file="AddressBookTool.cs" company="Sphere 10 Software">
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
using AddressBook;
using Foundation;
using System.Threading.Tasks;
using System.Threading;

namespace Hydrogen.iOS
{
	public static class AddressBookTool	{
		public static async Task<bool> RequestAddressBookAuthorization() {
			ManualResetEventSlim waiter = new ManualResetEventSlim();

			if (ABAddressBook.GetAuthorizationStatus () == ABAuthorizationStatus.NotDetermined) {
				var addressBook = new ABAddressBook ();
				addressBook.RequestAccess (delegate(bool arg1, NSError arg2) {
					waiter.Set();
				});
				await Task.Run(() => waiter.Wait());
			}

			return ABAddressBook.GetAuthorizationStatus () == ABAuthorizationStatus.Authorized;

			/*Version version = new Version (MonoTouch.Constants.Version);
			if (version < new Version (6,0)) {
				return true;
			}

			switch(ABAddressBook.GetAuthorizationStatus ()) {
				case ABAuthorizationStatus.Authorized:
					return true;
				case ABAuthorizationStatus.NotDetermined:
					return await Task.Run( () => {
					var waiter = new ManualResetEventSlim();
					bool result = true;

					NSError error;
					var addressBook = ABAddressBook.Create(out error);
					if (error != null)
						return false;

					addressBook.RequestAccess(delegate(bool arg1, NSError arg2) {
						if (arg2 != null)
							SystemLog.Error(arg2.LocalizedDescription);
						SystemLog.Debug("REQUESTED ACCESS - {0}", arg1);
						result = arg1;
						waiter.Set();
					});
						//await Task.Run(() => waiter.Wait());
					waiter.Wait();
					return result;
					});
				default:
					return false;
			}*/
			/*SystemLog.Debug("RequestAddressBookAuthorization->Before GetAuthorization");
			var xx = ABAddressBook.GetAuthorizationStatus();
			SystemLog.Debug("RequestAddressBookAuthorization->After GetAuthorization - {0}", xx);
			return xx == ABAuthorizationStatus.Authorized;*/
		}
	}
}

