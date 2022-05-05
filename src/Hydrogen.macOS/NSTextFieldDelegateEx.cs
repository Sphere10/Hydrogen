//-----------------------------------------------------------------------
// <copyright file="NSTextFieldDelegateEx.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;

namespace Hydrogen {
	public class NSTextFieldDelegateEx : NSTextFieldDelegate {

	

		public override void Changed(MonoMac.Foundation.NSNotification notification) {
			var x = 1;

		}


		public override void DidChangeValue(string forKey) {
			var x = 1;
		}

		public override void DidChange(MonoMac.Foundation.NSKeyValueChange changeKind, MonoMac.Foundation.NSIndexSet indexes, MonoMac.Foundation.NSString forKey) {
			var x = 1;
		}

		public override void DidChange(MonoMac.Foundation.NSString forKey, MonoMac.Foundation.NSKeyValueSetMutationKind mutationKind, MonoMac.Foundation.NSSet objects) {
			var x = 1;
		}

		public override void WillChange(MonoMac.Foundation.NSKeyValueChange changeKind, MonoMac.Foundation.NSIndexSet indexes, MonoMac.Foundation.NSString forKey) {
			var x = 1;
		}

		public override void WillChange(MonoMac.Foundation.NSString forKey, MonoMac.Foundation.NSKeyValueSetMutationKind mutationKind, MonoMac.Foundation.NSSet objects) {
			var x = 1;
		}

		public override void WillChangeValue(string forKey) {
			var x = 1;
		}

		public override void EditingBegan(MonoMac.Foundation.NSNotification notification) {
			var x = 1;
		}

		public override void EditingEnded(MonoMac.Foundation.NSNotification notification) {
			var x = 1;
		}

	
	}
}

