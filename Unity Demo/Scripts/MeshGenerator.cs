using System.Collections.Generic;
using Triangulation.Extensions;
using Unity.Mathematics;
using UnityEngine;
namespace Triangulation.Demo
{
    [RequireComponent (typeof(MeshFilter))]
    [RequireComponent (typeof(MeshRenderer))]
    [RequireComponent (typeof(Field))]
    public sealed class MeshGenerator : MonoBehaviour
    {

        Field _data;
        MeshFilter _meshFilter;
        Mesh _mesh;
        List<Vector3> _verts = new();
        List<int> _tris = new();
        List<Vector2> _uvs = new();
        Material _material;
        [SerializeField, Range(0, 1)]
        float _minAlpha;
        [SerializeField, Range(0, 1)]
        float _maxAlpha;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
            _mesh = new();
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;
            _data = GetComponent<Field>();
        }

        private void Start() => UpdateMesh();
        private void Update()
        {
            _material.SetVector("_MousePosition", _data.MousePosition);
            _material.SetColor("_MouseColor", _data.MouseColor);
            _material.SetFloat("_MouseRadius", _data.MouseRadius);
            _material.SetFloat("_MinAlpha", math.min(_minAlpha,_maxAlpha));
            _material.SetFloat("_MaxAlpha", math.max(_minAlpha,_maxAlpha));
        }
        private void UpdateMesh()
        {
            _mesh.Clear();
            _verts.Clear();
            _tris.Clear();
            _uvs.Clear();

            _verts.Add(_data.BottomLeft.float3());
            _verts.Add(_data.TopLeft.float3());
            _verts.Add(_data.TopRight.float3());
            _verts.Add(_data.BottomRight.float3());

            _tris.Add(0);
            _tris.Add(1);
            _tris.Add(2);
            _tris.Add(0);
            _tris.Add(2);
            _tris.Add(3);

            _uvs.Add(_data.BottomRight.float2());
            _uvs.Add(_data.TopLeft.float2());
            _uvs.Add(_data.TopRight.float2());
            _uvs.Add(_data.BottomRight.float2());

            _mesh.SetVertices(_verts);
            _mesh.SetTriangles(_tris, 0);
            _mesh.SetUVs(1, _uvs);
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
        }
    }
}