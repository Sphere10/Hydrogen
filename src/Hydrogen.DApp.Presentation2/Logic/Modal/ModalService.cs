// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation2.UI.Dialogs;

namespace Hydrogen.DApp.Presentation2.Logic.Modal {
	/// <summary>
	/// Modal service - provides modal facilities.
	/// </summary>
	public static class ModalService {
		private static ModalHost ModalInstance { get; set; }

		/// <summary>
		/// Initialize the modal service passing a reference to the modal host component. Must be completed
		/// before this object may be used.
		/// </summary>
		/// <param name="component"> modal host</param>
		public static void Initialize(ModalHost component) {
			ModalInstance = component ?? throw new ArgumentNullException(nameof(component));
		}

		/// <summary>
		/// Show the modal component of type,
		/// <typeparam name="T"> modal component to show. must implement modal component</typeparam>
		/// </summary>
		/// <param name="parameters"> optional parameters to pass to component instance T</param>
		/// <param name="options"> options to supply to the modal component.</param>
		/// <returns> modal result</returns>
		public static async Task<ModalResult> ShowAsync<T>(ParameterView? parameters = default, ModalOptions options = null)
			where T : ModalComponent {
			if (ModalInstance is null) {
				throw new InvalidOperationException("Modal service is not initialized, no modal component");
			}

			return await ModalInstance.ShowAsync<T>(parameters ?? ParameterView.Empty, options ?? new ModalOptions());
		}
	}
}
