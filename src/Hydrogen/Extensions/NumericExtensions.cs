//-----------------------------------------------------------------------
// <copyright file="NumericExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;

namespace Hydrogen {
	public static class NumericExtensions {

		public static UInt64 Sum(this IEnumerable<UInt64> source) {
			return source.Aggregate((x, y) => x + y);
		}

		public static UInt32 Sum(this IEnumerable<UInt32> source) {
			return source.Aggregate((x, y) => x + y);
		}


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

        /// <summary>
        /// Try to convert a string into a nullable uint. If the string cannot be converted
        /// the default value is returned.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="defaultValue">The value to return if the string could not be converted.</param>
        /// <returns>A nullable uint if the input was valid; otherwise, defaultValue.</returns>
        public static uint? ToUintOrDefault(this string value, uint? defaultValue = null)
        {
            if (!string.IsNullOrWhiteSpace(value) && uint.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
