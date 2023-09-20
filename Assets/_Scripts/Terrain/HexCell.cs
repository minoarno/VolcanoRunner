using _Scripts.Core;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Terrain
{
    public class HexCell : NetworkBehaviour
    {
        public HexCoordinates Coordinates
        {
            get
            {

                return _coordinates.Value;
            }
            set
            {
                _coordinates.Value = value;
                name = _coordinates.Value.ToString();
            }
        }
        [SerializeField]private NetworkVariable<HexCoordinates> _coordinates = new();
        //public RectTransform uiRect;
        
        public int Elevation 
        {
            get => _elevation.Value;
            set 
            {
                _elevation.Value = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.ElevationStep;
                transform.localPosition = position;
                
                //Vector3 uiPosition = uiRect.localPosition;
                //uiPosition.z = _elevation * -HexMetrics.ElevationStep;
                //uiRect.localPosition = uiPosition;
            }
        }

        public override void OnNetworkSpawn()
        {
            name = _coordinates.Value.ToString();
        }

        private NetworkVariable<int> _elevation = new();
        private NetworkVariable<float> _colorR = new();
        private NetworkVariable<float> _colorG = new();
        private NetworkVariable<float> _colorB = new();

        public Color Color
        {
            get
            {
                return new Color(_colorR.Value, _colorG.Value, _colorB.Value);
            }
            set
            {
                _colorR.Value = value.r;
                _colorG.Value = value.g;
                _colorB.Value = value.b;
            }
        }

        [SerializeField] private HexCell[] neighbors;
        
        public HexCell GetNeighbor (HexDirection direction) 
        {
            return neighbors[(int)direction];
        }
        
        public void SetNeighbor (HexDirection direction, HexCell cell) 
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
        
        public HexEdgeType GetEdgeType (HexDirection direction) 
        {
            return HexMetrics.GetEdgeType(Elevation, neighbors[(int)direction].Elevation);
        }
        
        public HexEdgeType GetEdgeType (HexCell otherCell) 
        {
            return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
        }
    }
}
