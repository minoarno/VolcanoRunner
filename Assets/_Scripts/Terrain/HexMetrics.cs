using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Terrain
{
    public static class HexMetrics
    {
        public const float OuterRadius = 10f;
        public const float InnerRadius = OuterRadius * 0.866025404f;
    
        public const float solidFactor = 0.75f;
        public const float blendFactor = 1f - solidFactor;
        
        static readonly Vector3[] Corners = {
            new(0f, 0f, OuterRadius),
            new(InnerRadius, 0f, 0.5f * OuterRadius),
            new(InnerRadius, 0f, -0.5f * OuterRadius),
            new(0f, 0f, -OuterRadius),
            new(-InnerRadius, 0f, -0.5f * OuterRadius),
            new(-InnerRadius, 0f, 0.5f * OuterRadius),
            new Vector3(0f, 0f, OuterRadius)
        };
        
        public static Vector3 GetFirstCorner (HexDirection direction) 
        {
            return Corners[(int)direction];
        }

        public static Vector3 GetSecondCorner (HexDirection direction) 
        {
            return Corners[(int)direction + 1];
        }
        
        public static Vector3 GetFirstSolidCorner (HexDirection direction) 
        {
            return Corners[(int)direction] * solidFactor;
        }

        public static Vector3 GetSecondSolidCorner (HexDirection direction) 
        {
            return Corners[(int)direction + 1] * solidFactor;
        }
        
        public static Vector3 GetBridge (HexDirection direction) 
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) * blendFactor;
        }
    }
    
    [System.Serializable]
    public struct HexCoordinates 
    {
        [SerializeField]
        private int x, z;
        public int X => x;

        public int Z => z;

        public int Y => -X - Z;

        public HexCoordinates (int x, int z) 
        {
            this.x = x;
            this.z = z;
        }
        
        public static HexCoordinates FromOffsetCoordinates (int x, int z) {
            return new HexCoordinates(x - z / 2, z);
        }
        
        public override string ToString () {
            return $"( {X}, {Y}, {Z} )";
        }

        public string ToStringOnSeparateLines ()
        {
            return $"{X}\n{Y}\n{Z}";
        }
        
        public static HexCoordinates FromPosition (Vector3 position) 
        {
            float x = position.x / (HexMetrics.InnerRadius * 2f);
            float y = -x;
            
            float offset = position.z / (HexMetrics.OuterRadius * 3f);
            x -= offset;
            y -= offset;
            
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x -y);

            if (iX + iY + iZ != 0) 
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x -y - iZ);

                if (dX > dY && dX > dZ) 
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY) 
                {
                    iZ = -iX - iY;
                }
            }
            
            return new HexCoordinates(iX, iZ);
        }
    }
}
