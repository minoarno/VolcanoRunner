using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Terrain
{
    public class HexGrid : MonoBehaviour
    {
        public int width = 6;
        public int height = 6;

        public Color defaultColor = Color.white;
        public Color touchedColor = Color.magenta;
        
        public HexCell cellPrefab;
    
        HexCell[] _cells;
        
        public Text cellLabelPrefab;

        Canvas _gridCanvas;
        HexMesh _hexMesh;
        
        void Awake () 
        {
            _gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh = GetComponentInChildren<HexMesh>();

            _cells = new HexCell[height * width];

            for (int z = 0, i = 0; z < height; z++) 
            {
                for (int x = 0; x < width; x++) 
                {
                    CreateCell(x, z, i++);
                }
            }
        }
	
        void Start () 
        {
            _hexMesh.Triangulate(_cells);
        }
        
        void CreateCell (int x, int z, int i) 
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.OuterRadius * 1.5f);

            HexCell cell = _cells[i] = Instantiate(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;
            
            Text label = Instantiate(cellLabelPrefab);
            label.rectTransform.SetParent(_gridCanvas.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }
        
        void Update () {
            if (Input.GetMouseButton(0)) {
                HandleInput();
            }
        }

        void HandleInput () {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit, Single.MaxValue)) {
                TouchCell(hit.point);
            }
        }
	
        void TouchCell (Vector3 position) {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            HexCell cell = _cells[index];
            cell.color = touchedColor;
            _hexMesh.Triangulate(_cells);
        }
    }
}
