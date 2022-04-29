//-----------------------------------------------------------------------
// <copyright file="iOSTool.cs" company="Sphere 10 Software">
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
using System.Collections;
using System.Collections.Generic;
using CoreGraphics;
using AVFoundation;
using ObjCRuntime;
using CoreMedia;
using Foundation;
using UIKit;
using Hydrogen;
using MediaPlayer;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using CoreAnimation;
using Hydrogen.iOS;
using System.Threading;

namespace Tools {
	public static class iOSTool {
		private static readonly Selector _setOrientationSelector;
		// Check for multi-tasking as a way to determine if we can probe for the "Scale" property,
		// only available on iOS4 
		public static bool HighRes;

		static iOSTool() {
			//HighRes = UIDevice.CurrentDevice.IsMultitaskingSupported && (float)UIScreen.MainScreen.Scale > 1;
			_setOrientationSelector = new Selector("setOrientation:");
			var xxx = _setOrientationSelector;
		}

		public static nfloat GetKeyboardHeight(UIView view, NSNotification notification) {
			CGRect keyboardRect = UIKeyboard.FrameEndFromNotification(notification);
			keyboardRect = view.ConvertRectToView(keyboardRect, null);
			return keyboardRect.Height;
		}

		public static bool IsiOS7 {
			get { return UIDevice.CurrentDevice.CheckSystemVersion(7, 0); }
		}

		public static float DefaultGroupedTableViewHeaderHeight {
			get {

				if (IsiOS7)
					return 69.0f; // header height
				return 69.0f;
			}
		}

		public static async Task WaitUntilViewStopsAnimating(UIView view, int pollIntervalMS = 100) {
			while (IsAnimating(view)) {
				await Task.Delay(pollIntervalMS);
			}
		}

		public static bool IsAnimating(UIView view) {
			var collectionView = view as UICollectionView;
			if (collectionView != null) {
				return collectionView.VisibleCells.Any(IsAnimating);
			}
			if (view.Layer.AnimationKeys == null)
				return false;

			return view.Layer.AnimationKeys.Length > 0;
		}

		private static async Task WaitUntilCollectionViewStopsAnimating(UICollectionView collectionView, int pollIntervalMS = 100) {
			if (collectionView.VisibleCells == null)
				return;

			while (!collectionView.VisibleCells.All(c => c.Layer.AnimationKeys == null || c.Layer.AnimationKeys.Length == 0)) {
				await Task.Delay(pollIntervalMS);
			}
		}

		public static bool TryGenerateVideoThumbnail(string localFile, string extensionHint, CGSize size, out UIImage image) {
			image = null;
			if (String.IsNullOrWhiteSpace(Path.GetExtension(localFile))) {
				var fileMan = new NSFileManager();
				var tmpFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToStrictAlphaString().ToUpperInvariant() + extensionHint);
				NSError error;
				if (!fileMan.Link(localFile, tmpFile, out error))
					return false;
				var result = TryGenerateVideoThumbnail(tmpFile, size, out image);
				fileMan.Remove(tmpFile, out error);
				return result;
			}
			return TryGenerateVideoThumbnail(localFile, size, out image);
		}

		public static bool TryGenerateVideoThumbnail(string localFile, CGSize size, out UIImage image) {
			image = null;
			try {
				const float secondToGet = 1.0f;

				using (var player = new MPMoviePlayerController(NSUrl.FromFilename(localFile))) {
					image = player.ThumbnailImageAt(
						secondToGet,
						MPMovieTimeOption.NearestKeyFrame
					);

					image = UIImageExtensions.ResizeAndDispose(image,
						size,
						ResizeMethod.AspectFill,
						ResizeAlignment.CenterCenter
					);
					player.Stop();
				}

			} catch {
				return false;
			}

			return true;
		}

