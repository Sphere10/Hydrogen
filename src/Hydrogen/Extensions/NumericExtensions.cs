// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public static class NumericExtensions {

	#region Rounding

	public static decimal Round(this decimal value, int decimals)
		=> Math.Round(value, decimals);

	public static float Round(this float value, int decimals)
		=> (float)Math.Round(value, decimals);

	public static double Round(this double value, int decimals)
		=> Math.Round(value, decimals);

	#endregion

	#region Sum

	public static UInt64 Sum(this IEnumerable<UInt64> source) {
		return source.Aggregate((x, y) => x + y);
	}

	public static UInt32 Sum(this IEnumerable<UInt32> source) {
		return source.Aggregate((x, y) => x + y);
	}

	#endregion


	#region Clipping

	public static TimeSpan ClipTo(this TimeSpan value, TimeSpan min, TimeSpan max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static DateTime ClipTo(this DateTime value, DateTime min, DateTime max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static double ClipTo(this double value, double min, double max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static float ClipTo(this float value, float min, float max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static decimal ClipTo(this decimal value, decimal min, decimal max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static sbyte ClipTo(this sbyte value, sbyte min, sbyte max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static byte ClipTo(this byte value, byte min, byte max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static short ClipTo(this short value, short min, short max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static ushort ClipTo(this ushort value, ushort min, ushort max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static int ClipTo(this int value, int min, int max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static uint ClipTo(this uint value, uint min, uint max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static long ClipTo(this long value, long min, long max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	public static ulong ClipTo(this ulong value, ulong min, ulong max) {
		if (value < min) {
			return min;
		} else if (value > max) {
			return max;
		}
		return value;
	}

	#endregion

	/// <summary>
	/// Try to convert a string into a nullable uint. If the string cannot be converted
	/// the default value is returned.
	/// </summary>
	/// <param name="value">The string to parse.</param>
	/// <param name="defaultValue">The value to return if the string could not be converted.</param>
	/// <returns>A nullable uint if the input was valid; otherwise, defaultValue.</returns>
	public static uint? ToUIntOrDefault(this string value, uint? defaultValue = null) {
		if (string.IsNullOrWhiteSpace(value) || !uint.TryParse(value, out var result))
			return defaultValue;
		return result;

	}
}
