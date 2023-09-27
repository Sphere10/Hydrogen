// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Hydrogen;

public static class ImageExtensions {

	static public Bitmap Copy(this Image srcImage, Rectangle section) {
		// Create the new bitmap and associated graphics object
		var bmp = new Bitmap(section.Width, section.Height);
		using (var g = Graphics.FromImage(bmp)) {
			// Draw the specified section of the source bitmap to the new one
			g.DrawImage(srcImage, 0, 0, section, GraphicsUnit.Pixel);

		}
		// Return the bitmap
		return bmp;
	}

	public static Bitmap Zoom(this Image image, float zoomFactor) {
		return image.Resize(new Size((int)(image.Width * zoomFactor), (int)(image.Height * zoomFactor)));
	}


	public static Bitmap ZoomAndDispose(this Image image, float zoomFactor) {
		using (var sourceImage = image) {
			return sourceImage.Resize(new Size((int)(image.Width * zoomFactor), (int)(image.Height * zoomFactor)));
		}
	}


	public static Bitmap FocusZoom(this Image image, float zoomFactor, int focusX, int focusY,
	                               ResizeMethod resizeMethod = ResizeMethod.Stretch,
	                               ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
	                               InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
	                               Color? paddingColor = null,
	                               PixelFormat? newPixelFormat = null
	) {
		if (Math.Abs(zoomFactor - 0.0f) < Tools.Maths.EPSILON_F) {
			throw new ArgumentOutOfRangeException("zoomFactor", string.Format("Must not be 0 (epsilon tested with {0})", Tools.Maths.EPSILON_F));
		}

		var actualPaddingColor = paddingColor.GetValueOrDefault(Color.Transparent);
		var pixelFormat = newPixelFormat.GetValueOrDefault(image.PixelFormat);

		var bitmap = new Bitmap(image.Width, image.Height, pixelFormat);
		bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

		using (Graphics graphics = Graphics.FromImage(bitmap)) {
			graphics.Clear(actualPaddingColor);
			graphics.InterpolationMode = interpolationMode;

			var sourceHeight = image.Height / zoomFactor;
			var sourceWidth = image.Width / zoomFactor;
			var startX = focusX - sourceWidth / 2;
			var startY = focusY - sourceHeight / 2;

			var sourceRectangle = new Rectangle(
					(int)Math.Round(startX),
					(int)Math.Round(startY),
					(int)Math.Round(sourceWidth),
					(int)Math.Round(sourceHeight)
				)
				.IntersectWith(0, 0, image.Width, image.Height);

			var destRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			graphics.DrawImage(image, sourceRectangle, destRectangle, GraphicsUnit.Pixel);
		}
		return bitmap;
	}


	public static Bitmap FocusZoomAndDispose(this Image image, float zoomFactor, int focusX, int focusY) {
		using (var sourceImage = image) {
			return sourceImage.FocusZoom(zoomFactor, focusX, focusY);
		}
	}

