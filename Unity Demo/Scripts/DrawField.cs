using Triangulation.Extensions;
using UnityEngine;

namespace Triangulation.Demo
{
    public sealed class DrawField : MonoBehaviour
    {
        Field _field;
        static Material lineMaterial;

        private void Awake() => _field = GetComponentInParent<Field>();

        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.MultMatrix(transform.localToWorldMatrix);

            DrawTriangles();
            DrawBounds();
        }

        private void DrawBounds()
        {
            GL.Begin(GL.LINES);
                Color c = _field.BoundsColor;
                GL.Color(c);
                GL.Vertex3(-_field.Bounds.x, 0, -_field.Bounds.y); //BL
                GL.Vertex3(_field.Bounds.x, 0, -_field.Bounds.y); //BR

                GL.Vertex3(-_field.Bounds.x, 0, -_field.Bounds.y); //BL
                GL.Vertex3(-_field.Bounds.x, 0, _field.Bounds.y); //TL

                GL.Vertex3(-_field.Bounds.x, 0, _field.Bounds.y); //TL
                GL.Vertex3(_field.Bounds.x, 0, _field.Bounds.y); //TR

                GL.Vertex3(_field.Bounds.x, 0, _field.Bounds.y); //TR
                GL.Vertex3(_field.Bounds.x, 0, -_field.Bounds.y); //BR
            GL.End();
        }

        void DrawTriangles()
        {
            Point mouse = new(_field.MousePosition.x, _field.MousePosition.z);
            foreach (Triangle tri in _field.Triangles)
            {
                GL.Begin(GL.TRIANGLES);

                Color c = mouse.DistanceSqr(tri.A) <= _field.MouseRadius*_field.MouseRadius ? _field.MouseColor : _field.EdgeColor;
                c.a = .1f;
                GL.Color(c);

                GL.Vertex(tri.A.float3());
                GL.Vertex(tri.B.float3());
                GL.Vertex(tri.C.float3());

                GL.End();

                for (int j = 0; j < 3; j++)
                {
                    Edge e = tri[j];
                    c = _field.TotalPoints + 4 < _field.Points.Count && (_field.IsNearMouse(e.A) || _field.IsNearMouse(e.B)) ? _field.MouseColor : _field.EdgeColor;
                    GL.Begin(GL.LINES);
                    GL.Color(c);
                    GL.Vertex(e.A.float3());
                    GL.Vertex(e.B.float3());
                    GL.End();

                }
            }

        }
    }
}