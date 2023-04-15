using Unity.Mathematics;
using UnityEngine;
namespace Triangulation.Extensions
{
    /// <summary>
    /// Extensions to support Unity3D Vector2, Vector3, float2 and float3 types
    /// </summary>
    public static class Extensions 
    {
        public static Vector2 Vector2(this Point point) => new (point.X,point.Y);
        public static Vector3 Vector3(this Point point) => new (point.X,0,point.Y);
        public static float2 float2(this Point point) => new (point.X,point.Y);
        public static float3 float3(this Point point) => new (point.X,0,point.Y);
    }
}