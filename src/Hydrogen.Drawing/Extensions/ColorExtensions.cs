// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;


namespace Hydrogen;

public static class ColorExtensions {

	/// <summary>
	/// Converts a Color to a string representation
	/// </summary>
	/// <param name="color">The Color to be converted</param>
	/// <returns>A string that represents the specified color</returns>
	public static string ToARGBString(this Color color) {
		if (color == Color.Empty) {
			return null;
		}
		return string.Format("{0}:{1}:{2}:{3}", color.A, color.R, color.G, color.B);
	}

	/// <summary>
	/// Gets a shade of a given color. A shade of 1 gives white, a shade of -1 gives black.
	/// </summary>
	public static Color GetShade(this Color color, float shade) {
		if (shade == 0)
			return color;
		if (shade > 1 || shade < -1)
			throw new ArgumentException("Must be within 1 and -1", "shade");

		float incrementR, incrementG, incrementB;

		if (shade < 0) {
			incrementR = ((float)(color.R)) / (float)100;
			incrementG = ((float)(color.G)) / (float)100;
			incrementB = ((float)(color.B)) / (float)100;
		} else {
			incrementR = ((float)(255 - color.R)) / (float)100;
			incrementG = ((float)(255 - color.G)) / (float)100;
			incrementB = ((float)(255 - color.B)) / (float)100;
		}

		int newR = color.R + (int)(incrementR * shade * (float)100);
		int newG = color.G + (int)(incrementG * shade * (float)100);
		int newB = color.B + (int)(incrementB * shade * (float)100);

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

		return Color.FromArgb(color.A, newR, newG, newB);
	}


	/// <summary>
	/// Gets a translucent shade of a given color. A shade of 1 gives fully lucent, a shade of -1 gives transparent.
	/// </summary>
	public static Color GetTranslucentShade(this Color color, float shade) {
		if (Math.Abs(shade - 0) < Tools.Maths.EPSILON_F)
			return color;

		if (shade > 1 || shade < -1)
			throw new ArgumentException("Must be within 1 and -1", "shade");

		float incrementA;

		if (shade < 0) {
			incrementA = ((float)(color.A)) / (float)100;
		} else {
			incrementA = ((float)(255 - color.A)) / (float)100;
		}


		int newA = color.A + (int)(incrementA * shade * (float)100);

		if (newA > 255)
			newA = 255;


		return Color.FromArgb(newA, color.R, color.G, color.B);
	}


}
