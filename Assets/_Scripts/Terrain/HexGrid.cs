using System;
using _Scripts.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Terrain
{
    public class HexGrid : NetworkBehaviour
    {
        public int width = 6;
        public int height = 6;

        public Color defaultColor = Color.white;
        public Color touchedColor = Color.magenta;
        
        public HexCell cellPrefab;
    
        NetworkVariable<HexCell>[] _cells;
        
        //public Text cellLabelPrefab;
//
        //Canvas _gridCanvas;
        HexMesh _hexMesh;


        public override void OnNetworkSpawn()
        {
            //_gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh = GetComponentInChildren<HexMesh>();

            if (IsHost)
            {
                _cells = new NetworkVariable<HexCell>[height * width];
                for (int z = 0, i = 0; z < height; z++) 
                {
                    for (int x = 0; x < width; x++) 
                    {
                        CreateCell(x, z, i++);
                    }
                }
            }
            
            _hexMesh.Triangulate(_cells);
        }
        
        void CreateCell (int x, int z, int i) 
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.OuterRadius * 1.5f);

            GameObject go = Instantiate(cellPrefab.gameObject);
            go.GetComponent<NetworkObject>().Spawn(true);
            _cells[i] = new NetworkVariable<HexCell>(go.GetComponent<HexCell>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

            HexCell cell = _cells[i].Value;
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.West, _cells[i - 1].Value);
            }
            if (z > 0)
            {
                if ((z & 1) == 0) 
                {
                    cell.SetNeighbor(HexDirection.SouthEast, _cells[i - width].Value);
                    if (x > 0) 
                    {
                        cell.SetNeighbor(HexDirection.SouthWest, _cells[i - width - 1].Value);
                    }
                }
                else 
                {
                    cell.SetNeighbor(HexDirection.SouthWest, _cells[i - width].Value);
                    if (x < width - 1) 
                    {
                        cell.SetNeighbor(HexDirection.SouthEast, _cells[i - width + 1].Value);
                    }
                }
            }
            
            //Text label = Instantiate(cellLabelPrefab);
            //label.rectTransform.SetParent(_gridCanvas.transform, false);
            //label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            //label.text = cell.coordinates.ToStringOnSeparateLines();
            
            //cell.uiRect = label.rectTransform;
        }

        public void ColorCell (Vector3 position, Color color) {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            HexCell cell = _cells[index].Value;
            cell.color = color;
            _hexMesh.Triangulate(_cells);
        }
        
        public HexCell GetCell (Vector3 position) 
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            return _cells[index].Value;
        }
        
        public void Refresh () 
        {
            _hexMesh.Triangulate(_cells);
        }
    }
}
