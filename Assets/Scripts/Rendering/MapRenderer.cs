using System.Collections.Generic;
using UnityEngine;
using Map.Tiles;
using System;
using Map;

namespace Rendering
{
  public class MapRenderer : MonoBehaviour
  {

    public GameObject FloorTile;

    public GameObject WallTile;

    public GameObject CeilingTile;

    public GameObject MarkerTile;

    public Vector2 MaxRegionSize;

    public Vector2 MinRegionSize;

    public GameObject Player;

    public GameObject Treasure;

    public int NumberOfRoomPlacementRetries;

    private List<GameObject> MapRender;

    public bool RandomizeParameters = true;

    public MapGenerator Map;

    /// <summary>
    /// The defined size of all tiles base
    /// </summary>
    public Vector2 MapSize;
    public float KeepSameDirectionPercentage = 50f;
    public float PossibilityToAddAnotherConnection = 50f;
    public float AdditionalConnectionAttempts = 2;
    public float PercentageOfTilesToKeep = 50;

    private void Awake()
    {
      MapRender = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {
      Map = new MapGenerator((int)MapSize.x, (int)MapSize.y);
      SetMapParameters();
      Map.GenerateMap();
      RenderMap();
    }

    private void SetMapParameters()
    {
      if (RandomizeParameters)
      {
        MapSize.x = (int)(UnityEngine.Random.Range(0, 200) / 2) * 2 + 1;
        MapSize.y = (int)(UnityEngine.Random.Range(0, 200) / 2) * 2 + 1;
        MinRegionSize.x = (int)(UnityEngine.Random.Range(0, MapSize.x / 5) / 2) * 2 + 1;
        MinRegionSize.y = (int)(UnityEngine.Random.Range(0, MapSize.y / 5) / 2) * 2 + 1;
        MaxRegionSize.x = (int)(UnityEngine.Random.Range(MinRegionSize.x + 1, MapSize.x / 2) / 2) * 2 + 1;
        MaxRegionSize.y = (int)(UnityEngine.Random.Range(MinRegionSize.y + 1, MapSize.x / 2) / 2) * 2 + 1;
        NumberOfRoomPlacementRetries = UnityEngine.Random.Range(0, 1000);
        KeepSameDirectionPercentage = UnityEngine.Random.Range(0, 100);
        PossibilityToAddAnotherConnection = UnityEngine.Random.Range(0, 100);
        AdditionalConnectionAttempts = UnityEngine.Random.Range(0, 5);
        PercentageOfTilesToKeep = UnityEngine.Random.Range(0, 100);
      }

      Map.MaxRegionHeight = (int)MaxRegionSize.y;
      Map.MaxRegionWidth = (int)MaxRegionSize.x;
      Map.MinRegionHeight = (int)MinRegionSize.y;
      Map.MinRegionWidth = (int)MinRegionSize.x;
      Map.NumberOfRoomPlacementRetries = NumberOfRoomPlacementRetries;
      Map.KeepSameDirectionPercentage = KeepSameDirectionPercentage;
      Map.AddAnotherConnectionPercentage = PossibilityToAddAnotherConnection;
      Map.AdditionalConnectionAttempts = AdditionalConnectionAttempts;
      Map.TilesToKeepPercentage = PercentageOfTilesToKeep;
      Map.MapHeight = (int)MapSize.y;
      Map.MapWidth = (int)MapSize.x;
    }

    void Update()
    {
      if (Input.GetKeyDown(KeyCode.KeypadEnter))
      {
        DeleteMap();
      }

      if (Input.GetKeyUp(KeyCode.KeypadEnter))
      {
        SetMapParameters();
        Map.GenerateMap();
        RenderMap();
      }
    }



    /// <summary>
    /// Converts the map coordinates to screen coordinates
    /// </summary>
    /// <param name="map">The map coordinates to convert</param>
    /// <returns>The map coordinates in screen coordinates</returns>
    public Vector2 ConvertMapToScreen(Vector2 map)
    {
      return new Vector2(map.x - map.y, (map.x + map.y) / 2);
    }

    /// <summary>
    /// Converts the screen coordinates to map coordinates
    /// </summary>
    /// <param name="screen">The screen coordinates to convert</param>
    /// <returns>the screen coordinates in map coordinates</returns>
    public Vector2 ConvertScreenToMap(Vector2 screen)
    {
      return new Vector2(
        (2 * screen.y + screen.x) / 2,
        (2 * screen.y - screen.x) / 2
      );
    }

    private void RenderMap()
    {
      for (int x = 0; x < Map.MapWidth; x++)
      {
        for (int y = 0; y < Map.MapHeight; y++)
        {
          Vector3 mapLoc = new Vector3(x, 0, y);
          Tile tile = Map.GetTile(x, y);

          if (tile.Visited)
          {
            GameObject floor = Instantiate(FloorTile, mapLoc, Quaternion.identity);
            floor.layer = 8; // render to minimap
            MapRender.Add(floor);
            MapRender.Add(Instantiate(CeilingTile, mapLoc + Vector3.up * 4, Quaternion.identity));
          }
          else
          {
            GameObject firstWall = Instantiate(WallTile, mapLoc + Vector3.up, Quaternion.identity);
            firstWall.layer = 8; // render to minimap
            MapRender.Add(firstWall);
            MapRender.Add(Instantiate(WallTile, mapLoc + Vector3.up * 2, Quaternion.identity));
            MapRender.Add(Instantiate(WallTile, mapLoc + Vector3.up * 3, Quaternion.identity));
          }

          if (tile.DebugMark)
          {
            MapRender.Add(Instantiate(MarkerTile, mapLoc, Quaternion.identity));
          }
        }
      }

      Tile startTile = Map.GetRandomVisitedTile();
      Tile endTile = Map.GetRandomVisitedTile();
      Treasure.transform.position = new Vector3(endTile.X, 1, endTile.Y);
      Player.transform.position = new Vector3(startTile.X, 2, startTile.Y);
    }

    private void DeleteMap()
    {
      foreach (GameObject tr in MapRender)
      {
        DestroyImmediate(tr.gameObject, true);
      }

      MapRender.Clear();
    }
  }
}