		public static bool TryGenerateVideoThumbnail2(string localFile, string extensionHint, CGSize size, out UIImage image) {
			image = null;
			if (String.IsNullOrWhiteSpace(Path.GetExtension(localFile))) {
				var fileMan = new NSFileManager();
				var tmpFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToStrictAlphaString().ToUpperInvariant() + extensionHint);
				NSError error;
				if (!fileMan.Link(localFile, tmpFile, out error))
					return false;
				var result = TryGenerateVideoThumbnail(tmpFile, size, out image);
				fileMan.Remove(tmpFile, out error);
				return result;
			}
			return TryGenerateVideoThumbnail2(localFile, size, out image);
		}

		public static bool TryGenerateVideoThumbnail2(string localFile, CGSize size, out UIImage image, string extensionHint = ".mov") {
			image = null;
			try {
				const int secondToGet = 1;
				const int timeScale = 60;
				var asset = AVAsset.FromUrl(NSUrl.FromFilename(localFile));
				var generator = new AVAssetImageGenerator(asset);
				var time = new CMTime(secondToGet, timeScale);
				CMTime actualTime;
				NSError error;
				var cgImage = generator.CopyCGImageAtTime(time, out actualTime, out error);
				if (error == null) {
					image = new UIImage(cgImage);
					image = UIImageExtensions.ResizeAndDispose(image,
						size,
						ResizeMethod.AspectFill,
						ResizeAlignment.CenterCenter
						);
					return true;
				}
			} catch {
			}
			return false;
		}

		public static UILabel AutoSizedLabel(string text) {
			var label = new UILabel() { Text = text };
			label.SizeToFit();
			return label;
		}

		public static UILabel AddLabel(UIView view, string text, UIColor color, ResizeAlignment alignment, UIEdgeInsets margins) {
			var label = new UILabel {
				Text = text,
				TextColor = color
			};
			return AddLabel(view, label, alignment, margins);
		}

		public static UILabel AddLabel(UIView view, UILabel label, ResizeAlignment alignment, UIEdgeInsets margins) {
			label.SizeToFit();
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			//label.AdjustsFontSizeToFitWidth = true;
			view.AddSubview(label);
			view.AddConstraints(new[] {
(NSLayoutConstraint)(               NSLayoutConstraint.Create(label, NSLayoutAttribute.Left, NSLayoutRelation.Equal, view, NSLayoutAttribute.Left, 1f, margins.Left)),
(NSLayoutConstraint)(               NSLayoutConstraint.Create(label, NSLayoutAttribute.Right, NSLayoutRelation.Equal, view, NSLayoutAttribute.Right, 1f, margins.Right)),

			});
			switch (alignment) {
				case ResizeAlignment.TopLeft:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, margins.Top)),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, + label.Frame.Height+margins.Top)),
					});
					label.TextAlignment = UITextAlignment.Left;
					break;
				case ResizeAlignment.TopCenter:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, margins.Top)),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, + label.Frame.Height+margins.Top)),
					});
					label.TextAlignment = UITextAlignment.Center;

					break;
				case ResizeAlignment.TopRight:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, margins.Top)),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Top, 1f, + label.Frame.Height+margins.Top)),
					});
					label.TextAlignment = UITextAlignment.Right;

					break;
				case ResizeAlignment.CenterLeft:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, view, NSLayoutAttribute.CenterY, 1f, 0f)),
					});
					label.TextAlignment = UITextAlignment.Left;
					break;
				case ResizeAlignment.CenterCenter:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, view, NSLayoutAttribute.CenterY, 1f, 0f)),
					});
					label.TextAlignment = UITextAlignment.Center;

					break;
				case ResizeAlignment.CenterRight:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, view, NSLayoutAttribute.CenterY, 1f, 0f)),
					});
					label.TextAlignment = UITextAlignment.Right;
					break;
				case ResizeAlignment.BottomLeft:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, -(label.Frame.Height + margins.Bottom))),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, margins.Bottom)),
					});
					label.TextAlignment = UITextAlignment.Left;
					break;
				case ResizeAlignment.BottomCenter:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, -(label.Frame.Height + margins.Bottom))),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, margins.Bottom)),
					});
					label.TextAlignment = UITextAlignment.Left;
					break;
				case ResizeAlignment.BottomRight:
					view.AddConstraints(new[] {
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Top, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, -(label.Frame.Height + margins.Bottom))),
(NSLayoutConstraint)(                       NSLayoutConstraint.Create(label, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, view, NSLayoutAttribute.Bottom, 1f, margins.Bottom)),
					});
					label.TextAlignment = UITextAlignment.Left;
					break;
			}
			return label;
		}

		public static void AddActivityIndicator(UIView view) {
			//var activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			//activity.TranslatesAutoresizingMaskIntoConstraints = false;
			//view.AddSubviewDockFull(activity);
			//activity.StartAnimating();
		}

		public static Action HandlingExceptions(Action action) {
			return () => {
				try {
					action();
				} catch (Exception error) {
					SystemLog.Exception(error);
					MessageBox.Show(error.ToDisplayString());
				}
			};
		}

		public static UIImage MissingImage {
			get { return EmbeddedImage("missing-image"); }
		}

		public static UIImage MissingFile {
			get { return EmbeddedImage("missing-file"); }
		}

		public static UIImage ErrorImage {
			get { return EmbeddedImage("error-image"); }
		}

		public static UIImage BlankImage {
			get { return EmbeddedImage("blank"); }
		}

		//public static UIImage CloseBoxImage {
		//    get { return UIImage.FromResource(Assembly.GetCallingAssembly(), "closebox.png"); }
		//}

		//public static UIImage ShadowImage {
		//    get { return UIImage.FromResource(Assembly.GetCallingAssembly(), "shadow.png"); }
		//}

		internal static UIImage EmbeddedImage(string name) {
			var xxx = Assembly.GetCallingAssembly();
			var yyy = xxx.GetManifestResourceNames();
			return UIImage.FromResource(Assembly.GetCallingAssembly(), name);
		}
		public static UIWindow PrimaryWindow {
			get {
				return UIApplication.SharedApplication.Windows.OrderBy(w => w.WindowLevel).First();
			}
		}

		public static UIViewController GetTopMostController() {
			var window = PrimaryWindow;
			var topController = window.RootViewController;
			while (topController.PresentedViewController != null) {
				topController = topController.PresentedViewController;
			}
			if (topController is UINavigationController) {
				var nav = (UINavigationController)topController;
				topController = nav.ViewControllers.Last();
				while (topController.PresentedViewController != null) {
					topController = topController.PresentedViewController;
				}
			}
			return topController;
		}


		#region NSObject tool

		private static readonly object ThreadLock = new object();
		private static readonly IDictionary<string, NSObject> KeyLookup = new Dictionary<string, NSObject>();

		public static void AddRuntimeProperty(NSObject @object, NSObject @property, string key, AssociationPolicy policy) {
			var alreadyHasProp = HasRuntimeProperty(@object, key);
			if (!alreadyHasProp) {
				iOSAPI.objc_setAssociatedObject(@object.Handle, GetKeyObject(key).Handle, property.Handle, policy);
			}
		}

		public static void RemoveRuntimeProperty(NSObject @object, string key) {
			var alreadyHasProp = HasRuntimeProperty(@object, key);
			if (alreadyHasProp) {
				iOSAPI.objc_setAssociatedObject(@object.Handle, GetKeyObject(key).Handle, IntPtr.Zero, AssociationPolicy.RETAIN);
			}
		}

		public static bool TryGetRuntimeProperty(NSObject @object, string key, out NSObject @property) {
			property = null;
			var valueptr = iOSAPI.objc_getAssociatedObject(@object.Handle, GetKeyObject(key).Handle);
			if (valueptr == IntPtr.Zero)
				return false;
			property = ObjCRuntime.Runtime.GetNSObject(valueptr);
			return true;
		}

		public static bool HasRuntimeProperty(NSObject @object, string key) {
			NSObject prop;
			return TryGetRuntimeProperty(@object, key, out prop);
		}

		private static NSObject GetKeyObject(string key) {
			lock (ThreadLock) {
				if (!KeyLookup.ContainsKey(key)) {
					KeyLookup.Add(key, new NSObject());
				}
				return KeyLookup[key];
			}
		}

		#endregion

		#region UIImage Tool

		/// <summary>
		/// Save an Image as a JPeg with a given compression
		///  Note: Filename suffix will not affect mime type which will be Jpeg.
		/// </summary>
		/// <param name="image">Image to save</param>
		/// <param name="fileName">File name to save the image as. Note: suffix will not affect mime type which will be Jpeg.</param>
		/// <param name="compression">Value between 0 and 100.</param>
		public static void SaveJpegWithCompression(UIImage image, string fileName, int compression) {
			if (!(0 <= compression && compression <= 100))
				throw new ArgumentOutOfRangeException("compression", "Must be between 0 and 100");

			using (var data = image.AsJPEG(compression / 100.0f)) {
				File.WriteAllBytes(fileName, data.ToArray());
			}

		}

		/// <summary>
		/// Save an Image as a JPeg with a given compression
		///  Note: Filename suffix will not affect mime type which will be Jpeg.
		/// </summary>
		/// <param name="image">Image to save</param>
		/// <param name="stream">The stream where the image will be saved.</param>
		/// <param name="compression">Value between 0 and 100.</param>
		public static void SaveJpegWithCompression(UIImage image, Stream stream, int compression) {
			if (!(0 <= compression && compression <= 100))
				throw new ArgumentOutOfRangeException("compression", "Must be between 0 and 100");
			using (var data = image.AsJPEG(compression / 100.0f)) {
				var bytes = data.ToArray();
				stream.Write(data.ToArray(), 0, bytes.Length);
			}
		}

		public static UIImage ToUIImage(byte[] imageBuffer) {
			using (var imageData = NSData.FromArray(imageBuffer))
				return UIImage.LoadFromData(imageData);
		}

		public static UIImage Antialias(UIImage image) {
			return ResizeAndDispose(image, (CGSize)image.Size, ResizeMethod.Stretch, antiAliasing: true);
		}

		public static UIImage RemoveColor(UIImage image, UIColor color, float tolerance) {
			// convert color to RGBA (may be greyscale for example);
			nfloat r, g, b, a;
			color.GetRGBA(out r, out g, out b, out a);
			color = UIColor.FromRGBA(r, g, b, a);

			var imageRef = image.CGImage;

			var width = (int)imageRef.Width;
			var height = (int)imageRef.Height;
			var colorSpace = CGColorSpace.CreateDeviceRGB();

			var bytesPerPixel = 4;
			var bytesPerRow = bytesPerPixel * width;
			var bitsPerComponent = 8;
			var bitmapByteCount = bytesPerRow * height;

			byte[] rawData = new byte[bitmapByteCount];


			var context = new CGBitmapContext(
				rawData,
				width,
				height,
				bitsPerComponent,
				bytesPerRow,
				colorSpace,
				CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			colorSpace.Dispose();

			context.DrawImage(new CGRect(0, 0, width, height), imageRef);

			var cgColor = color.CGColor;
			var components = cgColor.Components;

			r = components[0];
			g = components[1];
			b = components[2];
			//float a = components[3]; // not needed

			r = r * 255.0f;
			g = g * 255.0f;
			b = b * 255.0f;

			var redRange = new[] {
				(nfloat) Math.Max(r - (tolerance/2.0f), 0.0f),
				(nfloat) Math.Min(r + (tolerance/2.0f), 255.0f)
			};

			var greenRange = new[] {
				(nfloat)Math.Max(g - (tolerance/2.0f), 0.0f),
				(nfloat)Math.Min(g + (tolerance/2.0f), 255.0f)
			};

			var blueRange = new[]{
				(nfloat)Math.Max(b - (tolerance/2.0f), 0.0f),
				(nfloat)Math.Min(b + (tolerance/2.0f), 255.0f)
			};

			int byteIndex = 0;

			while (byteIndex < bitmapByteCount) {
				var red = rawData[byteIndex];
				var green = rawData[byteIndex + 1];
				var blue = rawData[byteIndex + 2];

				if (((red >= redRange[0]) && (red <= redRange[1])) &&
					((green >= greenRange[0]) && (green <= greenRange[1])) &&
					((blue >= blueRange[0]) && (blue <= blueRange[1]))) {
					// make the pixel transparent
					//
					rawData[byteIndex] = 0;
					rawData[byteIndex + 1] = 0;
					rawData[byteIndex + 2] = 0;
					rawData[byteIndex + 3] = 0;
				}

				byteIndex += 4;
			}

			var result = new UIImage(context.ToImage());
			context.Dispose();
			return result;
		}

		public static UIImage Crop(UIImage image, CGRect section) {
			UIGraphics.BeginImageContext(section.Size);
			var context = UIGraphics.GetCurrentContext();
			context.ClipToRect(new CGRect(0, 0, section.Width, section.Height));
			var drawRectangle = new CGRect(-section.X, -section.Y, image.Size.Width, image.Size.Height);
			image.Draw(drawRectangle);
			var croppedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return croppedImage;
		}

		public static UIImage Resize(
			UIImage sourceImage,
			CGSize requestedSize,
			ResizeMethod resizeMethod = ResizeMethod.Stretch,
			ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
			bool antiAliasing = true,
			CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
			UIColor paddingColor = null) {

			if (paddingColor == null)
				paddingColor = UIColor.Clear;

			var sourceImageSize = (CGSize)sourceImage.Size;
			var scaleWidth = ((float)requestedSize.Width / (float)sourceImageSize.Width);
			var scaleHeight = ((float)requestedSize.Height / (float)sourceImageSize.Height);
			CGSize destImageSize;
			CGRect sourceBlitRect = CGRect.Empty;
			CGRect destBlitRect = CGRect.Empty;
			bool alignSourceBlitRect = false;
			bool alignDestBlitRect = false;
			switch (resizeMethod) {
				case ResizeMethod.AspectFit:
					sourceBlitRect = new CGRect(CGPoint.Empty, sourceImageSize);
					scaleWidth = Math.Min(scaleWidth, scaleHeight);
					scaleHeight = Math.Min(scaleWidth, scaleHeight);
					destBlitRect.Width = sourceImageSize.Width * scaleWidth;
					destBlitRect.Height = sourceImageSize.Height * scaleHeight;
					destImageSize = destBlitRect.Size;

					break;
				case ResizeMethod.AspectFitPadded:
					sourceBlitRect = new CGRect(CGPoint.Empty, sourceImageSize);
					scaleWidth = Math.Min(scaleWidth, scaleHeight);
					scaleHeight = Math.Min(scaleWidth, scaleHeight);
					destBlitRect.Width = sourceImageSize.Width * scaleWidth;
					destBlitRect.Height = sourceImageSize.Height * scaleHeight;
					destImageSize = requestedSize;
					alignDestBlitRect = true;
					break;
				case ResizeMethod.AspectFill:
					var sourceAspect = sourceImageSize.Width / sourceImageSize.Height;
					var destAspect = requestedSize.Width / requestedSize.Height;
					if (destAspect > sourceAspect) {
						sourceBlitRect = new CGRect(0, 0, sourceImageSize.Width, sourceImageSize.Width / destAspect);
						alignSourceBlitRect = true;
					} else if (destAspect < sourceAspect) {
						sourceBlitRect = new CGRect(0, 0, sourceImageSize.Height * destAspect, sourceImageSize.Height);
						alignSourceBlitRect = true;
					} else {
						sourceBlitRect = new CGRect(CGPoint.Empty, sourceImageSize);
					}
					destBlitRect = new CGRect(CGPoint.Empty, requestedSize);
					destImageSize = requestedSize;
					break;
				case ResizeMethod.Stretch:
				default:
					sourceBlitRect = new CGRect(CGPoint.Empty, sourceImageSize);
					destBlitRect.Width = sourceImageSize.Width * scaleWidth;
					destBlitRect.Height = sourceImageSize.Height * scaleHeight;
					destImageSize = requestedSize;
					break;
			}

			if (alignDestBlitRect) {
				switch (resizeAlignment) {
					case ResizeAlignment.TopLeft:
						destBlitRect.Offset(0, 0);
						break;
					case ResizeAlignment.TopCenter:
						destBlitRect.Offset((destImageSize.Width - destBlitRect.Width) / 2, 0);
						break;
					case ResizeAlignment.TopRight:
						destBlitRect.Offset(destImageSize.Width - destBlitRect.Width, 0);
						break;
					case ResizeAlignment.CenterLeft:
						destBlitRect.Offset(0, (destImageSize.Height - destBlitRect.Height) / 2);
						break;
					case ResizeAlignment.CenterCenter:
						destBlitRect.Offset((destImageSize.Width - destBlitRect.Width) / 2, (destImageSize.Height - destBlitRect.Height) / 2);
						break;
					case ResizeAlignment.CenterRight:
						destBlitRect.Offset((destImageSize.Width - destBlitRect.Width), (destImageSize.Height - destBlitRect.Height) / 2);
						break;
					case ResizeAlignment.BottomLeft:
						destBlitRect.Offset(0, (destImageSize.Height - destBlitRect.Height));
						break;
					case ResizeAlignment.BottomCenter:
						destBlitRect.Offset((destImageSize.Width - destBlitRect.Width) / 2, destImageSize.Height - destBlitRect.Height);
						break;
					case ResizeAlignment.BottomRight:
						destBlitRect.Offset(destImageSize.Width - destBlitRect.Width, destImageSize.Height - destBlitRect.Height);
						break;
					default:
						break;
				}
			}

			if (alignSourceBlitRect) {
				switch (resizeAlignment) {
					case ResizeAlignment.TopLeft:
						sourceBlitRect.Offset(0, 0);
						break;
					case ResizeAlignment.TopCenter:
						sourceBlitRect.Offset((sourceImageSize.Width - sourceBlitRect.Width) / 2, 0);
						break;
					case ResizeAlignment.TopRight:
						sourceBlitRect.Offset(sourceImageSize.Width - sourceBlitRect.Width, 0);
						break;
					case ResizeAlignment.CenterLeft:
						sourceBlitRect.Offset(0, (sourceImageSize.Height - sourceBlitRect.Height) / 2);
						break;
					case ResizeAlignment.CenterCenter:
						sourceBlitRect.Offset((sourceImageSize.Width - sourceBlitRect.Width) / 2, (sourceImageSize.Height - sourceBlitRect.Height) / 2);
						break;
					case ResizeAlignment.CenterRight:
						sourceBlitRect.Offset((sourceImageSize.Width - sourceBlitRect.Width), (sourceImageSize.Height - sourceBlitRect.Height) / 2);
						break;
					case ResizeAlignment.BottomLeft:
						sourceBlitRect.Offset(0, sourceImageSize.Height - sourceBlitRect.Height);
						break;
					case ResizeAlignment.BottomCenter:
						sourceBlitRect.Offset((sourceImageSize.Width - sourceBlitRect.Width) / 2, sourceImageSize.Height - sourceBlitRect.Height);
						break;
					case ResizeAlignment.BottomRight:
						sourceBlitRect.Offset((sourceImageSize.Width - sourceBlitRect.Width), sourceImageSize.Height - sourceBlitRect.Height);
						break;
					default:
						break;
				}
			}

			var imageToUse = alignSourceBlitRect ? Crop(sourceImage, sourceBlitRect) : sourceImage;
			UIGraphics.BeginImageContext(destImageSize);
			var context = UIGraphics.GetCurrentContext();
			//context.TranslateCTM(0, destImageSize.Height);
			//context.ScaleCTM(1f, -1f);
			context.InterpolationQuality = interpolationQuality;
			context.SetAllowsAntialiasing(antiAliasing);
			context.SetFillColor(paddingColor.CGColor);
			context.FillRect(new CGRect(CGPoint.Empty, destImageSize));
			//context.DrawImage(destBlitRect, imageToUse.CGImage);
			imageToUse.Draw(destBlitRect);
			UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			if (imageToUse != sourceImage) {
				imageToUse.Dispose();
			}

			return resizedImage;

		}

		public static UIImage ResizeAndDispose(
			UIImage image,
			CGSize requestedSize,
			ResizeMethod resizeStrategy = ResizeMethod.Stretch,
			ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
			bool antiAliasing = true,
			CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
			UIColor paddingColor = null) {
			using (var sourceImage = image) {
				return Resize(sourceImage, requestedSize, resizeStrategy, resizeAlignment, antiAliasing, interpolationQuality, paddingColor);
			}
		}


		public static UIImage Zoom(UIImage image, float zoomFactor) {
			return Resize(image, new CGSize(image.Size.Width * zoomFactor, image.Size.Height * zoomFactor));
		}

		public static UIImage ZoomAndDispose(UIImage image, float zoomFactor) {
			using (var sourceImage = image) {
				return Zoom(sourceImage, zoomFactor);
			}
		}

		public static UIImage FocusZoom(
			UIImage image,
			float zoomFactor,
			int focusX,
			int focusY,
			bool antiAliasing = true,
			CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
			UIColor paddingColor = null) {

			if (paddingColor == null)
				paddingColor = UIColor.Clear;

			if (Math.Abs(zoomFactor - 0.0f) < Tools.Maths.EPSILON_F) {
				throw new ArgumentOutOfRangeException("zoomFactor", String.Format("Must not be 0 (epsilon tested with {0})", Tools.Maths.EPSILON_F));
			}
			var sourceHeight = (float)image.Size.Height / zoomFactor;
			var sourceWidth = (float)image.Size.Width / zoomFactor;
			var startX = (float)focusX - sourceWidth / 2;
			var startY = (float)focusY - sourceHeight / 2;

			var sourceRectangle = new CGRect(
					startX,
					startY,
					sourceWidth,
					sourceHeight
				)
				.IntersectWith(0, 0, image.Size.Width, image.Size.Height);
			var xClippedRatio = sourceRectangle.Width / sourceWidth;
			var yClippedRatio = sourceRectangle.Height / sourceHeight;

			var destRectangle = new CGRect(CGPoint.Empty, image.Size);
			destRectangle.Width *= xClippedRatio;
			destRectangle.Height *= yClippedRatio;

			UIImage result;
			using (var sourceImage = Crop(image, sourceRectangle)) {
				UIGraphics.BeginImageContext((CGSize)image.Size);
				var context = UIGraphics.GetCurrentContext();
				context.TranslateCTM(0, image.Size.Height);
				context.ScaleCTM(1f, -1f);
				context.InterpolationQuality = interpolationQuality;
				context.SetAllowsAntialiasing(antiAliasing);
				context.SetFillColor(paddingColor.CGColor);
				context.FillRect(new CGRect(CGPoint.Empty, (CGSize)image.Size));
				context.DrawImage(destRectangle, sourceImage.CGImage);
				result = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}

			return result;
		}

		public static UIImage FocusZoomAndDispose(
			UIImage image,
			float zoomFactor,
			int focusX,
			int focusY,
			bool antiAliasing = true,
			CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
			UIColor paddingColor = null) {
			using (var sourceImage = image) {
				return FocusZoom(sourceImage, zoomFactor, focusX, focusY);
			}
		}

		#endregion

		#region Graphics Tool

		public static UIViewController GetCurrentViewController() {
			UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while (currentController.PresentedViewController != null)
				currentController = currentController.PresentedViewController;

			return currentController;
		}

		public static UIView GetCurrentView() {
			return GetCurrentViewController().View;
		}

		public static bool IsCameraAvailable {
			get {
				return UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Rear);
			}
		}

		public static UIInterfaceOrientation CurrentInterfaceOrientation {
			get {
				return UIApplication.SharedApplication.StatusBarOrientation;
			}
		}

		public static CGRect FrameForOrientation(UIInterfaceOrientation orientation, bool subtractStatusBarHeight = true) {
			var screen = UIScreen.MainScreen;
			var fullScreenRect = (CGRect)screen.Bounds;      // always implicitly in Portrait orientation.

			// Initially assume portrait orientation.
			var width = fullScreenRect.Width;
			var height = fullScreenRect.Height;

			// Correct for orientation.
			if (IsLandscapeOrientation(orientation)) {
				width = fullScreenRect.Height;
				height = fullScreenRect.Width;
			}

			nfloat startHeight = 0.0f;
			if (subtractStatusBarHeight) {
				var appFrame = (CGRect)screen.ApplicationFrame;

				// Find status bar height by checking which dimension of the applicationFrame is narrower than screen bounds.
				// Little bit ugly looking, but it'll still work even if they change the status bar height in future.
				var statusBarHeight = (nfloat)Maths.Max((fullScreenRect.Width - appFrame.Width), (fullScreenRect.Height - appFrame.Height));

				// Account for status bar, which always subtracts from the height (since it's always at the top of the screen).
				height -= statusBarHeight;
				startHeight = statusBarHeight;
			}
			return new CGRect(0, startHeight, width, height);
		}

		public static CGSize CGSizeorOrientation(UIInterfaceOrientation orientation) {
			var frame = FrameForOrientation(orientation);
			return new CGSize(frame.Width, frame.Height);
		}

		public static bool IsLandscapeOrientation(UIInterfaceOrientation orientation) {
			return
				orientation == UIInterfaceOrientation.LandscapeLeft ||
				orientation == UIInterfaceOrientation.LandscapeRight;
		}

		public static void SetOrientation(UIInterfaceOrientation orientation) {

			iOSAPI.void_objc_msgSend_int(
				UIDevice.CurrentDevice.Handle,
				_setOrientationSelector.Handle,
				(int)orientation
			);
		}

		public static string GetPropertyValue(object inObject, string propertyName) {
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p => p.Name == propertyName).FirstOrDefault();
			if (prop != null)
				return prop.GetValue(inObject, null).ToString();
			return "";
		}

		public static object[] GetPropertyArray(object inObject, string propertyName) {
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p => p.Name == propertyName).FirstOrDefault();
			if (prop != null) {
				var currentObject = prop.GetValue(inObject, null);
				if (currentObject.GetType().GetGenericTypeDefinition() == typeof(List<>)) {
					return (new ArrayList((IList)currentObject)).ToArray();
				} else if (currentObject is System.Array) {
					return (object[])currentObject;
				} else {
					return new object[1];
				}
			}
			return new object[1];
		}

		private static string tallMagic = "-568h@2x";
		public static UIImage FromBundle16x9(string path) {
			//adopt the -568h@2x naming convention
			if (Is16x9()) {
				string imagePath = Path.GetDirectoryName(path.ToString());
				string imageFile = Path.GetFileNameWithoutExtension(path.ToString());
				string imageExt = Path.GetExtension(path.ToString());
				imageFile = imageFile + tallMagic + imageExt;
				return UIImage.FromFile(Path.Combine(imagePath, imageFile));

			} else {
				return UIImage.FromBundle(path.ToString());
			}
		}

		public static bool Is16x9() {
			return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone && UIScreen.MainScreen.Bounds.Height * (float)UIScreen.MainScreen.Scale >= 1136;
		}


		static CGPath smallPath = MakeRoundedPath(48);
		static CGPath largePath = MakeRoundedPath(73);
		public static UIImage AdjustImage(CGRect rect, UIImage template, CGBlendMode mode, UIColor color) {
			nfloat red = new float();
			nfloat green = new float();
			nfloat blue = new float();
			nfloat alpha = new float();
			if (color == null)
				color = UIColor.FromRGB(100, 0, 0);
			color.GetRGBA(out red, out green, out blue, out alpha);
			return AdjustImage(rect, template, mode, red, green, blue, alpha);
		}

		public static UIImage AdjustImage(CGRect rect, UIImage template, CGBlendMode mode, nfloat red, nfloat green, nfloat blue, nfloat alpha) {
			using (var cs = CGColorSpace.CreateDeviceRGB()) {
				using (var context = new CGBitmapContext(IntPtr.Zero, (int)rect.Width, (int)rect.Height, 8, (int)rect.Height * 4, cs, CGImageAlphaInfo.PremultipliedLast)) {
					context.TranslateCTM(0.0f, 0f);
					//context.ScaleCTM(1.0f,-1.0f);
					context.DrawImage(rect, template.CGImage);
					context.SetBlendMode(mode);
					context.ClipToMask(rect, template.CGImage);
					context.SetFillColor(red, green, blue, alpha);
					context.FillRect(rect);
					return UIImage.FromImage(context.ToImage());
				}
			}
		}
		static Selector sscale;
		public static void ConfigLayerHighRes(CALayer layer) {
			if (!HighRes)
				return;

			if (sscale == null)
				sscale = new Selector("setContentsScale:");

			iOSAPI.void_objc_msgSend_float(layer.Handle, sscale.Handle, 2.0f);
		}


		public static UIImage ResizeImage(CGSize size, UIImage image, bool keepRatio) {
			var curSize = image.Size;
			CGSize newSize;
			if (keepRatio) {
				var ratio = (nfloat)Maths.Min(size.Width / curSize.Width, size.Height / curSize.Height);
				newSize = new CGSize(curSize.Width * ratio, curSize.Height * ratio);
			} else {
				newSize = size;
			}
			return image.Scale(newSize);
		}


		// Child proof the image by rounding the edges of the image
		public static UIImage RemoveSharpEdges(UIImage image) {
			if (image == null) {
				throw new ArgumentNullException("image");
			}

			UIGraphics.BeginImageContext(image.Size);
			var c = UIGraphics.GetCurrentContext();

			c.AddPath(MakeRoundedPath((float)image.Size.Height));

			image.Draw(new CGRect(CGPoint.Empty, image.Size));
			var converted = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return converted;
		}

		static internal CGPath MakeRoundedPath(float size) {
			float hsize = size / 2;

			var path = new CGPath();
			path.MoveToPoint(size, hsize);
			path.AddArcToPoint(size, size, hsize, size, 4);
			path.AddArcToPoint(0, size, 0, hsize, 4);
			path.AddArcToPoint(0, 0, hsize, 0, 4);
			path.AddArcToPoint(size, 0, size, hsize, 4);
			path.CloseSubpath();

			return path;
		}

		public static CALayer MakeBackgroundLayer(UIImage image, CGRect frame) {
			var textureColor = UIColor.FromPatternImage(image);
			UIGraphics.BeginImageContext(frame.Size);
			var c = UIGraphics.GetCurrentContext();
			image.DrawAsPatternInRect(frame);

			//Images.MenuShadow.Draw (frame);
			var result = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();

			var back = new CALayer { Frame = frame };
			//TODO:
			//Graphics.ConfigLayerHighRes (back);
			back.Contents = result.CGImage;
			return back;
		}

		#endregion


		#region UIThread

		public static async Task InvokeOnUIThread(Func<Task> function) {			
			if (!NSThread.IsMain) {
				var signal = new ManualResetEventSlim();
				Exception exception = null;
				var obj = new NSObject();
				obj.InvokeOnMainThread(async () => {
					try {
						await function();
					} catch (Exception error) {
						exception = error;
					} finally {
						signal.Set();
					}
				});
				if (exception != null)
					throw new Exception("An exception occured whilst executing function in UI thread.", exception);
				await Task.Run(() => signal.Wait());
				return;
			}
			await function();
		}

		public static async Task<T1> InvokeOnUIThread<T1>(Func<Task<T1>> function) {			
			if (!NSThread.IsMain) {
				var signal = new ManualResetEventSlim();
				var result = default(T1);
				Exception exception = null;
				var obj = new NSObject();
				obj.InvokeOnMainThread(async () => {
					try {
						result = await function();
					} catch (Exception error) {
						exception = error;
					} finally {
						signal.Set();
					}
				});
				if (exception != null)
					throw new Exception("An exception occured whilst executing function in UI thread.", exception);
				await Task.Run(() => signal.Wait());
				return result;
			}
			return await function();
		}

		#endregion
	}
}

