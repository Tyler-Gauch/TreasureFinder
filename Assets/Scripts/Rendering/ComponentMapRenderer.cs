using Map.Regions;
using Map.Tiles;
using Rendering.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rendering
{
  [AddComponentMenu("Mapping/Rendering/Component Map Renderer")]
  public class ComponentMapRenderer : MapRenderer
  {
    [Header("Component Types")]
    public List<WallComponent> Walls;
    public List<FloorComponent> Floors;
    public List<CornerComponent> Corners;
    public List<DoorComponent> Doors;

    protected override void PlacePlayer()
    {
      // do nothing for now
    }

    void Update()
    {
      if (Input.GetKeyDown(KeyCode.R))
      {
        DeleteMap();
      }

      if (Input.GetKeyUp(KeyCode.R))
      {
        Map.GenerateMap();
        RenderMap();
        PrintMap();
      }
    }

    public void PrintMap()
    {
      string m = "";
      for (int y = Map.MapSize.y - 1; y >= 0; y--)
      {
        for (int x = 0; x < Map.MapSize.x; x++)
        {
          Tile t = Map.GetTile(x, y); 
          switch(t.Type)
          {
            case ETileType.FLOOR:
              m += "1";
              break;
            case ETileType.ROOMFLOOR:
              m += "1";
              break;
            case ETileType.WALL:
              m += "0";
              break;
            case ETileType.DOOR:
              m += "1";
              break;
          }
        }
        m += "\n";
      }

      Debug.Log(m);
    }

    protected override void RenderMap()
    {
      // first render the floor for all the rooms
      List<Region> rooms = Map.GetRegions().Where(r => r is Room).ToList();

      foreach (Room room in rooms)
      {
        if (room is SquareRoom)
        {
          // first calculate the floor size we need
          SquareRoom sroom = room as SquareRoom;
          int maxPossibleWidth = sroom.Width;
          int maxPossibleHeight = sroom.Height;
          int areaToCover = sroom.Width * sroom.Height;
          Vector3 floorPlacement = new Vector3(sroom.X, 0, sroom.Y);

          FloorComponent floor = Floors.Where(f => f.WidthInTiles == sroom.Width && f.DepthInTiles == sroom.Height).FirstOrDefault();

          if (floor == null)
          {
            throw new Exception("We failed to find a floor for the room " + sroom.Width + " X " + sroom.Height);
          }

          FloorComponent renderedComponent = Instantiate(floor, floorPlacement, Quaternion.identity);
          MapRender.Add(renderedComponent.gameObject);
         
        }
        else
        {
          throw new NotImplementedException("We don't know how to render " + room.GetType());
        }
      }
    }

  }
}
