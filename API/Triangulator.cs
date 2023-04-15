using System.Collections.Generic;

namespace Triangulation
{
    public static class Triangulator
    {
        private static List<Edge> _edges = new();
        private static List<Triangle> _badTriangles = new();

        public static void Triangulate(in IReadOnlyList<Point> verts, in List<Triangle> triangles)
        {
            ClearBuffers();
            triangles.Clear();

            Triangle superTriangle = GenerateSupraTriangle(verts);
            triangles.Add(superTriangle);

            for (int i = 0; i < verts.Count; i++)
            {
                var vert = verts[i];

                _badTriangles.Clear();
                GetBadTriangles(vert,triangles);

                _edges.Clear();
                GetUniqueEdges();

                triangles.RemoveAll(_badTriangles.Contains);

                CreateTrianglesFromEdges(vert, triangles);
            }
            //remove all triangles sharing vertices with the super triangle
            var a = superTriangle.A;
            var b = superTriangle.B;
            var c = superTriangle.C;

            triangles.RemoveAll(t => t.SharesVertex(a,b,c));
        }

        private static void CreateTrianglesFromEdges(Point vert, List<Triangle> triangles)
        {
            //add new triangles
            for (int j = 0; j < _edges.Count; j++) // job
                triangles.Add(Triangle.Create(vert, _edges[j].A, _edges[j].B));
        }

        //job system
        private static void GetBadTriangles(in Point vert, IReadOnlyList<Triangle> triangles)
        {
            //get all bad triangles
            for (int j = 0; j < triangles.Count; j++) // job
                if (triangles[j].IsPointInsideCircumCircle(vert))
                    _badTriangles.Add(triangles[j]);
        }

        private static void ClearBuffers()
        {
            _edges.Clear();
            _badTriangles.Clear();
        }

        public static Triangle GenerateSupraTriangle(IReadOnlyList<Point> points)
        {
            GetPointBounds(points, out var minX, out var minY, out var maxX, out var maxY);

            float dx = maxX - minX;
            float dy = maxY - minY;
            float dMax = ((dx > dy) ? dx : dy) * 2f;

            var center = new Point((minX+maxX)/2f, (minY + maxY)/2f);
            var a = new Point(center.X - 2.0f * dMax, center.Y - dMax);
            var b = new Point(center.X, center.Y + 2.0f * dMax);
            var c = new Point(center.X + 2.0f * dMax, center.Y - dMax);

            return Triangle.Create(a, b, c);
        }

        private static void GetPointBounds(IReadOnlyList<Point> points, out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = float.PositiveInfinity;
            minY = minX;
            maxX = float.NegativeInfinity;
            maxY = maxX;

            for (int i = 0; i < points.Count; i++)
            {
                var p = points[i];
                if (p.X < minX)
                    minX = p.X;
                 if(p.X > maxX)
                    maxX = p.X;

                if (p.Y < minY)
                    minY = p.Y;
                 if (p.Y > maxY)
                    maxY = p.Y;
            }
        }

        //job
        private static void GetUniqueEdges()
        {
            _edges.Clear();
            for (int j = 0; j < _badTriangles.Count; j++)
            {
                for (int e = 0; e < 3; e++)
                {
                    Edge edge = _badTriangles[j][e];
                    bool rejected = false;
                    for (int k = 0; k < _badTriangles.Count; k++)
                    {
                        if (k != j && _badTriangles[k].HasEdge(edge))
                        {
                            rejected = true;
                            break;
                        }
                    }
                    if (!rejected)
                        _edges.Add(edge);
                }
            }
        }
    }
}