//-----------------------------------------------------------------------
// <copyright file="Animations.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using System.Threading;
using UIKit;
using System.Linq;
using Hydrogen;

namespace Hydrogen.iOS {
    public class Animations {

        public static void FadeInAndOut(UIView view, FadeType initialFadeAction, float fadeInAlpha, TimeSpan fadeInDuration, TimeSpan fadeInDelay, float fadeOutAlpha, TimeSpan fadeOutDuration, TimeSpan fadeOutDelay, Func<UIView, bool> repeatCondition = null) {
            FadeInAndOut(new[] { view }, initialFadeAction, fadeInAlpha, fadeInDuration, fadeInDelay, fadeOutAlpha, fadeOutDuration, fadeOutDelay, (x) => repeatCondition(x.First()));
        }

        public static void FadeInAndOut(ICollection<UIView> views, FadeType initialFadeAction, float fadeInAlpha, TimeSpan fadeInDuration, TimeSpan fadeInDelay, float fadeOutAlpha, TimeSpan fadeOutDuration, TimeSpan fadeOutDelay, Func<IEnumerable<UIView>, bool> repeatCondition = null) {
            FadeInAndOut(Guid.NewGuid(), views, initialFadeAction, initialFadeAction, fadeInAlpha, fadeInDuration, fadeInDelay, fadeOutAlpha, fadeOutDuration, fadeOutDelay, repeatCondition);
        }

        private static void FadeInAndOut(Guid animationID, ICollection<UIView> views, FadeType initialFadeAction, FadeType nextFadeAction, float fadeInAlpha, TimeSpan fadeInDuration, TimeSpan fadeInDelay, float fadeOutAlpha, TimeSpan fadeOutDuration, TimeSpan fadeOutDelay, Func<IEnumerable<UIView>, bool> stopCondition = null) {
            if (stopCondition == null)
                stopCondition = (x) => false;


            switch (nextFadeAction) {
                case FadeType.FadeOut:

                    // fade out
                    Task.Run(() => {
                        Thread.Sleep(fadeOutDelay);
                        Fade(
                            views,
                            fadeOutAlpha,
                            fadeOutDuration,
                            () => {

                                if (!stopCondition(views) || initialFadeAction == FadeType.FadeOut) {
                                    FadeInAndOut(animationID, views, initialFadeAction, FadeType.FadeIn, fadeInAlpha, fadeInDuration, fadeInDelay, fadeOutAlpha, fadeOutDuration, fadeOutDelay, stopCondition);
                                }
                            }
                        );


                    });

                    break;
                case FadeType.FadeIn:
                    // fade in
                    Task.Run(() => {
                        Thread.Sleep(fadeInDelay);
                        Fade(
                            views,
                            fadeInAlpha,
                            fadeInDuration,
                            () => {
                                if (!stopCondition(views) || initialFadeAction == FadeType.FadeIn) {
                                    FadeInAndOut(animationID, views, initialFadeAction, FadeType.FadeOut, fadeInAlpha, fadeInDuration, fadeInDelay, fadeOutAlpha, fadeOutDuration, fadeOutDelay, stopCondition);
                                }
                            }
                        );
                    });
                    break;
                default:
                    break;
            }


        }

        public static void Fade(UIView view, float toAlpha, TimeSpan duration, Action completionAction) {
            Fade(new[] { view }, toAlpha, duration, completionAction);
        }

        public static async Task FadeAsync(UIView view, float toAlpha, TimeSpan duration) {
            var trigger = new ManualResetEventSlim();
            Fade(view, toAlpha, duration, () => trigger.Set());
            await Task.Run(() => trigger.Wait());
        }

        public static void Fade(ICollection<UIView> views, float toAlpha, TimeSpan duration, Action completionAction) {
            if (!views.Any())
                return;
            if (completionAction == null)
                completionAction = Tools.Lambda.NoOp;

            views.First().InvokeOnMainThread(() =>
                UIView.Animate(duration.TotalSeconds, () => views.Update(v => v.Alpha = toAlpha), () => completionAction()));
        }

        private static FadeType Toggle(FadeType fadeType) {
            if (fadeType == FadeType.FadeIn)
                return FadeType.FadeOut;
            return FadeType.FadeIn;
        }

        public enum FadeType {
            FadeIn,
            FadeOut
        }
    }
}

