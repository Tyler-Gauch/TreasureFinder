using Map.Regions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map.Tiles
{
  public class Tile
  {

    public static ETileDirection[] diagonals = new ETileDirection[] { ETileDirection.NORTH_EAST, ETileDirection.NORTH_WEST, ETileDirection.SOUTH_EAST, ETileDirection.SOUTH_WEST};
    public static ETileDirection[] cardinal = new ETileDirection[] { ETileDirection.NORTH, ETileDirection.SOUTH, ETileDirection.EAST, ETileDirection.WEST };

    public Vector2 location;
    public ETileType Type = ETileType.WALL;
    public Tile NorthTile {
      get
      {
        return ParentMap.GetTile(X, Y + 1);
      }
    }
    public Tile SouthTile {
      get
      {
        return ParentMap.GetTile(X, Y - 1);
      }
    }

    public Tile WestTile
    {
      get
      {
        return ParentMap.GetTile(X - 1, Y);
      }
    }

    public Tile EastTile
    {
      get
      {
        return ParentMap.GetTile(X + 1, Y);
      }
    }

    public Region ParentRegion;

    public bool DebugMark;

    public int X;
    public int Y;

    private MapGenerator ParentMap;

    public Tile(int x, int y, MapGenerator parent)
    {
      this.X = x;
      this.Y = y;
      this.ParentMap = parent;
      Type = ETileType.WALL;
    }

    public List<Tile> Neighbors
    {
      get
      {
        return new List<Tile>() { NorthTile, SouthTile, EastTile, WestTile};
      }
    }

    public bool IsDeadend()
    {
      return Neighbors.Where(t => t.IsWall()).Count() == 3;
    }

    public bool IsCorner()
    {
      return (NorthTile.IsWall() || SouthTile.IsWall())
        && (EastTile.IsWall() || WestTile.IsWall());
    }

    public bool IsWall()
    {
      return Type == ETileType.WALL;
    }

    public bool IsFloor()
    {
      return Type == ETileType.FLOOR || Type == ETileType.ROOMFLOOR;
    }

    public bool IsRoomFloor()
    {
      return Type == ETileType.ROOMFLOOR;
    }

    public static ETileDirection GetOppositeDirection(ETileDirection direction)
    {
      switch (direction)
      {
        case ETileDirection.EAST:
          return ETileDirection.WEST;
        case ETileDirection.WEST:
          return ETileDirection.EAST;
        case ETileDirection.NORTH:
          return ETileDirection.SOUTH;
        case ETileDirection.SOUTH:
          return ETileDirection.NORTH;
        case ETileDirection.NORTH_EAST:
          return ETileDirection.NORTH_WEST;
        case ETileDirection.NORTH_WEST:
          return ETileDirection.NORTH_EAST;
        case ETileDirection.SOUTH_EAST:
          return ETileDirection.SOUTH_WEST;
        case ETileDirection.SOUTH_WEST:
          return ETileDirection.SOUTH_EAST;
        default:
          return ETileDirection.NONE;
      }
    }

    public new string ToString()
    {
      return "(" + X + ", " + Y + ")";
    }
  }
}
