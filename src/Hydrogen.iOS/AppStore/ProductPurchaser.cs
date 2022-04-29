//-----------------------------------------------------------------------
// <copyright file="ProductPurchaser.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using StoreKit;

namespace Hydrogen.iOS {

	internal class ProductPurchaser : SKPaymentTransactionObserver {
		private readonly ManualResetEventSlim _waiter;
		private readonly IList<PurchaseResult> _result;

		public ProductPurchaser() {
			_waiter = new ManualResetEventSlim(false);
			_result = new List<PurchaseResult>();
		}

		public async Task<IEnumerable<PurchaseResult>> PurchaseProducts(IEnumerable<SKProduct> products) {
			if (products == null || !products.Any())
				throw new ArgumentException("null or empty", "products");
			try {
				SKPaymentQueue.DefaultQueue.AddTransactionObserver(this);
				foreach (var product in products) {
					SKPaymentQueue.DefaultQueue.AddPayment(SKPayment.PaymentWithProduct(product));
				}
				await Task.Run(() => _waiter.Wait());
				return _result;
			} finally {
				SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(this);
			}
		}

		// called when the transaction status is updated
		public override async void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions) {
			
			foreach (var transaction in transactions) {
				switch (transaction.TransactionState) {

					case SKPaymentTransactionState.Purchased:
						_result.Add(
							new PurchaseResult {
								Result = TransactionResult.Purchased,
								PurchaseDate = transaction.TransactionDate.ToDateTime(),
								AppStoreProductID = transaction.Payment.ProductIdentifier
							}
						);
						break;
					case SKPaymentTransactionState.Failed:
						_result.Add(
							new PurchaseResult {
								Result = TransactionResult.Failed,
								AppStoreProductID = transaction.Payment.ProductIdentifier
							}
						);
						break;
					case SKPaymentTransactionState.Restored:
						_result.Add(
							new PurchaseResult {
								Result = TransactionResult.Restored,
								AppStoreProductID = transaction.Payment.ProductIdentifier,
								PurchaseDate = transaction.OriginalTransaction.TransactionDate.ToNullableDateTime()
							}
						);
						break;
					default:
						break;
				}
				SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
			}
			_waiter.Set();
		}


		public override void PaymentQueueRestoreCompletedTransactionsFinished(SKPaymentQueue queue) {
			// Restore succeeded
			Console.WriteLine(" ** RESTORE PaymentQueueRestoreCompletedTransactionsFinished ");
		}

		public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error) {
			// Restore failed somewhere...
			Console.WriteLine(" ** RESTORE RestoreCompletedTransactionsFailedWithError " + error.LocalizedDescription);
		}
	}

}
