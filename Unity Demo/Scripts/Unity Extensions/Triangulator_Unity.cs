using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Triangulation.Unity
{
    /// <summary>
    /// Triangulator taking advantage of Unity Job System
    /// </summary>
    public static class Triangulator_Unity
    {
        /// <summary>
        /// This is a triangulation function using Unity's job system, however it is not particularly fast
        /// in some cases it is slower than the main thread - i made this mainly to learn the job system.
        /// </summary>
        [BurstCompatible]
        public static void Triangulate(in IReadOnlyList<Point> verts, in List<Triangle> triangles)
        {
            triangles.Clear();

            var tris = new NativeList<Triangle>((verts.Count / 3) + 1,Allocator.TempJob);
            var badTris = new NativeList<Triangle>(tris.Capacity, Allocator.TempJob);
            var edges = new NativeList<Edge>(tris.Capacity, Allocator.TempJob);
            var triBuffer = new NativeList<Triangle>(tris.Capacity, Allocator.TempJob);

            Triangle superTriangle = Triangulator.GenerateSupraTriangle(verts);
            tris.Add(superTriangle);

            for (int i = 0; i < verts.Count; i++)
            {
                var vert = verts[i];

                // get bad triangles
                badTris.Clear();
                if(badTris.Capacity < tris.Length)
                    badTris.Capacity = tris.Length;
                
                var job = new GetBadTriangles(vert, tris, badTris.AsParallelWriter());
                    job.Schedule(tris.Length, GetBatchSize(tris.Length)).Complete();
                ////////////////////

                //get unique edges
                edges.Clear();
                if (edges.Capacity < badTris.Length * 3)
                    edges.Capacity = badTris.Length * 3;

                var job2 = new GetUniqueEdges(badTris, edges.AsParallelWriter());
                    job2.Schedule(badTris.Length, GetBatchSize(badTris.Length)).Complete();
                ////////////////////

                // remove all bad triangles from tris
                triBuffer.Clear();
                if (triBuffer.Capacity < tris.Length)
                    triBuffer.Capacity = tris.Length;

                var job3 = new RemoveBadTriangles(badTris,tris, triBuffer.AsParallelWriter());
                    job3.Schedule(tris.Length, GetBatchSize(tris.Length)).Complete();
                tris.CopyFrom(triBuffer);
                ////////////////////

                // create new triangles from edges
                if(tris.Capacity < tris.Length+edges.Length)
                    tris.Capacity = tris.Length + edges.Length;

                var job4 = new CreateTrianglesFromEdges(vert, edges, tris.AsParallelWriter());
                    job4.Schedule(edges.Length, GetBatchSize(edges.Length)).Complete();
                /////////////////////
            }

            var output = new NativeList<Triangle>(tris.Length, Allocator.TempJob);
            var job5 = new RemoveIfSharingVertex(tris, output.AsParallelWriter(), superTriangle.A, superTriangle.B, superTriangle.C);
                job5.Schedule(tris.Length, GetBatchSize(tris.Length)).Complete();
            
            //copy final triangles back to managed
            triangles.AddRange(output.ToArray());

            badTris.Dispose();
            output.Dispose();
            tris.Dispose();
            edges.Dispose();
            triBuffer.Dispose();
        }
        private static int GetBatchSize(in int arrayLength)
        {
            int numBatches = math.max(1, JobsUtility.JobWorkerCount);
            return arrayLength / numBatches;
        }
    }
    [BurstCompile]
    struct RemoveIfSharingVertex : IJobParallelFor
    {
        [ReadOnly]
        NativeList<Triangle> _tris;
        NativeList<Triangle>.ParallelWriter _output;
        [ReadOnly]
        Point _a;
        [ReadOnly]
        Point _b;
        [ReadOnly]
        Point _c;

        public RemoveIfSharingVertex(in NativeList<Triangle> input, in NativeList<Triangle>.ParallelWriter output, in Point A, in Point B, in Point C)
        {
            _tris = input;
            _output = output;
            _a = A;
            _b = B;
            _c = C;
        }

        [BurstCompile]
        public void Execute(int index)
        {
            if (!_tris[index].SharesVertex(_a, _b, _c))
                _output.AddNoResize(_tris[index]);
        }
    }
    [BurstCompile]
    struct GetUniqueEdges : IJobParallelFor
    {
        NativeList<Edge>.ParallelWriter _edges;
        [ReadOnly]
        NativeList<Triangle> _badTris;

        public GetUniqueEdges(in NativeList<Triangle> badTris, in NativeList<Edge>.ParallelWriter edges)
        {
            _badTris = badTris;
            _edges = edges;
        }

        [BurstCompile]
        public void Execute(int index)
        {
            Triangle tri = _badTris[index];
            for (int e = 0; e < 3; e++)
            {
                Edge edge = tri[e];
                bool rejected = false;
                for (int k = 0; k < _badTris.Length; k++)
                {
                    if (k != index && _badTris[k].HasEdge(edge))
                    {
                        rejected = true;
                        break;
                    }
                }
                if (!rejected)
                    _edges.AddNoResize(edge);
            }
        }
    }
     
    [BurstCompile]
    struct GetBadTriangles : IJobParallelFor
    {
        [ReadOnly]
        Point _point;
        [ReadOnly]
        NativeList<Triangle> _tris;
        NativeList<Triangle>.ParallelWriter _badTris;
        public GetBadTriangles(in Point point, in NativeList<Triangle> tris, in NativeList<Triangle>.ParallelWriter badTris)
        {
            _point = point;
            _tris = tris;
            _badTris = badTris;
        }
        [BurstCompile]
        public void Execute(int index)
        {
            if (_tris[index].IsPointInsideCircumCircle(_point))
                _badTris.AddNoResize(_tris[index]);
        }
    }
    [BurstCompile]
    struct RemoveBadTriangles : IJobParallelFor
    {
        [ReadOnly]
        NativeList<Triangle> _badTris;
        [ReadOnly]
        NativeList<Triangle> _tris;
        NativeList<Triangle>.ParallelWriter _output;

        public RemoveBadTriangles(in NativeList<Triangle> badTris, in NativeList<Triangle> tris, in NativeList<Triangle>.ParallelWriter output)
        {
            _badTris = badTris;
            _tris = tris;
            _output = output;
        }
        [BurstCompile]
        public void Execute(int index)
        {
            for (int i = 0; i < _badTris.Length; i++)
            {
                if (_tris[index].Equals(_badTris[i]))
                    return;
            }
            _output.AddNoResize(_tris[index]);
        }
    }
    [BurstCompile]
    struct CreateTrianglesFromEdges : IJobParallelFor
    {
        [ReadOnly]
        Point _point;
        [ReadOnly]
        NativeList<Edge> _edges;
        NativeList<Triangle>.ParallelWriter _writer;
        public CreateTrianglesFromEdges(in Point point, NativeList<Edge> edges, NativeList<Triangle>.ParallelWriter writer)
        {
            _point = point;
            _edges = edges;
            _writer = writer;
        }

        [BurstCompile]
        public void Execute(int index) => _writer.AddNoResize(Triangle.Create(_point, _edges[index].A, _edges[index].B));


    }
}