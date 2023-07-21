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

        void Awake () {
            SelectColor(0);
        }

        void Update () {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) 
            {
                HandleInput();
            }
        }

        void HandleInput () {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) 
            {
                hexGrid.Value.ColorCellServerRpc(hit.point, _activeColor);
            }
        }

        public void SelectColor (int index) 
        {
            _activeColor = colorCollection.colors[index];
        }
    }
}
