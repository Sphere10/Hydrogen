// <copyright file="ImageResult.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hydrogen.Web.AspNetCore {
    /// <summary>
    /// Streams an Image into the output stream
    /// </summary>
    public class ImageResult : ActionResult {

		public Image Image { get; set; }

		public ImageFormat Format { get; set; }
		
		public ImageResult(Image image) : this (image, ImageFormat.Jpeg)	{
		}

		public ImageResult(Image image, ImageFormat imageFormat) {
			this.Image = image;
			this.Format = imageFormat;
		}

		public override void ExecuteResult(ActionContext context) {
			try {
				HttpResponse response = context.HttpContext.Response;
				if (Equals(Format, ImageFormat.Jpeg)) {
					response.ContentType = "image/jpeg";
				} else if (Equals(Format, ImageFormat.Png)) {
					response.ContentType = "image/png";
				} else if (Equals(Format, ImageFormat.Gif)) {
					response.ContentType = "image/gif";
				} else {
					throw (new Exception("Unsupported Image Format."));
				}
				Image.Save(response.Body, Format);
			} finally {
				Image.Dispose();
			}
		}

	}
}