	public static Bitmap Resize(
		this Image image,
		Size requestedSize,
		ResizeMethod resizeMethod = ResizeMethod.Stretch,
		ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
		InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
		Color? paddingColor = null,
		PixelFormat? newPixelFormat = null
	) {
		var actualPaddingColor = paddingColor.GetValueOrDefault(Color.Transparent);
		var pixelFormat = newPixelFormat.GetValueOrDefault(image.PixelFormat);
		var sourceImageSize = image.Size;
		var scaleWidth = (requestedSize.Width / (float)sourceImageSize.Width);
		var scaleHeight = (requestedSize.Height / (float)sourceImageSize.Height);
		Size destImageSize;
		var sourceBlitRect = Rectangle.Empty;
		var destBlitRect = Rectangle.Empty;
		var alignSourceBlitRect = false;
		var alignDestBlitRect = false;
		switch (resizeMethod) {
			case ResizeMethod.AspectFit:
				sourceBlitRect = new Rectangle(Point.Empty, sourceImageSize);
				scaleWidth = Math.Min(scaleWidth, scaleHeight);
				scaleHeight = Math.Min(scaleWidth, scaleHeight);
				destBlitRect.Width = (int)Math.Round(sourceImageSize.Width * scaleWidth, 0);
				destBlitRect.Height = (int)Math.Round(sourceImageSize.Height * scaleHeight, 0);
				destImageSize = destBlitRect.Size;
				break;
			case ResizeMethod.AspectFitPadded:
				sourceBlitRect = new Rectangle(Point.Empty, sourceImageSize);
				scaleWidth = Math.Min(scaleWidth, scaleHeight);
				scaleHeight = Math.Min(scaleWidth, scaleHeight);
				destBlitRect.Width = (int)Math.Round(sourceImageSize.Width * scaleWidth);
				destBlitRect.Height = (int)Math.Round(sourceImageSize.Height * scaleHeight);
				destImageSize = requestedSize;
				alignDestBlitRect = true;
				break;
			case ResizeMethod.AspectFill:
				var sourceAspect = sourceImageSize.Width / (float)sourceImageSize.Height;
				var destAspect = requestedSize.Width / (float)requestedSize.Height;
				if (destAspect > sourceAspect) {
					sourceBlitRect = new Rectangle(0, 0, sourceImageSize.Width, (int)Math.Round(sourceImageSize.Width / destAspect));
					alignSourceBlitRect = true;
				} else if (destAspect < sourceAspect) {
					sourceBlitRect = new Rectangle(0, 0, (int)Math.Round(sourceImageSize.Height * destAspect), sourceImageSize.Height);
					alignSourceBlitRect = true;
				} else {
					sourceBlitRect = new Rectangle(Point.Empty, sourceImageSize);
				}
				destBlitRect = new Rectangle(Point.Empty, requestedSize);
				destImageSize = requestedSize;
				break;
			case ResizeMethod.Stretch:
			default:
				sourceBlitRect = new Rectangle(Point.Empty, sourceImageSize);
				destBlitRect.Width = (int)Math.Round(sourceImageSize.Width * scaleWidth);
				destBlitRect.Height = (int)Math.Round(sourceImageSize.Height * scaleHeight);
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

		Bitmap bitmap;
		if (destImageSize.Width != 0 && destImageSize.Height != 0) {

			bitmap = new Bitmap(destImageSize.Width, destImageSize.Height, pixelFormat);
			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using (var graphics = Graphics.FromImage(bitmap)) {
				graphics.Clear(actualPaddingColor);
				graphics.InterpolationMode = interpolationMode;
				graphics.DrawImage(
					image,
					destBlitRect,
					sourceBlitRect,
					GraphicsUnit.Pixel
				);
			}
		} else {
			bitmap = new Bitmap(1, 1);
		}
		return bitmap;
	}


	public static Bitmap ResizeAndDispose(
		this Image image,
		Size requestedSize,
		ResizeMethod resizeMethod = ResizeMethod.Stretch,
		ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
		InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
		Color? paddingColor = null,
		PixelFormat? newPixelFormat = null
	) {
		using (var sourceImage = image) {
			return sourceImage.Resize(requestedSize,
				resizeMethod,
				resizeAlignment,
				interpolationMode,
				paddingColor,
				newPixelFormat);
		}
	}


	/// <summary>
	/// Converts the image to an array of byte using the specified format.
	/// </summary>
	/// <param name="image"></param>
	/// <param name="imgFormat"></param>
	/// <returns></returns>
	/// 
	public static byte[] ToByteArray(this Image image) {
		return image.ToByteArray(ImageFormat.Png);
	}

	public static byte[] ToByteArray(this Image image, ImageFormat imgFormat) {
		if (image == null)
			return new byte[0];

		using (var mem = new MemoryStream()) {
			image.Save(mem, imgFormat);
			return mem.ToArray();
		}
	}

	public static void SaveToJpeg(int quality) {
		quality = quality.ClipTo(0, 100);

	}


	/// <summary>
	/// Save an Image as a JPeg with a given compression
	/// Note: Filename suffix will not affect mime type which will be Jpeg.
	/// </summary>
	/// <param name="image">This image</param>
	/// <param name="fileName">File name to save the image as. Note: suffix will not affect mime type which will be Jpeg.</param>
	/// <param name="compression">Value between 0 and 100.</param>
	public static void SaveJpegWithCompression(this Image image, string fileName, int compression) {
		Tools.Drawing.SaveJpegWithCompression(image, fileName, compression);
	}

	/// <summary>
	/// Save an Image as a JPeg with a given compression
	/// Note: Filename suffix will not affect mime type which will be Jpeg.
	/// </summary>
	/// <param name="image">This image</param>
	/// <param name="stream">The stream where the image will be saved.</param>
	/// <param name="compression">Value between 0 and 100.</param>
	public static void SaveJpegWithCompression(this Image image, Stream stream, int compression) {
		Tools.Drawing.SaveJpegWithCompression(image, stream, compression);
	}

}
