using Map.Generators;
using Map.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rendering
{
  public class PlanetRenderer : MonoBehaviour
  {
    public PlanetGenerator Map;

    public int MapSizeX;
    public int MapSizeY;

    public Vector2 WaterThreshold;

    public GameObject Grass;
    public GameObject Water;
    public GameObject Tree;
    public GameObject Unknown;

    public int NoiseScale = 10;

    private List<GameObject> Objects;

    private void SetMapParameters()
    {
      Map = new PlanetGenerator(MapSizeX, MapSizeY);
      Map.NoiseScale = NoiseScale;
      Objects = new List<GameObject>();
    }

    // Use this for initialization
    public virtual void Start()
    {
      BuildMap();
    }

    public void BuildMap()
    {
      SetMapParameters();
      Map.GenerateMap();
      RenderMap();
    }

    public virtual void Update()
    {
      if (Input.GetKeyDown(KeyCode.R))
      {
        DeleteMap();
      }

      if (Input.GetKeyUp(KeyCode.R))
      {
        BuildMap();
      }
    }

    protected virtual void RenderMap()
    {
      for (int x = 0; x < Map.MapSize.x; x++)
      {
        for (int y = 0; y < Map.MapSize.y; y++)
        {
          Tile tile = Map.GetTile(x, y);
          Vector3 mapLoc = new Vector3(tile.X, 0, tile.Y);
          Objects.Add(Instantiate(GetPrefab(tile.Z), mapLoc, Quaternion.identity));
        }
      }
    }

    public GameObject GetPrefab(float TileType)
    {
      if (Between(TileType, WaterThreshold.x, WaterThreshold.y))
      {
        return Water;
      } else
      {
        return Grass;
      }
    }

    public bool Between(float value, float lower, float upper)
    {
      return value >= lower && value <= upper;
    }

    protected void DeleteMap()
    {
      foreach (GameObject tr in Objects)
      {
        DestroyImmediate(tr.gameObject, true);
      }

      Objects.Clear();
    }
  }
}
