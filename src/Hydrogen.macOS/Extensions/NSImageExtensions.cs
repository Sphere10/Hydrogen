//-----------------------------------------------------------------------
// <copyright file="NSImageExtensions.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;
using MonoMac.CoreGraphics;
using System.IO;

namespace Hydrogen {
	public static class NSImageExtensions {


		public static void SaveToFile(this NSImage image, string path) {
			//NSBitmapImageRep *imgRep = [[image representations] objectAtIndex: 0];
			//NSData *data = [imgRep representationUsingType: NSPNGFileType properties: nil];
			//[data writeToFile: @"/path/to/file.png" atomically: NO];

			var imageData = image.AsTiff();
			var imgRep = NSBitmapImageRep.ImageRepFromData(imageData) as NSBitmapImageRep;
			var imageProps = new NSDictionary();

			var data = imgRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Png, imageProps);

			NSError error;
			data.Save(path, false, out error);

			data.Dispose();
			imgRep.Dispose();

		}


		public static NSImage ToNSImage(this Image img) {
			MemoryStream s = new MemoryStream();
			img.Save(s, System.Drawing.Imaging.ImageFormat.Png);
			byte[] b = s.ToArray();
			CGDataProvider dp = new CGDataProvider(b,0,(int)s.Length);
			s.Flush();
			s.Close();
			CGImage img2 = 
				CGImage.FromPNG(dp,null,false,CGColorRenderingIntent.Default);
			return new NSImage(img2, new SizeF(img2.Width,img2.Height));
		}

		public static Image ToImage(this NSImage img) {
			using (var imageData = img.AsTiff()) { 
				var imgRep = NSBitmapImageRep.ImageRepFromData(imageData) as NSBitmapImageRep;
				var imageProps = new NSDictionary();
				var data = imgRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Png, imageProps);
				return Image.FromStream(data.AsStream());
			}
		}


	}
}

