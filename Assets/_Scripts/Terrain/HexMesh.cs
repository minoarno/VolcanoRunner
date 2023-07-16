using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Terrain
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour {
        private Mesh _hexMesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;

        private MeshCollider _meshCollider;

        private List<Color> _colors;

        private void Awake () 
        {
            GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            
            _hexMesh.name = "Hex Mesh";
            _vertices = new List<Vector3>();
            _colors = new List<Color>();
            _triangles = new List<int>();
        }

        public void Triangulate (HexCell[] cells) 
        {
            _hexMesh.Clear();
            _vertices.Clear();
            _triangles.Clear();
            _colors.Clear();
            
            foreach (var cell in cells)
            {
                Triangulate(cell);
            }
            _hexMesh.vertices = _vertices.ToArray();
            _hexMesh.colors = _colors.ToArray();
            _hexMesh.triangles = _triangles.ToArray();
            _hexMesh.RecalculateNormals();
            
            _meshCollider.sharedMesh = _hexMesh;
        }

        private void Triangulate (HexCell cell) 
        {
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++) {
                AddTriangle(
                    center,
                    center + HexMetrics.Corners[i],
                    center + HexMetrics.Corners[i + 1]
                );
                AddTriangleColor(cell.color);
            }
        }

        private void AddTriangleColor(Color cellColor)
        {
            _colors.Add(cellColor);
            _colors.Add(cellColor);
            _colors.Add(cellColor);
        }

        private void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }
    }
}