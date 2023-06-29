using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct TerrainInfo
    {
        public GameObject prefab;
        public int terrainCost;
        public float terrainPercentage;
    }
    
    [CreateAssetMenu(menuName = "Terrain/TerrainCollection", fileName = "TerrainCollection")]
    public class TerrainCollection : ScriptableObject
    {
        public List<TerrainInfo> terrainInfos;

        public GameObject GetTerrainInfoGameObject(float terrainPercentage)
        {
            return terrainInfos[0].prefab;
        }
    }
}
