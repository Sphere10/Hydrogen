// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Hydrogen.DApp.Presentation2.Logic.Wizard;
using Hydrogen.DApp.Presentation2.UI.Dialogs;
using Hydrogen.DApp.Presentation2.UI.Dialogs.Content;
using Hydrogen.DApp.Presentation2.UI.Wizard;

namespace Hydrogen.DApp.Presentation2.Logic.Modal {

	/// <summary>
	/// View service - provides common services to views.
	/// </summary>
	public static class ViewService {
		/// <summary>
		/// Show a wizard modal, with the supplied wizard model.
		/// </summary>
		/// <param name="wizard"> wizard</param>
		/// <param name="options"></param>
		/// <returns> modal result.</returns>
		public static async Task<ModalResult> WizardDialogAsync(IWizard wizard, ModalOptions options = null) {
			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ nameof(WizardHost.Wizard), wizard }
			};

			return await ModalService.ShowAsync<WizardModal>(ParameterView.FromDictionary(parameters), options);
		}

		/// <summary>
		/// Show confirm dialog, and await user interaction result
		/// </summary>
		/// <param name="title"> dialog title</param>
		/// <param name="message"></param>
		/// <param name="confirmText"></param>
		/// <returns>a value indicating whether the user confirmed or cancelled/closed dialog.</returns>
		public static async Task<bool>
			ConfirmDialogAsync(string title, string message, string confirmText = "OK") {
			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ nameof(Confirm.Title), title },
				{ nameof(Confirm.Message), message },
				{ nameof(Confirm.ConfirmMessageText), confirmText }
			};

			var result = await ModalService.ShowAsync<Confirm>(ParameterView.FromDictionary(parameters),
				new ModalOptions { Size = ModalSize.Small });

			return result.ResultType == ModalResultType.Ok;
		}

		/// <summary>
		/// Show a dialog with content
		/// </summary>
		/// <param name="level"> dialog content inforative level, differing template may be used for different level</param>
		/// <param name="title"> dialog title</param>
		/// <param name="message"> message content</param>
		/// <param name="confirmText"> optionally set confirm button text</param>
		/// <returns> a task that is complete once modal is dismissed</returns>
		public static async Task DialogAsync(string title, string message) {
			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ nameof(Dialog.Title), title },
				{ nameof(Dialog.Message), message },
				{ nameof(Dialog.ButtonsText), new[] { "Close" } }, {
					nameof(Dialog.Body), (RenderFragment)(builder => {
						builder.OpenComponent(0, typeof(MessageContent));
						builder.CloseComponent();
					})
				}
			};

			await ModalService.ShowAsync<Dialog>(ParameterView.FromDictionary(parameters));
		}

		public static async Task<int> DialogAsync(string title, string message, params string[] buttonText) {
			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ nameof(Dialog.Title), title },
				{ nameof(Dialog.Message), message },
				{ nameof(Dialog.ButtonsText), buttonText }, {
					nameof(Dialog.Body), (RenderFragment)(builder => {
						builder.OpenComponent(0, typeof(MessageContent));
						builder.CloseComponent();
					})
				}
			};

			ModalResult result = await ModalService.ShowAsync<Dialog>(ParameterView.FromDictionary(parameters));

			return result.GetData<int>();
		}

		/// <summary>
		/// Show a dialog with custom render fragment content
		/// </summary>
		/// <param name="content"> a render fragment to be displayed in the dialog.</param>
		/// <param name="title"> dialog title</param>
		/// <param name="confirmText"> optionally set confirm button text</param>
		/// <returns> a task that is complete once modal is dismissed</returns>
		public static async Task DialogAsync(RenderFragment content, string title, string confirmText = "Close") {
			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ nameof(Dialog.Title), title },
				{ nameof(Dialog.ButtonsText), new[] { confirmText } },
				{ nameof(Dialog.Body), content }
			};

			await ModalService.ShowAsync<Dialog>(ParameterView.FromDictionary(parameters));
		}

		public static async Task DialogAsync(Exception exception, string title) {
			RenderFragment body = builder => {
				builder.OpenComponent<ExceptionContent>(0);
				builder.AddAttribute(0, nameof(ExceptionContent.Exception), exception);
				builder.CloseComponent();
			};

			var parameters = new Dictionary<string, object> {
				{ nameof(Dialog.Title), title },
				{ nameof(Dialog.ButtonsText), new[] { "Close" } },
				{ nameof(Dialog.Body), body }
			};

			await ModalService.ShowAsync<Dialog>(ParameterView.FromDictionary(parameters));
		}
	}

}
