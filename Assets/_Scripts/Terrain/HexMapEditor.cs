using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Terrain
{
    public class HexMapEditor : MonoBehaviour
    {
        public TerrainColorCollection colorCollection;

        public HexGrid hexGrid;

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
            if (Physics.Raycast(inputRay, out hit)) {
                hexGrid.ColorCell(hit.point, _activeColor);
            }
        }

        public void SelectColor (int index) 
        {
            _activeColor = colorCollection.colors[index];
        }
    }
}
