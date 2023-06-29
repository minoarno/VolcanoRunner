using UnityEngine;

namespace Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private int size = 1;
        [SerializeField] private int cols = 1;
        [SerializeField] private int rows = 1;
        [SerializeField] private TerrainCollection terrainCollection;
        
        void GenerateMap()
        {
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    if (r % 2 == 0)
                    {
                        CreateTile(new Vector3(c * size * 1.6f, 0, r * size * 2), 0);
                    }
                    else
                    {
                        if (r == rows - 1)continue;
                        CreateTile(new Vector3((c - .5f) * size * 1.6f, 0, (r + .5f) * size * 2), 0);   
                    }
                }
            }
        }

        void CreateTile(Vector3 position, float percentage)
        {
            Instantiate(terrainCollection.GetTerrainInfoGameObject(percentage), position, Quaternion.identity);
        }
    }
}
