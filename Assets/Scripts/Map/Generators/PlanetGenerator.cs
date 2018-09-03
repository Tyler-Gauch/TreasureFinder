using Map.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Generators
{
  public class PlanetGenerator : BaseGenerator
  {
    public int NoiseScale = 10;

    public PlanetGenerator(int width, int height)
    {
      MapSize = new Vector2Int(width, height);
    }

    public PlanetGenerator(Vector2Int mapSize)
    {
      MapSize = mapSize;
    }

    public override void GenerateMap()
    {
      CreateDefaultTiles();
      float scale = UnityEngine.Random.Range(2, 15) / 100f;
      float xOffset = UnityEngine.Random.Range(0, 1000);
      float yOffset = UnityEngine.Random.Range(0, 1000);
      foreach (Tile t in this.Map)
      {
        t.Z = Mathf.PerlinNoise((t.X + xOffset) * scale, (t.Y + yOffset)* scale) * NoiseScale;
      }
    }
  }
}