//-----------------------------------------------------------------------
// <copyright file="iTunesAppStoreManager.cs" company="Sphere 10 Software">
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
using Hydrogen;
using StoreKit;

namespace Hydrogen.iOS {

	public class iTunesAppStoreManager {
		private readonly SynchronizedDictionary<string, SKProduct> _productCache;

		public iTunesAppStoreManager() {
			_productCache = new SynchronizedDictionary<string, SKProduct>();
		}

		public bool CanMakePayments {
			get {
				return SKPaymentQueue.CanMakePayments;
			}
		}

		public async Task PrefetchProductData(IEnumerable<string> productIDs) {
			await GetProductDataInternal(productIDs.Except(_productCache.Keys));
		}

		public async Task<SKProduct> GetProductData(string productId) {
			var products = await GetProductData(new[] { productId });
			if (!products.Any())
				throw new Exception(string.Format("No product with id '{0}' was found on the app store", productId));
			return products.First();
		}

		public async Task<IEnumerable<SKProduct>> GetProductData(IEnumerable<string> productIds) {
			var prefetched = _productCache.Keys.Intersect(productIds);
			var toFetch = productIds.Except (prefetched);
			if (toFetch.Any())
				await GetProductDataInternal(toFetch);
			return productIds.Select(x => _productCache[x]);
		}

		private async Task GetProductDataInternal(IEnumerable<string> productIDs) {
			if (productIDs == null)
				return;

			SystemLog.Info ("GetProductDataInternal - count of products requested {0}", productIDs.Count());

			productIDs.ForEach (x => {
				SystemLog.Info(x);
			}); 
			const int numRetry = 3;

			var productIdentifiers = productIDs.ToArray ();
			if (productIdentifiers.Length == 0)
				return;

			for (int i = 0; i < numRetry; i++) {
				try {
					using (_productCache.EnterWriteScope()) {
						var requestor = new ProductDataRequestor ();
						foreach (var product in (await requestor.GetProductData(productIdentifiers)).Products) {
							_productCache [product.ProductIdentifier] = product;
							SystemLog.Info ("RECEIVED iTUNES RESPONSE {0}", product.ProductIdentifier);
						}
					}
					break;
				} catch(Exception error){
					SystemLog.Exception (error);
					if (i == numRetry - 1)
						throw new Exception(string.Format("An error occured fetching product(s) '{0}' from iTunes.", productIdentifiers.ToDelimittedString(", ")), error);
				}
			}
		}


		public async Task<PurchaseResult> PurchaseProduct(string appStoreProductID) {
			return await PurchaseProduct(await GetProductData (appStoreProductID));
		}

		public async Task<PurchaseResult> PurchaseProduct(SKProduct product) {
			try {
				var purchaser = new ProductPurchaser();
				var result = await purchaser.PurchaseProduct(product);
				return result;
			} catch (Exception error) {
				throw new Exception(string.Format("An error occured purchasing product '{0}' from iTunes", product == null ? "NULL" : ( product.ProductIdentifier ?? "NULL PRODUCT IDENTIFIER" )), error);
			}
		}

		private class ProductDataRequestor : SKProductsRequestDelegate {
			private readonly ManualResetEventSlim _waiter;
			private SKProductsResponse _response;
			private NSError _error;

			public ProductDataRequestor() {
				_waiter = new ManualResetEventSlim(false);
			}

			public override void RequestFailed(SKRequest request, NSError error) {
				_error = error;
				_waiter.Set();
			}
			public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response) {
				_response = response;
				_waiter.Set();
			}

			public async Task<SKProductsResponse> GetProductData(params string[] appStoreProductIDs) {
				_response = null;
				_error = null;
				_waiter.Reset();


				if (appStoreProductIDs.Length == 0)
					throw new ArgumentException("No app store product IDs passed", "appStoreProductIDs");
				var array = new NSString[appStoreProductIDs.Length];
				for (var i = 0; i < appStoreProductIDs.Length; i++) {
					array[i] = new NSString(appStoreProductIDs[i]);
				}
				var productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);

