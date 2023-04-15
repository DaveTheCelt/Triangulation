using System;
using System.Collections.Generic;
using Triangulation.Unity;
using Unity.Mathematics;
using UnityEngine;

namespace Triangulation.Demo
{
    public sealed class Field : MonoBehaviour
    {
        [SerializeField]
        private List<MovingPoint> _field = new();
        public Point BottomLeft => new(-_bounds.x, -_bounds.y);
        public Point BottomRight => new(_bounds.x, -_bounds.y);
        public Point TopRight => new(_bounds.x, _bounds.y);
        public Point TopLeft => new(-_bounds.x, _bounds.y);

        private float2 _bounds;
        [SerializeField, Range(0, 150)]

        int _totalPoints = 3;
        private List<Point> _verts = new();
        private List<Triangle> _triangles = new();
        public IReadOnlyList<Triangle> Triangles => _triangles;

        public List<Point> Points => _verts;
        public Vector3 MousePosition { get; set; }

        [SerializeField]
        private bool _randomize;
        [SerializeField]
        private Color _edgeColor;
        public Color EdgeColor => _edgeColor;
        [SerializeField]
        private Color _boundsColor;
        [SerializeField]
        private Color _mouseColor;
        public Color MouseColor => _mouseColor;

        [SerializeField, Range(0, 5)]
        private float _mouseRadius;
        public float MouseRadius => _mouseRadius;
        public int TotalPoints => _totalPoints;

        public float2 Bounds => _bounds;
        public Color BoundsColor => _boundsColor;

        [SerializeField]
        private float2 _boundsMargin = 0;
        [SerializeField]
        private bool _useJobSystem;
        [SerializeField]
        private bool _continuous;
        [SerializeField]
        private bool _run;

        private void Start()
        {
            CalculateBounds();
            GeneratePointField();
        }
        private void GeneratePointField()
        {
            _field.Clear();
            _verts.Clear();

            for (int i = 0; i < _totalPoints; i++)
            {
                float x = UnityEngine.Random.Range(-_bounds.x, _bounds.x);
                float z = UnityEngine.Random.Range(-_bounds.y, _bounds.y);
                float3 startPos = new(x, 0, z);
                _field.Add(new(startPos, _bounds, UnityEngine.Random.Range(1, 4)));
                _verts.Add(new(x,z));
            }

            _verts.Add(BottomLeft);
            _verts.Add(BottomRight);
            _verts.Add(TopLeft);
            _verts.Add(TopRight);
        }
        private void Update()
        {
            for (int i = 0; i < _field.Count; i++)
            {
                var p = _field[i];
                p.Update(Time.deltaTime, _bounds);
                _field[i] = p;
                _verts[i] = new(_field[i].Position.x, _field[i].Position.z);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                MousePosition = ray.GetPoint(distance);
                if (MousePosition.x >= -_bounds.x && MousePosition.x <= _bounds.x && MousePosition.z <= _bounds.y && MousePosition.z >= -_bounds.y)
                {
                    if (_verts.Count == _totalPoints + 4)
                        _verts.Add(new(MousePosition.x, MousePosition.z));
                    else
                        _verts[^1] = new(MousePosition.x, MousePosition.z);
                }
                else
                    if (_verts.Count > _totalPoints + 4)
                    _verts.RemoveAt(_verts.Count - 1);
            }

            if (!_useJobSystem)
                Triangulator.Triangulate(_verts, _triangles);
            else
                Triangulator_Unity.Triangulate(_verts, _triangles);
        }
        void CalculateBounds()
        {
            var bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var topLeft = Camera.main.ScreenToWorldPoint(new(0, Camera.main.scaledPixelHeight + 5 - _boundsMargin.y));
            var bottomRight = Camera.main.ScreenToWorldPoint(new(Camera.main.scaledPixelWidth + 5 - _boundsMargin.x, 0));
            _bounds.x = math.distance(bottomLeft, bottomRight);
            _bounds.y = math.distance(bottomLeft, topLeft);
            _bounds += _boundsMargin;
            _bounds /= 2f;
        }

        public bool IsNearMouse(in Point a) =>  a.DistanceSqr(_verts[^1]) <= _mouseRadius * _mouseRadius;

        void OnValidate() => CalculateBounds();
    }
}