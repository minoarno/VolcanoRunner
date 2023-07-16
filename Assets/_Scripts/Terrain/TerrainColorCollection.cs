using UnityEngine;

namespace _Scripts.Terrain
{
    [CreateAssetMenu(menuName = "Terrain/TerrainColorCollection")]
    public class TerrainColorCollection : ScriptableObject
    {
        public Color[] colors;
    }
}
