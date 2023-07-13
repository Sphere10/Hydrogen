// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

// ReSharper disable CheckNamespace
namespace Tools;

public partial class Drawing {
	private static ImageAttributes _sDisabledImageAttr;

	/// <summary>
	/// Converts a string to a color
	/// </summary>
	/// <param name="col">The string to be converted</param>
	/// <returns>The converted Color</returns>
	public static Color ConvertStringToColor(string col) {
		if (string.IsNullOrWhiteSpace(col)) {
			return Color.Empty;
		}
		var s = col.Split(new char[] { ':' });
		switch (s.Length) {
			case 3: return Color.FromArgb(Int32.Parse(s[0]), Int32.Parse(s[1]), Int32.Parse(s[2]));
			case 4: return Color.FromArgb(Int32.Parse(s[0]), Int32.Parse(s[1]), Int32.Parse(s[2]), Int32.Parse(s[3]));
		}
		return Color.Empty;
	}

	/// <summary>
	/// Interpolate the specified number of times between start and end color
	/// </summary>
	/// <param name="p_StartColor"></param>
	/// <param name="p_EndColor"></param>
	/// <param name="p_NumberOfGradients"></param>
	/// <returns></returns>
	public static Color[] CalculateColorGradient(Color p_StartColor, Color p_EndColor, int p_NumberOfGradients) {
		if (p_NumberOfGradients < 2)
			throw new ArgumentException("Invalid Number of gradients, must be 2 or more");
		Color[] l_Colors = new Color[p_NumberOfGradients];
		l_Colors[0] = p_StartColor;
		l_Colors[l_Colors.Length - 1] = p_EndColor;

		float l_IncrementA = ((float)(p_EndColor.A - p_StartColor.A)) / (float)p_NumberOfGradients;
		float l_IncrementR = ((float)(p_EndColor.R - p_StartColor.R)) / (float)p_NumberOfGradients;
		float l_IncrementG = ((float)(p_EndColor.G - p_StartColor.G)) / (float)p_NumberOfGradients;
		float l_IncrementB = ((float)(p_EndColor.B - p_StartColor.B)) / (float)p_NumberOfGradients;

		for (int i = 1; i < (l_Colors.Length - 1); i++) {
			l_Colors[i] = Color.FromArgb((int)(p_StartColor.A + l_IncrementA * (float)i),
				(int)(p_StartColor.R + l_IncrementR * (float)i),
				(int)(p_StartColor.G + l_IncrementG * (float)i),
				(int)(p_StartColor.B + l_IncrementB * (float)i));
		}

		return l_Colors;
	}

	/// <summary>
	/// Calculate a darker or lighter color using the source specified.
	/// A light of 1 is White, a light of -1 is black. All the other values are an interpolation from the source color.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="light"></param>
	/// <returns></returns>
	public static Color CalculateLightDarkColor(Color source, float light) {
		if (light == 0)
			return source;
		if (light > 1 || light < -1)
			throw new ArgumentException("Must be between 1 and -1", "light");

		float _IncrementR, _IncrementG, _IncrementB;

		if (light < 0) {
			_IncrementR = ((float)(source.R)) / (float)100;
			_IncrementG = ((float)(source.G)) / (float)100;
			_IncrementB = ((float)(source.B)) / (float)100;
		} else {
			_IncrementR = ((float)(255 - source.R)) / (float)100;
			_IncrementG = ((float)(255 - source.G)) / (float)100;
			_IncrementB = ((float)(255 - source.B)) / (float)100;
		}

		int newR, newG, newB;

		newR = source.R + (int)(_IncrementR * light * (float)100);
		newG = source.G + (int)(_IncrementG * light * (float)100);
		newB = source.B + (int)(_IncrementB * light * (float)100);

		if (newR > 255)
			newR = 255;
		else if (newR < 0)
			newR = 0;
		if (newG > 255)
			newG = 255;
		else if (newG < 0)
			newG = 0;
		if (newB > 255)
			newB = 255;
		else if (newB < 0)
			newB = 0;

		return Color.FromArgb(source.A, newR, newG, newB);
	}

	/// <summary>
	/// Calculate the middle color between the start and the end color.
	/// </summary>
	/// <param name="p_StartColor"></param>
	/// <param name="p_EndColor"></param>
	/// <returns></returns>
	public static Color CalculateMiddleColor(Color p_StartColor, Color p_EndColor) {
		return CalculateColorGradient(p_StartColor, p_EndColor, 3)[1];
	}

	public static string ConvertColorToString(Color color) {
		return color.ToARGBString();
	}

