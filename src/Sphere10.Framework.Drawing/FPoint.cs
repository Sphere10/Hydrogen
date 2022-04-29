using System.Drawing;

namespace Sphere10.Framework {

    public struct PointFP {
        public FixedPoint X;
        public FixedPoint Y;

        public static PointFP Create(FixedPoint X, FixedPoint Y) {
            PointFP fp;
            fp.X = X;
            fp.Y = Y;
            return fp;
        }

        public static PointFP FromPoint(Point p) {
            PointFP f;
            f.X = (FixedPoint)p.X;
            f.Y = (FixedPoint)p.Y;
            return f;
        }

        public static Point ToPoint(PointFP f) {
            return new Point((int)f.X, (int)f.Y);
        }

        #region Vector Operations
        public static PointFP VectorAdd(PointFP F1, PointFP F2) {
            PointFP result;
            result.X = F1.X + F2.X;
            result.Y = F1.Y + F2.Y;
            return result;
        }

        public static PointFP VectorSubtract(PointFP F1, PointFP F2) {
            PointFP result;
            result.X = F1.X - F2.X;
            result.Y = F1.Y - F2.Y;
            return result;
        }

        public static PointFP VectorDivide(PointFP F1, int Divisor) {
            PointFP result;
            result.X = F1.X / (FixedPoint)Divisor;
            result.Y = F1.Y / (FixedPoint)Divisor;
            return result;
        }
        #endregion
    }

}