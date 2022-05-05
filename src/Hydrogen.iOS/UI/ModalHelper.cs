//-----------------------------------------------------------------------
// <copyright file="ModalHelper.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using UIKit;
using System.Threading;
using Foundation;
using Hydrogen;

namespace Hydrogen.iOS {

    public static class ModalHelper {

        public static Task ShowModal(
            UIViewController parentViewController,
            UIView view,
            UIModalPresentationStyle presentationStyle = UIModalPresentationStyle.FormSheet,
            UIModalTransitionStyle transitionStyle = UIModalTransitionStyle.CoverVertical,
            Action completionHandler = null,
            bool dismissOnTapOutside = true,
            bool disposeOnFinish = false
        ) {
            return ShowModal(
                parentViewController,
                new UIViewController { View = view },
                presentationStyle,
                transitionStyle,
                completionHandler,
                dismissOnTapOutside,
                disposeOnFinish
            );
        }

        public static Task ShowModal(
            UIViewController parentViewController,
            UIViewController viewController,
            UIModalPresentationStyle presentationStyle = UIModalPresentationStyle.FormSheet,
            UIModalTransitionStyle transitionStyle = UIModalTransitionStyle.CoverVertical,
            Action completionHandler = null,
            bool dismissOnTapOutside = true,
            bool disposeOnFinish = false
        ) {
            return ShowModal<ModalFrameController>(
                parentViewController,
                new ModalFrameController(viewController),
                (mfc, dismissAction) => {
                    mfc.DoneAction = dismissAction;
                },
                presentationStyle,
                transitionStyle,
                completionHandler,
                dismissOnTapOutside,
                disposeOnFinish
            );
        }

        public static Task ShowModal<TViewController>(
            UIViewController parentViewController,
            TViewController viewController,
            Action<TViewController, Action> dismissSubscriber,
            UIModalPresentationStyle presentationStyle = UIModalPresentationStyle.FormSheet,
            UIModalTransitionStyle transitionStyle = UIModalTransitionStyle.CoverVertical,
            Action completionHandler = null,
            bool dismissOnTapOutside = true,
            bool disposeOnFinish = false
        ) where TViewController : UIViewController {

            return ShowModal<TViewController, object>(
                parentViewController,
                viewController,
                dismissSubscriber,
                presentationStyle,
                transitionStyle,
                completionHandler,
                (vc) => null,
                dismissOnTapOutside,
                disposeOnFinish
            );
        }


        public static async Task<TResult> ShowModal<TViewController, TResult>(
            UIViewController parentViewController,
            TViewController viewController,
            Action<TViewController, Action> dismissSubscriber = null,
            UIModalPresentationStyle presentationStyle = UIModalPresentationStyle.FormSheet,
            UIModalTransitionStyle transitionStyle = UIModalTransitionStyle.CoverVertical,
            Action completionHandler = null,
            Func<TViewController, TResult> resultGetter = null,
            bool dismissOnTapOutside = true,
            bool disposeOnFinish = false
        ) where TViewController : UIViewController {
            try {
                if (completionHandler == null)
                    completionHandler = Tools.Lambda.NoOp;

                if (dismissOnTapOutside && presentationStyle.IsIn(UIModalPresentationStyle.FullScreen, UIModalPresentationStyle.OverFullScreen)) {
                    dismissOnTapOutside = false;
                }

                if (dismissSubscriber == null && dismissOnTapOutside == false) {
                    throw new ArgumentException("dismissSubscriber is null and dismissOnTapOutside is effectively false, this will result in an unclosable modal screen");
                }


                var signal = new ManualResetEventSlim();
                viewController.ModalPresentationStyle = presentationStyle;
                viewController.ModalTransitionStyle = transitionStyle;
                Action dismissModalDialogAction = signal.Set;
                dismissSubscriber(viewController, dismissModalDialogAction);
                var currentPresentedController = parentViewController.FindPresentedViewController();
                if (currentPresentedController != null)
                    parentViewController = currentPresentedController;

                viewController.ProvidesPresentationContextTransitionStyle = true;
                viewController.DefinesPresentationContext = true;
                await parentViewController.PresentViewControllerAsync(viewController, true);
                completionHandler();

                var window = parentViewController.View.Window;
                UITapGestureRecognizerEx tapOutsideGesture = null;
                if (dismissOnTapOutside) {
                    tapOutsideGesture = new UITapGestureRecognizerEx(async (x) => {
                        if (x.State == UIGestureRecognizerState.Ended) {
                            var location = x.LocationInView(null);//Passing nil gives us coordinates in the window
                            if (!viewController.View.PointInside(viewController.View.ConvertPointFromView(location, window), null)) {
                                //Convert tap location into the local view's coordinate system. If outside, dismiss the view.
                                signal.Set();
                            }
                        }
                    });
                    tapOutsideGesture.NumberOfTapsRequired = 1;
                    tapOutsideGesture.CancelsTouchesInView = false;
                    window.AddGestureRecognizer(tapOutsideGesture);
                }

                await Task.Run(() => signal.Wait());

                if (dismissOnTapOutside) {
                    window.RemoveGestureRecognizer(tapOutsideGesture);
                }

                if (!viewController.IsBeingDismissed && parentViewController.ModalViewController == viewController) {
                    await viewController.DismissViewControllerAsync(true);
                }
                return resultGetter == null ? default(TResult) : resultGetter(viewController);

            } finally {
                if (disposeOnFinish) {
                    viewController.Dispose();
                }
            }
        }
    }


    public class ModalFrameController : UIViewController {
        private Action _doneAction;
        private UIToolbar _toolbar;
        private UIViewController _contentViewController;
        private UIView _contentView;

        public ModalFrameController(UIViewController contentViewController)
            : this(contentViewController, null) {
        }

        public ModalFrameController(UIViewController contentViewController, Action doneTapped) {
            DoneAction = doneTapped;
            View = new UIView();
            _toolbar = new UIToolbar();
            View.AddSubviewDockBottom(_toolbar);
            _toolbar.SetItems(
                new UIBarButtonItem[] {
					new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
					new UIBarButtonItem(UIBarButtonSystemItem.Done, (s, e) => _doneAction())
				},
                false
            );

            _toolbar.BarStyle = UIBarStyle.Default;
            _toolbar.TintAdjustmentMode = UIViewTintAdjustmentMode.Normal;

            _contentView = new UIView();
            //_contentView.BackgroundColor = UIColor.White;
            this.View.AddSubviewDock(
                _contentView,
                leftTarget: DockTarget.ToContainer(),
                topTarget: DockTarget.ToContainer(),
                rightTarget: DockTarget.ToContainer(),
                bottomTarget: DockTarget.ToView(_toolbar)
            );

            ContentViewController = contentViewController;
        }

        public Action DoneAction {
            get {
                return _doneAction;
            }
            set {
                _doneAction = value ?? Tools.Lambda.NoOp;
            }
        }


        public UIViewController ContentViewController {
            get {
                return _contentViewController;
            }
            set {
                if (_contentViewController != null) {
                    _contentViewController.RemoveFromParentViewController();
                    _contentViewController.Dispose();
                    _contentViewController = null;
                }
                _contentView.RemoveAndDisposeChildSubViews();
                _contentViewController = value;

                if (_contentViewController != null) {
                    this.AddChildViewController(_contentViewController);
                    _contentView.AddSubviewDockFull(_contentViewController.View);
                }
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ContentViewController = null;
                View.RemoveAndDisposeChildSubViews();

            }
            base.Dispose(disposing);
        }
    }
}

