using SysMath = System.Math;
// ReSharper disable InconsistentNaming

/// <summary>
/// From http://sci.math.narkive.com/4iRcGoJ3/some-functions LOL
/// </summary>
namespace Sphere10.Framework {
	public static class SchoenfeldFunctions {
		private const long R = 10;

		public static long NUMDIGIT(long x) => (long) SysMath.Floor(SysMath.Log(x, R)) + 1;

		public static double SHIFTRIGHT(double x, long places) => x/SysMath.Pow(R, places);

		public static double SHIFTLEFT(double x, long places) => x*SysMath.Pow(R, places);

		public static long FIRSTDIGIT(long x) => (long) SysMath.Floor( SHIFTRIGHT(x, NUMDIGIT(x) - 1));

		public static long LASTDIGIT(long x) => x - R * (long)SysMath.Floor(x / (double)R);

		public static long DIGIT_LR(long x, long n) => LASTDIGIT((long)SysMath.Floor( SHIFTRIGHT(x, NUMDIGIT(x) - n - 1) ) );

		public static long DIGIT_RL(long x, long n) => LASTDIGIT((long)SysMath.Floor(SHIFTRIGHT(x, n)));

		public static long NEXTNUM(long x) => x + 1;

		public static long PREVNUM(long x) => x - 1;

		public static long NEXTDIGIT(long x) => LASTDIGIT(NEXTNUM(x));

		public static long PREVDIGIT(long x) => LASTDIGIT(PREVNUM(x));

	}
}
