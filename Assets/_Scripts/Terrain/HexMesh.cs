using System.Collections.Generic;
using _Scripts.Core;
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
            for (HexDirection d = HexDirection.NorthEast; d <= HexDirection.NorthWest; d++) 
            {
                Triangulate(d, cell);
            }
        }
        
        void Triangulate (HexDirection direction, HexCell cell) {
            Vector3 center = cell.transform.localPosition;
            Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
            Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

            AddTriangle(center, v1, v2);
            AddTriangleColor(cell.color);

            //TriangulateConnection(direction, cell, v1, v2);
            if (direction <= HexDirection.SouthEast) TriangulateConnection(direction, cell, v1, v2);
        }

        void TriangulateConnection (HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2) 
        {
            HexCell neighbor = cell.GetNeighbor(direction);
            if (neighbor == null) return;

            Vector3 bridge = HexMetrics.GetBridge(direction);
            Vector3 v3 = v1 + bridge;
            Vector3 v4 = v2 + bridge;

            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighbor.color);
            
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction > HexDirection.East || nextNeighbor == null) return;
            
            AddTriangle(v2, v4, v2 + HexMetrics.GetBridge(direction.Next()));
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }

        void AddTriangleColor (Color c) 
        {
            _colors.Add(c);
            _colors.Add(c);
            _colors.Add(c);
        }
        
        void AddTriangleColor (Color c1, Color c2, Color c3) 
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
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
        
        void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
        }

        void AddQuadColor (Color c1, Color c2, Color c3, Color c4) 
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
            _colors.Add(c4);
        }
        
        void AddQuadColor (Color c1, Color c2) 
        {
            _colors.Add(c1);
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c2);
        }
    }
}