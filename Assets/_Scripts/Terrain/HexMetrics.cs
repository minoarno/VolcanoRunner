using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Terrain
{
    public static class HexMetrics
    {
        public const float OuterRadius = 10f;
        public const float InnerRadius = OuterRadius * 0.866025404f;

        public const float SolidFactor = 0.8f;
        public const float BlendFactor = 1f - SolidFactor;

        public const float ElevationStep = 3f;

        public const int TerracesPerSlope = 2;
        public const int TerraceSteps = TerracesPerSlope * 2 + 1;
        public const float HorizontalTerraceStepSize = 1f / TerraceSteps;
        public const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

        public static Texture2D noiseSource;
        public const float cellPerturbStrength = 4f;
        public const float noiseScale = 0.003f;
        public const float elevationPerturbStrength = 1.5f;

        static readonly Vector3[] Corners =
        {
            new(0f, 0f, OuterRadius),
            new(InnerRadius, 0f, 0.5f * OuterRadius),
            new(InnerRadius, 0f, -0.5f * OuterRadius),
            new(0f, 0f, -OuterRadius),
            new(-InnerRadius, 0f, -0.5f * OuterRadius),
            new(-InnerRadius, 0f, 0.5f * OuterRadius),
            new(0f, 0f, OuterRadius)
        };

        public static Vector3 GetFirstCorner(HexDirection direction)
        {
            return Corners[(int)direction];
        }

        public static Vector3 GetSecondCorner(HexDirection direction)
        {
            return Corners[(int)direction + 1];
        }

        public static Vector3 GetFirstSolidCorner(HexDirection direction)
        {
            return Corners[(int)direction] * SolidFactor;
        }

        public static Vector3 GetSecondSolidCorner(HexDirection direction)
        {
            return Corners[(int)direction + 1] * SolidFactor;
        }

        public static Vector3 GetBridge(HexDirection direction)
        {
            return (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
        }

        public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            float h = step * HorizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            float v = ((step + 1) / 2) * VerticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        public static Color TerraceLerp(Color a, Color b, int step)
        {
            float h = step * HorizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexEdgeType.Flat;
            }

            int delta = elevation2 - elevation1;
            return delta is 1 or -1 ? HexEdgeType.Slope : HexEdgeType.Cliff;
        }
        
        public static Vector4 SampleNoise (Vector3 position) 
        {
            return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
        }
    }

    [System.Serializable]
    public struct HexCoordinates
    {
        [SerializeField] private int x, z;
        public int X => x;

        public int Z => z;

        public int Y => -X - Z;

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString()
        {
            return $"( {X}, {Y}, {Z} )";
        }

        public string ToStringOnSeparateLines()
        {
            return $"{X}\n{Y}\n{Z}";
        }

        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.InnerRadius * 2f);
            float y = -x;

            float offset = position.z / (HexMetrics.OuterRadius * 3f);
            x -= offset;
            y -= offset;

            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            if (iX + iY + iZ != 0)
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

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

    public struct EdgeVertices
    {
        public Vector3 v1, v2, v3, v4;

        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            v1 = corner1;
            v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
            v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
            v4 = corner2;
        }

        public static EdgeVertices TerraceLerp(EdgeVertices a, EdgeVertices b, int step)
        {
            EdgeVertices result;
            result.v1 = HexMetrics.TerraceLerp(a.v1, b.v1, step);
            result.v2 = HexMetrics.TerraceLerp(a.v2, b.v2, step);
            result.v3 = HexMetrics.TerraceLerp(a.v3, b.v3, step);
            result.v4 = HexMetrics.TerraceLerp(a.v4, b.v4, step);
            return result;
        }
    }
}
