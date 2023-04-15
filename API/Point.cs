using System;
using Unity.Mathematics;

namespace Triangulation
{
    public readonly struct Point: IEquatable<Point>
    {
        readonly float _x;
        readonly float _y;
        public float X => _x;
        public float Y => _y;
        public Point(in float x, in float y)
        {
            _x = x;
            _y = y;
        }
        public static Point operator +(Point a, Point b) => new Point(a._x + b._x, a._y + b._y);
        public static Point operator *(Point a, Point b) => new Point(a._x * b._x, a._y * b._y);
        public static Point operator *(Point a, float b) => new Point(a._x * b, a._y * b);
        public static Point operator /(Point a, Point b) => new Point(a._x / b._x, a._y / b._y);
        public static Point operator /(Point a, float b) => new Point(a._x / b, a._y / b);
        public static Point operator -(Point a, Point b) => new Point(a._x - b._x, a._y - b._y);
        public static Point operator -(Point a, float b) => new Point(a._x - b, a._y - b);
        public static bool operator ==(Point a, Point b) => a._x == b._x && a._y == b._y;
        public static bool operator !=(Point a, Point b) => !(a == b);
        public bool Equals(Point other) => other == this;
        public override bool Equals(object obj)
        {
            if (obj is Point point)
                return point == this;
            return false;
        }
        public override int GetHashCode()
        {
            int hashCode = _x.GetHashCode();
            hashCode += 3 * GetHashCode();
            return hashCode;
        }
        public float Distance(in Point b) => math.sqrt(DistanceSqr(b));
        public float DistanceSqr(in Point b)
        {
            var p = this - b;
            return p._x * p._x + p._y * p._y;
        }
    }
}