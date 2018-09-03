using System.Collections.Generic;
using UnityEngine;
using Map.Tiles;
using Map;
using Map.Regions;
using Map.Generators;

namespace Rendering
{
  [AddComponentMenu("Mapping/Rendering/Map Renderer")]
  public abstract class MapRenderer : MonoBehaviour
  {

    [Header("Tuning Parameters")]
    public bool RandomizeParameters = true;
    public Vector2 MapSize;
    public float KeepSameDirectionPercentage = 50f;
    public float PossibilityToAddAnotherConnection = 50f;
    public float AdditionalConnectionAttempts = 2;
    public float PercentageOfTilesToKeep = 50;

    [Header("Region Settings")]
    public Vector2Int MaxRoomSize;
    public Vector2Int MinRoomSize;
    public int NumberOfRoomPlacementRetries;
    public List<PredefinedRoom> PredefinedRooms;

    [Header("Misc")]
    public GameObject Player;

    protected List<GameObject> MapRender;

    [HideInInspector]
    public BaseGenerator Map;

    public virtual void Awake()
    {
      MapRender = new List<GameObject>();
    }

    // Use this for initialization
    public virtual void Start()
    {
      SetMapParameters();
      Map.GenerateMap();
      RenderMap();
    }

    protected virtual void SetMapParameters()
    {

      if (RandomizeParameters)
      {
        MapSize.x = (int)(UnityEngine.Random.Range(0, 200) / 2) * 2 + 1;
        MapSize.y = (int)(UnityEngine.Random.Range(0, 200) / 2) * 2 + 1;
        MinRoomSize.x = (int)(UnityEngine.Random.Range(0, MapSize.x / 5) / 2) * 2 + 1;
        MinRoomSize.y = (int)(UnityEngine.Random.Range(0, MapSize.y / 5) / 2) * 2 + 1;
        MaxRoomSize.x = (int)(UnityEngine.Random.Range(MinRoomSize.x + 1, MapSize.x / 2) / 2) * 2 + 1;
        MaxRoomSize.y = (int)(UnityEngine.Random.Range(MinRoomSize.y + 1, MapSize.x / 2) / 2) * 2 + 1;
        NumberOfRoomPlacementRetries = UnityEngine.Random.Range(0, 1000);
        KeepSameDirectionPercentage = UnityEngine.Random.Range(0, 100);
        PossibilityToAddAnotherConnection = UnityEngine.Random.Range(0, 100);
        AdditionalConnectionAttempts = UnityEngine.Random.Range(0, 5);
        PercentageOfTilesToKeep = UnityEngine.Random.Range(0, 100);
      }

      Map = new RandomlySizedRegionsGenerator((int)MapSize.x, (int)MapSize.y) {
          MaxRegionHeight = (int)MaxRoomSize.y,
          MaxRegionWidth = (int)MaxRoomSize.x,
          MinRegionHeight = (int)MinRoomSize.y,
          MinRegionWidth = (int)MinRoomSize.x,
          NumberOfRoomPlacementRetries = NumberOfRoomPlacementRetries,
          KeepSameDirectionPercentage = KeepSameDirectionPercentage,
          AddAnotherConnectionPercentage = PossibilityToAddAnotherConnection,
          AdditionalConnectionAttempts = AdditionalConnectionAttempts,
          TilesToKeepPercentage = PercentageOfTilesToKeep
      };
    }

    /// <summary>
    /// Converts the map coordinates to screen coordinates
    /// </summary>
    /// <param name="map">The map coordinates to convert</param>
    /// <returns>The map coordinates in screen coordinates</returns>
    public virtual Vector3 ConvertMapToScreen(Vector3 map)
    {
      return map;
    }

    /// <summary>
    /// Converts the screen coordinates to map coordinates
    /// </summary>
    /// <param name="screen">The screen coordinates to convert</param>
    /// <returns>the screen coordinates in map coordinates</returns>
    public virtual Vector3 ConvertScreenToMap(Vector3 screen)
    {
      return screen;
    }

    protected virtual void PlacePlayer()
    {
      Tile startTile = Map.GetRandomVisitedTile();
      Player.transform.position = new Vector3(startTile.X, 2, startTile.Y);
    }

    protected void DeleteMap()
    {
      foreach (GameObject tr in MapRender)
      {
        DestroyImmediate(tr.gameObject, true);
      }

      MapRender.Clear();
    }

    protected abstract void RenderMap();
  }
}