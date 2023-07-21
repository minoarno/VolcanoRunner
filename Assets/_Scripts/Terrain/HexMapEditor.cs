using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Terrain
{
    public class HexMapEditor : NetworkBehaviour
    {
        public TerrainColorCollection colorCollection;

        public NetworkVariable<HexGrid> hexGrid = new NetworkVariable<HexGrid>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner
            );

        private Color _activeColor;
        private int _activeElevation;
        
        void Awake () {
            SelectColor(0);
        }

        void Update () {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) 
            {
                HandleInput();
            }
        }

        void HandleInput () 
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) 
            {
                EditCell(hexGrid.Value.GetCell(hit.point));
            }
        }

        void EditCell (HexCell cell) 
        {
            cell.color = _activeColor;
            cell.Elevation = _activeElevation;
            hexGrid.Value.Refresh();
        }
        
        public void SelectColor (int index) 
        {
            _activeColor = colorCollection.colors[index];
        }
        
        public void SetElevation (float elevation) 
        {
            _activeElevation = (int)elevation;
        }
    }
}
