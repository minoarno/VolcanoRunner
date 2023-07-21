using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Terrain
{
    public class HexCell : MonoBehaviour, INetworkSerializable
    {
        public HexCoordinates coordinates;
        
        public Color color;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref coordinates);
            serializer.SerializeValue(ref color);
        }
    }
}