				//set up product request for in-app purchase
				var productsRequest = new SKProductsRequest(productIdentifiers);
				productsRequest.Delegate = this;
				productsRequest.Start();
				await Task.Run(() => _waiter.Wait());


				if (_response != null)
					return _response;

				if (_error != null)
					throw new Exception(_error.LocalizedDescription);

				throw new Exception("Unable to get product data due to unexpected cancellation of request");
			}
		}

		private class ProductPurchaser : SKPaymentTransactionObserver {
			private readonly ManualResetEventSlim _waiter;
			private PurchaseResult _result;
			private SKPayment _payment;

			public ProductPurchaser() {
				_waiter = new ManualResetEventSlim(false);
				_result = null;
			}

			public async Task<PurchaseResult> PurchaseProduct(SKProduct product) {
				try {
					SKPaymentQueue.DefaultQueue.AddTransactionObserver(this);

					_payment = SKPayment.PaymentWithProduct(product);
					SKPaymentQueue.DefaultQueue.AddPayment(_payment);
					await Task.Run(() => _waiter.Wait());
					return _result;
				} finally {
					SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(this);
				}
			}

			// called when the transaction status is updated
			public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions) {
				SystemLog.Debug ("UpdatedTransactions - {0}", transactions.Length);

				foreach (var transaction in transactions) {
					// ISSUE: The transaction (and any dangling ones) are marked as finished straight away. Apple recommends we finish after app has done
					// what it needs to. What happens if user's phone turns off right after finish? The app may not
					// have registered purchase and the user will have to re-buy again!
					//
					// Solution is to to return a scope in the "PurchaseMethod" that
					// finishes the transaction on dispose
					//
					// using(inAppPurchaseManager.Purchase("productid")) {
					//  UpgradeApp();
					// }  <-- dispose will finish transaction
					//
					if (transaction.TransactionState != SKPaymentTransactionState.Purchasing)
						SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);

				}

				if (transactions.Any(p => p.TransactionState.IsIn(SKPaymentTransactionState.Purchased, SKPaymentTransactionState.Restored))) {
					var paymentTransaction = transactions.First(t => t.TransactionState.IsIn(SKPaymentTransactionState.Purchased, SKPaymentTransactionState.Restored));

					switch(paymentTransaction.TransactionState) { 
						case SKPaymentTransactionState.Purchased:
						SystemLog.Info ("PURCHASED");
							_result =
								new PurchaseResult {
								AppStoreProductID = paymentTransaction.Payment.ProductIdentifier,
									Result = TransactionResult.Purchased,
								PurchaseDate = paymentTransaction.TransactionDate.ToNullableDateTime()
								};
						break;
						case SKPaymentTransactionState.Restored:
						SystemLog.Info ("RESTORED");
							_result =
								new PurchaseResult {
								AppStoreProductID = paymentTransaction.Payment.ProductIdentifier,
									Result = TransactionResult.Restored,
									PurchaseDate = paymentTransaction.OriginalTransaction.TransactionDate.ToNullableDateTime()
								};
							break;
						}
					_waiter.Set();
				} else if (transactions.All(t => t.TransactionState == SKPaymentTransactionState.Failed)) {
					SystemLog.Info ("FAILED");
					var paymentTransaction = transactions.First(t => t.TransactionState == SKPaymentTransactionState.Failed);
					SystemLog.Info ("WITH ERROR {0}", paymentTransaction.Error);
					_result = 
						new PurchaseResult {
							AppStoreProductID = paymentTransaction.Payment.ProductIdentifier,
							Result = TransactionResult.Failed,
							Error = paymentTransaction.Error,
							PurchaseDate = null
						};
					_waiter.Set();
				} else {
					// still waiting for response (this was just dangling response from prior requests)
				}
			}


			public override void PaymentQueueRestoreCompletedTransactionsFinished(SKPaymentQueue queue) {
				// Restore succeeded
				SystemLog.Info(" ** RESTORE PaymentQueueRestoreCompletedTransactionsFinished ");
			}

			public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error) {
				// Restore failed somewhere...
				SystemLog.Info(" ** RESTORE RestoreCompletedTransactionsFailedWithError " + error.LocalizedDescription);
			}
		}
	}
}
