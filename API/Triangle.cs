using System;
using Triangulation.Extensions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Triangulation
{
    public readonly struct Triangle : IEquatable<Triangle>
    {
        private readonly Point _a;
        private readonly Point _b;
        private readonly Point _c;
        private readonly Point _cc;
        private readonly float _rSq;

        public Point A => _a;
        public Point B => _b;
        public Point C => _c;

        public Edge this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException();
                if (index == 0) 
                    return new(_a,_b);
                else if (index == 1) 
                    return new(_b,_c);
                else 
                    return new(_c, _a);
            }
        }
        public static Triangle Create(in Point a, in Point b, in Point c)
        {
            Point cc = CircumCenter_Fast(a, b, c);
            float rSq = cc.DistanceSqr(a);
            return new(a, b, c, cc, rSq);
        }
        Triangle(in Point a, in Point b, in Point c, Point cc, float rSq)
        {
            _a = a;
            _b = b;
            _c = c;
            _cc = cc;
            _rSq = rSq;
        }

        public bool SharesVertex(in Point a, in Point b, in Point c)
        {
            return _a == a || _b == a || _c == a ||
                   _a == b || _b == b || _c == b ||
                   _a == c || _b == c || _c == c;
        }

        public bool IsPointInsideCircumCircle(in Point vert) => vert.DistanceSqr(_cc) < _rSq;
        public override string ToString() => "[" + _a + " , " + _b + " , " + _c + "]";

        public static Point CircumCenter_Fast(in Point a, Point b, Point c)
        {
            b -= a;
            c -= a;

            var d = 2 * (b.X * c.Y - b.Y * c.X);

            var a1 = 1 / d * (c.Y * (b.X * b.X + b.Y * b.Y) - b.Y * (c.X * c.X + c.Y * c.Y));
            var b1 = 1 / d * (b.X * (c.X * c.X + c.Y * c.Y) - c.X * (b.X * b.X + b.Y * b.Y));
            return new Point(a1, b1) + a;
        }
        public bool HasEdge(Edge edge) => this[0].Equals(edge) || this[1].Equals(edge) || this[2].Equals(edge);
        public bool Equals( Triangle other) => A == other.A && B == other.B && B == other.B && C == other.C;

#if UNITY_EDITOR
        public void Draw(in Color color, in float edgeThickness = 1)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(edgeThickness,_a.float3(),_b.float3());
            Handles.DrawAAPolyLine(edgeThickness,_b.float3(),_c.float3());
            Handles.DrawAAPolyLine(edgeThickness,_c.float3(),_a.float3());
        }
        public void DrawCircumCircle(in Color color, in float circleThickness = 1)
        {
            float r = math.sqrt(_rSq);
            Handles.color = color;
            Handles.DrawWireDisc(_cc.float3(), new(0,1,0), r, circleThickness);
        }
#endif
    }
}