	public static Color RandomColor(bool solid = true) {
		return Color.FromArgb(solid ? 255 : Tools.Maths.RNG.Next(256), Tools.Maths.RNG.Next(256), Tools.Maths.RNG.Next(256));
	}

	public static bool IsRecognisedImageFile(string fileName) {
		string targetExtension = Path.GetExtension(fileName);
		if (String.IsNullOrEmpty(targetExtension))
			return false;
		else
			targetExtension = "*" + targetExtension.ToLowerInvariant();

		var recognisedImageExtensions = new List<string>();

		foreach (var imageCodec in ImageCodecInfo.GetImageEncoders())
			recognisedImageExtensions.AddRange(imageCodec.FilenameExtension.ToLowerInvariant().Split(";".ToCharArray()));

		return recognisedImageExtensions.Any(extension => extension.Equals(targetExtension));
	}

	/// <summary>
	/// Retrieves the Encoder Information for a given MimeType
	/// </summary>
	/// <param name="mimeType">String: Mimetype</param>
	/// <returns>ImageCodecInfo: Mime info or null if not found</returns>
	public static ImageCodecInfo GetEncoderInfo(String mimeType) {
		var encoders = ImageCodecInfo.GetImageEncoders();
		return encoders.FirstOrDefault(t => t.MimeType == mimeType);
	}

	/// <summary>
	/// Save an Image as a JPeg with a given compression
	///  Note: Filename suffix will not affect mime type which will be Jpeg.
	/// </summary>
	/// <param name="image">Image to save</param>
	/// <param name="fileName">File name to save the image as. Note: suffix will not affect mime type which will be Jpeg.</param>
	/// <param name="compression">Value between 0 and 100.</param>
	public static void SaveJpegWithCompression(Image image, string fileName, int compression) {
		if (!(0 <= compression && compression <= 100))
			throw new ArgumentOutOfRangeException("compression", "Must be between 0 and 100");

		var eps = new EncoderParameters(1);
		eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
		var ici = GetEncoderInfo("image/jpeg");
		image.Save(fileName, ici, eps);
	}

	/// <summary>
	/// Save an Image as a JPeg with a given compression
	///  Note: Filename suffix will not affect mime type which will be Jpeg.
	/// </summary>
	/// <param name="image">Image to save</param>
	/// <param name="stream">The stream where the image will be saved.</param>
	/// <param name="compression">Value between 0 and 100.</param>
	public static void SaveJpegWithCompression(Image image, Stream stream, int compression) {
		if (!(0 <= compression && compression <= 100))
			throw new ArgumentOutOfRangeException("compression", "Must be between 0 and 100");

		var eps = new EncoderParameters(1);
		eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
		var ici = GetEncoderInfo("image/jpeg");
		image.Save(stream, ici, eps);
	}

	public static bool TryGetImageFormatFromHeader(string filePath, out ImageFormat imageFormat) {
		using (var fileStream = new FileStream(filePath, FileMode.Open)) {
			return TryGetImageFormatFromHeader(fileStream, out imageFormat);
		}
	}

	public static bool TryGetImageFormatFromHeader(Stream imageStream, out ImageFormat imageFormat) {
		const int mostBytesNeeded = 11; //For JPEG
		imageFormat = null;

		if (imageStream.Length < mostBytesNeeded) {
			return false;
		}

		var headerBytes = new byte[mostBytesNeeded];
		imageStream.Read(headerBytes, 0, mostBytesNeeded);

		//Sources:
		//http://stackoverflow.com/questions/9354747
		//http://en.wikipedia.org/wiki/Magic_number_%28programming%29#Magic_numbers_in_files
		//http://www.mikekunz.com/image_file_header.html

		//JPEG:
		if (headerBytes[0] == 0xFF && //FF D8
		    headerBytes[1] == 0xD8 &&
		    (
			    (headerBytes[6] == 0x4A && //'JFIF'
			     headerBytes[7] == 0x46 &&
			     headerBytes[8] == 0x49 &&
			     headerBytes[9] == 0x46)
			    ||
			    (headerBytes[6] == 0x45 && //'EXIF'
			     headerBytes[7] == 0x78 &&
			     headerBytes[8] == 0x69 &&
			     headerBytes[9] == 0x66)
		    ) &&
		    headerBytes[10] == 00) {
			imageFormat = ImageFormat.Jpeg;
			return true;
		}
		//PNG 
		if (headerBytes[0] == 0x89 && //89 50 4E 47 0D 0A 1A 0A
		    headerBytes[1] == 0x50 &&
		    headerBytes[2] == 0x4E &&
		    headerBytes[3] == 0x47 &&
		    headerBytes[4] == 0x0D &&
		    headerBytes[5] == 0x0A &&
		    headerBytes[6] == 0x1A &&
		    headerBytes[7] == 0x0A) {
			imageFormat = ImageFormat.Png;
			return true;
		}
		//GIF
		if (headerBytes[0] == 0x47 && //'GIF'
		    headerBytes[1] == 0x49 &&
		    headerBytes[2] == 0x46) {
			imageFormat = ImageFormat.Gif;
			return true;
		}
		//BMP
		if (headerBytes[0] == 0x42 && //42 4D
		    headerBytes[1] == 0x4D) {
			imageFormat = ImageFormat.Bmp;
			return true;
		}
		//TIFF
		if ((headerBytes[0] == 0x49 && //49 49 2A 00
		     headerBytes[1] == 0x49 &&
		     headerBytes[2] == 0x2A &&
		     headerBytes[3] == 0x00)
		    ||
		    (headerBytes[0] == 0x4D && //4D 4D 00 2A
		     headerBytes[1] == 0x4D &&
		     headerBytes[2] == 0x00 &&
		     headerBytes[3] == 0x2A)) {
			imageFormat = ImageFormat.Tiff;
			return true;
		}

		return false;
	}

