using System;
using System.Linq;
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
    
        HexCell[] _cells;

        private NetworkBehaviourReference _reference;
        
        //public Text cellLabelPrefab;

        //Canvas _gridCanvas;
        HexMesh _hexMesh;


        public override void OnNetworkSpawn()
        {
            //_gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh = GetComponentInChildren<HexMesh>();

            if (IsHost)
            {
                _cells = new HexCell[height * width];
                for (int z = 0, i = 0; z < height; z++) 
                {
                    for (int x = 0; x < width; x++) 
                    {
                        CreateCell(x, z, i++);
                    }
                }
            }
            else
            {
                Debug.Log(gameObject.transform.childCount);
                var cells = GetComponentsInChildren<HexCell>().ToList();
                cells.Sort((hexcell1, hexcell2) =>
                {
                    var ret = hexcell1.Coordinates.Z.CompareTo(hexcell2.Coordinates.Z);
                    if (ret == 0) ret = hexcell1.Coordinates.X.CompareTo(hexcell2.Coordinates.X);
                    return ret;
                });
                _cells = cells.ToArray();
                
                //position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
                //position.y = 0f;
                //position.z = z * (HexMetrics.OuterRadius * 1.5f);
                
                for (int i = 0; i < _cells.Length; i++)
                {
                    int x = (int)(_cells[i].transform.position.x / (HexMetrics.OuterRadius * 2f));
                    int z = (int)(_cells[i].transform.position.z / (HexMetrics.OuterRadius * 1.5f));
                    
                    if (x > 0)
                    {
                        _cells[i].SetNeighbor(HexDirection.West, _cells[i - 1]);
                    }
                    if (z > 0)
                    {
                        if ((z & 1) == 0) 
                        {
                            _cells[i].SetNeighbor(HexDirection.SouthEast, _cells[i - width]);
                            if (x > 0) 
                            {
                                _cells[i].SetNeighbor(HexDirection.SouthWest, _cells[i - width - 1]);
                            }
                        }
                        else 
                        {
                            _cells[i].SetNeighbor(HexDirection.SouthWest, _cells[i - width]);
                            if (x < width - 1) 
                            {
                                _cells[i].SetNeighbor(HexDirection.SouthEast, _cells[i - width + 1]);
                            }
                        }
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
            _cells[i] = go.GetComponent<HexCell>();

            HexCell cell = _cells[i];
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color = defaultColor;

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.West, _cells[i - 1]);
            }
            if (z > 0)
            {
                if ((z & 1) == 0) 
                {
                    cell.SetNeighbor(HexDirection.SouthEast, _cells[i - width]);
                    if (x > 0) 
                    {
                        cell.SetNeighbor(HexDirection.SouthWest, _cells[i - width - 1]);
                    }
                }
                else 
                {
                    cell.SetNeighbor(HexDirection.SouthWest, _cells[i - width]);
                    if (x < width - 1) 
                    {
                        cell.SetNeighbor(HexDirection.SouthEast, _cells[i - width + 1]);
                    }
                }
            }
        }

        public void ColorCell (Vector3 position, Color color) {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            HexCell cell = _cells[index];
            cell.Color = color;
            _hexMesh.Triangulate(_cells);
        }
        
        public HexCell GetCell (Vector3 position) 
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            return _cells[index];
        }
        
        public void Refresh () 
        {
            _hexMesh.Triangulate(_cells);
        }
    }
}
