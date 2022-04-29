//-----------------------------------------------------------------------
// <copyright file="UIViewExtensions.cs" company="Sphere 10 Software">
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
using CoreGraphics;
using UIKit;
using Hydrogen;
using Foundation;
using System.Linq;
using System.Collections.Generic;
using Hydrogen.iOS;

namespace Hydrogen.iOS {
    public static class UIViewExtensions {

        public static void AddSubviewDockFull(this UIView containerView, UIView subView, UIEdgeInsets? insets = null) {
            containerView.AddSubviewDock(
                subView,
                insets,
                DockTarget.ToContainer(),
                DockTarget.ToContainer(),
                DockTarget.ToContainer(),
                DockTarget.ToContainer()
            );
        }

        public static void AddSubviewDockTop(this UIView containerView, UIView subView, UIEdgeInsets? insets = null) {
            containerView.AddSubviewDock(
                subView,
                insets,
                DockTarget.ToContainer(),
                DockTarget.ToContainer(),
                DockTarget.ToContainer(),
                null
            );
        }

        public static void AddSubviewDockBottom(this UIView containerView, UIView subView, UIEdgeInsets? insets = null) {
            containerView.AddSubviewDock(
                subView,
                insets,
                DockTarget.ToContainer(),
                null,
                DockTarget.ToContainer(),
                DockTarget.ToContainer()
            );
        }