	public static bool TryGetImageFormatFromFilenameExtension(string filePath, out ImageFormat imageFormat) {
		string extension = Path.GetExtension(filePath);
		if (string.IsNullOrEmpty(extension))
			throw new ArgumentException(
				string.Format("Unable to determine file extension for fileName: {0}", filePath));

		switch (extension.ToLower()) {
			case @".bmp":
				imageFormat = ImageFormat.Bmp;
				return true;
			case @".gif":
				imageFormat = ImageFormat.Gif;
				return true;
			case @".ico":
				imageFormat = ImageFormat.Icon;
				return true;
			case @".jpg":
			case @".jpeg":
				imageFormat = ImageFormat.Jpeg;
				return true;
			case @".png":
				imageFormat = ImageFormat.Png;
				return true;
			case @".tif":
			case @".tiff":
				imageFormat = ImageFormat.Tiff;
				return true;
			case @".wmf":
				imageFormat = ImageFormat.Wmf;
				return true;
			default:
				imageFormat = null;
				return false;
		}
	}

	public static bool TryGetFileExtensionFromImageFormat(ImageFormat format, out string extension) {
		extension = null;
		var encoders = ImageCodecInfo.GetImageEncoders().ToArray();
		if (encoders.Any(x => x.FormatID == format.Guid)) {
			var extensions =
				encoders
					.First(x => x.FormatID == format.Guid)
					.FilenameExtension
					.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (extensions.Any()) {
				extension =
					extensions
						.First()
						.Trim('*')
						.ToLower();
				return true;
			}
		}
		return false;
	}

	public static Icon Resizeicon(Icon icon, int squareDimension) {
		return new Icon(icon, new Size(squareDimension, squareDimension));
	}

	/// <summary>
	/// Create a disabled version of the image.
	/// </summary>
	/// <param name="image">The image to convert</param>
	/// <param name="background">The Color of the background behind the image. The background parameter is used to calculate the fill color of the disabled image so that it is always visible against the background.</param>
	/// <returns></returns>
	public static Image CreateDisabledImage(Image image, Color background) {
		if (image == null)
			return null;

		Size imgSize = image.Size;
		if (_sDisabledImageAttr == null) {
			float[][] arrayJagged = new float[5][];
			arrayJagged[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0f, 0f };
			arrayJagged[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0f, 0f };
			arrayJagged[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0f, 0f };
			float[] arraySingle = new float[5];
			arraySingle[3] = 1f;
			arrayJagged[3] = arraySingle;
			arrayJagged[4] = new float[5] { 0.38f, 0.38f, 0.38f, 0f, 1f };
			System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix(arrayJagged);
			_sDisabledImageAttr = new System.Drawing.Imaging.ImageAttributes();
			_sDisabledImageAttr.ClearColorKey();

			_sDisabledImageAttr.SetColorMatrix(matrix);
		}

		Bitmap bitmap = new Bitmap(image.Width, image.Height);
		using (Graphics g = Graphics.FromImage(bitmap)) {
			g.DrawImage(image, new Rectangle(0, 0, imgSize.Width, imgSize.Height), 0, 0, imgSize.Width, imgSize.Height, GraphicsUnit.Pixel, _sDisabledImageAttr);
		}

		return bitmap;
	}
}
