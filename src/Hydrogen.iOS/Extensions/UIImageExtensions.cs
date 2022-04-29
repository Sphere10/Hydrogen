//-----------------------------------------------------------------------
// <copyright file="UIImageExtensions.cs" company="Sphere 10 Software">
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
using System.IO;
using UIKit;
using Foundation;
using CoreGraphics;
using CoreGraphics;
using Hydrogen;

namespace Hydrogen.iOS {
    public static class UIImageExtensions {

        public static UIImage ToUIImage(this byte[] imageBuffer) {
            return Tools.iOSTool.ToUIImage(imageBuffer);
        }

        public static UIImage Antialias(this UIImage image) {
            return Tools.iOSTool.Antialias(image);
        }

        public static UIImage RemoveColor(this UIImage image, UIColor color, float tolerance) {
            return Tools.iOSTool.RemoveColor(image, color, tolerance);
        }

        public static UIImage Crop(this UIImage image, CGRect section) {
            return Tools.iOSTool.Crop(image, section);
        }

        public static UIImage Resize(
            this UIImage sourceImage,
            CGSize requestedSize,
            ResizeMethod resizeMethod = ResizeMethod.Stretch,
            ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
            bool antiAliasing = true,
            CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
            UIColor paddingColor = null) {
            return Tools.iOSTool.Resize(sourceImage, requestedSize, resizeMethod, resizeAlignment, antiAliasing, interpolationQuality, paddingColor);
        }

        public static UIImage ResizeAndDispose(
            this UIImage image,
            CGSize requestedSize,
            ResizeMethod resizeStrategy = ResizeMethod.Stretch,
            ResizeAlignment resizeAlignment = ResizeAlignment.CenterCenter,
            bool antiAliasing = true,
            CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
            UIColor paddingColor = null) {
            return Tools.iOSTool.ResizeAndDispose(image, requestedSize, resizeStrategy, resizeAlignment, antiAliasing, interpolationQuality, paddingColor);
        }


        public static UIImage Zoom(this UIImage image, float zoomFactor) {
            return image.Resize(
                new CGSize(image.Size.Width*zoomFactor, image.Size.Height*zoomFactor)
                );
        }

        public static UIImage ZoomAndDispose(this UIImage image, float zoomFactor) {
            using (var sourceImage = image) {
                return sourceImage.Zoom(zoomFactor);
            }
        }

        public static UIImage FocusZoom(
            this UIImage image,
            float zoomFactor,
            int focusX,
            int focusY,
            bool antiAliasing = true,
            CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
            UIColor paddingColor = null) {
	        return Tools.iOSTool.FocusZoom(image, zoomFactor, focusX, focusY, antiAliasing, interpolationQuality, paddingColor);
        }

        public static UIImage FocusZoomAndDispose(
            this UIImage image,
            float zoomFactor,
            int focusX,
            int focusY,
            bool antiAliasing = true,
            CGInterpolationQuality interpolationQuality = CGInterpolationQuality.High,
            UIColor paddingColor = null) {
	        return Tools.iOSTool.FocusZoomAndDispose(image, zoomFactor, focusX, focusY, antiAliasing, interpolationQuality, paddingColor);
        }

        /// <summary>
        /// Save an Image as a JPeg with a given compression
        /// Note: Filename suffix will not affect mime type which will be Jpeg.
        /// </summary>
        /// <param name="image">This image</param>
        /// <param name="fileName">File name to save the image as. Note: suffix will not affect mime type which will be Jpeg.</param>
        /// <param name="compression">Value between 0 and 100.</param>
        public static void SaveAsJpegWithCompression(this UIImage image, string fileName, int compression) {
            Tools.iOSTool.SaveJpegWithCompression(image, fileName, compression);
        }

        /// <summary>
        /// Save an Image as a JPeg with a given compression
        /// Note: Filename suffix will not affect mime type which will be Jpeg.
        /// </summary>
        /// <param name="image">This image</param>
        /// <param name="stream">The stream where the image will be saved.</param>
        /// <param name="compression">Value between 0 and 100.</param>
        public static void SaveAsJpegWithCompression(this UIImage image, Stream stream, int compression) {
            Tools.iOSTool.SaveJpegWithCompression(image, stream, compression);
        }

    }

}

