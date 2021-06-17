//namespace Sphere10.Framework {
//	/// <summary>
//	/// Extension methods to do with ranges.
//	/// </summary>
//	public static class ValueRangeExtensions
//	{
//		/// <summary>
//		/// Creates an inclusive range between two values. The default comparer is used
//		/// to compare values.
//		/// </summary>
//		/// <typeparam name="T">Type of the values</typeparam>
//		/// <param name="start">Start of range.</param>
//		/// <param name="end">End of range.</param>
//		/// <returns>An inclusive range between the start point and the end point.</returns>
//		public static ValueRange<T> To<T>(this T start, T end)
//		{
//			return new ValueRange<T>(start, end);
//		}

//		/// <summary>
//		/// Returns a RangeIterator over the given range, where the stepping function
//		/// is to step by the given number of characters.
//		/// </summary>
//		/// <param name="range">The range to create an iterator for</param>
//		/// <param name="step">How many characters to step each time</param>
//		/// <returns>A RangeIterator with a suitable stepping function</returns>
//		public static ValueRangeIterator<char> StepChar(this ValueRange<char> range, int step)
//		{
//			return range.Step(c => (char)(c + step));
//		}
//	}
//}
