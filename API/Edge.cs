using System;
namespace Triangulation
{
    public readonly struct Edge : IEquatable<Edge>
    {
        private readonly Point _a;
        private readonly Point _b;
        public Point A => _a;
        public Point B => _b;

        public Edge(in Point a, in Point b)
        {
            _a = a;
            _b = b;
        }
        public bool Equals(Edge edge) => (_a == edge.A && _b == edge.B) || (_a == edge.B && _b == edge.A);
    }
}