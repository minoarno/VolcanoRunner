using Unity.Netcode;
using _Scripts.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Terrain
{
    public class HexCell : MonoBehaviour, INetworkSerializable
    {
        public HexCoordinates coordinates;
        public RectTransform uiRect;
        
        public int Elevation 
        {
            get => _elevation;
            set 
            {
                _elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.ElevationStep;
                transform.localPosition = position;
                
                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = _elevation * -HexMetrics.ElevationStep;
                uiRect.localPosition = uiPosition;
            }
        }
	
        private int _elevation;
        public Color color;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref coordinates);
            serializer.SerializeValue(ref color);
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