		public static void AddSubviewDock(this UIView containerView, UIView subView, UIEdgeInsets? insets = null, DockTarget leftTarget = null, DockTarget topTarget = null, DockTarget rightTarget = null, DockTarget bottomTarget = null) {
			if (insets == null)
				insets = UIEdgeInsets.Zero;

			subView.TranslatesAutoresizingMaskIntoConstraints = false;
			containerView.AddSubview(subView);

			NSLayoutConstraint leftConstraint = null, topConstraint = null, rightConstraint = null, bottomConstraint = null;

			if (leftTarget != null) {
				switch (leftTarget.TargetType) {
					case DockTargetType.DockToContainer:
						leftConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Left, 1f, insets.Value.Left);
						break;
					case DockTargetType.DockToView:
						leftConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, leftTarget.View, NSLayoutAttribute.Right, 1.0f, insets.Value.Left);
						break;
					case DockTargetType.FixedLength:
						leftConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Left, 1.0f, leftTarget.FixedLength + insets.Value.Left);
						break;
					case DockTargetType.PercentageOfContainer:
						throw new NotSupportedException("PercentageOfContainer is not valid for left dock target");
						break;
				}
			}

			if (topTarget != null) {
				switch (topTarget.TargetType) {
					case DockTargetType.DockToContainer:
						topConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Top, 1f, insets.Value.Top);
						break;
					case DockTargetType.DockToView:
						topConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, topTarget.View, NSLayoutAttribute.Bottom, 1.0f, insets.Value.Top);
						break;
					case DockTargetType.FixedLength:
						topConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Top, 1f, topTarget.FixedLength + insets.Value.Top);
						break;
					case DockTargetType.PercentageOfContainer:
						throw new NotSupportedException("PercentageOfContainer is not valid for top dock target");
						break;
				}
			}

			if (rightTarget != null) {
				switch (rightTarget.TargetType) {
					case DockTargetType.DockToContainer:
						rightConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Right, 1f, -insets.Value.Right);
						break;
					case DockTargetType.DockToView:
						rightConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, rightTarget.View, NSLayoutAttribute.Left, 1.0f, -insets.Value.Right);
						break;
					case DockTargetType.FixedLength:
						rightConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1.0f, rightTarget.FixedLength - insets.Value.Right);
						break;
					case DockTargetType.PercentageOfContainer:
						rightConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Width, rightTarget.Percentage, -insets.Value.Right);
						break;
				}
			}

			if (bottomTarget != null) {
				switch (bottomTarget.TargetType) {
					case DockTargetType.DockToContainer:
						bottomConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Bottom, 1f, -insets.Value.Bottom);
						break;
					case DockTargetType.DockToView:
						bottomConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, bottomTarget.View, NSLayoutAttribute.Top, 1.0f, -insets.Value.Bottom);
						break;
					case DockTargetType.FixedLength:
						bottomConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1.0f, bottomTarget.FixedLength - insets.Value.Bottom);
						break;
					case DockTargetType.PercentageOfContainer:
						bottomConstraint = NSLayoutConstraint.Create(subView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, containerView, NSLayoutAttribute.Height, bottomTarget.Percentage, -insets.Value.Bottom);
						break;
				}
			}

            var constraints = new[] { leftConstraint, topConstraint, rightConstraint, bottomConstraint }.Where(c => c != null).ToArray();
            if (constraints.Any()) {
                containerView.AddConstraints(constraints);
            }
        }

        public static void RemoveAndDisposeChildSubViews(this UIView view) {
            if (view == null)
                return;
            if (view.Handle == IntPtr.Zero)
                return;
            if (view.Subviews == null)
                return;
            view.Subviews.Update(RemoveFromSuperviewAndDispose);
        }

        public static void RemoveFromSuperviewAndDispose(this UIView view) {
            view.RemoveFromSuperview();
            view.DisposeEx();
        }

        public static bool IsDisposedOrNull(this UIView view) {
            if (view == null)
                return true;

            if (view.Handle == IntPtr.Zero)
                return true;
            ;

            return false;
        }

        public static void DisposeEx(this UIView view) {
            const bool enableLogging = true;
            try {
                if (view.IsDisposedOrNull())
                    return;

                var viewDescription = string.Empty;

                if (enableLogging) {
                    viewDescription = view.Description;
                    SystemLog.Debug("Destroying " + viewDescription);
                }

                var disposeView = true;
                var disconnectFromSuperView = true;
                var disposeSubviews = true;
                var removeGestureRecognizers = false; // WARNING: enable at your own risk, may causes crashes
                var removeConstraints = true;
                var removeLayerAnimations = true;
                var associatedViewsToDispose = new List<UIView>();
                var otherDisposables = new List<IDisposable>();

                if (view is UIActivityIndicatorView) {
                    var aiv = (UIActivityIndicatorView)view;
                    if (aiv.IsAnimating) {
                        aiv.StopAnimating();
                    }
                } else if (view is UITableView) {
                    var tableView = (UITableView)view;

                    if (tableView.DataSource != null) {
                        otherDisposables.Add(tableView.DataSource);
                    }
                    if (tableView.BackgroundView != null) {
                        associatedViewsToDispose.Add(tableView.BackgroundView);
                    }

                    tableView.Source = null;
                    tableView.Delegate = null;
                    tableView.DataSource = null;
                    tableView.WeakDelegate = null;
                    tableView.WeakDataSource = null;
                    associatedViewsToDispose.AddRange(tableView.VisibleCells ?? new UITableViewCell[0]);
                    //return;
                } else if (view is UITableViewCell) {
                    var tableViewCell = (UITableViewCell)view;
                    disposeView = false;
                    disconnectFromSuperView = false;
                    if (tableViewCell.ImageView != null) {
                        associatedViewsToDispose.Add(tableViewCell.ImageView);
                    }
                } else if (view is UICollectionView) {
                    var collectionView = (UICollectionView)view;
                    disposeView = false;
                    if (collectionView.DataSource != null) {
                        otherDisposables.Add(collectionView.DataSource);
                    }
                    if (!collectionView.BackgroundView.IsDisposedOrNull()) {
                        associatedViewsToDispose.Add(collectionView.BackgroundView);
                    }
                    //associatedViewsToDispose.AddRange(collectionView.VisibleCells ?? new UICollectionViewCell[0]);
                    collectionView.Source = null;
                    collectionView.Delegate = null;
                    collectionView.DataSource = null;
                    collectionView.WeakDelegate = null;
                    collectionView.WeakDataSource = null;
                } else if (view is UICollectionViewCell) {
                    var collectionViewCell = (UICollectionViewCell)view;
                    disposeView = false;
                    disconnectFromSuperView = false;
                    if (collectionViewCell.BackgroundView != null) {
                        associatedViewsToDispose.Add(collectionViewCell.BackgroundView);
                    }
                } else if (view is UIWebView) {
                    var webView = (UIWebView)view;
                    if (webView.IsLoading)
                        webView.StopLoading();
                    webView.LoadHtmlString(string.Empty, null); // clear display
                    webView.Delegate = null;
                    webView.WeakDelegate = null;
                } else if (view is UIImageView) {
                    var imageView = (UIImageView)view;
                    if (imageView.Image != null) {
                        otherDisposables.Add(imageView.Image);
                        imageView.Image = null;
                    }
                } else if (view is UIScrollView) {
                    var scrollView = (UIScrollView)view;
                    scrollView.UnsetZoomableContentView();
                }

                var gestures = view.GestureRecognizers;
                if (removeGestureRecognizers && gestures != null) {
                    foreach (var gr in gestures) {
                        view.RemoveGestureRecognizer(gr);
                        gr.Dispose();
                    }
                }

                if (removeLayerAnimations && view.Layer != null) {
                    view.Layer.RemoveAllAnimations();
                }

                if (disconnectFromSuperView && view.Superview != null) {
                    view.RemoveFromSuperview();
                }

                var constraints = view.Constraints;
                if (constraints != null && constraints.Any() && constraints.All(c => c.Handle != IntPtr.Zero)) {
                    view.RemoveConstraints(constraints);
                    foreach (var constraint in constraints) {
                        constraint.Dispose();
                    }
                }

                foreach (var otherDisposable in otherDisposables) {
                    otherDisposable.Dispose();
                }

                foreach (var otherView in associatedViewsToDispose) {
                    otherView.DisposeEx();
                }

                var subViews = view.Subviews;
                if (disposeSubviews && subViews != null) {
                    subViews.ForEach(DisposeEx);
                }

                if (view is ISpecialDisposable) {
                    ((ISpecialDisposable)view).SpecialDispose();
                } else if (disposeView) {
                    if (view.Handle != IntPtr.Zero)
                        view.Dispose();
                }

                if (enableLogging) {
                    SystemLog.Debug("Destroyed {0}", viewDescription);
                }

            } catch (Exception error) {
                SystemLog.Exception(error);
            }
        }

		public static void DrawRoundRectangle(this UIView view, CGRect rrect, float radius, UIColor color) {
			var context = UIGraphics.GetCurrentContext();
			
			color.SetColor();
			
			var minx = rrect.Left;
			var midx = rrect.Left + (rrect.Width)/2;
			var maxx = rrect.Right;
            var miny = rrect.Top;
            var midy = (rrect.Y + rrect.Size.Width) / 2;
            var maxy = rrect.Bottom;
			
			context.MoveTo(minx, midy);
			context.AddArcToPoint(minx, miny, midx, miny, radius);
			context.AddArcToPoint(maxx, miny, maxx, midy, radius);
			context.AddArcToPoint(maxx, maxy, midx, maxy, radius);
			context.AddArcToPoint(minx, maxy, minx, midy, radius);
			context.ClosePath();
			context.DrawPath(CGPathDrawingMode.Fill); // test others?
		}

		public static void ShowLoadingHUD(this UIView view, string title, Action<IProgressDelegate> action) {
			 bool  isCompleted = false;
			Exception error = null;
		//	view.InvokeOnMainThread(
			//	() => {
				var hudView = new LoadingHUDView(title, "processing");
			var hudFrame = hudView.Frame;
				hudFrame.Height += (float)(view.Window.Frame.GetMidY());
			hudView.Frame = hudFrame;
				view.Window.Add(hudView);
				
				hudView.StartAnimating();

				Tools.Threads.QueueAction(
					() => {
						try {
							action(	new ActionProgressDelegate(	(msg) => view.InvokeOnMainThread( () => hudView.Message = msg)));
						} catch(Exception actionError) {
							error = actionError;
						} finally { 
							view.InvokeOnMainThread( 
						       	() => {
									try {
										hudView.StopAnimating();
										hudView.RemoveFromSuperview();
									} catch {
										// ignore errors here
									} finally {
										isCompleted = true;
									}	
								}
							);
						isCompleted = true;
						}
					}
				);
		//	});
			while (!isCompleted)
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.2));
			
			if (error != null) {
				SystemLog.Exception(error);
				throw new SoftwareException(error, "An unexpected error occured in an asyncronous task");
			}
		}
	}

	
	public interface IProgressDelegate {
		void NotifyMesssage(string message);
	}

	public class ActionProgressDelegate : IProgressDelegate {
		Action<string> _action;
		public ActionProgressDelegate(Action<string> action) {
			_action = action;
		}

		public void NotifyMesssage(string message) {
			_action(message);
		}
	}
    
}

